using Elearning.Shared.Contracts.Portal.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Publising.Application.Interfaces
{
    public interface IChatbotService
    {
        Task<string> AskTutorAsync(ChatbotRequestDto request);
    }
}
