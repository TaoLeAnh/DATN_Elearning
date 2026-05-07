using Elearning.Shared.Commons.Querys.ModalQuery;
using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.Portal.Querys
{
    public class KhoaHocQuery : BaseQuery
    {
        public MonHocEnum? MonHoc { get; set; }
    }
}
