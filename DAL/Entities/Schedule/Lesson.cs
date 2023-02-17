using System;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Entities;

namespace DAL.Entities.Schedule
{
    /// <summary>
    /// Занятие
    /// </summary>
    public class Lesson : BaseEntity, ICloneable
    {
        /// <summary>
        /// Id группы
        /// </summary>
        public int StudyClassId { get; set; }
        /// <summary>
        /// Id преподавателя
        /// </summary>
        public int TeacherId { get; set; }

        /// <summary>
        /// Id дисциплины
        /// </summary>
        public int SubjectId { get; set; }

        /// <summary>
        /// Id вида занятия
        /// </summary>
        public int LessonTypeId { get; set; }

        /// <summary>
        /// Занятие является паралелью
        /// </summary>
        public bool IsParallel { get; set; }

        /// <summary>
        /// Занятие является занятием подгруппы
        /// </summary>
        public bool IsSubClassLesson { get; set; }

        /// <summary>
        /// Занятие является занятием по одной неделе
        /// </summary>
        public bool IsSubWeekLesson { get; set; }

        /// <summary>
        /// Id аудитории
        /// </summary>
        public int? ClassroomId { get; set; }

        /// <summary>
        /// Id потока
        /// </summary>
        public int? FlowId { get; set; }

        /// <summary>
        /// Индекс строки в таблице расписания
        /// </summary>
        public int? RowIndex { get; set; }

        /// <summary>
        /// Индекс столбца в таблице расписания
        /// </summary>
        public int? ColIndex { get; set; }

        /// <summary>
        /// Id версии расписания с настройками
        /// </summary>
        public int VersionId { get; set; }


        //Навигационные свойства
        //----------------------------------------------------
        /// <summary>
        /// Версия расписания с настройками
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// Аудитория
        /// </summary>
        public Classroom Classroom { get; set; }

        /// <summary>
        /// Поток
        /// </summary>
        public Flow Flow { get; set; }

        /// <summary>
        /// Вид занятия
        /// </summary>
        public LessonType LessonType { get; set; }

        /// <summary>
        /// Группа
        /// </summary>
        public StudyClass StudyClass { get; set; }

        /// <summary>
        /// Дисциплина
        /// </summary>
        public Subject Subject { get; set; }

        /// <summary>
        /// Преподаватель
        /// </summary>
        public Teacher Teacher { get; set; }

        public object Clone()
        {
            Lesson newLesson = new Lesson();

            newLesson.Id = Id;
            newLesson.StudyClassId = StudyClassId;
            newLesson.TeacherId = TeacherId;
            newLesson.SubjectId = SubjectId;
            newLesson.LessonTypeId = LessonTypeId;
            newLesson.IsParallel = IsParallel;
            newLesson.IsSubClassLesson = IsSubClassLesson;
            newLesson.IsSubWeekLesson = IsSubWeekLesson;
            newLesson.ClassroomId = ClassroomId;
            newLesson.FlowId = FlowId;
            newLesson.RowIndex = RowIndex;
            newLesson.ColIndex = ColIndex;
            newLesson.VersionId = VersionId;
            newLesson.Weight = Weight;
            newLesson.Version = Version;
            newLesson.Classroom = Classroom;
            newLesson.Flow = Flow;
            newLesson.LessonType = LessonType;
            newLesson.StudyClass = StudyClass;
            newLesson.Subject = Subject;
            newLesson.Teacher = Teacher;

            return newLesson;
        }

        //------------------------------------------------
        //Вычисляемые свойства
        /// <summary>
        /// Вес (используюется при генерации расписания)
        /// </summary>
        [NotMapped]
        public double Weight { get; set; }

        /// <summary>
        /// День занятия
        /// </summary>
        [NotMapped]
        public int? LessonDay
        {
            get
            {
                if(Version != null && RowIndex != null)
                {
                    return Version.UseSubWeek ? RowIndex / Version.MaxLesson / 2 : RowIndex / Version.MaxLesson;
                }
                else
                {
                    return null;
                }                
            }
            set
            {
                LessonDay = value;
            }
        }

        /// <summary>
        /// Номер занятия
        /// </summary>
        [NotMapped]
        public int? LessonNumber {
            get
            {
                if(Version !=null && RowIndex != null)
                {
                    int? result;

                    if (Version.UseSubWeek)
                    {
                        result = Convert.ToInt32(Math.Floor((decimal)(RowIndex - (Version.MaxLesson * 2 * LessonDay)) / 2));
                    }
                    else
                    {
                        result = Convert.ToInt32(Math.Floor((decimal)(RowIndex - (Version.MaxLesson * LessonDay))));
                    }

                    return result;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                LessonNumber = value;
            }
        }
    }
}
