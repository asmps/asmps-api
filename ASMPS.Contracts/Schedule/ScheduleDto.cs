using ASMPS.Contracts.Lesson;

namespace ASMPS.Contracts.Schedule;

/// <summary>
/// Модель представления расписания
/// </summary>
public class ScheduleDto
{
    /// <summary>
    /// Для кого рассписание
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Список занятий
    /// </summary>
    public List<LessonInScheduleDto> Lessons { get; set; } = new List<LessonInScheduleDto>();
}