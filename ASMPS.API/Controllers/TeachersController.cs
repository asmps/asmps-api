using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ASMPS.Models;
using ASMPS.Contracts.Teacher;

namespace ASMPS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    /// <param name="lessonId"></param>
    /// <returns></returns>
    [HttpGet("get-students-in-lesson/{lessonId}")]
    public async Task<IActionResult> GetStudentsInLesson(Guid lessonId)
    {
        var attendances = await _context.Attendances
            .Where(item => item.LessonId == lessonId)
            .ToListAsync();
        
        return Ok(attendances);
    }
}