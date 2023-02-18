using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;

namespace DAL.Configurations
{
    public class VersionConfiguration : IEntityTypeConfiguration<DAL.Entities.Version>
    {
        public void Configure(EntityTypeBuilder<DAL.Entities.Version> builder)
        {
            builder
            .Property(e => e.ShowReportingIds)
            .HasConversion(
                v => string.Join(',', v.OrderBy(v => v)),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries),
                        new ValueComparer<string[]>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToArray()));
        }
    }
}
