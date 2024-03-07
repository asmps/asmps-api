namespace ASMPS.Models;

/// <summary>
/// Модель деканата
/// </summary>
public class Deanery : User
{
    /// <summary>
    /// Список групп студентов, которые принадлежат деканату
    /// </summary>
    public virtual ICollection<GroupStudents> GroupStudents { get; set; } = new List<GroupStudents>();
}