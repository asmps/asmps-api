namespace ASMPS.Models;

/// <summary>
/// Модель расписания
/// </summary>
public class Schedule
{
    /// <summary>
    /// Идентификатор расписания
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// День занятий
    /// </summary>
    public DateOnly Date { get; set; }
    
    /// <summary>
    /// Идентификатор группы
    /// </summary>
    public Guid GroupId { get; set; }
    
    /// <summary>
    /// Группа
    /// </summary>
    public virtual GroupStudents Group { get; set; } = null!;

    /// <summary>
    /// Список занятий для группы
    /// </summary>
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}