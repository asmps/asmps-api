using System.Security.Claims;
using ASMPS.API.Helpers;
using ASMPS.Contracts.Attendance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ASMPS.Models;
using ASMPS.Contracts.Student;

namespace ASMPS.API.Controllers;

/// <summary>
/// Контроллер для работы со студентами
/// </summary>
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
    /// Получить всех студентов
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Deanery")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Students
            .Where(item => item.Role == UserRoles.Student)
            .Include(item => item.Group)
            .Select(item => new StudentDto()
            {
                Id = item.Id,
                Name = item.Name,
                Surname = item.Surname,
                Patronymic = item.Patronymic,
                StudentId = item.StudentId,
                Email = item.Email,
                TitleGroup = item.Group.Title
            }).ToListAsync());
    }
    
    [HttpPost("attendance")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetAttendance([FromBody] AttendanceAcceptDto attendanceAcceptDto)
    {
        var studentInfo = GetAuthUserInfo();
        if (studentInfo is null)
            return Unauthorized();
        
        if (!ModelState.IsValid) return BadRequest();
        
        var lesson = await _context.Lessons.FirstOrDefaultAsync(item => item.Id == attendanceAcceptDto.LessonId);
        if (lesson is null)
            return BadRequest("Такого занятия не существует!");
        
        var currentDateOnly = DateOnly.FromDateTime(DateTime.UtcNow);
        
        var schedule = await _context.Schedules.FirstOrDefaultAsync(item => item.Date == currentDateOnly && item.Lessons.Contains(lesson));
        if (schedule is null)
            return Conflict("Подтверждение не возможно! Занятие ещё не началось или уже закончилось!");
        
        var attendance = await _context.Attendances.FirstOrDefaultAsync(item =>
            item.StudentId == studentInfo.GuidId && item.LessonId == lesson.Id);
        if (attendance is not null && attendance.IsAttendance == true)
            return Conflict("Статус посещения уже был подтвержден!");

        DateTime lessonStart = DateTime.UtcNow.Date.Add(lesson.StartLesson.ToTimeSpan());
        DateTime lessonEnd = DateTime.UtcNow.Date.Add(lesson.EndLesson.ToTimeSpan());
        
        // Проверка своевременного подтверждения присутствия на занятии
        if (attendanceAcceptDto.AttendanceDateTime >= lessonStart && attendanceAcceptDto.AttendanceDateTime <= lessonEnd)
        {
            var audience = await _context.Audiences.FirstOrDefaultAsync(item => item.Id == lesson.AudienceId);
            if (audience is null)
                return Conflict();

            var passInfo = await _context.PassInfos
                .GroupBy(item => item.UserId)
                .Select(item => item
                    .OrderByDescending(c => c.DateTime)
                    .First())
                .FirstOrDefaultAsync();
            if (passInfo is null)
                return Conflict("Невозможно подтвердить занятие, ненаходясь в корпусе!");
            
            if (passInfo.CampusId != audience.CampusId)
                return Conflict("Невозможно подтвердить занятие, которое находится в другом корпусе!");

            if (attendance != null)
            {
                attendance.IsAttendance = true;
                attendance.AttendanceDateTime = DateTime.UtcNow;

                _context.Attendances.Update(attendance);
            }
            else
            {
                attendance = new Attendance()
                {
                    Id = Guid.NewGuid(),
                    IsAttendance = true,
                    StudentId = studentInfo.GuidId,
                    AttendanceDateTime = DateTime.UtcNow,
                    LessonId = lesson.Id
                };
                
                await _context.Attendances.AddAsync(attendance);
            }

            await _context.SaveChangesAsync();
        
            return Ok("Посещение подтверждено!");
        }
        return Conflict("Подтверждение не возможно! Занятие ещё не началось или уже закончилось!");
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