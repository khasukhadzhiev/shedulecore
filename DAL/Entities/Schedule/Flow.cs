using DAL.Entities;

namespace DAL.Entities.Schedule
{
    /// <summary>
    /// Потоки (объедененные группы)
    /// </summary>
    public class Flow : BaseEntity
    {
        /// <summary>
        /// Наименование потока
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Преподаватели потока
        /// </summary>
        public ICollection<Teacher> TeacherList { get; set; }

        /// <summary>
        /// Группы потока
        /// </summary>
        public ICollection<StudyClass> StudyClassList { get; set; }
    }
}
