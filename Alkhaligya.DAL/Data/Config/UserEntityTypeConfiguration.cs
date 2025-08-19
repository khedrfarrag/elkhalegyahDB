using Alkhaligya.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Data.Config
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("ApplicationUser");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.FirstName)
              .HasColumnType("NVARCHAR(255)")
              .IsRequired();

            builder.Property(u => u.LastName)
                   .HasColumnType("NVARCHAR(255)")
                   .IsRequired();


            builder.Property(u => u.Address)
                   .HasColumnType("NVARCHAR(255)");

            builder.Property(u => u.ImageUrl)
                   .HasMaxLength(500);

            builder.Property(u => u.City)
                   .HasConversion<int>()
                   .IsRequired();

        }
    }
}
