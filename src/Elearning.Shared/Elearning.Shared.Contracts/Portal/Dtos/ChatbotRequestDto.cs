using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class ChatbotRequestDto
    {
        public Guid CourseId { get; set; } // Bắt buộc: ID của khóa học
        public Guid? LessonId { get; set; } // Có thể null: Đang học bài nào thì truyền ID bài đó
        public string UserMessage { get; set; } // Câu hỏi của học viên
    }
}
