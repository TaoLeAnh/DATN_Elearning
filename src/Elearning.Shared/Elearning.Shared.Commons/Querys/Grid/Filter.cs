using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Querys.Grid
{
    public class Filter
    {
        //Nếu không có logic thì hiểu mặc định là so sánh bằng
        public string? Value { get; set; }
        public string? Field { get; set; }
        public string? Method { get; set; }


        public List<Filter> Filters { get; set; } = new List<Filter>();
        public string? Logic { get; set; }
    }
}
