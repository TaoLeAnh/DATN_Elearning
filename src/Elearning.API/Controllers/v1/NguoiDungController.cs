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
    /// Dịch vụ api quản lý Người Dùng
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NguoiDungController : ControllerBase
    {
        private readonly INguoiDungService _service;

        /// <summary>
        /// Constructor
        /// </summary>
        public NguoiDungController(INguoiDungService service)
        {
            _service = service;
        }

        /// <summary>
        /// Tạo mới người dùng
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] NguoiDungForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _service.CreateAsync(form);

            return Ok(id);
        }

        /// <summary>
        /// Lấy danh sách phân trang
        /// </summary>
        [HttpPost("getpaged")]
        public async Task<ActionResult<DataTableJson>> GetPaged([FromBody] NguoiDungQuery query)
        {
            var result = await _service.GetPaged(query);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết theo Id
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<NguoiDungDto>> GetById(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null)
                return NotFound();
            return Ok(dto);
        }

        /// <summary>
        /// Cập nhật người dùng
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] NguoiDungForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(id, form);
            if (!updated)
                return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Xóa người dùng
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }

    }
}
