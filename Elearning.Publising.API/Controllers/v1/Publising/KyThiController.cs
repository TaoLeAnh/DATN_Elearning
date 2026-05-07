
using Elearning.Publising.Application.Interfaces;
using Elearning.Shared.Contracts.Portal.Dtos;
using Elearning.Shared.Contracts.Portal.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Elearning.Publising.API.Controllers.v1.Publising
{
    [ApiController]
    [Route("api/v1/Publising/[controller]")]
    public class KyThiController : ControllerBase
    {
        private readonly IKyThiService _service;

        public KyThiController(IKyThiService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<PublicKyThiDto>>> GetPublicExams([FromQuery] MonHocEnum? monHoc)
        {
            var data = await _service.GetPublicExamsAsync(monHoc);
            return Ok(data);
        }

        [HttpPost("random")]
        public async Task<ActionResult<Guid>> GenerateRandomExam([FromBody] RandomExamRequest request)
        {
            try
            {
                var examId = await _service.GenerateRandomExamAsync(request.MonHocId);
                return Ok(examId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Báo lỗi nếu ngân hàng hết câu hỏi
            }
        }

        public class RandomExamRequest
        {
            public MonHocEnum MonHocId { get; set; }
        }

        [HttpGet("{id:guid}/lam-bai")]
        public async Task<ActionResult<BoCauHoiOnTapDto>> GetDeThiLamBai(Guid id)
        {
            var data = await _service.GetDeThiLamBaiAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        //[HttpPost("nop-bai")]
        //public async Task<ActionResult<float>> NopBaiThi([FromBody] NopBaiRequest request)
        //{
        //    var diem = await _service.NopBaiThiAsync(request);
        //    return Ok(diem);
        //}
        [HttpPost("nop-bai")]
        public async Task<ActionResult> NopBaiThi([FromBody] NopBaiRequest request)
        {
            try
            {
                if (request.IsLiveExam)
                {
                    // Nhánh 1: Đẩy vào Queue (Thi trực tiếp)
                    var (success, message) = await _service.DayBaiNopVaoQueueAsync(request);
                    if (success)
                    {
                        return Ok(new { isLive = true, success = true, message = "Nộp bài thành công! Điểm sẽ được cập nhật sau khi kết thúc ca thi." });
                    }
                    return BadRequest(new { success = false, message = message });
                }
                else
                {
                    var diem = await _service.NopBaiThiAsync(request);
                    return Ok(new { isLive = false, success = true, diem = diem });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ==================================================================
        // CÁC API DÀNH CHO MODULE GIÁM THỊ THỜI GIAN THỰC
        // ==================================================================

        [HttpPost("bat-dau-thi")]
        public async Task<ActionResult<Guid>> BatDauThi([FromBody] BatDauThiRequest request)
        {
            try
            {
                var baiLamId = await _service.BatDauThiAsync(request.KyThiId, request.UserId);
                return Ok(baiLamId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("log-vi-pham")]
        public async Task<ActionResult<bool>> LogViPham([FromBody] LogViPhamRequest request)
        {
            try
            {
                var result = await _service.GhiNhanViPhamRealTimeAsync(request.BaiLamId, request.LoaiViPham, request.ChiTiet);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // --- CÁC CLASS REQUEST HỨNG DỮ LIỆU TỪ UI ---
        public class BatDauThiRequest
        {
            public Guid KyThiId { get; set; }
            public Guid UserId { get; set; }
        }

        public class LogViPhamRequest
        {
            public Guid BaiLamId { get; set; }
            public EnumLoaiViPham LoaiViPham { get; set; }
            public string? ChiTiet { get; set; }
        }
    }
}
