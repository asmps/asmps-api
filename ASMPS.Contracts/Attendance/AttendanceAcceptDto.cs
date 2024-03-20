namespace ASMPS.Contracts.Attendance;

/// <summary>
/// Модель подтверждения посещаемости студентом
/// </summary>
public class AttendanceAcceptDto
{
    /// <summary>
    /// Идентификатор студента
    /// </summary>
    public Guid StudentId { get; set; }
    
    /// <summary>
    /// Дата подтверждения
    /// </summary>
    public DateTime AttendanceDateTime { get; set; }
    
    /// <summary>
    /// Идентификатор занятия
    /// </summary>
    public Guid LessonId { get; set; }
    
    /// <summary>
    /// Посещаемость
    /// </summary>
    public bool IsAttendance { get; set; } = false;
}