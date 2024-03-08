using System.ComponentModel.DataAnnotations;

namespace ASMPS.Models;

/// <summary>
/// Типа занятий
/// </summary>
public enum LessonTypes
{
    /// <summary>
    /// Лекция
    /// </summary>
    [Display(Name = "Лекция")]
    Lecture,
    
    /// <summary>
    /// Практика
    /// </summary>
    [Display(Name = "Практика")]
    Practice,
    
    /// <summary>
    /// Лабораторная работа
    /// </summary>
    [Display(Name = "Лабораторная работа")]
    LabWork,
    
    /// <summary>
    /// Экзамен
    /// </summary>
    [Display(Name = "Экзамен")]
    Exam,
    
    /// <summary>
    /// Зачет
    /// </summary>
    [Display(Name = "Зачет")]
    Test
}