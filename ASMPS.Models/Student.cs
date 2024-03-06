namespace ASMPS.Models;

public class Student : Person
{
    /// <summary>
    /// Номер студенческого билета
    /// </summary>
    public string StudentID { get; set; } = string.Empty;
    
    /// <summary>
    /// Идентификатор группы
    /// </summary>
    public Guid GroupId { get; set; }

    /// <summary>
    /// Группа
    /// </summary>
    public virtual GroupStudents Group { get; set; } = null!;
}
