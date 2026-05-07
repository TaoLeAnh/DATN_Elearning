using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons;
using Microsoft.AspNetCore.Mvc;

namespace Elearning.Publising.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseController : ControllerBase
    {

        /// <summary>
        /// CôngVM
        /// </summary>
        protected CurrentUserDto CurrentUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestContext"></param>
        public BaseController(IRequestContext requestContext)
        {
            CurrentUser = requestContext.CurrentUser;
        }

    }
}
