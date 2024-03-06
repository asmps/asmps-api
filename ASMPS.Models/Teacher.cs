namespace ASMPS.Models;

/// <summary>
/// Модель учителя
/// </summary>
public class Teacher : Person
{
    /// <summary>
    /// Должность учителя
    /// </summary>
    public string Position { get; set; } = string.Empty;
    
    /// <summary>
    /// Список дисциплин, которые ведет учитель
    /// </summary>
    public ICollection<Discipline> Disciplines { get; set; } = new List<Discipline>();
}