using Elearning.Publising.Application.Interfaces;
using Elearning.Shared.Contracts.Portal.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Elearning.Publising.API.Controllers.v1.Publising
{
    [ApiController]
    [Route("api/v1/Publising/[controller]")]
    public class ChatbotController : ControllerBase
    {
        private readonly IChatbotService _service;

        public ChatbotController(IChatbotService service)
        {
            _service = service;
        }

        [HttpPost("ask-tutor")]
        public async Task<IActionResult> AskTutor([FromBody] ChatbotRequestDto request)
        {
            try
            {
                var answer = await _service.AskTutorAsync(request);
                return Ok(new { success = true, reply = answer });
            }
            catch (Exception ex)
            {
                // Bắt lỗi nếu gọi AI xịt hoặc đứt cáp
                return StatusCode(500, new { success = false, reply = "Xin lỗi, đường truyền đến não bộ AI đang bị gián đoạn. Bạn thử lại sau nhé! Lỗi: " + ex.Message });
            }
        }
    }
}
