namespace DRG.Ads
{
    using Logs;
    using System;

    public class AdsController : IAdsController
    {
        public event Action<IAdImpression> onAdStarted;
        public event Action<IAdImpression> onAdCompleted;

        private readonly IFullscreenAd interstitial;
        private readonly IFullscreenAd rewarded;

        private readonly ILogger logger;

        public AdsController(IFullscreenAd interstitial, IFullscreenAd rewarded, ILogger logger)
        {
            this.interstitial = interstitial;
            this.rewarded = rewarded;
            this.logger = logger;
        }

        public bool isReadyInterstitial => interstitial.isReady;
        public bool isReadyRewarded => rewarded.isReady;
        public bool isReadyBanner => false;

        public void ShowInterstitial(Action<IAdImpression> onClose, string placement = "")
        {
            ShowFullscreenAd(AdFormat.Interstitial, interstitial, onClose, placement);
        }

        public void ShowRewarded(Action<IAdImpression> onClose, string placement = "")
        {
            ShowFullscreenAd(AdFormat.Rewarded, rewarded, onClose, placement);
        }

        public void ShowBanner(string placement)
        {
            // NOT READY
        }

        public void HideBanner(string placement)
        {
            // NOT READY
        }

        private void ShowFullscreenAd(AdFormat format, IFullscreenAd ad, Action<IAdImpression> onClose, string placement)
        {
            if (!ad.isReady)
            {
                return;
            }

            try
            {
                onAdStarted?.Invoke(new AdImpressionEmpty(format, placement));
            }
            catch (Exception e)
            {
                logger.LogException(e);
            }

            ad.Show(OnAdCompleted, placement);

            void OnAdCompleted(IAdImpression impression)
            {
                try
                {
                    onClose?.Invoke(impression);
                }
                catch (Exception e)
                {
                    logger.LogException(e);
                }

                try
                {
                    onAdCompleted?.Invoke(impression);
                }
                catch (Exception e)
                {
                    logger.LogException(e);
                }
            }
        }

    }
}