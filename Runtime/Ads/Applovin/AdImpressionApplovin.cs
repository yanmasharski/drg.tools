#if DRG_APPLOVIN
namespace DRG.Ads
{
    public class AdImpressionApplovin : IAdImpression
    {
        public AdFormat format { get; }
        public string provider { get; }
        public string placement { get; }
        public bool success { get; }

        public AdImpressionApplovin(AdFormat format, string placement, bool success)
        {
            this.format = format;
            provider = "Applovin";
            this.placement = placement;
            this.success = success;
        }
    }
}
#endif