namespace ASMPS.Models;

/// <summary>
/// Модель скаренера
/// </summary>
public class ScannerOwnership
{
    /// <summary>
    /// Идентификатор сканера
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Идентификатор корпуса
    /// </summary>
    public Guid CampusId { get; set; }
}