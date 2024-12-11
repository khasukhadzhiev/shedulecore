using DAL.Entities.Schedule;

namespace DTL.Dto.ScheduleDto
{
    /// <summary>
    /// Поток
    /// </summary>
    public class FlowDto : BaseDto
    {
        /// <summary>
        /// Преподаватели потока
        /// </summary>
        public ICollection<Teacher> TeacherList { get; set; }
    }
}
