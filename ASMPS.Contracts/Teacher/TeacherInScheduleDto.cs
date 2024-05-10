namespace ASMPS.Contracts.Teacher;

/// <summary>
/// Модель представления учителя в рассписании
/// </summary>
public class TeacherInScheduleDto
{
    /// <summary>
    /// Полное имя (ФИО)
    /// </summary>
    public string FullName { get; set; }
    
    /// <summary>
    /// Должность
    /// </summary>
    public string Position { get; set; }
}