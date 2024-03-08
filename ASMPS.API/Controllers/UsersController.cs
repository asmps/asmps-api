using ASMPS.Contracts.Student;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ASMPS.Models;
using ASMPS.Contracts.User;

namespace ASMPS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Deanery")]
public class UsersController : ControllerBase
{
    private readonly DatabaseContext _context;
    
    /// <summary>
    /// Конструктор класса <see cref="UsersController"/>
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    public UsersController(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить всех пользователей
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Users.Where(item => item.Role != UserRoles.Deanery)
            .Select(item => new UserDto
            {
                Id = item.Id,
                Name = item.Name,
                Surname = item.Surname,
                Patronymic = item.Patronymic,
                Email = item.Email,
                CreatedDate = item.CreatedDate,
                Role = item.Role
            }).ToListAsync());
    }

    /// <summary>
    /// Добавить пользователя
    /// </summary>
    /// <param name="userAddDto">Данные по пользователю</param>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] UserAddDto userAddDto)
    {
        if (!ModelState.IsValid) return BadRequest();
        
        var item = new User
        {
            Id = Guid.NewGuid(),
            Login = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8),
            Password = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8),
            Name = userAddDto.Name,
            Surname = userAddDto.Surname,
            Patronymic = userAddDto.Patronymic,
            Email = userAddDto.Email,
            CreatedDate = DateTime.UtcNow,
            Role = UserRoles.Student
        };

        await _context.Users.AddAsync(item);
        await _context.SaveChangesAsync();
        return Ok(item.Id);
    }
}