using MudBlazor;

namespace TerraInfinite.Shared
{
    public partial class MainLayout
    {
        private readonly MudTheme _terraTheme = new();
        private bool _drawerOpen = false;

        protected override void OnInitialized()
        {
            _terraTheme.PaletteDark.Primary = Colors.Yellow.Default;
            _terraTheme.PaletteDark.PrimaryContrastText = Colors.Grey.Darken4;
            _terraTheme.PaletteDark.Secondary = Colors.Yellow.Darken4;
            _terraTheme.PaletteDark.SecondaryContrastText = Colors.Grey.Darken4;
        }

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }
    }
}
