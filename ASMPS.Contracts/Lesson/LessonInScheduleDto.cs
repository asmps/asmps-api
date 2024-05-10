namespace ASMPS.Contracts.Lesson;

/// <summary>
/// Модель представления занятия в расписании
/// </summary>
public class LessonInScheduleDto
{
    /// <summary>
    /// Информация о занятии
    /// </summary>
    public LessonInScheduleInfoDto LessonInScheduleInfoDto { get; set; } = null!;

    /// <summary>
    /// День занятия
    /// </summary>
    public int DayId { get; set; }
    
    /// <summary>
    /// Позиция занятия
    /// </summary>
    public int LessonOrderId { get; set; }
    
    /// <summary>
    /// Описание занятия
    /// </summary>
    public string? Note { get; set; }
}