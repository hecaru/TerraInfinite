/*========== general functions ==========*/

window.toAssetAmount = function (ethAmount) {
    var wei = parseInt(parseFloat(ethAmount) * Math.pow(10, chain.nativeCurrency.decimals));
    return (wei / Math.pow(10, asset.decimals)).toString();
}

window.metaMaskInstalled = function () {
    return typeof ethereum !== "undefined";
}

window.requestAccount = async function () {
    try {
        var accounts = await ethereum.request({ method: "eth_requestAccounts" });
        if (await ethereum.request({ method: "eth_chainId" }) != chain.chainId) {
            try {
                await ethereum.request({
                    method: "wallet_switchEthereumChain",
                    params: [{ chainId: chain.chainId }]
                });
            }
            catch (ex) {
                if (ex.code === 4902) {
                    await ethereum.request({
                        method: "wallet_addEthereumChain",
                        params: [chain]
                    });
                }
                else {
                    throw ex;
                }
            }
        }
        return accounts[0];
    }
    catch (ex) {
        return null;
    }
}

window.watchAsset = async function () {
    await ethereum.request({
        method: "wallet_watchAsset",
        params: {
            type: "ERC20",
            options: asset
        }
    });
}

window.ethBalanceOf = async function (account) {
    if (!account) {
        return "0";
    }
    return Web3.utils.fromWei(await ethereum.request({
        method: "eth_getBalance",
        params: [account, "latest"]
    }));
}



/*========== account functions ==========*/

window.contractAddress = function () {
    return asset.address;
}

window.viewOnEtherscan = function (address) {
    open(chain.blockExplorerUrls[0] + "address/" + address, "_blank");
}

window.smartContract = function () {
    var web3 = new Web3(window.ethereum);
    return new web3.eth.Contract(metadata.output.abi, window.contractAddress());
}

window.smartContract.balanceOf = async function (account) {
    return window.toAssetAmount(Web3.utils.fromWei(await this().methods.balanceOf(account).call()));
}

window.smartContract.totalSupply = async function () {
    return window.toAssetAmount(Web3.utils.fromWei(await this().methods.totalSupply().call()));
}

window.smartContract.invest = async function (ethAmount) {
    var wei = parseInt(parseFloat(ethAmount) * Math.pow(10, chain.nativeCurrency.decimals));
    try {
        await this().methods.invest().send({ from: await window.requestAccount(), value: Web3.utils.toHex(wei) });
    } catch { }
}

window.smartContract.remainingRewardFor = async function (account) {
    return Web3.utils.fromWei(await this().methods.remainingRewardFor(account).call());
}

window.smartContract.availableRewardFor = async function (account) {
    return Web3.utils.fromWei(await this().methods.availableRewardFor(account).call());
}

window.smartContract.nextRewardUnlockFor = async function (account) {
    return parseInt(await this().methods.nextRewardUnlockFor(account).call());
}

window.smartContract.reinvestReward = async function () {
    try {
        await this().methods.reinvestReward().send({ from: await window.requestAccount() });
    } catch { }
}

window.smartContract.withdrawReward = async function () {
    try {
        await this().methods.withdrawReward().send({ from: await window.requestAccount() });
    } catch { }
}

window.smartContract.withdrawnRewardFor = async function (account) {
    return Web3.utils.fromWei(await this().methods.withdrawnRewardFor(account).call());
}



/*========== whitelisted functions ==========*/

window.smartContract.whitelistedLength = async function () {
    return parseInt(await this().methods.whitelistedLength().call());
}

window.smartContract.whitelisted = async function (i) {
    return await this().methods.whitelisted(i).call();
}

window.smartContract.isWhitelisted = async function (account) {
    return await this().methods.isWhitelisted(account).call();
}



/*========== holders functions ==========*/

window.smartContract.holdersLength = async function () {
    return parseInt(await this().methods.holdersLength().call());
}

window.smartContract.holders = async function (i) {
    return await this().methods.holders(i).call();
}

window.smartContract.isHolder = async function (account) {
    return await this().methods.isHolder(account).call();
}



/*========== rewards functions ==========*/

window.smartContract.lockdownPeriod = async function () {
    return await this().methods.lockdownPeriod().call();
}

window.smartContract.rewardsTimestampsLength = async function () {
    return parseInt(await this().methods.rewardsTimestampsLength().call());
}

window.smartContract.rewardsTimestamps = async function (i) {
    return parseInt(await this().methods.rewardsTimestamps(i).call());
}

window.smartContract.rewardsLength = async function (timestamp) {
    return parseInt(await this().methods.rewardsLength(timestamp).call());
}

window.smartContract.rewards = async function (timestamp, i) {
    var rewards = await this().methods.rewards(timestamp, i).call();
    rewards.amount = Web3.utils.fromWei(rewards.amount);
    return rewards;
}

window.smartContract.rewardsWithdrawn = async function (timestamp, address) {
    return await this().methods.rewardsWithdrawn(timestamp, address).call();
}



/*========== liquidity pool functions ==========*/

window.smartContract.lpFee = async function () {
    return await this().methods.lpFee().call();
}

window.smartContract.liquidityPool = async function () {
    return Web3.utils.fromWei(await this().methods.liquidityPool().call());
}
