using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Dtos
{
    public class QuizHistoryDto
    {
        public Guid Id { get; set; }
        public float Diem { get; set; }
        public int SoCauDung { get; set; }
        public DateTime ThoiDiemBatDau { get; set; }
        public DateTime? ThoiDiemNop { get; set; }
    }
}
