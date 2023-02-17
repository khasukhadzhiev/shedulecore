using Microsoft.EntityFrameworkCore;
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
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
