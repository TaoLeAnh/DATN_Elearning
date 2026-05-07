using Elearning.Shared.Commons.Extensions;
using Elearning.Shared.Commons.Querys.Grid;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Elearning.Shared.Commons.Querys.ModalQuery
{
    public class BaseQuery
    {
#pragma warning disable IDE1006 // Naming Styles
        #region Tham số phục vụ hiện thị trên grid

        public string? tgridRequest { get; set; }

        public int draw { get; set; }
        public int ModerationStatus { get; set; }

        public List<Sort> sort { get; set; }



        private GridRequest _gridRequest;
        public GridRequest gridRequest
        {

            get
            {
                if (_gridRequest == null)
                {
                    _gridRequest = new GridRequest();
                }

                if (string.IsNullOrEmpty(_gridRequest.filter.Method) && _gridRequest.filter.Filters.Count > 0)// Kiểm tra dữ liệu đầu vào
                {
                    var lstFilter = _gridRequest.filter.Filters;
                    if (lstFilter.Any(x => string.IsNullOrEmpty(x.Method) || string.IsNullOrEmpty(x.Field) || string.IsNullOrEmpty(x.Value)))
                    {
                        throw new ArgumentNullException("Bộ lọc filter không hợp lệ");
                    }
                }
                if (!string.IsNullOrEmpty(Keyword) && SearchIn.Count > 0)
                {
                    Filter oFilterSearchIn = new Filter()
                    {
                        Logic = "or",
                        Filters = SearchIn.Select(x => new Filter()
                        {
                            Method = "contains",
                            Field = x,
                            Value = Keyword
                        }).ToList()
                    };
                    _gridRequest.filter = oFilterSearchIn;
                }

                if (!string.IsNullOrEmpty(dateField))
                {
                    if (SearchTuNgay != null)
                    {
                        _gridRequest.filter.Filters.Add(new Filter()
                        {
                            Method = "gt",
                            Field = dateField,
                            Value = SearchTuNgay.Value.ToString("yyyy/MM/dd HH:mm:ss")
                        });
                    }
                    ;
                    if (SearchDenNgay != null)
                    {
                        _gridRequest.filter.Filters.Add(new Filter()
                        {
                            Method = "lt",
                            Field = dateField,
                            Value = SearchDenNgay.Value.ToString("yyyy/MM/dd HH:mm:ss")
                        });
                    }

                }
                if (_gridRequest.sort == null || _gridRequest.sort.Count == 0)
                {
                    _gridRequest.sort = new List<Sort>(){ new Sort()
                    {
                        field = "Created",
                        dir = "desc"

                    } };
                }

                return _gridRequest;
            }
            set { _gridRequest = value; }

        }
        #endregion
        public string DenNgay { get; set; }
        public string? dateField { get; set; }
        public bool isgetBylisID { get; set; }
        public string? lstID { get; set; }
        public List<Guid> lstIDGet
        {
            get
            {

                if (!string.IsNullOrEmpty(lstID))
                {
                    isgetBylisID = true;
                    return SIConvert.GetIDsFromString(lstID);
                }
                return new List<Guid>();


            }
        }


#pragma warning restore IDE1006 // Naming Styles
        public string? Keyword { get; set; }
        public string? KeywordSearchFullText { get; set; }
        public List<string> SearchIn { get; set; }
        public string TuNgay { get; set; }
        public DateTime? SearchTuNgay
        {

            get
            {

                DateTimeFormatInfo dtfiParser;
                dtfiParser = new DateTimeFormatInfo();
                dtfiParser.ShortDatePattern = "dd/MM/yyyy";
                dtfiParser.DateSeparator = "/";
                DateTime? temp = null;
                if (!string.IsNullOrEmpty(TuNgay))
                {
                    temp = Convert.ToDateTime(TuNgay, dtfiParser).Date.AddTicks(-1);
                }
                return temp;
            }

        }

        public DateTime? SearchDenNgay
        {

            get
            {
                DateTimeFormatInfo dtfiParser;
                dtfiParser = new DateTimeFormatInfo();
                dtfiParser.ShortDatePattern = "dd/MM/yyyy";
                dtfiParser.DateSeparator = "/";
                DateTime? temp = null;
                if (!string.IsNullOrEmpty(DenNgay))
                {
                    temp = Convert.ToDateTime(DenNgay, dtfiParser);
                    temp = temp.Value.Date.AddTicks(-1).AddDays(1);
                }
                return temp;
            }

        }
        /// <summary>
        /// Tìm theo listid
        /// </summary>

        public BaseQuery()
        {
            _gridRequest = new GridRequest();
            TuNgay = string.Empty;
            DenNgay = string.Empty;
            sort = new List<Sort>();
            SearchIn = new List<string>();
            isgetBylisID = false;
            draw = 0;
            ModerationStatus = -1;
        }

    }
}
