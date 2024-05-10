using ASMPS.Contracts.Attendance;

namespace ASMPS.Contracts.GroupStudent;

/// <summary>
/// Модель группы со студентами
/// </summary>
public class GroupStudentDto
{
    /// <summary>
    /// Название группы
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Список студентов данной группы
    /// </summary>
    public List<AttendanceStudentInLessonDto> StudentInLesson { get; set; } = null!;
}