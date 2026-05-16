using Elearning.Application.Interfaces;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elearning.API.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MaTranDeThiMacDinhController : ControllerBase
    {
        private readonly IMaTranDeThiMacDinhService _service;

        public MaTranDeThiMacDinhController(IMaTranDeThiMacDinhService service)
        {
            _service = service;
        }

        [HttpPost("getpaged")]
        public async Task<ActionResult<DataTableJson>> GetPaged([FromBody] MaTranDeThiMacDinhQuery query)
        {
            var result = await _service.GetPaged(query);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MaTranDeThiMacDinhDto>> GetById(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] MaTranDeThiMacDinhForm form)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var id = await _service.CreateAsync(form);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MaTranDeThiMacDinhForm form)
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
        [HttpPut("{id:guid}/toggle-active")]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            var success = await _service.ToggleActiveAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy ma trận" });

            // Trả về một Object JSON để CallServiceRegistryAPI không bị lỗi Parse
            return Ok(true);
        }
        [HttpGet("active-by-kythi/{kyThiId:guid}")]
        public async Task<ActionResult<List<MaTranDeThiMacDinhDto>>> GetActiveByKyThiId(Guid kyThiId)
        {
            // Gọi xuống service để xử lý logic lấy danh sách ma trận
            var result = await _service.GetActiveByKyThiIdAsync(kyThiId);
            return Ok(result);
        }
    }
}
