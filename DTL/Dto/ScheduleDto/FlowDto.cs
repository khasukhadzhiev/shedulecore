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
        public ICollection<TeacherDto> TeacherList { get; set; }

        /// <summary>
        /// Группы потока
        /// </summary>
        public ICollection<StudyClassDto> StudyClassList { get; set; }
    }
}
