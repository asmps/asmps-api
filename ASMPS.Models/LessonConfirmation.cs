namespace ASMPS.Models;

/// <summary>
/// Модель подтверждение урока
/// </summary>
public class LessonConfirmation
{
    /// <summary>
    /// Идентификтор урока
    /// </summary>
    public Guid LessonId { get; set; }
    
    /// <summary>
    /// Статус подтверждения
    /// </summary>
    public bool TeacherSignature { get; set; }
}