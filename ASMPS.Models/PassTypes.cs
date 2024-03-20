namespace ASMPS.Models;

/// <summary>
/// Тип пропуска
/// </summary>
public enum PassTypes
{
    /// <summary>
    /// Пропуск через RFID-ключ
    /// </summary>
    RFID,
    
    /// <summary>
    /// Пропуск через мобильное приложение
    /// </summary>
    QR
}