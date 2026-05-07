using Elearning.Shared.Commons.Extensions;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Elearning.Shared.Commons.Model.SQL
{
    public abstract class IntermediaryEntity
    {
        /// <summary>
        /// ID cho bản ghi
        /// </summary>
        [Key]
        public Guid Id { get; protected set; }
        /// <summary>
        /// Thời gian tạo cho bản ghi 
        /// </summary>
        private DateTime _created;
        public DateTime Created
        {
            get
            {
                return _created;
            }
            protected set
            {
                _created = value;
            }
        }
        public void FillDataForInsert(Guid IdUser)
        {
            if (IdUser == Guid.Empty)
                throw new ArgumentException("Người dùng không thích hợp để thêm mới.");

            if (this.Id == Guid.Empty)
                this.Id = Uuid7.NewUuid7();

            Created = DateTime.Now;
        }
    }
    public abstract class BaseDomainEntity
    {
        /// <summary>
        /// ID cho bản ghi
        /// </summary>
        public Guid Id { get; protected set; }
        public Guid DepartmentId { get; protected set; }

        /// <summary>
        /// Thời gian tạo cho bản ghi 
        /// </summary>
        private DateTime _created;
        public DateTime Created
        {
            get
            {
                return _created;
            }
            protected set
            {
                _created = value;
            }
        }

        /// <summary>
        /// Thời gian chỉnh sửa cuối cho bản ghi (lưu dưới dạng UTC)
        /// </summary>
        private DateTime _lastModified;
        public DateTime LastModified
        {
            get
            {
                return _lastModified;
            }
            protected set
            {
                _lastModified = value;
            }
        }

        /// <summary>
        /// Người tạo đầu tiên
        /// </summary>
        public Guid CreatedBy { get; protected set; }

        /// <summary>
        /// Người sửa cuối cùng
        /// </summary>
        public Guid LastModifiedBy { get; protected set; }

        /// <summary>
        /// Trạng thái duyệt của bản ghi
        /// </summary>
        public ModerationStatus ModerationStatus { get; set; }

        /// <summary>
        /// Phần chú thích của trạng thái duyệt
        /// </summary>
        public string ModerationStatusTxt => SIConvert.GetEnumDescription(ModerationStatus);

        public BaseDomainEntity()
        {
            CreatedBy = default;
            LastModifiedBy = default;
        }

        public void FillDataForInsert(Guid IdUser, Guid DepartmentId)
        {
            if (IdUser == Guid.Empty)
                throw new ArgumentException("Người dùng không thích hợp để thêm mới.");

            if (DepartmentId == Guid.Empty)
                throw new ArgumentException("Đơn vị không thích hợp để thêm mới.");

            if (this.Id == Guid.Empty)
                Id = Uuid7.NewUuid7();

            this.Created = DateTime.Now;
            this.LastModified = DateTime.Now;
            this.CreatedBy = LastModifiedBy = IdUser;
            this.DepartmentId = DepartmentId;
        }

        public void FillDataForUpdate(Guid IdUser)
        {
            if (IdUser == Guid.Empty)
                throw new ArgumentException("Người dùng không thích hợp để cập nhật bản ghi.");

            this.LastModified = DateTime.Now;
            this.LastModifiedBy = IdUser;
        }

    }
}
