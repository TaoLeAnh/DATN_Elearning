using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;

namespace Elearning.API.Controllers.v1
{
    using Elearning.Application.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class UploadController : ControllerBase
    {
        private readonly IStorageService _storage;

        public UploadController(IStorageService storage)
        {
            _storage = storage;
        }

        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Không có file");

            using var stream = file.OpenReadStream();

            // Dùng đúng tên method UploadFileAsync
            var url = await _storage.UploadFileAsync(stream, file.FileName, file.ContentType);

            return Ok(url);
        }
    }
}
