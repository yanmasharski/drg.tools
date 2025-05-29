#if DRG_APPLOVIN
namespace DRG.Ads
{
    public class AdImpressionApplovin : IAdImpression
    {
        public AdFormat format { get; }
        public string provider { get; }
        public string placement { get; }
        public bool success { get; }

        public AdImpressionApplovin(AdFormat format, string provider, string placement, bool success)
        {
            this.format = format;
            this.provider = provider;
            this.placement = placement;
            this.success = success;
        }
    }
}
#endif