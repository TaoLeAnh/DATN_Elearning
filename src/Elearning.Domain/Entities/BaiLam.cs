using Elearning.Shared.Commons.Model.SQL;
using Elearning.Shared.Contracts.Portal.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Domain.Entities
{
    public class BaiLam : BaseDomainEntity
    {
        public Guid? KyThiId { get; set; }
        public virtual KyThi? KyThi { get; set; }

        public Guid? BoCauHoiOnTapId { get; set; }
        public virtual BoCauHoiOnTap? BoCauHoiOnTap { get; set; }

        public Guid NguoiDungId { get; set; }

        public DateTime ThoiDiemBatDau { get; set; }

        public DateTime? ThoiDiemNop { get; set; }

        public float Diem { get; set; }

        public int SoCauDung { get; set; }

        public EnumTrangThaiBaiLam TrangThai { get; set; }


        public virtual NguoiDung NguoiDung { get; set; } = default!;

        public virtual ICollection<ChiTietBaiLam> ChiTietBaiLams { get; set; } = new List<ChiTietBaiLam>();

        public virtual ICollection<LogViPham> LogViPhams { get; set; } = new List<LogViPham>();
    }
}
