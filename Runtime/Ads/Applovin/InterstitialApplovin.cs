#if DRG_APPLOVIN
namespace DRG.Ads
{
    using Utils;
    using System;
    using UnityEngine;

    public class InterstitialApplovin : IFullscreenAd
    {
        private readonly string adUnitId;
        private readonly IDebouncedExecutor debouncedExecutor;

        private IDebouncedExecutor.ICommand loadCommand;
        
        private Action<IAdImpression> callback;
        private string placement = "";
        private int retryCount = 0;

        public InterstitialApplovin(string adUnitId, IDebouncedExecutor debouncedExecutor)
        {
            this.adUnitId = adUnitId;
            this.debouncedExecutor = debouncedExecutor;

            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += HandleInterstitialAvailable;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += HandleInterstitialUnavailable;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += HandleInterstitialAdClosed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += HandleInterstitialShowFail;

            LoadAd();
        }

        ~InterstitialApplovin()
        {
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= HandleInterstitialAvailable;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= HandleInterstitialUnavailable;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= HandleInterstitialAdClosed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent -= HandleInterstitialShowFail;
            loadCommand?.Cancel();
        }

        public bool isReady => MaxSdk.IsInterstitialReady(adUnitId);
        public float bid { get; private set; }

        public void Show(Action<IAdImpression> callback, string placement = "")
        {
            this.callback = callback;
            this.placement = placement;
            MaxSdk.ShowInterstitial(adUnitId, placement);
        }

        private void HandleInterstitialAvailable(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != this.adUnitId)
            {
                return;
            }

            bid = (float)adInfo.Revenue;
        }

        private void HandleInterstitialShowFail(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != this.adUnitId)
            {
                return;
            }

            callback?.Invoke(new AdImpressionApplovin(AdFormat.Interstitial, placement, false));
            LoadAd();
        }

        private void HandleInterstitialAdClosed(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (adUnitId != this.adUnitId)
            {
                return;
            }

            callback?.Invoke(new AdImpressionApplovin(AdFormat.Interstitial, placement, true));
            LoadAd();
        }

        private void HandleInterstitialUnavailable(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
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
                MaxSdk.LoadInterstitial(adUnitId);
            }
            else
            {
                retryCount++;
                var retryDelay = Mathf.Min(retryCount * 2f, 30f);
                loadCommand?.Cancel();
                
                loadCommand = debouncedExecutor.Execute((int)retryDelay * 60, () =>
                {
                    MaxSdk.LoadInterstitial(adUnitId);
                });
            }
        }
    }
}
#endif