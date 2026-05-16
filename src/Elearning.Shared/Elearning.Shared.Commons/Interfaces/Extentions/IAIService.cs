using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Interfaces.Extentions
{
    public interface IAIService
    {
        Task<string> ChatWithGeminiAsync(string prompt);
    }
}
