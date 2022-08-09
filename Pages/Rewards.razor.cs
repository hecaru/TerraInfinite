using Microsoft.AspNetCore.Components;
using TerraInfinite.Services;

namespace TerraInfinite.Pages
{
    public partial class Rewards
    {
        private bool _metaMaskInstalled = false;
        private readonly DateTime _now = DateTime.Now;
        private TimeSpan _lockdownPeriod = TimeSpan.Zero;
        private Dictionary<DateTime, Dictionary<string, string>>? _rewards = null;
        private bool _sortAscendenting;

        [Inject] private JavascriptService? Javascript { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (_metaMaskInstalled = await Javascript!.MetaMaskInstalledAsync())
            {
                await RefreshAsync();
            }
        }

        private async Task RefreshAsync()
        {
            _rewards = null;
            StateHasChanged();
            _lockdownPeriod = await Javascript!.LockdownPeriodAsync();
            _rewards = await Javascript!.RewardsAsync();
        }

        private void Reorder()
        {
            _sortAscendenting = !_sortAscendenting;
            _rewards = _rewards!.Reverse().ToDictionary(reward => reward.Key, reward => reward.Value);
        }
    }
}
