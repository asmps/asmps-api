using System.Security.Claims;
using ASMPS.API.Helpers;
using ASMPS.Contracts.Lesson;
using ASMPS.Contracts.Schedule;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ASMPS.Models;
using ASMPS.Contracts.Student;
using ASMPS.Contracts.Teacher;
using Microsoft.OpenApi.Extensions;

namespace ASMPS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : Controller
{
    private readonly DatabaseContext _context;
    private readonly JwtHelper _jwtHelper;
    
    /// <summary>
    /// Конструктор класса <see cref="StudentsController"/>
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="jwtHelper">Расшифровщик данных пользователя из JWT</param>
    public StudentsController(DatabaseContext context, JwtHelper jwtHelper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _jwtHelper = jwtHelper ?? throw new ArgumentNullException(nameof(jwtHelper));

    }
    
    /// <summary>
    /// Добавить студента
    /// </summary>
    /// <param name="studentAddDto">Данные по студенту</param>
    [Authorize(Roles = "Deanery")]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] StudentAddDto studentAddDto)
    {
        if (!ModelState.IsValid) return BadRequest();

        var group = await _context.GroupStudents.FirstOrDefaultAsync(x => x.Title == studentAddDto.TitleGroup);
        if (group is null) return NotFound($"Данной группы '{studentAddDto.TitleGroup}' не существует!");
        var item = new Student
        {
            Id = Guid.NewGuid(),
            Login = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8),
            Password = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8),
            Name = studentAddDto.Name,
            Surname = studentAddDto.Surname,
            Patronymic = studentAddDto.Patronymic,
            Email = studentAddDto.Email == string.Empty ? null : studentAddDto.Email,
            CreatedDate = DateTime.UtcNow,
            Role = UserRoles.Student,
            Group = group,
            GroupId = group.Id, 
            StudentID = string.Concat(new Random().Next(10000001, 99999999).ToString())
        };

        await _context.Users.AddAsync(item);
        await _context.SaveChangesAsync();
        return Ok(item.Id);
    }

    /// <summary>
    /// Получить расписание
    /// </summary>
    /// <returns>Расписание</returns>
    [Route("schedule"), HttpGet]
    public async Task<IActionResult> GetSchedule()
    {
        var studentInfo = GetAuthUserInfo();
        if (studentInfo is null)
            return Unauthorized();

        var student = await _context.Students.FirstOrDefaultAsync(item => item.Id == studentInfo.GuidId);
        if (student is null)
            return Conflict();

        var schedule = await _context.Schedules
            .Include(item => item.Lessons)
            .Select(item => new ScheduleDto
            {
                Date = item.Date,
                GroupId = item.GroupId,
                Lessons = item.Lessons
                    .Select(lesson => new LessonDto
                    {
                        Note = lesson.Note,
                        Type = lesson.Type.GetDisplayName(),
                        Audience = lesson.Audience.Title,
                        Discipline = lesson.Discipline.Name,
                        TeacherFullName = $"{lesson.Teacher.Surname} {lesson.Teacher.Name} {lesson.Teacher.Patronymic}",
                        StartLesson = lesson.StartLesson,
                        EndLesson = lesson.EndLesson
                    }).ToList()
            }).Where(item => item.GroupId == student.GroupId).ToListAsync();
        
        return Ok(schedule);
    }
    
    #region Claims
    
    private AuthUserInfo? GetAuthUserInfo()
    {
        string? authHeader = Request.Headers["Authorization"];
        var token = authHeader?.Replace("Bearer ", "") ?? throw new ArgumentNullException($"Bearer token not found");

        _ = _jwtHelper.ReadAccessToken(token, out var claims, out var validTo);
        if (claims is null) return null;

        var userInfo = new AuthUserInfo(
            Id: claims.Claims.FirstOrDefault(a => a.Type == ClaimsIdentity.DefaultIssuer)?.Value ?? throw new ArgumentNullException($"User's id from bearer token not found"),
            Name: claims.Claims.FirstOrDefault(a => a.Type == ClaimsIdentity.DefaultNameClaimType)?.Value ?? throw new ArgumentNullException($"User's name from bearer token not found"),
            Surname: claims.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Surname)?.Value ?? throw new ArgumentNullException($"User's surname from bearer token not found"),
            Role: claims.Claims.FirstOrDefault(a => a.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value ?? throw new ArgumentNullException($"User's role from bearer token not found")
        );

        return userInfo;
    }

    private record AuthUserInfo(string Id, string Name, string Surname, string Role)
    {
        public Guid GuidId => Guid.TryParse(Id, out var guidId) ? guidId : throw new ArgumentNullException();
    }

    #endregion
}