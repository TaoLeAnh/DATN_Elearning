using Elearning.Shared.Contracts.Portal.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Application.Interfaces
{
    public interface IBaiLamService
    {
        Task<List<QuizHistoryDto>> GetQuizHistoryAsync(Guid quizId, Guid userId);
    }
}
