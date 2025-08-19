using Alkhaligya.BLL.Dtos.Auth;
using Alkhaligya.BLL.Dtos.CategoryDtos;
using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.BLL.Services.CategoryServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // Get all categories - accessible to authenticated users
    [HttpGet]
    public async Task<IActionResult> GetAllCategoriesAsync()
    {
        var response = await _categoryService.GetAllCategoriesAsync();
        return response.Succeeded ? Ok(response.Data) : BadRequest(response.Errors);
    }

    // Get category by ID - public
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryByIdAsync(int id)
    {
        var response = await _categoryService.GetCategoryByIdAsync(id);
        return response.Succeeded ? Ok(response.Data) : NotFound(response.Errors);
    }

    // Add new category - restricted to Admin
    [HttpPost]
    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
    public async Task<IActionResult> AddCategoryAsync([FromBody] CategoryAddDto dto)
    {
        var response = await _categoryService.AddCategoryAsync(dto);
        return response.Succeeded ? Ok(response.Message) : BadRequest(response.Errors);
    }

    // Update existing category - restricted to Admin
    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
    public async Task<IActionResult> UpdateCategoryAsync(int id, [FromBody] CategoryUpdateDto dto)
    {
        var response = await _categoryService.UpdateCategoryAsync(id, dto);
        return response.Succeeded ? Ok(response.Message) : NotFound(response.Errors);
    }

    // Delete a category - restricted to Admin
    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
    public async Task<IActionResult> DeleteCategoryAsync(int id)
    {
        var response = await _categoryService.DeleteCategoryAsync(id);
        return response.Succeeded ? Ok(response.Message) : NotFound(response.Errors);
    }

    // Get subcategories by category ID - public
    [HttpGet("{id}/subcategories")]
    public async Task<IActionResult> GetSubCategoriesByCategoryIdAsync(int id)
    {
        var response = await _categoryService.GetSubCategoriesByCategoryIdAsync(id);
        return response.Succeeded ? Ok(response.Data) : NotFound(response.Errors);
    }
}
