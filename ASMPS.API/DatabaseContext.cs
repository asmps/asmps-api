using Microsoft.EntityFrameworkCore;
using ASMPS.Models;

namespace ASMPS.API;

public sealed class DatabaseContext : DbContext
{
    #region Tables

    /// <summary>
    /// Таблица пользователей
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;
    
    /// <summary>
    /// Таблица студентов
    /// </summary>
    public DbSet<Student> Students { get; set; } = null!;
    
    /// <summary>
    /// Таблица учителей
    /// </summary>
    public DbSet<Teacher> Teachers { get; set; } = null!;

    /// <summary>
    /// Таблица групп студентов
    /// </summary>
    public DbSet<GroupStudents> GroupStudents { get; set; } = null!;

    /// <summary>
    /// Таблица дисциплин
    /// </summary>
    public DbSet<Discipline> Disciplines { get; set; } = null!;

    /// <summary>
    /// Таблица занятий
    /// </summary>
    public DbSet<Lesson> Lessons { get; set; } = null!;

    /// <summary>
    /// Таблица расписаний
    /// </summary>
    public DbSet<Schedule> Schedules { get; set; } = null!;
    
    /// <summary>
    /// Таблица кампусов
    /// </summary>
    public DbSet<Campus> Campuses { get; set; } = null!;
    
    /// <summary>
    /// Таблица аудиторий
    /// </summary>
    public DbSet<Audience> Audiences { get; set; } = null!;

    #endregion
    
    /// <summary>
    /// Конструктор по умолчанию
    /// </summary>
    public DatabaseContext() { }
    
    /// <summary>
    /// Конструктор с параметрами
    /// </summary>
    /// <param name="options">Параметры</param>
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) 
    {
        if (Database.GetPendingMigrations().Any())
            Database.Migrate();
    }
    
    /// <inheritdoc cref="DbContext.OnModelCreating(ModelBuilder)"/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Login).IsRequired();
            entity.Property(e => e.Password).IsRequired();
            entity.Property(e => e.Email).IsRequired(false);
            entity.Property(e => e.Role).IsRequired();
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.RefreshToken).IsRequired(false);
            entity.Property(e => e.RefreshTokenExpires).IsRequired(false);
            
            entity.HasData(new User()
            {
                Id = Guid.NewGuid(),
                Role = UserRoles.Deanery,
                Login = "admin",
                Password = "admin",
                CreatedDate = DateTime.UtcNow,
                Name = "Павел",
                Surname = "Маркелов",
                Email = "pmarkelo77@gmail.com"
            });
        });
        
        modelBuilder.Entity<Student>(entity =>
        {
            entity.Property(e => e.StudentID).IsRequired();
            
            entity.HasOne(s => s.Group).WithMany(g => g.Students).HasForeignKey(s => s.GroupId);
        });
        
        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.Property(e => e.Position).IsRequired();
            
            entity.HasMany(t => t.Disciplines).WithMany(d => d.Teachers);
            entity.HasMany(t => t.Lessons).WithOne(l => l.Teacher).HasForeignKey(l => l.TeacherId);
        });

        modelBuilder.Entity<GroupStudents>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired();

            entity.HasMany(g => g.Students).WithOne(s => s.Group).HasForeignKey(s => s.GroupId);
            entity.HasMany(g => g.Schedules).WithOne(s => s.Group).HasForeignKey(s => s.GroupId);

            entity.HasOne(g => g.Deanery).WithMany(d => d.GroupStudents).HasForeignKey(g => g.DeaneryId);
        });

        modelBuilder.Entity<Discipline>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Note).IsRequired(false);

            entity.HasMany(d => d.Teachers).WithMany(t => t.Disciplines);
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(l => l.Discipline).WithMany(d => d.Lessons).HasForeignKey(d => d.DisciplineId);
            entity.HasOne(l => l.Schedule).WithMany(s => s.Lessons).HasForeignKey(s => s.ScheduleId);
            entity.HasOne(l => l.Teacher).WithMany(t => t.Lessons).HasForeignKey(d => d.TeacherId);
            
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.StartLesson).IsRequired();
            entity.Property(e => e.EndLesson).IsRequired();
            entity.Property(e => e.Note).IsRequired(false);
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Date).IsRequired();
            
            entity.HasOne(s => s.Group).WithMany(g => g.Schedules).HasForeignKey(s => s.GroupId);
            entity.HasMany(s => s.Lessons).WithOne(l => l.Schedule).HasForeignKey(l => l.ScheduleId);
        });
        
        modelBuilder.Entity<Campus>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.Note).IsRequired(false);
            
            entity.HasMany(c => c.Audiences).WithOne(a => a.Campus).HasForeignKey(a => a.CampusId);
        });
        
        modelBuilder.Entity<Audience>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.Note).IsRequired(false);
            
            entity.HasOne(a => a.Campus).WithMany(c => c.Audiences).HasForeignKey(a => a.CampusId);
        });

        modelBuilder.Entity<Deanery>(entity =>
        {
            entity.HasMany(d => d.GroupStudents).WithOne(g => g.Deanery).HasForeignKey(g => g.DeaneryId);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}