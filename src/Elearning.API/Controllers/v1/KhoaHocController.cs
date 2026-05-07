using Elearning.Application.Interfaces;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Forms;
using Elearning.Shared.Contracts.Portal.Querys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elearning.API.Controllers.v1
{
    /// <summary>
    /// Dịch vụ api quản lý Khóa Học
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class KhoaHocController : ControllerBase
    {
        private readonly IKhoaHocService _service;

        /// <summary>
        /// Constructor
        /// </summary>
        public KhoaHocController(IKhoaHocService service)
        {
            _service = service;
        }

        /// <summary>
        /// Tạo mới khóa học
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] KhoaHocForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _service.CreateAsync(form);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        /// <summary>
        /// Lấy danh sách phân trang
        /// </summary>
        [HttpPost("getpaged")]
        public async Task<ActionResult<DataTableJson>> GetPaged([FromBody] KhoaHocQuery query)
        {
            var result = await _service.GetPaged(query);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết theo Id
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<KhoaHocDto>> GetById(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null)
                return NotFound();
            return Ok(dto);
        }

        /// <summary>
        /// Cập nhật khóa học
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] KhoaHocForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(id, form);
            if (!updated)
                return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Xóa khóa học
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
        /// <summary>
        /// Lấy toàn bộ danh sách khóa học (Dùng cho Dropdown/Combobox)
        /// </summary>
        [HttpGet("get-all")]
        public async Task<ActionResult<List<KhoaHocDto>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }
    }
}
