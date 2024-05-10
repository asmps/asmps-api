namespace ASMPS.Contracts.PassInfo;

/// <summary>
/// Модель получения данных о пропуске
/// </summary>
public class PassInfoDto
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Тип пропуска
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Дата пропуска
    /// </summary>
    public DateTime DateTime { get; set; } = DateTime.UtcNow;
}