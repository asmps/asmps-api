using ASMPS.Contracts.Lesson;

namespace ASMPS.Contracts.Schedule;

/// <summary>
/// Модель вывода расписания
/// </summary>
public class ScheduleDto
{
    /// <summary>
    /// День занятий
    /// </summary>
    public DateOnly Date { get; set; }
    
    /// <summary>
    /// Идентификатор группы
    /// </summary>
    public Guid GroupId { get; set; }

    /// <summary>
    /// Список занятий для группы
    /// </summary>
    public List<LessonDto> Lessons { get; set; } = new List<LessonDto>();
}