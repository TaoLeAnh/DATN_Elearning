using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Elearning.Shared.Contracts.AIM.Enums
{
    public enum OrganizationUnitGroup : byte
    {
        [Description("Không xác định")]
        None = 0,
        [Description("Đơn vị, phòng ban nghiệp vụ")]
        DONVIPHONGBAN = 1,

        [Description("Lưu trữ cơ quan")]
        LUUTRUCOQUAN = 2,


        [Description("Lưu trữ lịch sử")]
        LUUTRULICHSU = 3,

        [Description("Quản lý nhà nước về lưu trữ")]
        QUANLYNHANUOCVELUUTRU = 4,

    }
}
