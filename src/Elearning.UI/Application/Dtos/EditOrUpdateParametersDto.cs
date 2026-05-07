using Elearning.Shared.Commons.Model.ServiceCustomHttpClient;
using Microsoft.AspNetCore.Components;

namespace Elearning.UI.Application.Dtos
{
    public class EditOrUpdateParametersDto
    {
        public Guid Id { get; set; }
        public bool IsEditMode { get; set; } = false;
        public ApiRequestModel RequestApi { get; set; } = new ApiRequestModel();
        public EventCallback OnRefresh { get; set; } = new EventCallback();
        public EventCallback<object> OnDataSubmit { get; set; }
        public string? Parameter { get; set; }
        public int ParameterInt { get; set; }
        public decimal ParameterDecimal { get; set; }
        public Guid ParameterGuid { get; set; }
        public Guid ParameterGuid2 { get; set; }
        public object? DataObject { get; set; }
    }
}
