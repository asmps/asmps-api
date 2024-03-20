namespace ASMPS.Models;

/// <summary>
/// Модель посещаемости
/// </summary>
public class Attendance
{
    /// <summary>
    /// Идентификатор студента
    /// </summary>
    public Guid StudentId { get; set; }
    
    /// <summary>
    /// Дата
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