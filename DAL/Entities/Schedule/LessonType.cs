using DAL.Entities;

namespace DAL.Entities.Schedule
{
    /// <summary>
    /// Вид занятия
    /// </summary>
    public class LessonType : BaseEntity
    {
        /// <summary>
        /// Наименование вида занятия
        /// </summary>
        public string Name { get; set; }
    }
}
