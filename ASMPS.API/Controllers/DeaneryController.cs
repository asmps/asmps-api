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
/// Контроллер для работы деканата
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Deanery")]
public class DeaneryController : Controller
{
    private readonly DatabaseContext _context;
    private readonly JwtHelper _jwtHelper;
    
    /// <summary>
    /// Конструктор класса <see cref="DeaneryController"/>
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="jwtHelper">Расшифровщик данных пользователя из JWT</param>
    public DeaneryController(DatabaseContext context, JwtHelper jwtHelper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _jwtHelper = jwtHelper ?? throw new ArgumentNullException(nameof(jwtHelper));
    }
    
    /// <summary>
    /// Получить всех студентов
    /// </summary>
    [HttpGet("get-students")]
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
    
    /// <summary>
    /// Получить всех студентов нужной группы
    /// </summary>
    [HttpGet("get-students-for-group")]
    public async Task<IActionResult> GetStudentForGroup(string groupName)
    {
        return Ok(await _context.Students
            .Where(item => item.Role == UserRoles.Student)
            .Include(item => item.Group)
            .Where(item => item.Group.Title == groupName)
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
    
    /// <summary>
    /// Получить всех студентов нужной группы
    /// </summary>
    [HttpGet("get-attendance-students")]
    public async Task<IActionResult> GetAttendanceStudent()
    {
        return Ok();
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