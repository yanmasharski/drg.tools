using System;
using DRG.Ads;
using DRG.Core;
using DRG.Logs;
using DRG.Utils;

namespace DRG.Examples
{
    public class SystemInitExample
    {
        public SystemInitExample()
        {
            var logger = new LoggerUnity();
            var signalBus = new SignalBus(logger);
            var debouncedExecutor = new DebouncedExecutorUnity(StaticMonoBehaviour.instance, logger);

            var applovinAdFactory = new ApplovinAdFactory("appKey", debouncedExecutor);
            var interstitial = applovinAdFactory.GetInterstitial("interstitial_adUnitId");
            var rewarded = applovinAdFactory.GetRewardedVideo("rewardedVideo_adUnitId");
            var adsController = new AdsController(
                new InterstitalBalancer(interstitial),
                new FullscreenAdAuction(new[] { interstitial, rewarded }, AdFormat.Rewarded),
                logger);
            
            adsController.onAdCompleted += AdCompleted;
            return;

            void AdCompleted(IAdImpression obj)
            {
                logger.Log("Ad completed");
            }
        }
    }

    public class InterstitalBalancer : IFullscreenAd
    {
        private int MAX_INT_PER_SESSION = 3;

        private readonly IFullscreenAd ad;
        private int counter;

        public InterstitalBalancer(IFullscreenAd ad)
        {
            this.ad = ad;
            counter = 0;
        }

        public bool isReady => counter < MAX_INT_PER_SESSION && ad.isReady;
        public float bid => ad.bid;

        public void Show(Action<IAdImpression> onClose, string placement = "")
        {
            if (!isReady)
            {
                return;
            }

            ad.Show(OnClose, placement);
            return;

            void OnClose(IAdImpression impression)
            {
                if (!impression.success)
                {
                    return;
                }

                counter++;
            }
        }
    }
}