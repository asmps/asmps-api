namespace ASMPS.Models;

/// <summary>
/// Модель занятия
/// </summary>
public class Lesson
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Идентификатор дисциплины
    /// </summary>
    public Guid DisciplineId { get; set; }
    
    /// <summary>
    /// Дисциплина
    /// </summary>
    public virtual Discipline Discipline { get; set; } = null!;
    
    /// <summary>
    /// Идентификатор учителя
    /// </summary>
    public Guid TeacherId { get; set; }
    
    /// <summary>
    /// Учитель
    /// </summary>
    public virtual Teacher Teacher { get; set; } = null!;
    
    /// <summary>
    /// Идентификатор группы
    /// </summary>
    public Guid GroupId { get; set; }
    
    /// <summary>
    /// Группа
    /// </summary>
    public virtual GroupStudents Group { get; set; } = null!;
    
    /// <summary>
    /// Идентификатор расписания
    /// </summary>
    public Guid ScheduleId { get; set; }
    
    /// <summary>
    /// Расписание
    /// </summary>
    public virtual Schedule Schedule { get; set; } = null!;
    
    /// <summary>
    /// Идентификатор аудитории
    /// </summary>
    public Guid AudienceId { get; set; }
    
    /// <summary>
    /// Аудитория
    /// </summary>
    public virtual Audience Audience { get; set; } = null!;
    
    /// <summary>
    /// Тип занятия
    /// </summary>
    public LessonTypes Type { get; set; }
    
    /// <summary>
    /// Начало занятия
    /// </summary>
    public TimeOnly StartLesson { get; set; }
    
    /// <summary>
    /// Конец занятия
    /// </summary>
    public TimeOnly EndLesson { get; set; }
    
    /// <summary>
    /// Описание занятия
    /// </summary>
    public string? Note { get; set; }
}