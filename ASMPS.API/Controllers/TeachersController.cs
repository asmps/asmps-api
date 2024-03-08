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
    /// Добавить учителя
    /// </summary>
    /// <param name="teacherAddDto">Данные по учителю</param>
    [Authorize(Roles = "Deanery")]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] TeacherAddDto teacherAddDto)
    {
        if (!ModelState.IsValid) return BadRequest();

        var item = new Teacher()
        {
            Id = Guid.NewGuid(),
            Login = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8),
            Password = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8),
            Name = teacherAddDto.Name,
            Surname = teacherAddDto.Surname,
            Patronymic = teacherAddDto.Patronymic,
            Email = teacherAddDto.Email == string.Empty ? null : teacherAddDto.Email,
            CreatedDate = DateTime.UtcNow,
            Role = UserRoles.Teacher,
            Position = teacherAddDto.Position
        };

        await _context.Users.AddAsync(item);
        await _context.SaveChangesAsync();
        return Ok(item.Id);
    }
}