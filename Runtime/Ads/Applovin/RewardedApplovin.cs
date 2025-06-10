#if DRG_APPLOVIN
namespace DRG.Ads
{
    using Utils;
    using System;
    using UnityEngine;
    public class RewardedApplovin : IFullscreenAd
    {
        private readonly string adUnitId;
        private readonly IDebouncedExecutor debouncedExecutor;
        
        private Action<IAdImpression> callback;
        private IDebouncedExecutor.ICommand loadCommand;
        private bool isRewarded = false;
        private string placement = "";
        private int retryCount = 0;

        public RewardedApplovin(string adUnitId, IDebouncedExecutor debouncedExecutor)
        {
            this.adUnitId = adUnitId;
            this.debouncedExecutor = debouncedExecutor;

            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += HandleAdLoaded;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += HandleAdLoadFailed;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += HandleAdClosed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += HandleAdDisplayFailed;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += HandleAdReward;

            LoadAd();
        }

        ~RewardedApplovin()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= HandleAdLoaded;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= HandleAdLoadFailed;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= HandleAdClosed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= HandleAdDisplayFailed;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= HandleAdReward;
            loadCommand?.Cancel();
        }

        public bool isReady => MaxSdk.IsRewardedAdReady(adUnitId);
        public float bid { get; private set; }

        public void Show(Action<IAdImpression> callback, string placement = "")
        {
            this.callback = callback;
            isRewarded = false;
            this.placement = placement;
            MaxSdk.ShowRewardedAd(adUnitId, placement);
        }

        private void HandleAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != this.adUnitId)
            {
                return;
            }

            bid = (float)adInfo.Revenue;
        }

        private void HandleAdReward(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != this.adUnitId)
            {
                return;
            }

            isRewarded = true;
        }

        private void HandleAdDisplayFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != this.adUnitId)
            {
                return;
            }

            callback?.Invoke(new AdImpressionApplovin(AdFormat.Rewarded, placement, isRewarded));

            LoadAd();
        }

        private void HandleAdClosed(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != this.adUnitId)
            {
                return;
            }

            callback?.Invoke(new AdImpressionApplovin(AdFormat.Rewarded, placement, isRewarded));

            LoadAd();
        }

        private void HandleAdLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            if (adUnitId != this.adUnitId)
            {
                return;
            }

            LoadAd();
        }

        private void LoadAd(bool resetDelay = false)
        {
            if (resetDelay)
            {
                retryCount = 0;
            }

            if (loadCommand != null && loadCommand.isRunning)
            {
                return;
            }

            if (retryCount == 0)
            {
                MaxSdk.LoadRewardedAd(adUnitId);
            }
            else
            {
                retryCount++;
                var retryDelay = Mathf.Min(retryCount * 2f, 30f);
                loadCommand?.Cancel();
                loadCommand = debouncedExecutor.Execute((int) retryDelay * 60, () =>
                {
                    MaxSdk.LoadRewardedAd(adUnitId);
                });
            }

        }

    }
}
#endif