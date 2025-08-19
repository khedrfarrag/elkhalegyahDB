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


    public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.Description)
                .HasColumnType("nvarchar(max)");

            builder.Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(p => p.StockQuantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(p => p.DiscountPercentage)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValue(null);

            builder.Property(p => p.DiscountedPrice)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(null);

            builder.Property(p => p.ImageUrl)
                .HasMaxLength(500); // optional

            builder.Property(p => p.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(p => p.IsPopular)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(p => p.SubCategory)
                .WithMany(sc => sc.Products)
                .HasForeignKey(p => p.SubCategoryId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }

    //public class ProductDetailEntityTypeConfiguration : IEntityTypeConfiguration<ProductDetail>
    //{
    //    public void Configure(EntityTypeBuilder<ProductDetail> builder)
    //    {

    //        builder.ToTable("ProductDetails");


    //        builder.HasKey(pd => pd.Id);


    //        builder.Property(pd => pd.Title)
    //            .IsRequired()
    //            .HasMaxLength(255);


    //        builder.Property(pd => pd.Description)
    //            .HasColumnType("nvarchar(max)");

    //    }
    //}

    public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);


            // Category -> SubCategory (One to Many)
            builder.HasMany(c => c.SubCategories)
                .WithOne(sc => sc.Category)
                .HasForeignKey(sc => sc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);


            // Category -> product (One to Many)
            builder.HasMany(c => c.Products)
                .WithOne(sc => sc.Category)
                .HasForeignKey(sc => sc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }

    public class SubCategoryEntityTypeConfiguration : IEntityTypeConfiguration<SubCategory>
    {
        public void Configure(EntityTypeBuilder<SubCategory> builder)
        {
            builder.ToTable("SubCategories");

            builder.HasKey(sc => sc.Id);

            builder.Property(sc => sc.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(sc => sc.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // SubCategory -> Product (One to Many)
            builder.HasMany(sc => sc.Products)
                .WithOne(p => p.SubCategory)
                .HasForeignKey(p => p.SubCategoryId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }


}
