using System.Text.RegularExpressions;
using ASMPS.Contracts.Schedule;
using ASMPS.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ASMPS.API.Services;

public class ScheduleConverter
{
    private readonly DatabaseContext _context;
    
    public ScheduleConverter(DatabaseContext context)
    {
        _context = context;
    }

    private void SetLessonTime(Lesson lesson, int lessonOrderId)
    {
        switch (lessonOrderId)
        {
            case 0:
                lesson.StartLesson = new TimeOnly(8, 30);
                lesson.EndLesson = new TimeOnly(10, 0);
                break;
            case 1:
                lesson.StartLesson = new TimeOnly(10, 15);
                lesson.EndLesson = new TimeOnly(11, 45);
                break;
            case 2:
                lesson.StartLesson = new TimeOnly(12, 0);
                lesson.EndLesson = new TimeOnly(13, 30);
                break;
            case 3:
                lesson.StartLesson = new TimeOnly(14, 0);
                lesson.EndLesson = new TimeOnly(15, 30);
                break;
            case 4:
                lesson.StartLesson = new TimeOnly(15, 45);
                lesson.EndLesson = new TimeOnly(17, 15);
                break;
            case 5:
                lesson.StartLesson = new TimeOnly(17, 30);
                lesson.EndLesson = new TimeOnly(19, 0);
                break;
            case 6:
                lesson.StartLesson = new TimeOnly(19, 15);
                lesson.EndLesson = new TimeOnly(20, 45);
                break;
            default:
                lesson.StartLesson = TimeOnly.MinValue;
                lesson.EndLesson = TimeOnly.MinValue;
                break;
        }
    }
    
    public async Task<Schedule?> ConvertToScheduleForStudent(ScheduleDto scheduleDto)
    {
        var group = await _context.GroupStudents.FirstOrDefaultAsync(g => g.Title == scheduleDto.Title);

        if (group is null)
        {
            return null;
        }

        var schedule = await _context.Schedules
            .Include(item => item.Lessons)
            .ThenInclude(l => l.Audience)
            .ThenInclude(a => a.Campus)
            .Include(item => item.Lessons)
            .ThenInclude(l => l.Teacher)
            .Include(item => item.Lessons)
            .ThenInclude(l => l.Discipline)
            .FirstOrDefaultAsync(item => item.GroupId == group.Id);

        if (schedule is not null)
        {
            schedule.Date = DateOnly.FromDateTime(DateTime.UtcNow);
            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }
        
        var newSchedule = new Schedule 
        {
            Id = Guid.NewGuid(),
            WeekType = false, // Пример значения для WeekType
            Date = DateOnly.FromDateTime(DateTime.UtcNow), // Пример значения для Date
            GroupId = group.Id, // Пример значения для GroupId
            Group = group
        };
        
        foreach (var lessonDto in scheduleDto.Lessons)
        {
            var campus = await _context.Campuses
                    .Where(item => item.Title == lessonDto.LessonInScheduleInfoDto.Audience)
                    .FirstOrDefaultAsync();

            if (campus is null)
            {
                var newCampus = new Campus
                {
                    Id = Guid.NewGuid(),
                    Number = Convert.ToInt32(lessonDto.LessonInScheduleInfoDto.Audience[0]),
                    Title = lessonDto.LessonInScheduleInfoDto.Audience  
                };

                campus = newCampus;
                
                _context.Campuses.Add(newCampus); 
                await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных
            }
            
            var audience = await _context.Audiences
                .Where(item => item.Title == lessonDto.LessonInScheduleInfoDto.Audience)
                .FirstOrDefaultAsync();

            if (audience is null)
            {
                var newAudience = new Audience
                {
                    Id = Guid.NewGuid(),
                    Campus = campus,
                    CampusId = campus.Id,
                    Number = Convert.ToInt32(lessonDto.LessonInScheduleInfoDto.Audience[2]),
                    Title = lessonDto.LessonInScheduleInfoDto.Audience
                };

                audience = newAudience;
                
                _context.Audiences.Add(audience); 
                await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных
            }
            
            var lesson = new Lesson
            {
                    Id = Guid.NewGuid(),
                    Group = group,
                    GroupId = group.Id,
                    LessonOrderId = lessonDto.LessonOrderId,
                    DayId = lessonDto.DayId,
                    Audience = audience,
                    AudienceId = audience.Id,
                    StartLesson = new TimeOnly(), // Пример значения для StartLesson
                    EndLesson = new TimeOnly(), // Пример значения для EndLesson
                    Note = lessonDto.Note, // Пример значения для Note
                    Type = Enum.TryParse<LessonTypes>(lessonDto.LessonInScheduleInfoDto.Type, true, out var lessonType) ? lessonType : LessonTypes.Other,
            };
            
            SetLessonTime(lesson, lessonDto.LessonOrderId);

            var disciplineName = lessonDto.LessonInScheduleInfoDto.Discipline; 
            var discipline = _context.Disciplines.FirstOrDefault(d => d.Name == disciplineName); 
            if (discipline != null)
            {
                    lesson.DisciplineId = discipline.Id;
            }
            else 
            {
                // Обработка случая, когда дисциплина не найдена
                discipline = new Discipline 
                {
                    Id = Guid.NewGuid(),
                    Name = disciplineName, 
                    // Остальные свойства Discipline можно назначить по аналогии с остальными свойствами объектов
                }; 
                _context.Disciplines.Add(discipline); 
                await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных
                lesson.DisciplineId = discipline.Id;
            }
            
            var teacherFullName = lessonDto.LessonInScheduleInfoDto.Teacher; 
            var teacher = _context.Teachers.FirstOrDefault(t => t.Name == teacherFullName);
            if (teacher != null)
            {
                lesson.TeacherId = teacher.Id; 
            }
            else 
            {
                // Обработка случая, когда учитель не найден
                teacher = new Teacher
                {
                    Id = Guid.NewGuid(), 
                    Name = teacherFullName,
                    // Остальные свойства Teacher можно назначить по аналогии с остальными свойствами объектов
                };
                _context.Teachers.Add(teacher); 
                await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных
                lesson.TeacherId = teacher.Id;
            }

            newSchedule.Lessons.Add(lesson); 
        }
        
        _context.Schedules.Add(newSchedule);
        await _context.SaveChangesAsync();
        
        return newSchedule;
    }

