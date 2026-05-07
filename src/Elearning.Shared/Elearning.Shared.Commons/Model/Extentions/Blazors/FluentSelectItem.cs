using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Model.Extentions.Blazors
{
    public class FluentSelectItem
    {
        public string Text { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool Selected { get; set; }
        public string IconTxt { get; set; } = string.Empty;
    }
}
