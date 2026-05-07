using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Interfaces.Extentions
{
    public interface IHttpClientService
    {
        Task<ForwardResponseDto> SendAsync(string url, ForwardRequestDto request);
    }
}
