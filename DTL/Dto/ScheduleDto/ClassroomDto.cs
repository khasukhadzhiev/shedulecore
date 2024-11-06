namespace DTL.Dto.ScheduleDto
{
    /// <summary>
    /// Аудитория
    /// </summary>
    public class ClassroomDto : BaseDto
    {
        /// <summary>
        /// Id типа аудитории
        /// </summary>
        public int ClassroomTypeId { get; set; }

        /// <summary>
        /// Число посадочных мест
        /// </summary>
        public int SeatsCount { get; set; }

        /// <summary>
        /// Id типа аудитории
        /// </summary>
        public int BuildingId { get; set; }

        /// <summary>
        /// Учебный корпус
        /// </summary>
        public BuildingDto BuildingDto { get; set; }

        /// <summary>
        /// Тип аудитории
        /// </summary>
        public ClassroomTypeDto ClassroomTypeDto { get; set; }
    }
}
