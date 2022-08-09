using Microsoft.AspNetCore.Components;
using TerraInfinite.Services;

namespace TerraInfinite.Pages
{
    public partial class Index
    {
        private bool _metaMaskInstalled = false;
        private string _account = string.Empty;
        private bool _isWhitelisted = false;
        private string _assetBalance = string.Empty;
        private string _totalSupply = string.Empty;
        private string _stake = string.Empty;
        private string _ethBalance = string.Empty;
        private decimal _ethAmountToInvest = 0;
        private string _assetAfterInvestment = string.Empty;
        private string _remainingReward = string.Empty;
        private string _availableReward = string.Empty;
        private DateTime? _nextRewardUnlock = null;
        private string _withdrawnReward = string.Empty;

        [Inject] private JavascriptService? Javascript { get; set; }

        private decimal EthAmountToInvest
        {
            get => _ethAmountToInvest;
            set
            {
                _ethAmountToInvest = value;
                _ = Javascript!.ToAssetAmountAsync(_ethAmountToInvest.ToString())
                    .AsTask()
                    .ContinueWith(task =>
                    {
                        _assetAfterInvestment = task.Result;
                        StateHasChanged();
                    });
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (_metaMaskInstalled = await Javascript!.MetaMaskInstalledAsync())
            {
                await RefreshAsync();
            }
        }

        private async Task InvestAsync()
        {
            await Javascript!.InvestAsync(_ethAmountToInvest.ToString());
        }

        private async Task RefreshAsync()
        {
            _account = string.Empty;
            StateHasChanged();
            _account = await Javascript!.RequestAccountAsync();
            _isWhitelisted = await Javascript!.IsWhitelistedAsync(_account);
            _assetBalance = await Javascript!.BalanceOfAsync(_account);
            _totalSupply = await Javascript!.TotalSupplyAsync();
            _stake = (_totalSupply == "0") ? "no" : $"{Math.Round(Convert.ToDecimal(_assetBalance) * 100 / Convert.ToDecimal(_totalSupply), 4)}%";
            _ethBalance = await Javascript!.EthBalanceOfAsync(_account);
            EthAmountToInvest = 1M;
            _remainingReward = await Javascript!.RemainingRewardForAsync(_account);
            _availableReward = await Javascript!.AvailableRewardForAsync(_account);
            _nextRewardUnlock = await Javascript!.NextRewardUnlockForAsync(_account);
            _withdrawnReward = await Javascript!.WithdrawnRewardForAsync(_account);
        }

        private async Task ReinvestRewardAsync()
        {
            await Javascript!.ReinvestRewardAsync();
        }

        private async Task WithdrawRewardAsync()
        {
            await Javascript!.WithdrawRewardAsync();
        }
    }
}
