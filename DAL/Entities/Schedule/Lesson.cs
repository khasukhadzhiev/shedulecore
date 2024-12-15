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
        /// Дата и время последнего изменения
        /// </summary>
        public DateTime LastModified { get; set; }

        private int _studyClassId;
        /// <summary>
        /// Id группы
        /// </summary>
        public int StudyClassId
        {
            get { return _studyClassId; }
            set
            {
                if (_studyClassId != value)
                {
                    _studyClassId = value;
                    UpdateLastModified();  // Обновление времени последнего изменения
                }
            }
        }

        private int _teacherId;
        /// <summary>
        /// Id преподавателя
        /// </summary>
        public int TeacherId
        {
            get { return _teacherId; }
            set
            {
                if (_teacherId != value)
                {
                    _teacherId = value;
                    UpdateLastModified();  // Обновление времени последнего изменения
                }
            }
        }

        private int _subjectId;
        /// <summary>
        /// Id дисциплины
        /// </summary>
        public int SubjectId
        {
            get { return _subjectId; }
            set
            {
                if (_subjectId != value)
                {
                    _subjectId = value;
                    UpdateLastModified();  // Обновление времени последнего изменения
                }
            }
        }

        private int _lessonTypeId;
        /// <summary>
        /// Id вида занятия
        /// </summary>
        public int LessonTypeId
        {
            get { return _lessonTypeId; }
            set
            {
                if (_lessonTypeId != value)
                {
                    _lessonTypeId = value;
                    UpdateLastModified();  // Обновление времени последнего изменения
                }
            }
        }

        private bool _isParallel;
        /// <summary>
        /// Занятие является паралелью
        /// </summary>
        public bool IsParallel
        {
            get { return _isParallel; }
            set
            {
                if (_isParallel != value)
                {
                    _isParallel = value;
                    UpdateLastModified();  // Обновление времени последнего изменения
                }
            }
        }

        private bool _isSubClassLesson;
        /// <summary>
        /// Занятие является занятием подгруппы
        /// </summary>
        public bool IsSubClassLesson
        {
            get { return _isSubClassLesson; }
            set
            {
                if (_isSubClassLesson != value)
                {
                    _isSubClassLesson = value;
                    UpdateLastModified();  // Обновление времени последнего изменения
                }
            }
        }

        private bool _isSubWeekLesson;
        /// <summary>
        /// Занятие является занятием по одной неделе
        /// </summary>
        public bool IsSubWeekLesson
        {
            get { return _isSubWeekLesson; }
            set
            {
                if (_isSubWeekLesson != value)
                {
                    _isSubWeekLesson = value;
                    UpdateLastModified();  // Обновление времени последнего изменения
                }
            }
        }

        private int? _classroomId;
        /// <summary>
        /// Id аудитории
        /// </summary>
        public int? ClassroomId
        {
            get { return _classroomId; }
            set
            {
                if (_classroomId != value)
                {
                    _classroomId = value;
                    UpdateLastModified();  // Обновление времени последнего изменения
                }
            }
        }

        private int? _flowId;
        /// <summary>
        /// Id потока
        /// </summary>
        public int? FlowId
        {
            get { return _flowId; }
            set
            {
                if (_flowId != value)
                {
                    _flowId = value;
                    UpdateLastModified();  // Обновление времени последнего изменения
                }
            }
        }

        private int? _rowIndex;
        /// <summary>
        /// Индекс строки в таблице расписания
        /// </summary>
        public int? RowIndex
        {
            get { return _rowIndex; }
            set
            {
                if (_rowIndex != value)
                {
                    _rowIndex = value;
                    UpdateLastModified();  // Обновление времени последнего изменения
                }
            }
        }

        private int? _colIndex;
        /// <summary>
        /// Индекс столбца в таблице расписания
        /// </summary>
        public int? ColIndex
        {
            get { return _colIndex; }
            set
            {
                if (_colIndex != value)
                {
                    _colIndex = value;
                    UpdateLastModified();  // Обновление времени последнего изменения
                }
            }
        }

        private int _versionId;
        /// <summary>
        /// Id версии расписания с настройками
        /// </summary>
        public int VersionId
        {
            get { return _versionId; }
            set
            {
                if (_versionId != value)
                {
                    _versionId = value;
                    UpdateLastModified();  // Обновление времени последнего изменения
                }
            }
        }

        // Навигационные свойства (оставляем без изменений)

        public Version Version { get; set; }
        public Classroom Classroom { get; set; }
        public Flow Flow { get; set; }
        public LessonType LessonType { get; set; }
        public StudyClass StudyClass { get; set; }
        public Subject Subject { get; set; }
        public Teacher Teacher { get; set; }

        /// <summary>
        /// Метод для обновления времени последнего изменения
        /// </summary>
        private void UpdateLastModified()
        {
            LastModified = DateTime.UtcNow;  // Устанавливаем текущее время
        }

        // Метод Clone (оставляем без изменений)

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
            newLesson.LastModified = LastModified;  // Копируем LastModified

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

        /// <summary>
        /// Четность(номер) недели
        /// </summary>
        [NotMapped]
        public int? WeekNumber
        {
            get
            {
                if (Version != null && RowIndex != null)
                {
                    int? result;

                    if (IsSubWeekLesson)
                    {
                        result = RowIndex % 2 == 0 ? 1 : 2;
                    }
                    else
                    {
                        result = null;
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
                WeekNumber = value;
            }
        }
    }
}
