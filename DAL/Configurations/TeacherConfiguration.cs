using DAL.Entities.Schedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;

namespace DAL.Configurations
{
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder
            .Property(e => e.WeekDays)
            .HasConversion(
                v => string.Join(',', v.OrderBy(v => v).Select(n => n.ToString())),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(n => Convert.ToInt32(n)).ToArray());

            builder
            .Property(e => e.LessonNumbers)
            .HasConversion(
                v => string.Join(',', v.OrderBy(v => v).Select(n => n.ToString())),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(n => Convert.ToInt32(n)).ToArray());
        }
    }
}
