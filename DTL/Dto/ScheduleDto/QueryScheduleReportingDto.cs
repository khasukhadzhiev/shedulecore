using System;

namespace DTL.Dto.ScheduleDto
{
    public class QueryScheduleReportingDto
    {
        public int StudyClassId { get; set; }

        public string StudyClassName { get; set; }

        public int SubjectId { get; set; }

        public string SubjectName { get; set; }

        public int TeacherId { get; set; }

        public string TeacherName { get; set; }

        public int ReportingTypeId { get; set; }

        public string ReportingTypeName { get; set; }

        public int? ClassroomId { get; set; }

        public string ClassroomName { get; set; }

        public DateTime? Date { get; set; }

        public int VersionId { get; set; }
    }
}
