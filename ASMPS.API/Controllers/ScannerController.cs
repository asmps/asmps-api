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

    [HttpGet("pass/{userId}/{scanerId}/{method}")]
    public async Task<IActionResult> Pass(Guid userId, Guid scanerId, string method)
    {
        var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == userId);
        if (user is null)
            return Conflict();
        
        var scanner = await _context.ScannerOwnerships.FirstOrDefaultAsync(item => item.Id == scanerId);
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
        
        return Ok("Пропуск разрешен");
    }
}