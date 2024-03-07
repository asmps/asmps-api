namespace ASMPS.Models;

/// <summary>
/// Модель пользователя системы
/// </summary>
public class User
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid Id { get; set; } 
    
    /// <summary>
    /// Логин
    /// </summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Пароль
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// Имя
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Фамилия
    /// </summary>
    public string Surname { get; set; } = string.Empty;

    /// <summary>
    /// Отчество
    /// </summary>
    public string? Patronymic { get; set; }

    /// <summary>
    /// Электронный адрес
    /// </summary>
    public string? Email {  get; set; }

    /// <summary>
    /// Роль пользователя
    /// </summary>
    public UserRoles Role { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Токен обновления
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Дата истечения токена обновления
    /// </summary>
    public DateTime? RefreshTokenExpires { get; set; }
}