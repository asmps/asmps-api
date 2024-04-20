using ASMPS.Models;
using Microsoft.EntityFrameworkCore;

namespace ASMPS.API.Services;

/// <summary>
/// Сервис установления студентов на занятия
/// </summary>
public class SetStudentsInClassHostedService : IHostedService
{
    private const int ServiceDefaultDelay = 1;
    private readonly IServiceProvider _services;
    private readonly TimeSpan _period;
    private Timer _timer = null!;
    
    // Время начала занятий
    // todo: перенести все в базу по хорошему
    private readonly TimeSpan[] LessonStartTimes = {
        new TimeSpan(8, 30, 0), // 8:30
        new TimeSpan(10, 15, 0), // 10:15
        new TimeSpan(12, 0, 0), // 12:00
        new TimeSpan(14, 00, 0), // 14:00
        new TimeSpan(15, 45, 0), // 15:45
        new TimeSpan(17, 30, 0), // 17:30\
    };

    /// <summary>
    /// Конструктор класса <seealso cref="SetStudentsInClassHostedService"/>
    /// </summary>
    /// <param name="configuration">Конфигурация</param>
    /// <param name="services">Поставщик сервисов</param>
    /// <exception cref="ArgumentException">Период времени проверки из конфигурации не получен</exception>
    /// <exception cref="ArgumentNullException">Проблемы с провайдером</exception>
    public SetStudentsInClassHostedService(IConfiguration configuration, IServiceProvider services)
    {
        var delay = configuration.GetValue("OverdueGoalsServiceDelay", ServiceDefaultDelay);
        if (delay <= 0)
            throw new ArgumentException("Invalid delay value");

        _period = TimeSpan.FromHours(delay);
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Вычисляем ближайшее время начала следующего занятия
        var nextLessonStartTime = LessonStartTimes.FirstOrDefault(t => t > DateTime.UtcNow.TimeOfDay) != default 
            ? LessonStartTimes.FirstOrDefault(t => t > DateTime.UtcNow.TimeOfDay)
            : LessonStartTimes[0];
        var timeUntilNextLesson = nextLessonStartTime - DateTime.UtcNow.TimeOfDay;
        
        Console.WriteLine($"Now time {DateTime.UtcNow.TimeOfDay}. Next start service to {nextLessonStartTime}");
        
        _timer = new Timer(SetStudentsInClass, null, timeUntilNextLesson, _period);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private async void SetStudentsInClass(object? state)
    {
        using var scope = _services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var logger = _services.GetRequiredService<ILogger<SetStudentsInClassHostedService>>();

        logger.LogInformation($"Background job (SetStudentsInClassHostedService) started at {DateTime.Now}");

        try 
        {
            // Берем каждого занятия, которое началось, и всех его студентов, которые должны быть на нем
            // DateTime currentTime = DateTime.UtcNow;
            DateTime currentTime = new DateTime(2024, 4, 20, 8, 45, 0);
            TimeOnly timeOnly = new TimeOnly(currentTime.Hour, currentTime.Minute, currentTime.Second);

            var nearestLessons = await context.Lessons
                .Where(item => item.StartLesson <= timeOnly && item.EndLesson >= timeOnly)
                .Include(lesson => lesson.Group).ThenInclude(groupStudents => groupStudents.Students)
                .ToListAsync();
            
            // Берем каждое посещение
            var attendances = await context.Attendances.ToListAsync();
            
            // Пробегаемся по всем занятиями
            foreach (var lesson in nearestLessons)
            {
                // Для каждого запоминаем данные
                var tempLessonId = lesson.Id;
                var tempDate = DateTime.UtcNow;
                
                // Пробегаемся по всем студентам, которые должны быть на занятии
                foreach (var student in lesson.Group.Students)
                {
                    // Для каждого студента проверяем, есть ли уже запись посещения для этого занятия и этого студента
                    var existingAttendance = await context.Attendances.FirstOrDefaultAsync(a => a.LessonId == lesson.Id && a.StudentId == student.Id);

                    // Если запись отсутствует, то добавляем ее
                    if (existingAttendance == null)
                    {
                        var attendance = new Attendance
                        {
                            Id = Guid.NewGuid(),
                            LessonId = lesson.Id,
                            StudentId = student.Id,
                            AttendanceDateTime = tempDate,
                            IsAttendance = false
                        };
                    
                        await context.Attendances.AddAsync(attendance);
                    }
                }
            }   

            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Background service error");
        }
        
        logger.LogInformation($"Background job (SetOverdueGoalsHostedService) finished at {DateTime.Now}");
    }
}