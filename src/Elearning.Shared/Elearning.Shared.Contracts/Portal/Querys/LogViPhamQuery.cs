using Elearning.Shared.Commons.Querys.ModalQuery;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Querys
{
    public class LogViPhamQuery : BaseQuery 
    {
        public Guid? BaiLamId { get; set; }
        public Guid? NguoiDungId { get; set; }
    }
}
