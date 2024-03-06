namespace ASMPS.Models;

/// <summary>
/// Модель дисциплин
/// </summary>
public class Discipline
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Название дисциплины
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Описание дисциплины
    /// </summary>
    public string? Note { get; set; }
    
    /// <summary>
    /// Список учителей, которые ведут это занятие
    /// </summary>
    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}
