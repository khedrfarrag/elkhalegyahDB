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

    public class CartShopEntityTypeConfiguration : IEntityTypeConfiguration<CartShop>
    {
        public void Configure(EntityTypeBuilder<CartShop> builder)
        {
            builder.ToTable("CartShops");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.SessionId)
                .HasMaxLength(100)
                .IsRequired(false);


            builder.Property(o => o.UserId)
              .IsRequired(false); // لو Guest

            builder.HasOne(c => c.User)
                .WithOne(u => u.CartShop)
                .HasForeignKey<CartShop>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.CartItems)
                .WithOne(ci => ci.CartShop)
                .HasForeignKey(ci => ci.CartShopId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.IsDeleted)
                .HasDefaultValue(false);

            builder.Ignore(nameof(CartShop.GetTotalQuantity));
            builder.Ignore(nameof(CartShop.GetTotalPrice));

        }
    }

    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");

            builder.HasKey(ci => ci.Id);

           
            builder.HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // لا يتم حذف المنتج عند حذف العنصر من العربة

            builder.Property(ci => ci.Quantity)
                .IsRequired()
                .HasDefaultValue(1);

              
            builder.Property(ci => ci.IsDeleted)
                .HasDefaultValue(false);
        }
    }
}


