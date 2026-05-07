using Elearning.Publising.Application.Interfaces;
using Elearning.Shared.Contracts.Portal.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Elearning.Publising.API.Controllers.v1.Publising
{
    [ApiController]
    [Route("api/v1/Publising/[controller]")]
    // [Authorize] // Bật cái này nếu bạn bắt buộc phải có Token API hợp lệ
    public class BaiLamController : ControllerBase
    {
        private readonly IBaiLamService _service;

        public BaiLamController(IBaiLamService service)
        {
            _service = service;
        }

        [HttpGet("history-quiz/{quizId:guid}/user/{userId:guid}")]
        public async Task<ActionResult<List<QuizHistoryDto>>> GetQuizHistory(Guid quizId, Guid userId)
        {
            try
            {
                var result = await _service.GetQuizHistoryAsync(quizId, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
