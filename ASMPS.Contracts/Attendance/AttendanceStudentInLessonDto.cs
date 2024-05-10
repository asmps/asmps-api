namespace ASMPS.Contracts.Attendance;

/// <summary>
/// Модель студентов, которые должны посетить занятие
/// </summary>
public class AttendanceStudentInLessonDto
{
    /// <summary>
    /// ФИО студента
    /// </summary>
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// Дата
    /// </summary>
    public DateTime AttendanceDateTime { get; set; }
    
    /// <summary>
    /// Посещаемость
    /// </summary>
    public bool IsAttendance { get; set; } = false;
}