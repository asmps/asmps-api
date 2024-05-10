namespace ASMPS.Contracts.User;

/// <summary>
/// Модель изменения данных пользователя
/// </summary>
public class UserUpdateDto
{
    /// <summary>
    /// Имя
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Фамилия
    /// </summary>
    public string Surname { get; set; }
    
    /// <summary>
    /// Отчество
    /// </summary>
    public string Patronymic { get; set; }
    
    /// <summary>
    /// Почтовый адрес
    /// </summary>
    public string Email { get; set; }
}