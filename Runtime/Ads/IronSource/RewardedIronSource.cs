#if DRG_IRONSOURCE
namespace DRG.Ads
{
    using System;
    using Utils;
    using Unity.Services.LevelPlay;
    using UnityEngine;

    public class RewardedIronSource : IFullscreenAd
    {
        private readonly string adUnitId;
        private readonly IDebouncedExecutor debouncedExecutor;
        private readonly LevelPlayRewardedAd rewardedAd;

        private IDebouncedExecutor.ICommand loadCommand;
        private Action<IAdImpression> callback;
        private string placement = "";
        private int retryCount = 0;
        private bool isRewarded = false;

        public RewardedIronSource(string adUnitId, IDebouncedExecutor debouncedExecutor)
        {
            this.adUnitId = adUnitId;
            this.debouncedExecutor = debouncedExecutor;

            rewardedAd = new LevelPlayRewardedAd(adUnitId);

            rewardedAd.OnAdLoaded += HandleAdLoaded;
            rewardedAd.OnAdLoadFailed += HandleAdLoadFailed;
            rewardedAd.OnAdDisplayFailed += HandleAdDisplayFailed;
            rewardedAd.OnAdClosed += HandleAdClosed;
            rewardedAd.OnAdInfoChanged += HandleAdInfoChanged;
            rewardedAd.OnAdRewarded += HandleAdReward;
            LoadAd();
        }

        ~RewardedIronSource()
        {
            rewardedAd.OnAdLoaded -= HandleAdLoaded;
            rewardedAd.OnAdLoadFailed -= HandleAdLoadFailed;
            rewardedAd.OnAdDisplayFailed -= HandleAdDisplayFailed;
            rewardedAd.OnAdClosed -= HandleAdClosed;
            rewardedAd.OnAdInfoChanged -= HandleAdInfoChanged;
            rewardedAd.OnAdRewarded -= HandleAdReward;
            loadCommand?.Cancel();
        }

        public bool isReady => rewardedAd.IsAdReady();
        public float bid { get; private set; }

        public void Show(Action<IAdImpression> onClose, string placement = "")
        {
            isRewarded = false;
            this.placement = placement;
            callback = onClose;
            rewardedAd.ShowAd(placement);
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
            callback?.Invoke(new AdImpressionIronSource(AdFormat.Rewarded, placement, isRewarded));
            LoadAd();
        }

        private void HandleAdClosed(LevelPlayAdInfo adInfo)
        {
            if (adUnitId != adInfo.AdUnitId)
            {
                return;
            }

            callback?.Invoke(new AdImpressionIronSource(AdFormat.Rewarded, placement, isRewarded));
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

        private void HandleAdReward(LevelPlayAdInfo adInfo, LevelPlayReward reward)
        {
            if (adUnitId != adInfo.AdUnitId)
            {
                return;
            }

            isRewarded = true;
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
                rewardedAd.LoadAd();
            }
            else
            {
                retryCount++;
                var retryDelay = Mathf.Min(retryCount * 2f, 30f);
                loadCommand?.Cancel();

                loadCommand =
                    debouncedExecutor.Execute((int)retryDelay * 60, () => { rewardedAd.LoadAd(); });
            }
        }
    }
}
#endif