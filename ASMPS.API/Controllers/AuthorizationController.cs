using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ASMPS.API.Options;
using ASMPS.API.Helpers;
using ASMPS.Contracts.Authorization;

namespace ASMPS.API.Controllers;

/// <summary>
/// Контроллер работы с авторизацией
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthorizationController : Controller
{
    private readonly DatabaseContext _context;
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly JwtHelper _jwtHelper;
    private readonly ILogger<AuthorizationController> _logger;

    /// <summary>
    /// Конструктор класса <see cref="AuthorizationController"/>
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="jwtOptions">Настройки Jwt-токена</param>
    /// <param name="jwtHelper">Класс-помощник работы с Jwt</param>
    /// <param name="logger">Логер</param>
    public AuthorizationController(DatabaseContext context, IOptions<JwtOptions> jwtOptions, 
        JwtHelper jwtHelper, ILogger<AuthorizationController> logger)
    {
        _context = context;
        _jwtOptions = jwtOptions;
        _jwtHelper = jwtHelper;
        _logger = logger;
    }
    
    /// <summary>
    /// Авторизовать пользователя
    /// </summary>
    /// <param name="authDto">Данные по пользователю</param>
    /// <returns>Токены</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Auth([FromBody] AuthorizationDto authDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(item => item.Login == authDto.Login);
        
        if (user is null) return NotFound("Пользователь не найден!");
        if (user.Password != authDto.Password) return Conflict("Неверный пароль!");

        user.RefreshToken = JwtHelper.CreateRefreshToken();
        user.RefreshTokenExpires = DateTime.UtcNow.AddSeconds(_jwtOptions.Value.RefreshTokenLifetime);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        var tokens = new TokensDto
        {
            AccessToken = _jwtHelper.CreateAccessToken(user, _jwtOptions.Value.AccessTokenLifetime),
            RefreshToken = user.RefreshToken,
        };

        return Ok(tokens);
    }

    /// <summary>
    /// Обновляет токены с использованием валидного токена обновления. 
    /// </summary>
    /// <param name="refreshTokensDto">Данные токена обновления</param>
    /// <response code="200">Токены успешно обновлены</response>
    /// <response code="400">Передан некорректный токен обновления</response>
    /// <response code="401">Токен обновления устарел</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokensDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RefreshTokensDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshTokens([FromBody] RefreshTokensDto refreshTokensDto)
    {
        if (string.IsNullOrEmpty(refreshTokensDto.RefreshToken)) return BadRequest($"Поле {nameof(refreshTokensDto.RefreshToken)} не может быть пустым");

        var user = _context.Users.FirstOrDefault(c => c.RefreshToken == refreshTokensDto.RefreshToken);
        if (user is null) return NotFound($"Пользователь с токеном обновления {refreshTokensDto.RefreshToken} не найден");

        if (user.RefreshTokenExpires < DateTime.UtcNow) return Unauthorized($"Токен обновления устарел");

        user.RefreshToken = JwtHelper.CreateRefreshToken();
        user.RefreshTokenExpires = DateTime.UtcNow.AddMinutes(_jwtOptions.Value.RefreshTokenLifetime);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        var tokens = new TokensDto
        {
            AccessToken = _jwtHelper.CreateAccessToken(user, _jwtOptions.Value.AccessTokenLifetime),
            RefreshToken = user.RefreshToken,
        };
        return Ok(tokens);
    }
}