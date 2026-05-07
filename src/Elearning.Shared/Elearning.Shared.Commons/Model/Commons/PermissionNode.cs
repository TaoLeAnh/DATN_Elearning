using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Model.Commons
{
    public class PermissionNode
    {
        public string Code { get; set; } = default!;       // "QUANLYMENU_ADD"
        public string Name { get; set; } = default!;       // "Quản Lý Menu Add"
        public int Value { get; set; }                     // 101
        public string? ParentCode { get; set; }            // "QUANLYMENU"
        public List<PermissionNode> Children { get; set; } = new(); // con
    }
}
