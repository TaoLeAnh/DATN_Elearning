using Elearning.Publising.Application.Interfaces;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Querys;
using Microsoft.AspNetCore.Mvc;

namespace Elearning.Publising.API.Controllers.v1.Publising
{
    [ApiController]
    [Route("api/v1/Publising/[controller]")]
    public class KhoaHocController : ControllerBase
    {
        private readonly IKhoaHocService _khoaHocService;

        public KhoaHocController(IKhoaHocService khoaHocService)
        {
            _khoaHocService = khoaHocService;
        }

        /// <summary>
        /// Lấy danh sách khóa học (Có phân trang, phục vụ Trang chủ / Danh sách)
        /// </summary>
        [HttpPost("getpaged")]
        public async Task<ActionResult<DataTableJson>> GetPaged([FromBody] KhoaHocQuery query)
        {
            var result = await _khoaHocService.GetPaged(query);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết khóa học theo Id
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<KhoaHocDto>> GetById(Guid id)
        {
            var dto = await _khoaHocService.GetByIdAsync(id);
            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        /// <summary>
        /// Lấy chi tiết khóa học theo đường dẫn Slug (Phục vụ SEO Web)
        /// </summary>
        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<KhoaHocDto>> GetBySlug(string slug)
        {
            var dto = await _khoaHocService.GetBySlugAsync(slug);
            if (dto == null)
                return NotFound();

            return Ok(dto);
        }
        [HttpGet("detail/{id:guid}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            try
            {
                var result = await _khoaHocService.GetDetailByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { message = "Không tìm thấy khóa học" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Bắt lỗi luôn để không bao giờ bị 500 "mù" nữa
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
