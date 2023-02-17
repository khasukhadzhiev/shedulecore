namespace DTL.Dto.ScheduleDto
{
    /// <summary>
    /// Преподаватель
    /// </summary>
    public class TeacherDto : BaseDto
    {
        /// <summary>
        /// Фамилия
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Полное ФИО
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Приоритетные дни для расписания. (Пожелания преподавателя)
        /// </summary>
        public int[] WeekDays { get; set; }

        /// <summary>
        /// Приоритетные порядковые номера занятий для расписания. (Пожелания преподавателя)
        /// </summary>
        public int[] LessonNumbers { get; set; }
    }
}
