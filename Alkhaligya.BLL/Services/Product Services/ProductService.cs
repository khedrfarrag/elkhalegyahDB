using Alkhaligya.BLL.Dtos.Order;
using Alkhaligya.BLL.Dtos.ProductDtos;
using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.BLL.Services.Cashing;
using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.UnitOfWork;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<ApiResponse<List<ProductReadDto>>> GetAllProductsAsync(int pageNumber = 1, int pageSize = 8)
        {
            var query = _unitOfWork.Products.GetAll();
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ProductReadDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var response = new ApiResponse<List<ProductReadDto>>(products);
            response.Pagination = new PaginationDto
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
            return response;
        }

        public async Task<ApiResponse<ProductReadDto>> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                return new ApiResponse<ProductReadDto>("لم يتم العثور على المنتج");

            return new ApiResponse<ProductReadDto>(_mapper.Map<ProductReadDto>(product));
        }

        public async Task<ApiResponse<string>> AddProductAsync(ProductAddDto dto)
        {
            // Validate category exists
            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return new ApiResponse<string>("لم يتم العثور على التصنيف");

            // Validate subcategory exists if provided
            if (dto.SubCategoryId.HasValue && dto.SubCategoryId.Value != 0)
            {
                var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(dto.SubCategoryId.Value);
                if (subCategory == null)
                    return new ApiResponse<string>("لم يتم العثور على التصنيف الفرعي");
            }

            string imageUrl = null;

            if (dto.ImageUrl != null && dto.ImageUrl.Length > 0)
            {
                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageUrl.FileName);
                var filePath = Path.Combine(imagesFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageUrl.CopyToAsync(stream);
                }

                imageUrl = $"/images/{fileName}";
            }

            var product = _mapper.Map<Product>(dto);
            product.ImageUrl = imageUrl;
            product.CalculateDiscountedPrice();

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CommitChangesAsync();

            return new ApiResponse<string>(null, "تمت إضافة المنتج بنجاح");
        }

        public async Task<ApiResponse<string>> UpdateProductAsync(int id, ProductUpdateDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                return new ApiResponse<string>("لم يتم العثور على المنتج");

            // Update basic product fields
            product.Name = dto.Name;
            product.Price = dto.Price ?? product.Price;
            product.StockQuantity = dto.StockQuantity ?? product.StockQuantity;
            if (dto.DiscountPercentage.HasValue)
            {
                product.DiscountPercentage = dto.DiscountPercentage;
                product.CalculateDiscountedPrice();
            }
            product.Description = dto.Description;

            // Validate category exists if provided
            if (dto.CategoryId.HasValue)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId.Value);
                if (category == null)
                    return new ApiResponse<string>("لم يتم العثور على التصنيف");
                product.CategoryId = dto.CategoryId.Value;
            }

            // Validate subcategory exists if provided
            if (dto.SubCategoryId.HasValue && dto.SubCategoryId.Value != 0)
            {
                var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(dto.SubCategoryId.Value);
                if (subCategory == null)
                    return new ApiResponse<string>("لم يتم العثور على التصنيف الفرعي");
                product.SubCategoryId = dto.SubCategoryId.Value;
            }
            else if (dto.SubCategoryId.HasValue && dto.SubCategoryId.Value == 0)
            {
                product.SubCategoryId = null;
            }

            // Update string fields, allowing null values
            product.Title1 = dto.Title1;
            product.Body1 = dto.Body1;
            product.Title2 = dto.Title2;
            product.Body2 = dto.Body2;

            // Handle optional image
            if (dto.ImageUrl != null)
            {
                // Delete old image if it exists
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImageUrl.TrimStart('/'));
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImageUrl.FileName)}";
                var path = Path.Combine("wwwroot/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await dto.ImageUrl.CopyToAsync(stream);
                }

                product.ImageUrl = $"/images/{fileName}";
            }

            await _unitOfWork.CommitChangesAsync();
            return new ApiResponse<string>(null, "تم تحديث المنتج بنجاح");
        }

        public async Task<ApiResponse<string>> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                return new ApiResponse<string>("لم يتم العثور على المنتج");

            // Delete the product's image file if it exists
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImageUrl.TrimStart('/'));
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }

            // Mark all favorites for this product as deleted
            var favorites = await _unitOfWork.UserFavorites.GetAll()
                .Where(uf => uf.ProductId == id && !uf.IsDeleted)
                .ToListAsync();

            foreach (var favorite in favorites)
            {
                favorite.IsDeleted = true;
            }

            await _unitOfWork.Products.DeleteByIdAsync(id);
            await _unitOfWork.CommitChangesAsync();
            return new ApiResponse<string>(null, "تم حذف المنتج بنجاح");
        }

        public async Task<ApiResponse<List<ProductReadDto>>> SearchProductsAsync(string productName, int pageNumber = 1, int pageSize = 8)
        {
            var query = _unitOfWork.Products.GetAll()
                .Where(p => p.Name.ToLower().Contains(productName.ToLower()));

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ProductReadDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var response = new ApiResponse<List<ProductReadDto>>(products);
            response.Pagination = new PaginationDto
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
            return response;
        }

        public async Task<ApiResponse<int>> GetOutOfStockCountAsync()
        {
            var count = await _unitOfWork.Products.GetAll()
                .Where(p => p.StockQuantity == 0)
                .CountAsync();

            return new ApiResponse<int>(count);
        }

        public async Task<ApiResponse<List<ProductReadDto>>> GetTopDiscountedProductsAsync(int pageNumber = 1, int pageSize = 1)
        {
            // Get the top 4 discounted products
            var products = await _unitOfWork.Products.GetAll()
                .Where(p => p.DiscountPercentage != null && p.DiscountPercentage > 0)
                .OrderByDescending(p => p.DiscountPercentage)
                .Take(4)
                .ProjectTo<ProductReadDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var totalCount = products.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Paginate the top 4 in-memory
            var pagedProducts = products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new ApiResponse<List<ProductReadDto>>(pagedProducts);
            response.Pagination = new PaginationDto
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
            return response;
        }

        public async Task<ApiResponse<string>> MarkProductAsPopularAsync(int productId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
                return new ApiResponse<string>("لم يتم العثور على المنتج");

            product.IsPopular = true;
            await _unitOfWork.CommitChangesAsync();
            return new ApiResponse<string>(null, "تم تمييز المنتج كمميز بنجاح");
        }

        public async Task<ApiResponse<string>> MarkProductAsNotPopularAsync(int productId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
                return new ApiResponse<string>("لم يتم العثور على المنتج");

            product.IsPopular = false;
            await _unitOfWork.CommitChangesAsync();
            return new ApiResponse<string>(null, "تم إزالة تمييز المنتج كمميز بنجاح");
        }

        public async Task<ApiResponse<List<ProductReadDto>>> GetPopularProductsAsync(int pageNumber = 1, int pageSize = 8)
        {
            var query = _unitOfWork.Products.GetAll().Where(p => p.IsPopular);
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ProductReadDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var response = new ApiResponse<List<ProductReadDto>>(products);
            response.Pagination = new PaginationDto
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
            return response;
        }

        public async Task<ApiResponse<List<ProductReadDto>>> FilterProductsAsync(ProductFilterDto filter, int pageNumber = 1, int pageSize = 8)
        {
            string cacheKey = $"filter:{filter.CategoryId}:{filter.SubCategoryId}:{filter.HasDiscount}:{filter.MinPrice}:{filter.MaxPrice}:{filter.FeedbackScore}:{pageNumber}:{pageSize}";

            var cached = await _cacheService.GetAsync<List<ProductReadDto>>(cacheKey);
            if (cached != null)
            {
                var cachedResponse = new ApiResponse<List<ProductReadDto>>(cached);
                // You may want to cache pagination info as well if needed
                return cachedResponse;
            }

            var query = _unitOfWork.Products.GetAll().Where(p => !p.IsDeleted);

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

            if (filter.SubCategoryId.HasValue)
                query = query.Where(p => p.SubCategoryId == filter.SubCategoryId.Value);

            if (filter.HasDiscount.HasValue)
            {
                if (filter.HasDiscount.Value)
                {
                    query = query.Where(p => p.DiscountPercentage != null && p.DiscountPercentage > 0);
                }
                else
                {
                    query = query.Where(p => p.DiscountPercentage == null || p.DiscountPercentage == 0);
                }
            }

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            if (filter.FeedbackScore.HasValue)
            {
                var targetScore = filter.FeedbackScore.Value;
                query = query.Where(p =>
                    (!p.ProductFeedbacks.Any() && targetScore == 0)
                    ||
                    (p.ProductFeedbacks.Any() &&
                     Math.Round(p.ProductFeedbacks.Average(f => f.Rate)) == targetScore)
                );
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ProductReadDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            await _cacheService.SetAsync(cacheKey, products, TimeSpan.FromHours(7));

            var response = new ApiResponse<List<ProductReadDto>>(products);
            response.Pagination = new PaginationDto
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
            return response;
        }

        public async Task<ApiResponse<List<ProductReadDto>>> GetProductsByCategoryIdAsync(int categoryId)
        {
            var products = await _unitOfWork.Products.GetAll()
                .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
                .ProjectTo<ProductReadDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (products == null || !products.Any())
                return new ApiResponse<List<ProductReadDto>>("لم يتم العثور على منتجات لهذا القسم");

            return new ApiResponse<List<ProductReadDto>>(products);
        }

        public async Task<ApiResponse<string>> AddToFavoritesAsync(string userId, int productId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
                return new ApiResponse<string>("لم يتم العثور على المنتج");

            var existingFavorite = await _unitOfWork.UserFavorites.GetAll()
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId && !f.IsDeleted);

            if (existingFavorite != null)
                return new ApiResponse<string>("المنتج موجود بالفعل في المفضلة");

            // Add to favorites
            await _unitOfWork.UserFavorites.AddAsync(new UserFavorite
            {
                UserId = userId,
                ProductId = productId,
                IsDeleted = false
            });

            await _unitOfWork.CommitChangesAsync();
            return new ApiResponse<string>(null, "تم إضافة المنتج إلى المفضلة بنجاح");
        }

        public async Task<ApiResponse<string>> RemoveFromFavoritesAsync(string userId, int productId)
        {
            var favorite = await _unitOfWork.UserFavorites.GetAll()
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId && !f.IsDeleted);

            if (favorite == null)
                return new ApiResponse<string>("المنتج غير موجود في المفضلة");

            favorite.IsDeleted = true;
            await _unitOfWork.CommitChangesAsync();
            return new ApiResponse<string>(null, "تم إزالة المنتج من المفضلة بنجاح");
        }

        public async Task<ApiResponse<List<ProductReadDto>>> GetUserFavoritesAsync(string userId, int pageNumber = 1, int pageSize = 8)
        {
            var query = _unitOfWork.UserFavorites.GetAll()
                .Where(f => f.UserId == userId && !f.IsDeleted)
                .Join(_unitOfWork.Products.GetAll(),
                    favorite => favorite.ProductId,
                    product => product.Id,
                    (favorite, product) => product)
                .Where(p => !p.IsDeleted);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ProductReadDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var response = new ApiResponse<List<ProductReadDto>>(products);
            response.Pagination = new PaginationDto
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
            return response;
        }

        public async Task<ApiResponse<List<ProductReadDto>>> GetDiscountedProductsAsync(int pageNumber = 1, int pageSize = 8)
        {
            var query = _unitOfWork.Products.GetAll().Where(p => p.DiscountPercentage != null && p.DiscountPercentage > 0);
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ProductReadDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var response = new ApiResponse<List<ProductReadDto>>(products);
            response.Pagination = new PaginationDto
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
            return response;
        }
    }
}