    public async Task<Schedule?> ConvertToScheduleForTeacher(ScheduleDto scheduleDto, Guid teacherId)
    {
        /*var teacherPositionAndName = Regex.Match(scheduleDto.Title, @"^(?<position>[\w.]+)\s(?<name>.+)$");

        string position = "", name = "";
        
        if (teacherPositionAndName.Success)
        {
            position = teacherPositionAndName.Groups["position"].Value;
            name = teacherPositionAndName.Groups["name"].Value;
        }
                
        string[] parts = name.Split(' ');
        string lastName = parts[0];*/
        
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.Id == teacherId);

        if (teacher is null)
        {
            // Обработка случая, когда учитель не найден
            teacher = new Teacher
            {
                Id = Guid.NewGuid(), 
                Name = scheduleDto.Title,
                // Остальные свойства Teacher можно назначить по аналогии с остальными свойствами объектов
            };
            _context.Teachers.Add(teacher); 
            await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных
        }

        var schedule = await _context.Schedules
            .Include(item => item.Lessons)
            .ThenInclude(l => l.Audience)
            .ThenInclude(a => a.Campus)
            .Include(item => item.Lessons)
            .ThenInclude(l => l.Teacher)
            .Include(item => item.Lessons)
            .ThenInclude(l => l.Discipline)
            .FirstOrDefaultAsync(item => item.GroupId == teacher.Id);

