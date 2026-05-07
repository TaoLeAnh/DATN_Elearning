using Microsoft.FluentUI.AspNetCore.Components;

namespace Elearning.UI.Components.Layout.Component
{
    public partial class SiteSettings
    {
        private IDialogReference? _dialog;

        private async Task OpenSiteSettingsAsync()
        {
            _dialog = await DialogService.ShowPanelAsync<SiteSettingsPanel>(new DialogParameters()
            {
                ShowTitle = true,
                Title = "Cài đặt trang web",
                Alignment = HorizontalAlignment.Right,
                PrimaryAction = "Đồng ý",
                SecondaryAction = null,
                ShowDismiss = true
            });

            DialogResult result = await _dialog.Result;
        }
    }

}
