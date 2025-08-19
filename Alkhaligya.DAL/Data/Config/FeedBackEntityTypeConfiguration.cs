using Alkhaligya.DAL.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Data.Config
{
    public class ProductFeedbackConfiguration : IEntityTypeConfiguration<ProductFeedback>
    {
        public void Configure(EntityTypeBuilder<ProductFeedback> builder)
        {
            builder.ToTable("ProductFeedbacks");

            builder.HasKey(pf => pf.Id);

            builder.HasOne(pf => pf.User)
                .WithMany(u => u.ProductFeedbacks) 
                .HasForeignKey(pf => pf.UserId)
                .OnDelete(DeleteBehavior.Cascade); // حذف التقييم عند حذف المستخدم

            builder.HasOne(pf => pf.Product)
                .WithMany(p => p.ProductFeedbacks)
                .HasForeignKey(pf => pf.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // حذف التقييم عند حذف المنتج

            builder.Property(pf => pf.Rate)
                .IsRequired()
                .HasDefaultValue(1)
                .HasComment("التقييم من 1 إلى 5");

            builder.Property(pf => pf.Comment)
                .HasColumnType("NVARCHAR(1000)")
                .IsRequired(false);

            builder.Property(sf => sf.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(pf => pf.IsDeleted)
                .HasDefaultValue(false);
        }
    }

    public class SiteFeedbackConfiguration : IEntityTypeConfiguration<SiteFeedback>
    {
        public void Configure(EntityTypeBuilder<SiteFeedback> builder)
        {
            builder.ToTable("SiteFeedbacks");

            builder.HasKey(sf => sf.Id);

            builder.HasOne(sf => sf.User)
                .WithMany(u => u.SitetFeedbacks) 
                .HasForeignKey(sf => sf.UserId)
                .OnDelete(DeleteBehavior.Cascade); // حذف التقييم عند حذف المستخدم

            builder.Property(sf => sf.Rating)
                .IsRequired()
                .HasDefaultValue(1)
                .HasComment("التقييم من 1 إلى 5");

            builder.Property(sf => sf.Comment)
                .HasColumnType("NVARCHAR(1000)")
                .IsRequired(false);

            builder.Property(sf => sf.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // حذف منطقي
            builder.Property(sf => sf.IsDeleted)
                .HasDefaultValue(false);
        }
    }
}
