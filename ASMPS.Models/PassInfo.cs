namespace ASMPS.Models;

/// <summary>
/// Модель информации о пропуске
/// </summary>
public class PassInfo
{
    /// <summary>
    /// Идентификатор пропуска  
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Время посещения
    /// </summary>
    public DateTime DateTime { get; set; }
    
    /// <summary>
    /// Идентификатор корпуса
    /// </summary>
    public Guid CampusId { get; set; }
    
    /// <summary>
    /// Тип пропуска
    /// </summary>
    public PassTypes PassType { get; set; }
}