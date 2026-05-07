using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Querys.Grid
{
    [Serializable]
    public struct Sort
    {
#pragma warning disable IDE1006 // Naming Styles
        public string field { get; set; }

        public string dir { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
