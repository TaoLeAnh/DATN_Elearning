using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Interfaces.Extentions
{
    public interface ICallServiceRegistry
    {
        Task<ResultAPI> Delete(ApiRequestModel apiRequestModel);
        Task<ResultAPI> Put(ApiRequestModel apiRequestModel, object data);
        Task<ResultAPI<T>> Get<T>(ApiRequestModel apiRequestModel);
        Task<ResultAPI<T>> Post<T>(ApiRequestModel apiRequestModel, object data);
        Task<ResultAPI<byte[]>> PostForFile(ApiRequestModel apiRequestModel, object data);

    }
}
