using System;
using DAL.Entities;

namespace DAL.Entities.Schedule
{
    public class StudyClassReporting : BaseEntity
    {
        public int StudyClassId { get; set; }

        public int SubjectId { get; set; }

        public int TeacherId { get; set; }

        public int ReportingTypeId { get; set; }

        public int? ClassroomId { get; set; }

        public int VersionId { get; set; }

        public DateTime? Date { get; set; }

        public StudyClass StudyClass { get; set; }

        public Subject Subject { get; set; }

        public Teacher Teacher { get; set; }

        public ReportingType ReportingType { get; set; }

        public Classroom Classroom { get; set; }

        public Version Version { get; set; }
    }
}
