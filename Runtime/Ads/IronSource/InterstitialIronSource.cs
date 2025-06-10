#if DRG_IRONSOURCE
namespace DRG.Ads
{
    using System;
    using Utils;
    using Unity.Services.LevelPlay;
    using UnityEngine;

    public class InterstitialIronSource : IFullscreenAd
    {
        private readonly string adUnitId;
        private readonly IDebouncedExecutor debouncedExecutor;
        private readonly LevelPlayInterstitialAd interstitialAd;

        private IDebouncedExecutor.ICommand loadCommand;

        private Action<IAdImpression> callback;
        private string placement = "";
        private int retryCount = 0;

        public InterstitialIronSource(string adUnitId, IDebouncedExecutor debouncedExecutor)
        {
            this.adUnitId = adUnitId;
            this.debouncedExecutor = debouncedExecutor;

            interstitialAd = new LevelPlayInterstitialAd(adUnitId);

            interstitialAd.OnAdLoaded += HandleAdLoaded;
            interstitialAd.OnAdLoadFailed += HandleAdLoadFailed;
            interstitialAd.OnAdDisplayFailed += HandleAdDisplayFailed;
            interstitialAd.OnAdClosed += HandleAdClosed;
            interstitialAd.OnAdInfoChanged += HandleAdInfoChanged;
            LoadAd();
        }

        ~InterstitialIronSource()
        {
            interstitialAd.OnAdLoaded -= HandleAdLoaded;
            interstitialAd.OnAdLoadFailed -= HandleAdLoadFailed;
            interstitialAd.OnAdDisplayFailed -= HandleAdDisplayFailed;
            interstitialAd.OnAdClosed -= HandleAdClosed;
            interstitialAd.OnAdInfoChanged -= HandleAdInfoChanged;
            loadCommand?.Cancel();
        }

        public bool isReady => interstitialAd.IsAdReady();
        public float bid { get; private set; }

        public void Show(Action<IAdImpression> onClose, string placement = "")
        {
            this.placement = placement;
            callback = onClose;
            interstitialAd.ShowAd(placement);
        }

        private void HandleAdLoaded(LevelPlayAdInfo adInfo)
        {
            if (adUnitId != adInfo.AdUnitId)
            {
                return;
            }

            bid = (float)(adInfo.Revenue ?? 0f);
        }

        private void HandleAdLoadFailed(LevelPlayAdError error)
        {
            LoadAd();
        }
        
        private void HandleAdDisplayFailed(LevelPlayAdDisplayInfoError infoError)
        {
            callback?.Invoke(new AdImpressionIronSource(AdFormat.Interstitial, placement, false));
            LoadAd();
        }

        private void HandleAdClosed(LevelPlayAdInfo adInfo)
        {
            if (adUnitId != adInfo.AdUnitId)
            {
                return;
            }

            callback?.Invoke(new AdImpressionIronSource(AdFormat.Interstitial, placement, true));
            LoadAd();
        }

        private void HandleAdInfoChanged(LevelPlayAdInfo adInfo)
        {
            if (adUnitId != adInfo.AdUnitId)
            {
                return;
            }

            bid = (float)(adInfo.Revenue ?? 0f);
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
                interstitialAd.LoadAd();
            }
            else
            {
                retryCount++;
                var retryDelay = Mathf.Min(retryCount * 2f, 30f);
                loadCommand?.Cancel();

                loadCommand =
                    debouncedExecutor.Execute((int)retryDelay * 60, () => { interstitialAd.LoadAd(); });
            }
        }
    }
}
#endif