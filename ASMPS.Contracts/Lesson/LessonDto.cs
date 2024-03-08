namespace ASMPS.Contracts.Lesson;

/// <summary>
/// Модель вывода занятия
/// </summary>
public class LessonDto
{
    /// <summary>
    /// Название дисциплины
    /// </summary>
    public string Discipline { get; set; } = string.Empty;

    /// <summary>
    /// Полное имя учителя
    /// </summary>
    public string TeacherFullName { get; set; } = string.Empty;

    /// <summary>
    /// Аудитория
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Тип занятия
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
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