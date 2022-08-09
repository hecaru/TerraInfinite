using Microsoft.JSInterop;

namespace TerraInfinite.Services
{
    public class JavascriptService
    {
        private readonly IJSRuntime _jsRuntime;

        public JavascriptService(
            IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        private static DateTime? TimestampToDateTime(long timestamp)
        {
            return timestamp == 0 ? null : new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp).ToLocalTime();
        }



        /*========== general functions ==========*/

        public ValueTask<string> ToAssetAmountAsync(string ethAmount)
        {
            return _jsRuntime.InvokeAsync<string>("toAssetAmount", ethAmount);
        }

        public ValueTask<bool> MetaMaskInstalledAsync()
        {
            return _jsRuntime.InvokeAsync<bool>("metaMaskInstalled");
        }

        public ValueTask<string> RequestAccountAsync()
        {
            return _jsRuntime.InvokeAsync<string>("requestAccount");
        }

        public ValueTask WatchAssetAsync()
        {
            return _jsRuntime.InvokeVoidAsync("watchAsset");
        }

        public ValueTask<string> EthBalanceOfAsync(string account)
        {
            return _jsRuntime.InvokeAsync<string>("ethBalanceOf", account);
        }



        /*========== account functions ==========*/

        public ValueTask<string> ContractAddressAsync()
        {
            return _jsRuntime.InvokeAsync<string>("contractAddress");
        }

        public ValueTask ViewOnEtherscanAsync(string address)
        {
            return _jsRuntime.InvokeVoidAsync("viewOnEtherscan", address);
        }

        public ValueTask<string> BalanceOfAsync(string account)
        {
            return _jsRuntime.InvokeAsync<string>("smartContract.balanceOf", account);
        }

        public ValueTask<string> TotalSupplyAsync()
        {
            return _jsRuntime.InvokeAsync<string>("smartContract.totalSupply");
        }

        public ValueTask InvestAsync(string ethAmount)
        {
            return _jsRuntime.InvokeVoidAsync("smartContract.invest", ethAmount);
        }

        public ValueTask<string> RemainingRewardForAsync(string account)
        {
            return _jsRuntime.InvokeAsync<string>("smartContract.remainingRewardFor", account);
        }

        public ValueTask<string> AvailableRewardForAsync(string account)
        {
            return _jsRuntime.InvokeAsync<string>("smartContract.availableRewardFor", account);
        }

        public async ValueTask<DateTime?> NextRewardUnlockForAsync(string account)
        {
            return TimestampToDateTime(await _jsRuntime.InvokeAsync<long>("smartContract.nextRewardUnlockFor", account));

        }

        public ValueTask ReinvestRewardAsync()
        {
            return _jsRuntime.InvokeVoidAsync("smartContract.reinvestReward");
        }

        public ValueTask WithdrawRewardAsync()
        {
            return _jsRuntime.InvokeVoidAsync("smartContract.withdrawReward");
        }

        public ValueTask<string> WithdrawnRewardForAsync(string account)
        {
            return _jsRuntime.InvokeAsync<string>("smartContract.withdrawnRewardFor", account);
        }



        /*========== whitelisted functions ==========*/

        public async ValueTask<List<string>> WhitelistedAsync()
        {
            List<string> whitelisted = new();
            long whitelistedLength = await _jsRuntime.InvokeAsync<long>("smartContract.whitelistedLength");
            for (long i = 0; i < whitelistedLength; i++)
            {
                whitelisted.Add(await _jsRuntime.InvokeAsync<string>("smartContract.whitelisted", i));
            }
            return whitelisted;
        }

        public ValueTask<long> WhitelistedLengthAsync()
        {
            return _jsRuntime.InvokeAsync<long>("smartContract.whitelistedLength");
        }

        public ValueTask<bool> IsWhitelistedAsync(string account)
        {
            return _jsRuntime.InvokeAsync<bool>("smartContract.isWhitelisted", account);
        }



        /*========== holders functions ==========*/

        public async ValueTask<Dictionary<string, string>> HoldersAsync()
        {
            Dictionary<string, string> holders = new();
            long holdersLength = await _jsRuntime.InvokeAsync<long>("smartContract.holdersLength");
            for (long i = 0; i < holdersLength; i++)
            {
                string holder = await _jsRuntime.InvokeAsync<string>("smartContract.holders", i);
                holders.Add(holder, await _jsRuntime.InvokeAsync<string>("smartContract.balanceOf", holder));
            }
            return holders
                .OrderByDescending(holder => Convert.ToDecimal(holder.Value))
                .ToDictionary(holder => holder.Key, holder => holder.Value);
        }

        public ValueTask<long> HoldersLengthAsync()
        {
            return _jsRuntime.InvokeAsync<long>("smartContract.holdersLength");
        }

        public ValueTask<bool> IsHolderAsync(string account)
        {
            return _jsRuntime.InvokeAsync<bool>("smartContract.isHolder", account);
        }



        /*========== rewards functions ==========*/

        public async ValueTask<TimeSpan> LockdownPeriodAsync()
        {
            return TimeSpan.FromSeconds(Convert.ToInt64(await _jsRuntime.InvokeAsync<string>("smartContract.lockdownPeriod")));
        }

        public async ValueTask<Dictionary<DateTime, Dictionary<string, string>>> RewardsAsync()
        {
            Dictionary<DateTime, Dictionary<string, string>> rewards = new();
            long rewardsTimestampsLength = await _jsRuntime.InvokeAsync<long>("smartContract.rewardsTimestampsLength");
            for (long i = 0; i < rewardsTimestampsLength; i++)
            {
                long timestamp = await _jsRuntime.InvokeAsync<long>("smartContract.rewardsTimestamps", i);
                long rewardsLength = await _jsRuntime.InvokeAsync<long>("smartContract.rewardsLength", timestamp);
                Dictionary<string, string> rewardsAtTimestamp = new();
                for (long j = 0; j < rewardsLength; j++)
                {
                    Dictionary<string, string> reward = await _jsRuntime.InvokeAsync<Dictionary<string, string>>("smartContract.rewards", timestamp, j);
                    if (await _jsRuntime.InvokeAsync<bool>("smartContract.rewardsWithdrawn", timestamp, reward["account"]))
                    {
                        reward["account"] += "~";
                    }
                    rewardsAtTimestamp.Add(reward["account"], reward["amount"]);
                }
                rewards.Add(TimestampToDateTime(timestamp)!.Value, rewardsAtTimestamp);
            }
            return rewards;
        }

        public ValueTask<long> RewardsTimestampsLengthAsync()
        {
            return _jsRuntime.InvokeAsync<long>("smartContract.rewardsTimestampsLength");
        }



        /*========== liqudidity pool functions ==========*/

        public ValueTask<string> LpFeeAsync()
        {
            return _jsRuntime.InvokeAsync<string>("smartContract.lpFee");
        }

        public ValueTask<string> LiquidityPoolAsync()
        {
            return _jsRuntime.InvokeAsync<string>("smartContract.liquidityPool");
        }
    }
}
