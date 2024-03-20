using ASMPS.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ASMPS.Models;
using ASMPS.Contracts.User;
using Microsoft.IdentityModel.Tokens;

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
    /// Получить пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор</param>
    /// <returns>Модель представления пользователя</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == id);
        if (user is null) return NotFound();

        return Ok(new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Patronymic = user.Patronymic,
            Email = user.Email,
            CreatedDate = user.CreatedDate,
            Role = user.Role
        });
    }

    /// <summary>
    /// Добавить пользователя
    /// </summary>
    /// <param name="userAddDto">Данные по пользователю</param>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] UserAddDto userAddDto)
    {
        if (!ModelState.IsValid) return BadRequest();

        try
        {
            var user = UserFactoryHelper.CreateUser(userAddDto.Role);

            if (user.GetType() == typeof(Student))
            {
                var group = await _context.GroupStudents.FirstOrDefaultAsync(x => x.Title == userAddDto.TitleGroup);
                if (group is null) 
                    return NotFound($"Данной группы '{userAddDto.TitleGroup}' не существует!");
                
                user = new Student
                {
                    Group = group,
                    GroupId = group.Id,
                    StudentId = string.Concat(new Random().Next(10000001, 99999999).ToString())
                };
            }
            else if (user.GetType() == typeof(Teacher))
            {
                user = new Teacher
                {
                    Position = userAddDto.Position
                };
            }
            
            user.Id = Guid.NewGuid();
            user.Login = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            user.Password = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            user.Name = userAddDto.Name;
            user.Surname = userAddDto.Surname;
            user.Patronymic = userAddDto.Patronymic;
            user.Email = userAddDto.Email;
            user.CreatedDate = DateTime.UtcNow;
            user.Role = userAddDto.Role;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok(user.Id);
        }
        catch (Exception ex)
        {
            return Conflict(ex.ToString());
        }
    }
}