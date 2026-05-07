using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.AIM.Dtos
{
    public class ImageItemDto
    {
        /// <summary>
        /// Id của ảnh (nếu là ảnh cũ đã lưu trong DB thì sẽ có Id, ảnh mới tải lên sẽ là null)
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Đường dẫn của ảnh (URL từ MinIO trả về)
        /// </summary>
        public string Url { get; set; } = string.Empty;
    }
}
