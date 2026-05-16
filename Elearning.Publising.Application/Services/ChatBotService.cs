using Elearning.Domain.Interfaces;
using Elearning.Publising.Application.Interfaces;
using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Contracts.Portal.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Application.Services
{
    public class ChatBotService : IChatbotService
    {
        private readonly IUnitOfWorkPublising _UnitOfWork;
        private readonly IRequestContext _RequestContext;
        private readonly IAIService _aiService;

        public ChatBotService(IUnitOfWorkPublising UnitOfWork, IRequestContext requestContext, IAIService aiService)
        {
            _UnitOfWork = UnitOfWork;
            _RequestContext = requestContext;
            _aiService = aiService;
        }

        public async Task<string> AskTutorAsync(ChatbotRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.UserMessage))
                return "Bạn cần hỏi gì nào?";

            string contextText = "";

            if (request.LessonId.HasValue && request.LessonId.Value != Guid.Empty)
            {
                var lesson = await _UnitOfWork.BaiHocRepository.GetTableNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.LessonId.Value);

                if (lesson != null)
                    contextText = lesson.TaiLieuAI;
            }
            else
            {
                // 👉 ĐÃ SỬA LỖI 2: Link qua ChuongHoc để lấy KhoaHocId
                // Lưu ý: Tùy vào tên Navigation Property của bác, có thể là x.ChuongHoc.KhoaHocId hoặc x.PhanHoc.KhoaHocId
                var lessons = await _UnitOfWork.BaiHocRepository.GetTableNoTracking()
                    .Include(x => x.ChuongHoc)
                    .Where(x => x.ChuongHoc != null && x.ChuongHoc.KhoaHocId == request.CourseId)
                    .Select(x => x.TaiLieuAI)
                    .ToListAsync();

                contextText = string.Join("\n\n", lessons.Where(x => !string.IsNullOrEmpty(x)));
            }

            if (string.IsNullOrWhiteSpace(contextText))
            {
                contextText = "Tài liệu cho bài giảng này hiện tại đang trống.";
            }

            string prompt = $@"
            Bạn là một gia sư AI thân thiện trong hệ thống E-Learning.
            Bạn xưng là 'Gia sư' và gọi người dùng là 'Bạn'.

            MỤC TIÊU:
            - Hỗ trợ học viên hiểu bài học.
            - Giải thích dễ hiểu, ngắn gọn, đúng trọng tâm.
            - Có thể dùng ví dụ đơn giản nếu tài liệu có liên quan.

            QUY TẮC:
            1. Ưu tiên sử dụng thông tin trong phần [TÀI LIỆU BÀI HỌC].
            2. Không tự bịa ra kiến thức không liên quan đến tài liệu.
            3. Nếu câu hỏi hoàn toàn ngoài phạm vi bài học, hãy trả lời đúng nguyên văn:
            'Chào Bạn, Gia sư xin lỗi, tài liệu bài học hiện tại chưa đề cập đến vấn đề này, bạn có thể hỏi câu khác liên quan đến bài học nhé!'
            4. Trả lời tự nhiên, thân thiện.
            5. Dùng Markdown để in đậm ý chính khi cần.

            [TÀI LIỆU BÀI HỌC]
            {contextText}

            [CÂU HỎI]
            {request.UserMessage}

            [TRẢ LỜI]
            ";

            return await _aiService.ChatWithGeminiAsync(prompt);
        }
    }
}
