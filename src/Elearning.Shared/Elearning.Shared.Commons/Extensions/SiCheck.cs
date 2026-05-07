using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Extensions
{
    public static class SiCheck
    {
        /// <summary>
        /// Kiểm tra xem Guid có phải là rỗng (Guid.Empty) hay không.
        /// </summary>
        /// <param name="guid">Giá trị Guid cần kiểm tra.</param>
        /// <returns>True nếu Guid là rỗng, ngược lại trả về False.</returns>
        public static bool IsEmpty(this Guid guid)
        {
            return guid == Guid.Empty;
        }
    }
}
