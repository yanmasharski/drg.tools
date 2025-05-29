#if DRG_APPLOVIN
namespace DRG.Ads
{
    using Utils;
    using System;
    using UnityEngine;
    public class RewardedApplovin : IFullscreenAd
    {
        private const string PROVIDER_NAME = "Applovin";
        private readonly string adUnitId;
        private readonly IDebouncedExecutor debouncedExecutor;
        
        private Action<IAdImpression> callback;
        private IDebouncedExecutor.ICommand loadCommand;

        public RewardedApplovin(string adUnitId, IDebouncedExecutor debouncedExecutor)
        {
            this.adUnitId = adUnitId;
            this.debouncedExecutor = debouncedExecutor;

            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += HandleRewardedVideoAvailable;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += HandleRewardedVideoUnavailable;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += HandleRewardedVideoAdClosed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += HandleRewardedVideoShowFail;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += HandleRewardedVideoAdRewarded;

            LoadAd();
        }

        ~RewardedApplovin()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= HandleRewardedVideoAvailable;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= HandleRewardedVideoUnavailable;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= HandleRewardedVideoAdClosed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= HandleRewardedVideoShowFail;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= HandleRewardedVideoAdRewarded;
            loadCommand?.Cancel();
        }

        public bool isReady => MaxSdk.IsRewardedAdReady(adUnitId);
        public float bid { get; private set; }

        private bool isRewarded = false;
        private string placement = "";
        private int retryCount = 0;

        public void Show(Action<IAdImpression> callback, string placement = "")
        {
            this.callback = callback;
            isRewarded = false;
            this.placement = placement;
            MaxSdk.ShowRewardedAd(adUnitId, placement);
        }

        private void HandleRewardedVideoAvailable(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != this.adUnitId)
            {
                return;
            }

            bid = (float)adInfo.Revenue;
        }

        private void HandleRewardedVideoAdRewarded(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != this.adUnitId)
            {
                return;
            }

            isRewarded = true;
        }

        private void HandleRewardedVideoShowFail(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != this.adUnitId)
            {
                return;
            }

            callback?.Invoke(new AdImpressionApplovin(AdFormat.Rewarded, PROVIDER_NAME, placement, isRewarded));

            LoadAd();
        }

        private void HandleRewardedVideoAdClosed(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != this.adUnitId)
            {
                return;
            }

            callback?.Invoke(new AdImpressionApplovin(AdFormat.Rewarded, PROVIDER_NAME, placement, isRewarded));

            LoadAd();
        }

        private void HandleRewardedVideoUnavailable(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
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