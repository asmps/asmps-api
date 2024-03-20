using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ASMPS.Models;
using ASMPS.Contracts.Teacher;

namespace ASMPS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeachersController : Controller
{
    private readonly DatabaseContext _context;
    
    /// <summary>
    /// Конструктор класса <see cref="TeachersController"/>
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    public TeachersController(DatabaseContext context)
    {
        _context = context;
    }
}