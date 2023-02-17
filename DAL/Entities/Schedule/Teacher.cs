using DAL.Entities;

namespace DAL.Entities.Schedule
{
    /// <summary>
    /// Преподаватели
    /// </summary>
    public class Teacher : BaseEntity
    {
        /// <summary>
        /// Фамилия
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; set; }

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
