using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Querys.Grid
{
    public class Paging
    {
        public int Page { get; set; }
        public int PageSize { get; set; }

        public int TotalItems { get; set; } = 0;


        // Tính toán tự động
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        public Paging(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }
        public Paging() { }
    }
}
