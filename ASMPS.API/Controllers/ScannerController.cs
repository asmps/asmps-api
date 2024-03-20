using System.Security.Claims;
using ASMPS.API.Helpers;
using ASMPS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASMPS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScannerController : Controller
{
    private readonly DatabaseContext _context;
    private readonly JwtHelper _jwtHelper;
    private readonly ILogger<ScannerController> _logger;

    /// <summary>
    /// Конструктор класса <see cref="ScannerController"/>
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="jwtHelper">Расшифровщик данных пользователя из JWT</param>
    /// <param name="logger">Логер</param>
    public ScannerController(DatabaseContext context, JwtHelper jwtHelper, ILogger<ScannerController> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _jwtHelper = jwtHelper ?? throw new ArgumentNullException(nameof(jwtHelper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("pass/{id}/{method}")]
    public async Task<IActionResult> Pass(Guid id, string method)
    {
        var userInfo = GetAuthUserInfo();
        if (userInfo is null)
            return Unauthorized();

        var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == userInfo.GuidId);
        if (user is null)
            return Conflict();
        
        var scanner = await _context.ScannerOwnerships.FirstOrDefaultAsync(item => item.Id == id);
        if (scanner is null)
            return Conflict();

        var campus = await _context.Campuses.FirstOrDefaultAsync(item => item.Id == scanner.CampusId);
        if (campus is null)
            return Conflict();
        
        var codeScanner = new PassInfo
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            DateTime = DateTime.UtcNow,
            CampusId = scanner.CampusId,
            PassType = (PassTypes) Enum.Parse(typeof(PassTypes), method)
        };
        
        _logger.LogInformation($"{DateTime.UtcNow} | Пользователь {user.Id} прошел в корпус {campus.Number} через {codeScanner.PassType}");
        
        await _context.PassInfos.AddAsync(codeScanner);
        await _context.SaveChangesAsync();
        
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