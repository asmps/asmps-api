using ASMPS.Contracts.GroupStudent;

namespace ASMPS.Contracts.Attendance;

/// <summary>
/// Модель подтверждения списка студентов, которые посещают занятие
/// </summary>
public class AttendanceListConfirmedDto
{
    /// <summary>
    /// Идентификатор занятия
    /// </summary>
    public Guid LessonId { get; set; }
    
    /// <summary>
    /// Подпись учителя
    /// </summary>
    public bool TeacherSignature { get; set; } = false;
}