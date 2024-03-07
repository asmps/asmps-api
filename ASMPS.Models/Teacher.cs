namespace ASMPS.Models;

/// <summary>
/// Модель учителя
/// </summary>
public class Teacher : User
{
    /// <summary>
    /// Должность учителя
    /// </summary>
    public string Position { get; set; } = string.Empty;
    
    /// <summary>
    /// Список дисциплин, которые ведет учитель
    /// </summary>
    public ICollection<Discipline> Disciplines { get; set; } = new List<Discipline>();
    
    /// <summary>
    /// Список занятий, которые ввел учитель
    /// </summary>
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}