namespace DTL.Dto.ScheduleDto
{
    public class StudyClassReportingDto : BaseDto
    {
        public int StudyClassId { get; set; }

        public int SubjectId { get; set; }

        public int TeacherId { get; set; }

        public int ReportingTypeId { get; set; }

        public int? ClassroomId { get; set; }

        public int VersionId { get; set; }

        public DateTime? Date { get; set; }

        public StudyClassDto StudyClass { get; set; }

        public SubjectDto Subject { get; set; }

        public TeacherDto Teacher { get; set; }

        public ReportingTypeDto ReportingType { get; set; }

        public ClassroomDto Classroom { get; set; }

        public VersionDto Version { get; set; }
    }
}
