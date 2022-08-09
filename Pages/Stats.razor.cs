using Microsoft.AspNetCore.Components;
using TerraInfinite.Services;

namespace TerraInfinite.Pages
{
    public partial class Stats
    {
        private bool _metaMaskInstalled = false;
        private string _contractAddress = string.Empty;
        private string _totalSupply = string.Empty;
        private long _holdersLength = 0;
        private string _investRate = string.Empty;
        private string _lpFee = string.Empty;
        private string _liquidityPool = string.Empty;
        private TimeSpan _lockdownPeriod = TimeSpan.Zero;
        private long _whitelistedLength = 0;
        private long _rewardsTimestampsLength = 0;

        [Inject] private JavascriptService? Javascript { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _contractAddress = await Javascript!.ContractAddressAsync();
            if (_metaMaskInstalled = await Javascript!.MetaMaskInstalledAsync())
            {
                await RefreshAsync();
            }
        }

        private async Task ViewOnEtherscanAsync()
        {
            await Javascript!.ViewOnEtherscanAsync(_contractAddress);
        }

        private async Task RefreshAsync()
        {
            _totalSupply = string.Empty;
            StateHasChanged();
            _totalSupply = await Javascript!.TotalSupplyAsync();
            _holdersLength = await Javascript!.HoldersLengthAsync();
            _investRate = await Javascript!.ToAssetAmountAsync("1");
            _lpFee = await Javascript!.LpFeeAsync();
            _liquidityPool = await Javascript!.LiquidityPoolAsync();
            _lockdownPeriod = await Javascript!.LockdownPeriodAsync();
            _whitelistedLength = await Javascript!.WhitelistedLengthAsync();
            _rewardsTimestampsLength = await Javascript!.RewardsTimestampsLengthAsync();
        }
    }
}
