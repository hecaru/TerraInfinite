using Microsoft.AspNetCore.Components;
using TerraInfinite.Services;

namespace TerraInfinite.Pages
{
    public partial class Holders
    {
        private bool _metaMaskInstalled = false;
        private List<string>? _whitelisted = null;
        private Dictionary<string, string>? _holders = null;
        private Dictionary<string, string>? _stake = null;
        private string _totalSupply = string.Empty;

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
            _whitelisted = null;
            StateHasChanged();
            _whitelisted = await Javascript!.WhitelistedAsync();
            _holders = await Javascript!.HoldersAsync();
            _totalSupply = await Javascript!.TotalSupplyAsync();
            _stake = _holders
                .Select(holder => new KeyValuePair<string, string>(holder.Key, (_totalSupply == "0") ? "no" : $"{Math.Round(Convert.ToDecimal(holder.Value) * 100 / Convert.ToDecimal(_totalSupply), 4)}%"))
                .ToDictionary(holder => holder.Key, holder => holder.Value);
        }
    }
}
