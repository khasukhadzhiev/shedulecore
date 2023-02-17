using System;
namespace Infrastructure
{
	public class ScheduleGeneratedProgress
	{
        public bool Start { get; set; }

        public bool End { get; set; }

        public bool SaveWithMistakes { get; set; }

        public int Generation { get; set; }

        public List<string> MistakeType { get; set; }

        public string Message { get; set; }

        public string SpentTime { get; set; }
    }
}

