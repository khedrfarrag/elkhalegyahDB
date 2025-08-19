using Alkhaligya.BLL.Dtos.SiteFeedbackDtos;
using Alkhaligya.BLL.Services.SiteFeedbackServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Alkhaligya.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SiteFeedbackController : ControllerBase
    {
        private readonly ISiteFeedbackService _siteFeedbackService;

        public SiteFeedbackController(ISiteFeedbackService siteFeedbackService)
        {
            _siteFeedbackService = siteFeedbackService;
        }

        // POST: api/SiteFeedback
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddSiteFeedback([FromBody] SiteFeedbackAddDto dto)
        {
            // The userId should now be present in the dto
            var response = await _siteFeedbackService.AddSiteFeedbackAsync(dto, dto.UserId);

            if (response.Succeeded)
            {
                return Ok(new { Message = response.Message, Succeeded = response.Succeeded });
            }

            return BadRequest(response.Errors);
        }

        // GET: api/SiteFeedback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllSiteFeedback([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _siteFeedbackService.GetAllSiteFeedbackAsync(pageNumber, pageSize);
            return response.Succeeded
                ? Ok(new { data = response.Data, pagination = response.Pagination })
                : BadRequest(response.Errors);
        }

        // DELETE: api/SiteFeedback/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSiteFeedback(int id)
        {
            var response = await _siteFeedbackService.DeleteSiteFeedbackAsync(id);

            if (response.Succeeded)
            {
                return NoContent(); // Successfully deleted, return 204 No Content
            }

            return NotFound(response); // Feedback with the given ID not found, return 404 Not Found with the response
        }
    }
}
