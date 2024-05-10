using ASMPS.Models;

namespace ASMPS.Contracts.User;

/// <summary>
/// Модель вывода пользователя
/// </summary>
public class UserDto
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid Id { get; set; } 
    
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
    public DateTime CreatedDate { get; set; }
}