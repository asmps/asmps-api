using System.ComponentModel.DataAnnotations;

namespace ASMPS.Models;

/// <summary>
/// Роли пользователя
/// </summary>
public enum UserRoles
{
    /// <summary>
    /// Студент
    /// </summary>
    [Display(Name = "Студент")]
    Student,

    /// <summary>
    /// Преподаватель
    /// </summary>
    [Display(Name = "Преподаватель")]
    Teacher,

    /// <summary>
    /// Деканат
    /// </summary>
    [Display(Name = "Деканат")]
    Deanery
}
