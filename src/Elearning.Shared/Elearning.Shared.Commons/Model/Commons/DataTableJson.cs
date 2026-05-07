using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Model.Commons
{
    // "Một sản phẩm từ phòng sharepoint. SIMAX-CôngVM"

    using System.Text.Json.Serialization;

    namespace Service.Shared.Commons.Model.Commons
    {
        public class DataTableJson
        {
            public DataTableJson() { }
            public DataTableJson Message(string exMessage)
            {
                this.exMessage = exMessage;

                return this;
            }
            public DataTableJson(string exMessage)
            {
                this.exMessage = exMessage;
            }
            public DataTableJson(string exMessage, object data)
            {
                recordsTotal = recordsTotal;
                recordsFiltered = recordsFiltered;
            }

            public DataTableJson(object data, int draw, int recordsTotal, int recordsFiltered)
            {
                this.data = data;
                this.draw = draw;
                this.recordsTotal = recordsTotal;
                this.recordsFiltered = recordsFiltered;
            }
            public DataTableJson(object data, int draw, int recordsTotal)
            {
                this.data = data;
                this.draw = draw;
                this.recordsTotal = recordsTotal;
                recordsFiltered = recordsTotal;
            }

#pragma warning disable IDE1006 // Bỏ qua thông báo cái này
            public int? draw { get; set; }

            public int? recordsTotal { get; set; }
            public int? recordsFiltered { get; set; }
            public object? data { get; set; }
            public string? exMessage { get; set; }
            public string? querytext { get; set; }
#pragma warning restore IDE1006
        }




        /// <summary>
        /// Kết quả trả về cho DataTables với kiểu dữ liệu mạnh.
        /// </summary>
        /// <typeparam name="T">Kiểu của phần tử trong danh sách data</typeparam>
        public class DataTableJson<T>
        {
            /// <summary>
            /// Chu kỳ draw (DataTables truyền lên, gửi trả về nguyên vẹn)
            /// </summary>
            [JsonPropertyName("draw")]
            public int? Draw { get; set; }

            /// <summary>
            /// Tổng số bản ghi (chưa paging)
            /// </summary>
            [JsonPropertyName("recordsTotal")]
            public int RecordsTotal { get; set; }

            /// <summary>
            /// Số bản ghi sau filter (nếu có)
            /// </summary>
            [JsonPropertyName("recordsFiltered")]
            public int RecordsFiltered { get; set; }

            /// <summary>
            /// Dữ liệu phân trang, kiểu mạnh List<T>
            /// </summary>
            [JsonPropertyName("data")]
            public IEnumerable<T> Data { get; set; }

            /// <summary>
            /// Thông báo lỗi (nếu có)
            /// </summary>
            [JsonPropertyName("error")]
            public string? ErrorMessage { get; set; }

            /// <summary>
            /// Query text (debug)
            /// </summary>
            [JsonPropertyName("queryText")]
            public string? QueryText { get; set; }

            public DataTableJson()
            {
                Data = new List<T>();
            }
            public DataTableJson(IEnumerable<T> data, int draw, int recordsTotal, int recordsFiltered)
            {
                Data = data;
                Draw = draw;
                RecordsTotal = recordsTotal;
                RecordsFiltered = recordsFiltered;
            }

            public DataTableJson(IEnumerable<T> data, int draw, int recordsTotal)
                : this(data, draw, recordsTotal, recordsTotal)
            {
            }

            public DataTableJson(string errorMessage)
            {
                Data = new List<T>();
                ErrorMessage = errorMessage;
            }

            /// <summary>
            /// Gán thêm error message và trả về đối tượng
            /// </summary>
            public DataTableJson<T> WithError(string errorMessage)
            {
                ErrorMessage = errorMessage;
                return this;
            }
        }
    }
}
