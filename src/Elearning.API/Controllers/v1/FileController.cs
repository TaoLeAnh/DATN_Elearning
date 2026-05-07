using Elearning.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elearning.API.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Bắt buộc đăng nhập mới được up ảnh
    public class FileController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public FileController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Không có file được chọn");

            // Chỉ cho phép up ảnh
            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("Vui lòng chỉ tải lên file hình ảnh.");

            using var stream = file.OpenReadStream();

            // Đẩy lên MinIO và nhận lại URL
            var url = await _storageService.UploadFileAsync(stream, file.FileName, file.ContentType);

            // Trả về URL cho Blazor
            return Ok(new { Url = url });
        }
    }
}
