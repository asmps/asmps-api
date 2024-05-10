namespace ASMPS.Models;

public class Student : User
{
    /// <summary>
    /// Номер студенческого билета
    /// </summary>
    public string StudentId { get; set; } = string.Empty;
    
    /// <summary>
    /// Идентификатор группы
    /// </summary>
    public Guid GroupId { get; set; }

    /// <summary>
    /// Группа
    /// </summary>
    public virtual GroupStudents Group { get; set; } = null!;
}
