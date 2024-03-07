namespace ASMPS.Models;

/// <summary>
/// Модель кампуса
/// </summary>
public class Campus
{
    /// <summary>
    /// Идентификатор кампуса
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Заголовок/название кампуса
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Описание
    /// </summary>
    public string? Note { get; set; }
    
    /// <summary>
    /// Список аудиторий у кампуса
    /// </summary>
    public ICollection<Audience> Audiences { get; set; } = new List<Audience>();
}