        if (schedule is not null)
        {
            schedule.Date = DateOnly.FromDateTime(DateTime.UtcNow);
            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        var tempDeanery = new Deanery
        {
            Id = Guid.Empty,
            Role = UserRoles.Deanery
        };

        var tempGroup = new GroupStudents
        {
            Id = Guid.Empty,
            Title = scheduleDto.Title,
            DeaneryId = tempDeanery.Id,
            Deanery = tempDeanery
        };
        
        var newSchedule = new Schedule 
        {
            Id = Guid.NewGuid(),
            WeekType = false, // Пример значения для WeekType
            Date = DateOnly.FromDateTime(DateTime.UtcNow), // Пример значения для Date
            GroupId = tempGroup.Id, // Пример значения для GroupId
            Group = tempGroup
        };
        
        foreach (var lessonDto in scheduleDto.Lessons)
        {
            var campus = await _context.Campuses
                    .Where(item => item.Title == lessonDto.LessonInScheduleInfoDto.Audience)
                    .FirstOrDefaultAsync();

            if (campus is null)
            {
                var newCampus = new Campus
                {
                    Id = Guid.NewGuid(),
                    Number = Convert.ToInt32(lessonDto.LessonInScheduleInfoDto.Audience[0]),
                    Title = lessonDto.LessonInScheduleInfoDto.Audience  
                };

                campus = newCampus;
                
                _context.Campuses.Add(newCampus); 
                await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных
            }
            
            var audience = await _context.Audiences
                .Where(item => item.Title == lessonDto.LessonInScheduleInfoDto.Audience)
                .FirstOrDefaultAsync();

            if (audience is null)
            {
                var newAudience = new Audience
                {
                    Id = Guid.NewGuid(),
                    Campus = campus,
                    CampusId = campus.Id,
                    Number = Convert.ToInt32(lessonDto.LessonInScheduleInfoDto.Audience[2]),
                    Title = lessonDto.LessonInScheduleInfoDto.Audience
                };

                audience = newAudience;
                
                _context.Audiences.Add(audience); 
                await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных
            }

            lessonDto.LessonInScheduleInfoDto.Groups ??= new List<string>();
            
            /*var groupTitles = lessonDto.LessonInScheduleInfoDto.Groups; // Получаем названия групп из lessonDto
            var groups = await _context.GroupStudents
                .Where(item => groupTitles.Contains(item.Title))
                .ToListAsync();

            var tempGroups = new List<GroupStudents>();
            
            if (groups is null)
            {
                foreach (var titleGroup in groupTitles)
                {
                    var newGroup = new GroupStudents
                    {
                        Id = Guid.NewGuid(),
                        Title = titleGroup,
                        Deanery = tempDeanery,
                        DeaneryId = tempDeanery.Id
                    };

                    tempGroups.Add(newGroup);
                    
                    await _context.GroupStudents.AddAsync(newGroup);
                    await _context.SaveChangesAsync();
                }
            }

            groups = tempGroups;
            
            foreach (var group in groups)
            {
                
            }*/
            
            var lesson = new Lesson
            {
                Id = Guid.NewGuid(),
                Group = tempGroup,
                GroupId = tempGroup.Id,
                LessonOrderId = lessonDto.LessonOrderId,
                DayId = lessonDto.DayId,
                Audience = audience,
                AudienceId = audience.Id,
                StartLesson = new TimeOnly(), // Пример значения для StartLesson
                EndLesson = new TimeOnly(), // Пример значения для EndLesson
                Note = lessonDto.Note, // Пример значения для Note
                Type = Enum.TryParse<LessonTypes>(lessonDto.LessonInScheduleInfoDto.Type, true, out var lessonType) ? lessonType : LessonTypes.Other,
            };
                
            SetLessonTime(lesson, lessonDto.LessonOrderId);
            
            var disciplineName = lessonDto.LessonInScheduleInfoDto.Discipline; 
            var discipline = _context.Disciplines.FirstOrDefault(d => d.Name == disciplineName); 
            if (discipline != null)
            {
                lesson.DisciplineId = discipline.Id;
            }
            else 
            {
                // Обработка случая, когда дисциплина не найдена
                discipline = new Discipline 
                {
                    Id = Guid.NewGuid(),
                    Name = disciplineName, 
                    // Остальные свойства Discipline можно назначить по аналогии с остальными свойствами объектов
                }; 
                _context.Disciplines.Add(discipline); 
                await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных
                lesson.DisciplineId = discipline.Id;
            }
            
            lesson.TeacherId = teacher.Id;

            newSchedule.Lessons.Add(lesson); 
        }
        
        _context.Schedules.Add(newSchedule);
        await _context.SaveChangesAsync();
        
        return newSchedule;
    }
}