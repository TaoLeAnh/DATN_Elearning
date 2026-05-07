using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Model.ServiceCustomHttpClient
{
    public enum ServicesRegistryEnum
    {
        /// <summary>
        /// Khi muốn custom api ngoài những endpoint trong appsetting. dùng nó
        /// </summary>
        CustomApi,
        ServicePortal,
        ServicePublising,
        ServiceAIInteraction
    }
}
