﻿using QualityMeter.Core.Models;
using System.Data.Entity;

namespace QualityMeter.Infrastructure.Data
{
    public class EfQualityMeterBaseDb : DbContext
    {
        public EfQualityMeterBaseDb()
            : base("QualityMeterDB")
        {


        }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Factor> Factors { get; set; }
        public DbSet<Criteria> Criterias { get; set; }
        public DbSet<QualityAttributesMetric> QualityAttributesMetrics { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QualityAttributesMetric>().
                HasOptional(e => e.RelatedTo).
                WithMany().
                HasForeignKey(m => m.RelatedToId);

            modelBuilder.Entity<QualityAttributesMetric>().
                HasOptional(e => e.Aganist).
                WithMany().
                HasForeignKey(m => m.AganistId);

            modelBuilder.Entity<Factor>().HasRequired(i => i.Subject).WithMany().WillCascadeOnDelete(false);

            modelBuilder.Entity<QualityAttributesMetric>().HasRequired(i => i.Factor).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<QualityAttributesMetric>().HasRequired(i => i.Criteria).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<QualityAttributesMetric>().HasRequired(i => i.Subject).WithMany().WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}