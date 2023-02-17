namespace DTL.Dto.ScheduleDto
{
    /// <summary>
    /// Форма обучения
    /// </summary>
    public class EducationFormDto : BaseDto
    {
        public int[] WeekDays { get; set; } = { 0, 1, 2, 3, 4, 5 };
    }
}
