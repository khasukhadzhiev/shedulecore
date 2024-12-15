namespace DTL.Dto.ScheduleDto
{
    /// <summary>
    /// Занятие
    /// </summary>
    public class LessonDto : BaseDto
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
        /// Наименование групп которые находятся в потоке
        /// </summary>
        public string FlowStudyClassNames { get; set; }

        /// <summary>
        /// Id версии расписания с настройками
        /// </summary>
        public int VersionId { get; set; }

        /// <summary>
        /// День занятия
        /// </summary>
        public int? LessonDay { get; set; }

        /// <summary>
        /// Номер занятия
        /// </summary>
        public int? LessonNumber { get; set; }

        /// <summary>
        /// Номер занятия
        /// </summary>
        public int? WeekNumber { get; set; }


        /// <summary>
        /// Версия расписания с настройками
        /// </summary>
        public VersionDto Version { get; set; }

        /// <summary>
        /// Аудитория
        /// </summary>
        public ClassroomDto Classroom { get; set; }

        /// <summary>
        /// Поток
        /// </summary>
        public FlowDto Flow { get; set; }

        /// <summary>
        /// Вид занятия
        /// </summary>
        public LessonTypeDto LessonType { get; set; }

        /// <summary>
        /// Группа
        /// </summary>
        public StudyClassDto StudyClass { get; set; }

        /// <summary>
        /// Дисциплина
        /// </summary>
        public SubjectDto Subject { get; set; }

        /// <summary>
        /// Преподаватель
        /// </summary>
        public TeacherDto Teacher { get; set; }
    }
}
