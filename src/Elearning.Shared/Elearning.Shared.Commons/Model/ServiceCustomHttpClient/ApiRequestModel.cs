using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Elearning.Shared.Commons.Model.ServiceCustomHttpClient
{
    public class ApiRequestModel
    {

        public ServicesRegistryEnum ApiService { get; set; }

        /// <summary>
        /// Khi muốn dùng domain ngoài appsetting thì truyền vào đây kèm set ApiService = Custom
        /// </summary>
        public string ApiServiceCustom { get; set; } = string.Empty;
        public float Version { get; set; }
        public string Endpoint { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool HasAuthorization { get; set; } = true;
        public Dictionary<string, string>? QueryParams { get; set; } = null;

        public ApiRequestModel()
        {
            Version = 1.0f;
        }
        public ApiRequestModel(string endpoint)
        {
            Endpoint = endpoint;
            Version = 1.0f;
        }
    }
}
