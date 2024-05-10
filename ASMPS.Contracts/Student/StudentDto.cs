namespace ASMPS.Contracts.Student;

/// <summary>
/// Модель представления студента
/// </summary>
public class StudentDto
{   
    /// <summary>
    /// Идентификатор студента
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Номер студенческого билета
    /// </summary>
    public string StudentId { get; set; } = null!;
    
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
    /// Название группы
    /// </summary>
    public string TitleGroup { get; set; } = string.Empty;
}