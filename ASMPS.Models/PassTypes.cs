using System.ComponentModel.DataAnnotations;

namespace ASMPS.Models;

/// <summary>
/// Тип пропуска
/// </summary>
public enum PassTypes
{
    /// <summary>
    /// Пропуск через RFID-ключ
    /// </summary>
    [Display(Name = "RFID")]
    RFID,
    
    /// <summary>
    /// Пропуск через мобильное приложение
    /// </summary>
    [Display(Name = "QR")]
    QR
}