using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Application.Interfaces
{
    public interface ITienDoHocService
    {
        Task<List<Guid>> GetCompletedLessonIdsAsync(Guid courseId, Guid userId);
        Task<bool> MarkLessonCompleteAsync(Guid userId, Guid baiHocId);
    }
}
