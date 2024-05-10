using System.Security.Claims;
using ASMPS.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ASMPS.Models;
using ASMPS.Contracts.User;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;

namespace ASMPS.API.Controllers;

/// <summary>
/// Контроллер для работы с пользователями
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly JwtHelper _jwtHelper;

    /// <summary>
    /// Конструктор класса <see cref="UsersController"/>
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="jwtHelper"></param>
    public UsersController(DatabaseContext context, JwtHelper jwtHelper)
    {
        _context = context;
        _jwtHelper = jwtHelper;
    }

    /// <summary>
    /// Получить всех пользователей
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Deanery")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Users.AsQueryable()
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
    /// Получить информацию о текущем пользователе
    /// </summary>
    /// <returns></returns>
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentUserData()
    {
        var userInfo = GetAuthUserInfo();
        if (userInfo is null)
            return Unauthorized();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userInfo.GuidId);
        
        if (user is null)
        {
            return NotFound();
        }
        
        var userDto = new UserCurrentDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Patronymic = user.Patronymic,
            Email = user.Email,
            Role = user.Role.GetDisplayName(),
            CreatedDate = user.CreatedDate
        };

        return Ok(userDto);
    }
    
    /// <summary>
    /// Обновить данные о пользователе
    /// </summary>
    /// <param name="userUpdateDto"></param>
    /// <returns></returns>
    [HttpPut("current")]
    public async Task<IActionResult> UpdateCurrentUser(UserUpdateDto userUpdateDto)
    {
        try
        {
            var userInfo = GetAuthUserInfo();
            if (userInfo is null)
                return Unauthorized();

            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userInfo.GuidId);

            // Проверяем, что текущий пользователь существует
            if (currentUser == null)
            {
                return NotFound();
            }

            // Обновляем данные пользователя
            currentUser.Name = userUpdateDto.Name;
            currentUser.Surname = userUpdateDto.Surname;
            currentUser.Patronymic = userUpdateDto.Patronymic;
            currentUser.Email = userUpdateDto.Email;
            
            // Сохраняем изменения в базе данных
            _context.Users.Update(currentUser);
            await _context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            // Обработка ошибок при обновлении данных пользователя
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
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