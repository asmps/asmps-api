using ASMPS.Contracts.Attendance;
using ASMPS.Contracts.GroupStudent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ASMPS.Models;
using ASMPS.Contracts.Teacher;

namespace ASMPS.API.Controllers;

/// <summary>
/// Контроллер для работы с учителями
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Teacher")]
public class TeachersController : Controller
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// Конструктор класса <see cref="TeachersController"/>
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    public TeachersController(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить список студентов, которые должны посетить занятие
    /// </summary>
    /// <param name="lessonId">Идентификатор занятия</param>
    /// <returns></returns>
    [HttpGet("get-students-in-lesson/{lessonId}")]
    public async Task<IActionResult> GetStudentsInLesson(Guid lessonId)
    {
        // Достаем все посещения для занятия
        var attendances = await _context.Attendances
            .Where(item => item.LessonId == lessonId)
            .ToListAsync();

        // Получаем идентификаторы студентов, которые посетили занятие
        var attendedStudentIds = attendances.Select(a => a.StudentId).ToList();

        // Достаем всех студентов, которая посещает занятие
        var studentsToAttend = await _context.Students
            .Where(student => attendedStudentIds.Contains(student.Id))
            .ToListAsync();

        // Получаем все группы из базы данных
        var groups = await _context.GroupStudents.ToListAsync();
        
        // Создаем список, который будет содержать данные о группах со студентами на занятии
        var groupStudentDtos = new List<GroupStudentDto>();
        
        // Словарь для хранения студентов, сгруппированных по группам
        Dictionary<GroupStudents, List<Student>> groupedStudents = new Dictionary<GroupStudents, List<Student>>();

        // Пробегаемся по каждой группе
        foreach (var group in groups)
        {
            // Получаем студентов, принадлежащих к текущей группе и присутствующих на занятии
            var studentsInGroup = studentsToAttend
                .Where(student => student.GroupId == group.Id)
                .ToList();

            // Создаем объект GroupStudentDto для текущей группы
            var groupDto = new GroupStudentDto
            {
                Name = group.Title,
                StudentInLesson = new List<AttendanceStudentInLessonDto>()
            };

            // Добавляем студентов в список StudentInLesson текущего объекта GroupStudentDto
            foreach (var student in studentsInGroup)
            {
                // Ищем соответствующее посещение для текущего студента
                var attendance = attendances.FirstOrDefault(a => a.StudentId == student.Id);

                if (attendance != null)
                {
                    var dto = new AttendanceStudentInLessonDto
                    {
                        FullName = $"{student.Surname} {student.Name} {student.Patronymic}",
                        AttendanceDateTime = attendance.AttendanceDateTime,
                        IsAttendance = attendance.IsAttendance
                    };

                    // Добавляем объект в список StudentInLesson текущего объекта GroupStudentDto
                    groupDto.StudentInLesson.Add(dto);
                }
            }

            // Добавляем объект GroupStudentDto в список groupStudentDtos
            groupStudentDtos.Add(groupDto);
        }

        return Ok(groupStudentDtos);
    }
    
    /// <summary>
    /// Подтверждение списка студентов, которые посетили занятие
    /// </summary>
    /// <param name="attendanceListConfirmedDto"></param>
    /// <returns></returns>
    [HttpPost("confirm-students-in-lesson")]
    public async Task<IActionResult> ConfirmStudentsInLesson([FromBody] AttendanceListConfirmedDto attendanceListConfirmedDto)
    {
        if (!ModelState.IsValid) 
            return BadRequest();
        
        try
        {
            // todo: для тех, кто не смог подтвердить занятие через приложение
            // todo: добавить возможность изменения статуса посещения студента
            // todo: получать список студентов и корректировать его
            // todo: а после пробегаться по нему и сохранять данные
            
            var lessonConfirmation = new LessonConfirmation
            {
                LessonId = attendanceListConfirmedDto.LessonId,
                TeacherSignature = attendanceListConfirmedDto.TeacherSignature
            };

            await _context.LessonConfirmations.AddAsync(lessonConfirmation);
            await _context.SaveChangesAsync();
            
            return Ok("Список студентов успешно подтвержден!");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Не удалось подтвердить список студентов.");
        }
    }
}