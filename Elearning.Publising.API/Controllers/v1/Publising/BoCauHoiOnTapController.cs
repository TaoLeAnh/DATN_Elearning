using Elearning.Publising.Application.Interfaces;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Contracts.Portal.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Elearning.Publising.API.Controllers.v1.Publising
{
    [ApiController]
    [Route("api/v1/Publising/[controller]")]
    public class BoCauHoiOnTapController : ControllerBase
    {
        private readonly IBoCauHoiOnTapService _boCauHoiService;
        private readonly IRequestContext _requestContext;

        public BoCauHoiOnTapController(IBoCauHoiOnTapService boCauHoiService, IRequestContext requestContext)
        {
            _boCauHoiService = boCauHoiService;
            _requestContext = requestContext;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BoCauHoiOnTapDto>> GetDetailForStudent(Guid id)
        {
            var dto = await _boCauHoiService.GetQuizDetailForStudentAsync(id);
            if (dto == null) return NotFound(new { message = "Không tìm thấy bài thi này hoặc bài thi đã bị ẩn." });

            return Ok(dto);
        }
        [HttpPost("nop-bai")]
        public async Task<IActionResult> SubmitQuiz([FromBody] NopBaiRequest request)
        {
            // API chỉ việc "mở gói hàng" ra và lấy UserId đã được UI chuẩn bị sẵn
            var diemSo = await _boCauHoiService.NopBaiVaChamDiemAsync(request, request.UserId);

            return Ok(diemSo);
        }
    }
}
