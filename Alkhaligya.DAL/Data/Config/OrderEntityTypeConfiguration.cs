using Alkhaligya.DAL.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alkhaligya.DAL.Models.PayMob;

namespace Alkhaligya.DAL.Data.Config
{
    public class PaymentTransactionEntityTypeConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.ToTable("PaymentTransactions");

            builder.HasKey(pt => pt.Id);

            builder.Property(pt => pt.TransactionId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pt => pt.OrderId)
                .IsRequired();

            builder.Property(pt => pt.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(pt => pt.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("EGP");

            builder.Property(pt => pt.PaymentMethod)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(pt => pt.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(pt => pt.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(pt => pt.PaymobOrderId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pt => pt.IsDeleted)
                .HasDefaultValue(false);

            // علاقة One-to-One مع Order
            builder.HasOne(pt => pt.Order)
                .WithOne(o => o.PaymentTransaction)
                .HasForeignKey<PaymentTransaction>(pt => pt.OrderId);

            //Soft Delete Query Filter
            builder.HasQueryFilter(pt => !pt.IsDeleted);
        }
    }

    public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.OrderDate)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(o => o.TotalPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(o => o.TotalQuantity)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(o => o.UserId)
                .IsRequired(false); 

            builder.Property(o => o.SessionId)
                .HasMaxLength(100) 
                .IsRequired(false); 

            builder.Property(o => o.IsGuestOrder)
                .HasDefaultValue(false);

            builder.Property(o => o.PaymentStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(PaymentStatus.Pending);

            builder.Property(o => o.PaymentTransactionId)
                .IsRequired(false);

            builder.Property(o => o.IsDeleted)
                .HasDefaultValue(false);

            // بيانات الفورم
            builder.Property(o => o.FirstName)
                .HasMaxLength(100)
                .HasColumnType("NVARCHAR(255)")
                .IsRequired();

            builder.Property(o => o.LastName)
                .HasMaxLength(100)
                .HasColumnType("NVARCHAR(255)")
                .IsRequired();

             builder.Property(o => o.MobileNumber)
             .HasMaxLength(11)         
             .IsRequired();            


            builder.Property(o => o.Address)
                 .HasColumnType("NVARCHAR(255)")
                 .IsRequired();

            builder.Property(o => o.Governorate)
                .IsRequired()
                .HasConversion<string>(); 

            builder.Property(o => o.PaymentMethod)
                .IsRequired()
                .HasConversion<string>();

            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

         
            builder.HasOne(o => o.PaymentTransaction)
                .WithOne(pt => pt.Order)
                .HasForeignKey<PaymentTransaction>(pt => pt.OrderId);

            builder.Property(o => o.IsDeleted)
            .HasDefaultValue(false);

        }
    }

    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(oi => oi.Id);

            builder.HasOne(oi => oi.Order)
           .WithMany(o => o.OrderItems)
           .HasForeignKey(oi => oi.OrderId)
           .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false); // ← هذا هو الصح



            builder.HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // لا يتم حذف المنتج عند حذف الطلب


            builder.Property(oi => oi.Quantity)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(oi => oi.TotalPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");


            builder.Property(oi => oi.IsDeleted)
                .HasDefaultValue(false);
        }
    }
}
