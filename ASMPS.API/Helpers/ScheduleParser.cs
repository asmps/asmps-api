using System.Text.RegularExpressions;
using ASMPS.Contracts.Lesson;
using ASMPS.Contracts.Schedule;
using ASMPS.Contracts.Teacher;
using ASMPS.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ASMPS.API.Helpers;

/// <summary>
/// Парсер данных рассписания
/// </summary>
public class ScheduleParser
{
    /// <summary>
    /// Парсим рассписание из АГТУ для студентов
    /// </summary>
    /// <param name="jsonString">JSON строка</param>
    /// <returns>Рассписание</returns>
    public ScheduleDto ParseScheduleForStudent(string jsonString)
    {
        var schedule = JsonSerializer.Deserialize<RootJson>(jsonString);

        var scheduleDto = new ScheduleDto()
        {
            Title = schedule.name
        };

        foreach (var lessonDto in schedule.lessons)
        {
            var lessonInScheduleDto = new LessonInScheduleDto()
            {
                DayId = lessonDto.dayId,
                LessonOrderId = lessonDto.lessonOrderId,
            };

            foreach (var entryDto in lessonDto.entries)
            {
                // todo: нужно для добавления в базу
                var teacherPositionAndName = Regex.Match(entryDto.teacher, @"^(?<position>[\w.]+)\s(?<name>.+)$");
                if (teacherPositionAndName.Success)
                {
                    string position = teacherPositionAndName.Groups["position"].Value;
                    string name = teacherPositionAndName.Groups["name"].Value;
                }

                // todo: нужно для добавления в базу
                var audienceBuildingAndNumber = Regex.Match(entryDto.audience, @"^(?<corpus>\w+)\.(?<auditorium>.+)$");
                if (audienceBuildingAndNumber.Success)
                {
                    string campus = audienceBuildingAndNumber.Groups["corpus"].Value;
                    string auditorium = audienceBuildingAndNumber.Groups["auditorium"].Value;
                }
                
                lessonInScheduleDto.LessonInScheduleInfoDto = new LessonInScheduleInfoDto()
                {
                    Teacher = entryDto.teacher,
                    Audience = entryDto.audience,
                    Discipline = entryDto.discipline,
                    Type = entryDto.type
                };
            }

            // Добавляем урок в список уроков расписания
            scheduleDto.Lessons.Add(lessonInScheduleDto);
        }

        return scheduleDto;
    }

    /// <summary>
    /// Парсим рассписание из АГТУ для учителей
    /// </summary>
    /// <param name="jsonString">JSON строка</param>
    /// <param name="teacherDto">Модель представления учителя</param>
    /// <returns>Рассписание</returns>
    public ScheduleDto ParseScheduleForTeacher(string jsonString, TeacherDto teacherDto)
    {
        var schedule = JsonSerializer.Deserialize<RootJson>(jsonString);

        var scheduleDto = new ScheduleDto()
        {
            Title = schedule.name
        };
        
        var teacherString =
            $"{teacherDto.Position} {teacherDto.Surname} {teacherDto.Name[0]}.{teacherDto.Patronymic![0]}.";

        foreach (var lessonDto in schedule.lessons)
        {
            var lessonInScheduleDto = new LessonInScheduleDto()
            {
                DayId = lessonDto.dayId,
                LessonOrderId = lessonDto.lessonOrderId,
            };

            foreach (var entryDto in lessonDto.entries)
            {
                lessonInScheduleDto.LessonInScheduleInfoDto = new LessonInScheduleInfoDto()
                {
                    Teacher = teacherString,
                    Audience = entryDto.audience,
                    Discipline = entryDto.discipline,
                    Type = entryDto.type,
                    Groups = entryDto.groups
                };
            }

            // Добавляем урок в список уроков расписания
            scheduleDto.Lessons.Add(lessonInScheduleDto);
        }

        return scheduleDto;
    }
    
    /// <summary>
    /// Парсим рассписание из АГТУ
    /// </summary>
    /// <param name="jsonString">JSON строка</param>
    /// <returns>Рассписание</returns>
    public ScheduleDto ParseSchedule(string jsonString)
    {
        var schedule = JsonSerializer.Deserialize<RootJson>(jsonString);

        var scheduleDto = new ScheduleDto()
        {
            Title = schedule.name
        };

        foreach (var lessonDto in schedule.lessons)
        {
            var lessonInScheduleDto = new LessonInScheduleDto()
            {
                DayId = lessonDto.dayId,
                LessonOrderId = lessonDto.lessonOrderId,
            };

            foreach (var entryDto in lessonDto.entries)
            {
                var groupList = new List<string>();
                
                if (entryDto.groups is not null)
                {
                    foreach (var groupDto in entryDto.groups)
                    {
                        groupList.Add(groupDto);
                    }
                }
                
                lessonInScheduleDto.LessonInScheduleInfoDto = new LessonInScheduleInfoDto()
                {
                    Groups = groupList,
                    Teacher = entryDto.teacher.IsNullOrEmpty() ? scheduleDto.Title : entryDto.teacher,
                    Audience = entryDto.audience,
                    Discipline = entryDto.discipline,
                    Type = entryDto.type
                };
            }

            // Добавляем урок в список уроков расписания
            scheduleDto.Lessons.Add(lessonInScheduleDto);
        }

        return scheduleDto;
    }
    
    /// <summary>
    /// Парсим рассписание из базы данных
    /// </summary>
    /// <param name="schedule">Рассписание</param>
    /// <returns>Рассписание</returns>
    public ScheduleDto ParseSchedule(Schedule schedule)
    {
        var scheduleTemp = schedule;

        var scheduleDto = new ScheduleDto()
        {
            Title = scheduleTemp.Group.Title
        };

        foreach (var lesson in scheduleTemp.Lessons)
        {
            var lessonInScheduleDto = new LessonInScheduleDto()
            {
                DayId = lesson.DayId,
                LessonOrderId = lesson.LessonOrderId,
            };

            lessonInScheduleDto.LessonInScheduleInfoDto = new LessonInScheduleInfoDto()
            {
                Id = lesson.Id,
                Teacher = $"{lesson.Teacher.Position} {lesson.Teacher.Surname} {lesson.Teacher.Name} {lesson.Teacher.Patronymic}",
                Audience = $"{lesson.Audience.Campus.Number}.{lesson.Audience.Number}",
                Discipline = $"{lesson.Discipline.Name}",
                Type = $"{lesson.Type.GetDisplayName()}"
            };

            // Добавляем урок в список уроков расписания
            scheduleDto.Lessons.Add(lessonInScheduleDto);
        }

        return scheduleDto;
    }
    
    #region JsonSerializerModels

    public class Entry
    {
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public List<string>? groups { get; set; }
        public string teacher { get; set; }
        public string audience { get; set; }
        public string discipline { get; set; }
        public string type { get; set; }
    }

    public class Lesson
    {
        public List<Entry> entries { get; set; }
        public int dayId { get; set; }
        public int lessonOrderId { get; set; }
    }

    public class RootJson
    {
        public List<Lesson> lessons { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    #endregion
}
