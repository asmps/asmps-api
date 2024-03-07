using System.ComponentModel.DataAnnotations;

namespace ASMPS.Contracts.Authorization;

/// <summary>
/// Модель авторизации
/// </summary>
public class AuthorizationDto
{
    /// <summary>
    /// Логин пользователя
    /// </summary>
    [Required(ErrorMessage = "Введите логин!")]
    public string? Login { get; set; }

    /// <summary>
    /// Пароль пользователя
    /// </summary>
    [Required(ErrorMessage = "Введите пароль!")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}