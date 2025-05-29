namespace DRG.Ads
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FullscreenAdAuction : IFullscreenAd
    {
        private readonly List<IFullscreenAd> adsList = new List<IFullscreenAd>();

        public bool isReady => adsList.Count > 0 && adsList.Any(ad => ad.isReady);
        public float bid => adsList.Max(ad => ad.bid);

        private AdFormat primaryFormat;


        public FullscreenAdAuction(IFullscreenAd[] ads, AdFormat primaryFormat)
        {
            this.primaryFormat = primaryFormat;
            if (ads == null || ads.Length == 0)
            {
                return;
            }

            adsList.AddRange(ads);
        }

        public void AddAd(IFullscreenAd ad)
        {
            adsList.Add(ad);
        }

        public void Show(Action<IAdImpression> onClose, string placement = "")
        {
            var ad = GetAds();
            if (ad == null)
            {
                onClose?.Invoke(new AdImpressionEmpty(primaryFormat, placement));
                return;
            }

            ad.Show(onClose, placement);
        }

        private IFullscreenAd GetAds()
        {
            return adsList.OrderBy(ads => ads.bid).FirstOrDefault(ads => ads.isReady);
        }
    }
}