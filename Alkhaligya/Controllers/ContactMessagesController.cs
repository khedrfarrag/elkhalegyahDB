using Alkhaligya.BLL.Dtos.Auth;
using Alkhaligya.BLL.Dtos.Contact;
using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.BLL.Services.Contact;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ContactMessagesController : ControllerBase
{
    private readonly IContactMessageService _contactMessageService;

    public ContactMessagesController(IContactMessageService contactMessageService)
    {
        _contactMessageService = contactMessageService;
    }


    [HttpGet("all-messages")]
    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
    public async Task<IActionResult> GetAllMessages([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _contactMessageService.GetAllAsync(pageNumber, pageSize);

        return response.Succeeded
            ? Ok(new { data = response.Data, pagination = response.Pagination })
            : BadRequest(response.Errors);
    }



    [HttpGet("{id}")]
    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _contactMessageService.GetByIdAsync(id);
        return response.Succeeded ? Ok(response.Data) : NotFound(response.Errors);
    }


    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateContactMessageDto dto)
    {
        var response = await _contactMessageService.CreateAsync(dto);
        return response.Succeeded ? Ok(response.Message) : BadRequest(response.Errors);
    }


    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateContactMessageDto dto)
    {
        if (id != dto.Id)
            return BadRequest(new ApiResponse<string>("ID mismatch"));

        var response = await _contactMessageService.UpdateMessageAsync(dto);
        return response.Succeeded ? Ok(response.Message) : NotFound(response.Errors);
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _contactMessageService.DeleteAsync(id);
        return response.Succeeded ? Ok(response.Message) : NotFound(response.Errors);
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetMessagesByUserId(string userId)
    {
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId != userId && !User.IsInRole("Admin"))
            return Unauthorized("You can only view your own messages");

        var response = await _contactMessageService.GetMessagesByUserIdAsync(userId);
        return response.Succeeded ? Ok(response.Data) : NotFound(response.Errors);
    }
}
