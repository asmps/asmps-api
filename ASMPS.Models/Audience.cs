namespace ASMPS.Models;

/// <summary>
/// Модель аудитории
/// </summary>
public class Audience
{
    /// <summary>
    /// Идентификатор аудитории
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Заголовок/название аудитории
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Описание
    /// </summary>
    public string? Note { get; set; }
    
    /// <summary>
    /// Идентификатор кампуса
    /// </summary>
    public Guid CampusId { get; set; }
    
    /// <summary>
    /// Кампус
    /// </summary>
    public virtual Campus Campus { get; set; } = null!;
}