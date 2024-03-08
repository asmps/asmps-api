namespace ASMPS.Contracts.Teacher;

/// <summary>
/// Модель вывода учителя
/// </summary>
public class TeacherDto
{
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
    /// Должность учителя
    /// </summary>
    public string Position { get; set; } = string.Empty;
}