using Elearning.Shared.Commons.Model.Commons;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Interfaces.Extentions
{
    public interface IRequestContext
    {
        CurrentUserDto CurrentUser { get; set; }
        OAuthClients CurrentClients { get; set; }

        /// <summary>
        /// Người dùng hay dùng khóa truy cập để vào phần mềm
        /// True => người dùng xác thực jwt,
        /// False => dùng khóa truy cập 
        /// </summary>
        bool IsUser { get; set; }



        /// <summary>
        /// Trả về ID người dùng hiện tại
        /// nếu IsUser => True => sẽ là ID của người dùng
        /// nếu IsUser => FALSE => sẽ là ID của Client
        /// </summary>
        Guid CurrentIdUser { get; }

        /// <summary>
        /// Trả về ID người dùng hiện tại
        /// nếu IsUser => True => sẽ là Token của người dùng dùng để liên lạc
        /// nếu IsUser => FALSE => trả về Empty
        /// </summary>
        string CurrentTokenUser { get; }

        /// <summary>
        /// Địa chỉ IP của người dùng
        /// </summary>
        string IpAddress { get; set; }


    }
}
