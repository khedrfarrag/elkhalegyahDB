using Alkhaligya.BLL.Dtos.CategoryDtos;
using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.UnitOfWork;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IQueryable<CategoryReadDto>>> GetAllCategoriesAsync()
        {
            // Include subcategories in the query
            var categories = await _unitOfWork.Categories.GetAll()
                .Include(c => c.SubCategories)
                .ToListAsync();

            // Map to DTOs with subcategories included
            var categoryDtos = categories
                .AsQueryable()
                .ProjectTo<CategoryReadDto>(_mapper.ConfigurationProvider);

            return new ApiResponse<IQueryable<CategoryReadDto>>(categoryDtos);
        }

        public async Task<ApiResponse<CategoryReadDto>> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return new ApiResponse<CategoryReadDto>("لم يتم العثور على التصنيف");

            return new ApiResponse<CategoryReadDto>(_mapper.Map<CategoryReadDto>(category));
        }

        public async Task<ApiResponse<string>> AddCategoryAsync(CategoryAddDto dto)
        {
            var category = _mapper.Map<Category>(dto); // This maps SubCategories too

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CommitChangesAsync();

            return new ApiResponse<string>(null, "تمت إضافة التصنيف والتصنيفات الفرعية بنجاح");
        }

        //public async Task<ApiResponse<string>> UpdateCategoryAsync(CategoryUpdateDto dto)
        //{
        //    var category = await _unitOfWork.Categories.GetByIdAsync(dto.Id);
        //    if (category == null)
        //        return new ApiResponse<string>("Category not found");

        //    _mapper.Map(dto, category);
        //    await _unitOfWork.CommitChangesAsync();
        //    return new ApiResponse<string>(null, "Category updated successfully");
        //}


        //public async Task<ApiResponse<string>> UpdateCategoryAsync(CategoryUpdateDto dto)
        //{
        //    var category = await _unitOfWork.Categories.GetByIdAsync(dto.Id);

        //    if (category == null)
        //        return new ApiResponse<string>("Category not found");

        //    // Update category name
        //    category.Name = dto.Name;

        //    // Track incoming subcategory IDs
        //    var incomingSubIds = dto.SubCategories.Select(s => s.Id).ToList();

        //    // 1. Update existing or add new
        //    foreach (var subDto in dto.SubCategories)
        //    {
        //        // Existing subcategory
        //        var existingSub = category.SubCategories.FirstOrDefault(s => s.Id == subDto.Id && s.Id != 0);
        //        if (existingSub != null)
        //        {
        //            existingSub.Name = subDto.Name;
        //        }
        //        // New subcategory (Id == 0)
        //        else if (subDto.Id == 0)
        //        {
        //            category.SubCategories.Add(new SubCategory
        //            {
        //                Name = subDto.Name
        //            });
        //        }
        //    }

        //    // 2. Remove subcategories not in the update list
        //    var toRemove = category.SubCategories
        //        .Where(s => s.Id != 0 && !incomingSubIds.Contains(s.Id))
        //        .ToList();

        //    foreach (var sub in toRemove)
        //    {
        //        // Optional: Soft delete
        //        // sub.IsDeleted = true;
        //        // Or actual removal:
        //        category.SubCategories.Remove(sub);
        //    }

        //    await _unitOfWork.CommitChangesAsync();
        //    return new ApiResponse<string>(null, "Category and subcategories updated successfully");
        //}



        //public async Task<ApiResponse<string>> UpdateCategoryAsync(CategoryUpdateDto dto)
        //{
        //    var category = await _unitOfWork.Categories.GetByIdAsync(dto.Id);

        //    if (category == null)
        //        return new ApiResponse<string>("Category not found");

        //    // Update category name
        //    category.Name = dto.Name;

        //    // Ensure SubCategories is initialized
        //    category.SubCategories ??= new List<SubCategory>();

        //    // Track incoming subcategory IDs
        //    var incomingSubIds = dto.SubCategories.Select(s => s.Id).ToList();

        //    // Update existing or add new subcategories
        //    foreach (var subDto in dto.SubCategories)
        //    {
        //        var existingSub = category.SubCategories.FirstOrDefault(s => s.Id == subDto.Id && s.Id != 0);
        //        if (existingSub != null)
        //        {
        //            existingSub.Name = subDto.Name;
        //        }
        //        else if (subDto.Id == 0)
        //        {
        //            category.SubCategories.Add(new SubCategory
        //            {
        //                Name = subDto.Name
        //            });
        //        }
        //    }

        //    await _unitOfWork.CommitChangesAsync();
        //    return new ApiResponse<string>(null, "Category and subcategories updated successfully");
        //}


        public async Task<ApiResponse<string>> UpdateCategoryAsync(int id ,CategoryUpdateDto dto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return new ApiResponse<string>("لم يتم العثور على التصنيف");

            category.Name = dto.Name;
            category.SubCategories ??= new List<SubCategory>();

            var incomingSubIds = dto.SubCategories.Select(s => s.Id).ToList();

            foreach (var subDto in dto.SubCategories)
            {
                var existingSub = category.SubCategories.FirstOrDefault(s => s.Id == subDto.Id && s.Id != 0);
                if (existingSub != null)
                {
                    existingSub.Name = subDto.Name;
                }
                else if (subDto.Id == 0)
                {
                    category.SubCategories.Add(new SubCategory
                    {
                        Name = subDto.Name
                    });
                }
            }

            // 💣 Remove subcategories not included in incoming list
            var toRemove = category.SubCategories
                .Where(s => s.Id != 0 && !incomingSubIds.Contains(s.Id))
                .ToList();

            foreach (var sub in toRemove)
            {
                category.SubCategories.Remove(sub);
            }

            await _unitOfWork.CommitChangesAsync();
            return new ApiResponse<string>(null, "تم تحديث التصنيف والتصنيفات الفرعية بنجاح");
        }




        public async Task<ApiResponse<string>> DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return new ApiResponse<string>("لم يتم العثور على التصنيف");

            // Check if category has related products
            if (category.Products != null && category.Products.Any())
            {
                return new ApiResponse<string>("لا يمكن حذف هذا القسم لأنه مرتبط بمنتجات");
            }

            await _unitOfWork.Categories.DeleteByIdAsync(id);
            await _unitOfWork.CommitChangesAsync();
            return new ApiResponse<string>(null, "تم حذف التصنيف بنجاح");
        }


        public async Task<ApiResponse<IQueryable<SubCategoryReadDto>>> GetSubCategoriesByCategoryIdAsync(int categoryId)
        {
            // Retrieve the category WITH its subcategories
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            if (category == null)
                return new ApiResponse<IQueryable<SubCategoryReadDto>>("لم يتم العثور على التصنيف");

            // Now, category.SubCategories should be loaded
            var subCategories = category.SubCategories.AsQueryable();

            // Map the subcategories to DTOs
            var subCategoryDtos = subCategories.ProjectTo<SubCategoryReadDto>(_mapper.ConfigurationProvider);

            return new ApiResponse<IQueryable<SubCategoryReadDto>>(subCategoryDtos);
        }

    }
}
