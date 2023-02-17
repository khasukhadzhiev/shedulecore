using System;
namespace Infrastructure
{
	public class ImportProgress
	{
        public int TotalLessonCount { get; set; }

        public int CheckedLessonCount { get; set; }

        public bool ImportError { get; set; }

        public string ErrorMessage { get; set; }

        public bool ImportFinished { get; set; }

        public bool InProcess { get; set; }
    }
}

