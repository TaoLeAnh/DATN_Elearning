using Elearning.Publising.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Elearning.Publising.API.Controllers.v1.Publising
{
    [ApiController]
    [Route("api/v1/Publising/[controller]")]
    public class TienDoHocController : ControllerBase
    {
        private readonly ITienDoHocService _service;

        public TienDoHocController(ITienDoHocService service)
        {
            _service = service;
        }

        [HttpGet("khoa-hoc/{courseId:guid}/user/{userId:guid}")]
        public async Task<ActionResult<List<Guid>>> GetCompletedLessons(Guid courseId, Guid userId)
        {
            var result = await _service.GetCompletedLessonIdsAsync(courseId, userId);
            return Ok(result);
        }

        [HttpPost("mark-complete")]
        public async Task<ActionResult<bool>> MarkComplete([FromBody] MarkCompleteRequest request)
        {
            try
            {
                var result = await _service.MarkLessonCompleteAsync(request.NguoiDungId, request.BaiHocId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class MarkCompleteRequest
    {
        public Guid NguoiDungId { get; set; }
        public Guid BaiHocId { get; set; }
    }
}
