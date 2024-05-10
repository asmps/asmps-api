using Newtonsoft.Json;

namespace ASMPS.Contracts.Lesson;

/// <summary>
/// Модель представления подробной информации о рассписании
/// </summary>
public class LessonInScheduleInfoDto
{
    /// <summary>
    /// Идентификатор занятия
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Список групп
    /// </summary>
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public List<string>? Groups { get; set; }
    
    /// <summary>
    /// Название дисциплины
    /// </summary>
    public string Discipline { get; set; } = string.Empty;

    /// <summary>
    /// Полное имя учителя
    /// </summary>
    public string Teacher { get; set; } = string.Empty;

    /// <summary>
    /// Аудитория
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Тип занятия
    /// </summary>
    public string Type { get; set; } = string.Empty;
}