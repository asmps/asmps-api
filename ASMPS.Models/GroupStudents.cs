namespace ASMPS.Models;

/// <summary>
/// Модель группы студентов
/// </summary>
public class GroupStudents
{
    /// <summary>
    /// Идентификатор группы
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Название группы
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Студенты группы
    /// </summary>
    public ICollection<Student> Students { get; set; } = new List<Student>();

    /// <summary>
    /// Список расписаний
    /// </summary>
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    
    /// <summary>
    /// Идентификатор деканата
    /// </summary>
    public Guid DeaneryId { get; set; }
    
    /// <summary>
    /// Деканат
    /// </summary>
    public virtual Deanery Deanery { get; set; } = null!;
}
