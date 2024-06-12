using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASMPS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Deanery")]
public class StatisticsController : Controller
{
    private readonly DatabaseContext _context;
    
    /// <summary>
    /// Конструктор класса <see cref="StatisticsController"/>
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    public StatisticsController(DatabaseContext context)
    {
        _context = context;
    }

    [HttpGet("TotalStudentsCount")]
    public async Task<int> GetTotalStudentsCount()
    {
        return await _context.PassInfos.Where(p => p.DateTime == DateTime.UtcNow).CountAsync();
    }

    [HttpGet("AttendanceRateByStudent/{studentId}")]
    public async Task<double> GetAttendanceRate(Guid studentId)
    {
        var totalLessons = await _context.Lessons
            .Include(l => l.Group)
            .ThenInclude(g => g.Students)
            .Where(l => l.Group.Students.Any(s => s.Id == studentId))
            .CountAsync();
        var totalAttendances = await _context.Attendances
            .Where(a => a.StudentId == studentId && a.IsAttendance)
            .CountAsync();
        if (totalLessons == 0)
        {
            return 0; // Возвращаем 0 в случае отсутствия занятий
        }

        return ((double)totalAttendances / totalLessons) * 100; // Возвращаем процентное соотношение
    }
    
    [HttpGet("AttendanceRates")]
    public async Task<Dictionary<string, double>> GetStudentsAttendanceRates()
    {
        var studentsAttendanceRates = new Dictionary<string, double>();

        var students = await _context.Students.ToListAsync();
        foreach (var student in students)
        {
            var attendanceRate = await GetAttendanceRate(student.Id);
            studentsAttendanceRates.Add($"{student.StudentId} {student.Name} {student.Surname} {student.Patronymic}", attendanceRate);
        }

        return studentsAttendanceRates;
    }

    [HttpGet("AttendanceRateByGroup")]
    public async Task<Dictionary<string, double>> GetAttendanceRateByGroup()
    {
        var attendanceRatesByGroup = new Dictionary<string, double>();

        var groups = await _context.GroupStudents.ToListAsync();
        foreach (var group in groups)
        {
            int totalAttendances = await _context.Attendances
                .Where(a => _context.Lessons.Any(l => l.Id == a.LessonId && l.GroupId == group.Id) && a.IsAttendance == true)
                .CountAsync();
            int totalStudentsInGroup = await _context.Students
                .Where(s => s.GroupId == group.Id)
                .CountAsync();
            double attendanceRate = totalStudentsInGroup == 0 ? 0 : (double)totalAttendances / totalStudentsInGroup * 100;
            attendanceRatesByGroup.Add(group.Title, attendanceRate);
        }

        return attendanceRatesByGroup;
    }


    [HttpGet("AudienceOccupancy")]
    public async Task<Dictionary<string, int>> GetAudienceOccupancy()
    {
        var audienceOccupancy = new Dictionary<string, int>();
        var audiences = await _context.Audiences
            .Include(item => item.Campus)
            .ToListAsync();
        foreach (var audience in audiences)
        {
            var lessonsCount = await _context.Lessons
                .Where(l => l.AudienceId == audience.Id)
                .CountAsync();
            if (audienceOccupancy.ContainsKey($"К-{audience.Campus.Number} А-{audience.Number}"))
                audienceOccupancy[$"К-{audience.Campus.Number} А-{audience.Number}"] += lessonsCount;
            else
                audienceOccupancy.Add($"К-{audience.Campus.Number} А-{audience.Number}", lessonsCount);
        }
        
        return audienceOccupancy.Where(item => item.Value > 0)
            .ToDictionary(item => item.Key, item => item.Value);
    }

    [HttpGet("TeacherAttendanceRate")]
    public async Task<Dictionary<string, double>> GetTeacherAttendanceRate()
    {
        var attendanceRatesByTeacher = new Dictionary<string, double>();

        var teachers = await _context.Teachers.ToListAsync();
        foreach (var teacher in teachers)
        {
            int totalAttendances = await _context.Attendances
                .Where(a => _context.Lessons.Any(l => l.Id == a.LessonId && l.TeacherId == teacher.Id))
                .CountAsync();
            int totalLessons = await _context.Lessons
                .Where(l => l.TeacherId == teacher.Id)
                .CountAsync();
            var attendanceRate = totalLessons == 0 ? 0 : (double)totalAttendances / totalLessons * 100;
            attendanceRatesByTeacher.Add(teacher.Name, attendanceRate);
        }

        return attendanceRatesByTeacher;
    }

}