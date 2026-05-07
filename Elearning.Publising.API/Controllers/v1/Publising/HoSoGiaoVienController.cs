using Elearning.Publising.Application.Interfaces;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elearning.Publising.API.Controllers.v1.Publising
{
    [ApiController]
    [Route("api/v1/Publising/[controller]")]
    public class HoSoGiaoVienController : ControllerBase
    {
        private readonly IHoSoGiaoVienService _service;

        public HoSoGiaoVienController(IHoSoGiaoVienService service)
        {
            _service = service;
        }

        // Lấy danh sách (truyền ?monHoc=1 để lọc, không truyền sẽ lấy tất cả)
        [HttpGet]
        public async Task<ActionResult<List<HoSoGiaoVienDto>>> GetAll([FromQuery] MonHocEnum? monHoc)
        {
            var result = await _service.GetDanhSachGiaoVienAsync(monHoc);
            return Ok(result);
        }

        // Lấy chi tiết 1 giáo viên
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<HoSoGiaoVienDto>> GetById(Guid id)
        {
            var result = await _service.GetChiTietGiaoVienAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
