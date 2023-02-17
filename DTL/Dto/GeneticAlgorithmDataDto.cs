using DTL.Dto.ScheduleDto;
using System.Collections.Generic;

namespace DTL.Dto
{
    public class GeneticAlgorithmDataDto
    {
        public int VersionId { get; set; }
        public int ClassShiftId { get; set; }
        public bool AllowEmptyLesson { get; set; }
        public List<EducationFormDto> EducationFormData { get; set; }
    }
}
