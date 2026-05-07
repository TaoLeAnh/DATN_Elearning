using Elearning.Publising.Application.Interfaces;
using Elearning.Shared.Contracts.Portal.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Elearning.Publising.API.Controllers.v1.Publising
{
    [ApiController]
    [Route("api/v1/Publising/[controller]")]
    [Authorize]
    public class DangKyKhoaHocController : ControllerBase
    {
        private readonly IDangKyKhoaHocService _service;

        public DangKyKhoaHocController(IDangKyKhoaHocService service)
        {
            _service = service;
        }

        [HttpGet("my-courses")]
        public async Task<ActionResult<List<MyCourseDto>>> GetMyCourses()
        {
            // 1. Lấy UserId từ Token người dùng đang đăng nhập
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized("Không tìm thấy thông tin người dùng.");
            }

            // 2. Gọi Service xử lý
            var result = await _service.GetMyCoursesAsync(userId);

            return Ok(result);
        }
        [HttpPost("register/{courseId:guid}")]
 // Bắt buộc đăng nhập mới được đăng ký
        public async Task<IActionResult> RegisterCourse(Guid courseId)
        {
            // Lấy ID người dùng từ Token
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized(new { message = "Vui lòng đăng nhập để đăng ký khóa học." });
            }

            var result = await _service.DangKyKhoaHocMoiAsync(userId, courseId);

            if (result == "BẠN_ĐÃ_ĐĂNG_KÝ")
            {
                return BadRequest(new { message = "Bạn đã đăng ký khóa học này rồi! Hãy vào mục Khóa học của tôi." });
            }

            if (result == "THÀNH_CÔNG")
            {
                return Ok(new { message = "Đăng ký khóa học thành công! Chúc bạn học tốt." });
            }

            return StatusCode(500, new { message = "Có lỗi xảy ra trong quá trình đăng ký." });
        }
        [HttpGet("count-all")]
        [AllowAnonymous] // QUAN TRỌNG: Mở khóa phân quyền cho riêng API này
        public async Task<ActionResult<int>> CountAllHocVien()
        {
            try
            {
                var count = await _service.CountTatCaHocVienAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần thiết
                return StatusCode(500, new { message = "Lỗi khi đếm số học viên", error = ex.Message });
            }
        }
    }
}
