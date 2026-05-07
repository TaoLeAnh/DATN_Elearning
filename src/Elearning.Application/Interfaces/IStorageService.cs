using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Application.Interfaces
{
    public interface IStorageService
    {
        /// <summary>
        /// Upload file lên MinIO
        /// </summary>
        /// <param name="fileStream">Luồng dữ liệu của file</param>
        /// <param name="fileName">Tên file gốc</param>
        /// <param name="contentType">Định dạng file (vd: image/jpeg)</param>
        /// <returns>Đường dẫn URL Public để lưu vào DB</returns>
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);

        /// <summary>
        /// Xóa file trên MinIO
        /// </summary>
        /// <param name="fileUrl">Đường dẫn URL của file lưu trong DB</param>
        Task DeleteFileAsync(string fileUrl);
    }
}
