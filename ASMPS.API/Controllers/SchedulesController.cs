using System.Security.Claims;
using ASMPS.API.Helpers;
using ASMPS.API.Services;
using ASMPS.Contracts.Lesson;
using ASMPS.Contracts.Schedule;
using ASMPS.Contracts.Teacher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace ASMPS.API.Controllers;

/// <summary>
/// Контроллер для работы с расписанием
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SchedulesController : Controller
{
    private readonly DatabaseContext _context;
    private readonly JwtHelper _jwtHelper;
    private readonly HttpClient _httpClient;
    private readonly ScheduleConverter _scheduleConverter;

    /// <summary>
    /// Конструктор класса <see cref="SchedulesController"/>
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="jwtHelper">Расшифровщик данных пользователя из JWT</param>
    /// <param name="httpClient">Клиент для общения с АГТУ</param>
    public SchedulesController(DatabaseContext context, JwtHelper jwtHelper, HttpClient httpClient, ScheduleConverter scheduleConverter)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _jwtHelper = jwtHelper ?? throw new ArgumentNullException(nameof(jwtHelper));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _scheduleConverter = scheduleConverter ?? throw new ArgumentNullException(nameof(scheduleConverter));
    }
    
    /// <summary>
    /// Получить расписание для студента
    /// </summary>
    /// <returns>Расписание</returns>
    [HttpGet("schedule-for-student")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetScheduleForStudent()
    {
        var studentInfo = GetAuthUserInfo();
        if (studentInfo is null)
            return Unauthorized();

        var student = await _context.Students.FirstOrDefaultAsync(item => item.Id == studentInfo.GuidId);
        if (student is null)
            return Conflict("Ошибка! Пользователь не является студентом!");
        
        var group = await _context.GroupStudents.FirstOrDefaultAsync(item => item.Id == student.GroupId);
        if (group is null)
            return Conflict("Ошибка! У студента отсутствует группа!");

        var schedule = await _context.Schedules
            .Include(item => item.Lessons)
            .ThenInclude(item => item.Teacher)
            .Include(item => item.Lessons)
            .ThenInclude(item => item.Discipline)
            .Include(item => item.Lessons)
            .ThenInclude(item => item.Audience)
            .ThenInclude(item => item.Campus)
            .FirstOrDefaultAsync(item => item.GroupId == group.Id);
        if (schedule is null)
            return Conflict("Ошибка! У студента отсутствует рассписание!");

        var scheduleParser = new ScheduleParser().ParseSchedule(schedule);
        return Ok(scheduleParser);
        /*//// Получаем рассписание для студента, в соответствии с его группой, от АГТУ
        // var response = await _httpClient.GetAsync($"https://apitable.astu.org/search/get?q={group.Title}&t=group");

        //// Получаем содержимое ответа как строку JSON
        // var jsonString = await response.Content.ReadAsStringAsync();
        // var scheduleParser = new ScheduleParser().ParseScheduleForStudent(jsonString);

        /#1#/ todo: Здесь мы достаем рассписание ИЗ СВОЕЙ БАЗЫ ДАННЫХ
        // todo: Нужно как-то мержить. Ну, или оставить только от АГТУ
        // todo: Тогда их нужно будет как-то парсить и добавлять себе в базу
        var sc = await _context.Schedules
            .Include(item => item.Group)
            .Include(item => item.Lessons)
            .ThenInclude(lesson => lesson.Teacher)
            .Include(item => item.Lessons)
            .ThenInclude(lesson => lesson.Audience)
            .ThenInclude(audience => audience.Campus)
            .Include(item => item.Lessons)
            .ThenInclude(lesson => lesson.Discipline)
            .FirstOrDefaultAsync();
        var scheduleDbParser = new ScheduleParser().ParseSchedule(sc);

        var schedule = await _context.Schedules
            .Include(s => s.Lessons)
            .ThenInclude(l => l.Teacher)
            .ToListAsync();#1#

        /*try
        {
            var schedule = await _scheduleConverter.ConvertToScheduleForStudent(scheduleParser);
            Console.WriteLine(schedule);
            var scPar = new ScheduleParser().ParseSchedule(schedule);
            return Ok(scPar);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }#1#*/
    }
    
    /// <summary>
    /// Получить расписание для учителя
    /// </summary>
    /// <returns>Расписание</returns>
    [HttpGet("schedule-for-teacher")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetScheduleForTeacher()
    {
        var teacherInfo = GetAuthUserInfo();
        if (teacherInfo is null)
            return Unauthorized();

        var teacher = await _context.Teachers.FirstOrDefaultAsync(item => item.Id == teacherInfo.GuidId);
        if (teacher is null)
            return Conflict("Ошибка! Пользователь не является учителем!");

        var lessons = await _context.Lessons.Where(item => item.TeacherId == teacher.Id)
            .Include(item => item.Audience)
            .ThenInclude(item => item.Campus)
            .Include(item => item.Discipline).Include(lesson => lesson.Teacher)
            .ToListAsync();
        if (lessons is null)
            return NotFound("Ошибка! У учителя отсутствуют занятия!");

        var schedule = new ScheduleDto
        {
            Title = teacher.Surname + " " + teacher.Name[0] + "." + teacher.Patronymic[0] + ".",
        };
        
        foreach (var lesson in lessons)
        {
            var groups = await _context.GroupStudents.Where(item => item.Id == lesson.GroupId)
                .Select(item => item.Title)
                .ToListAsync();
            
            schedule.Lessons.Add(new LessonInScheduleDto
            {
                DayId = lesson.DayId,
                LessonOrderId = lesson.LessonOrderId,
                LessonInScheduleInfoDto = new LessonInScheduleInfoDto
                {
                    Audience = lesson.Audience.Campus.Number + "." + lesson.Audience.Number,
                    Discipline = lesson.Discipline.Name,
                    Teacher = lesson.Teacher.Surname + " " + lesson.Teacher.Name[0] + "." + lesson.Teacher.Patronymic[0] + ".",
                    Id = lesson.Id,
                    Type = lesson.Type.GetDisplayName(),
                    Groups = groups
                }
            });
        }
        
        return Ok(schedule);
        /*var teacherDto = new TeacherDto
        {
            Name = teacher.Name,
            Surname = teacher.Surname,
            Patronymic = teacher.Patronymic,
            Position = teacher.Position
        };

        /*var schedule = await _context.Schedules
            .Include(item => item.Lessons)
            .ThenInclude(l => l.Teacher)
            .Include(item => item.Lessons)
            .ThenInclude(l => l.Group)
            .Include(item => item.Lessons)
            .ThenInclude(l => l.Audience)
            .Include(item => item.Lessons)
            .ThenInclude(l => l.Discipline)
            .FirstOrDefaultAsync();#1#

        /*if (schedule is null)
        {
            return NotFound();
        }

        schedule.Lessons = schedule.Lessons
            .Where(l => l.Teacher.Id == teacher.Id)
            .ToList();#1#

        /*var scheduleParser = new ScheduleParser().ParseSchedule(schedule);#1#

        try
        {
            // Получаем рассписание для учителя, от АГТУ
            // todo: после нужно из бд тянуть
            var response = await _httpClient.GetAsync($"https://apitable.astu.org/search/get?q={teacher.Position} {teacher.Surname} {teacher.Name[0]}.{teacher.Patronymic![0]}.&t=teacher");

            // Получаем содержимое ответа как строку JSON
            var jsonString = await response.Content.ReadAsStringAsync();
            var scheduleDtoParser = new ScheduleParser().ParseScheduleForTeacher(jsonString, teacherDto);

            var schedule = await _scheduleConverter.ConvertToScheduleForTeacher(scheduleDtoParser, teacherInfo.GuidId);
            Console.WriteLine(schedule);
            var scPar = new ScheduleParser().ParseSchedule(schedule);
            return Ok(scPar);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }*/
    }
    
    /// <summary>
    /// Поиск расписания
    /// </summary>
    /// <returns>Поисковое значение</returns>
    [HttpGet("schedule/find")]
    public async Task<IActionResult> FindSchedule(string query)
    {
        // Получаем информацию по строке, от АГТУ
        var response = await _httpClient.GetAsync($"https://apitable.astu.org/search/find?q={query}");

        // Получаем содержимое ответа как строку JSON
        var jsonString = await response.Content.ReadAsStringAsync();
        var answer = new QueryParser().ParseQueryToList(jsonString);
        
        return Ok(answer);
    }
    
    /// <summary>
    /// Выбрать позицию из поисковика и получить рассписание
    /// </summary>
    /// <returns>Поисковое значение</returns>
    [HttpGet("schedule/select")]
    public async Task<IActionResult> SelectSchedule(string name, string type)
    {
        // Получаем информацию по строке, от АГТУ
        var response = await _httpClient.GetAsync($"https://apitable.astu.org/search/get?q={name}&t={type}");

        // Получаем содержимое ответа как строку JSON
        var jsonString = await response.Content.ReadAsStringAsync();
        var answer = new ScheduleParser().ParseSchedule(jsonString);
        
        return Ok(answer);
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