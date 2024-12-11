using DAL.Entities.Schedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;

namespace DAL.Configurations
{
    public class FlowTeachersIdsConfiguration : IEntityTypeConfiguration<Flow>
    {
        public void Configure(EntityTypeBuilder<Flow> builder)
        {
            builder
            .Property(f => f.TeachersIds)
            .HasConversion(
                v => string.Join(',', v.OrderBy(v => v).Select(n => n.ToString())),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(n => Convert.ToInt32(n)).ToArray(),
                        new ValueComparer<int[]>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToArray()));
        }
    }
}
