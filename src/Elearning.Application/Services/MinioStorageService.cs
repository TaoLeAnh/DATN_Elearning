using Elearning.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Application.Services
{
    public class MinioStorageService : IStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly ILogger<MinioStorageService> _logger;
        private readonly string _bucketName;
        private readonly string _baseUrl;

        public MinioStorageService(IConfiguration configuration, ILogger<MinioStorageService> logger)
        {
            _logger = logger;
            var settings = configuration.GetSection("MinioSettings");

            var endpoint = settings["Endpoint"] ?? "localhost:9000";
            var accessKey = settings["AccessKey"] ?? "admin";
            var secretKey = settings["SecretKey"] ?? "password123";

            _bucketName = settings["BucketName"] ?? "elearning";
            _baseUrl = settings["BaseUrl"] ?? $"http://{endpoint}/{_bucketName}";

            // Khởi tạo MinioClient
            _minioClient = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                // .WithSSL() // Chỉ bật lên nếu Server chạy HTTPS
                .Build();
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            try
            {
                // 1. Lấy đuôi file (ví dụ: .jpg, .png)
                var extension = Path.GetExtension(fileName);

                // 2. Tạo tên file mới để chống ghi đè ảnh trùng tên
                var uniqueFileName = $"{Guid.NewGuid():N}{extension}";

                // 3. Upload lên MinIO
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(uniqueFileName)
                    .WithStreamData(fileStream)
                    .WithObjectSize(fileStream.Length)
                    .WithContentType(contentType);

                await _minioClient.PutObjectAsync(putObjectArgs);

                // 4. Trả về URL đường dẫn chuẩn để cất vào DB
                return $"{_baseUrl}/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi upload file lên MinIO: {FileName}", fileName);
                throw new Exception($"Lỗi máy chủ khi lưu file: {ex.Message}");
            }
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileUrl)) return;

                // Cắt lấy tên file từ URL (vd: http://localhost:9000/elearning/123.jpg => 123.jpg)
                var uri = new Uri(fileUrl);
                var fileName = Path.GetFileName(uri.LocalPath);

                if (string.IsNullOrEmpty(fileName)) return;

                var removeObjectArgs = new RemoveObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(fileName);

                await _minioClient.RemoveObjectAsync(removeObjectArgs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa file trên MinIO: {FileUrl}", fileUrl);
                // Bỏ qua lỗi ném ra ngoài vì nếu file không tồn tại cũng không sao
            }
        }
    }
}
