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
 
    public class ContactMessageEntityTypeConfiguration : IEntityTypeConfiguration<ContactMessage>
    {
        public void Configure(EntityTypeBuilder<ContactMessage> builder)
        {
            builder.ToTable("ContactMessages");

            builder.HasKey(cm => cm.Id);

            builder.Property(cm => cm.Name)
                .IsRequired()
                .HasColumnType("NVARCHAR(255)");

            builder.Property(cm => cm.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(cm => cm.Phone)
                .HasMaxLength(20);

            builder.Property(cm => cm.Message)
                .IsRequired()
                .HasColumnType("NVARCHAR(1000)");

            builder.Property(cm => cm.SentAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(cm => cm.IsRead)
                .IsRequired()
                .HasDefaultValue(false);

            // العلاقة مع المستخدم (User)
            builder.HasOne(cm => cm.User)
                .WithMany(u => u.ContactMessages) 
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.SetNull); // لا نحذف الرسائل عند حذف المستخدم
        }
    }
}
