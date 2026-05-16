using Elearning.Application.Interfaces;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos.KyThi;
using Elearning.Shared.Contracts.Portal.Forms.KyThi;
using Elearning.Shared.Contracts.Portal.Querys.KyThi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elearning.API.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class KyThiController : ControllerBase
    {
        private readonly IKyThiService _service;

        public KyThiController(IKyThiService service)
        {
            _service = service;
        }

        [HttpPost("getpaged")]
        public async Task<ActionResult<DataTableJson>> GetPaged([FromBody] KyThiQuery query)
        {
            var result = await _service.GetPaged(query);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<KyThiDto>> GetById(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] KyThiForm form)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = await _service.CreateAsync(form);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] KyThiForm form)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(id, form);
            if (!updated) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }

        [HttpGet("{id:guid}/cau-hinh")]
        public async Task<ActionResult<List<CauHoiKyThiDto>>> GetCauHinh(Guid id)
        {
            var result = await _service.GetCauHinhDeThiAsync(id);
            return Ok(result);
        }

        [HttpPost("{id:guid}/cau-hinh")]
        public async Task<IActionResult> SaveCauHinh(Guid id, [FromBody] CauHinhDeThiForm form)
        {
            var success = await _service.SaveCauHinhDeThiAsync(id, form);
            if (!success) return BadRequest("Lỗi khi lưu cấu hình.");
            return Ok();
        }

        [HttpPost("{id:guid}/cau-hinh/random")]
        public async Task<IActionResult> GenerateRandomExam(Guid id, [FromBody] MaTranDeThiForm form)
        {
            try
            {
                var success = await _service.GenerateRandomExamAsync(id, form);
                if (!success) return BadRequest("Lỗi khi tạo đề thi tự động.");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // Thêm class này ở đầu file hoặc ném vào folder DTO
        public class RandomTheoMaTranRequest
        {
            public Guid KyThiId { get; set; }
            public Guid MaTranId { get; set; }
        }

        [HttpPost("random-theo-matran")]
        public async Task<IActionResult> RandomTheoMaTran([FromBody] RandomTheoMaTranRequest request)
        {
            try
            {
                var success = await _service.GenerateRandomExamTheoMaTranAsync(request.KyThiId, request.MaTranId);

                if (!success)
                {
                    // Trả về Object JSON để Frontend map được vào ResponseErrorAPI
                    return BadRequest(new { Message = "Lỗi không xác định khi tạo đề thi." });
                }

                // Trả về true cho trường hợp Success
                return Ok(true);
            }
            catch (Exception ex)
            {
                // Trả về Object JSON chứa câu thông báo lỗi
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
