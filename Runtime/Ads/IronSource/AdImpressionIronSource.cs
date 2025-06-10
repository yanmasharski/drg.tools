#if DRG_IRONSOURCE
namespace DRG.Ads
{
    public class AdImpressionIronSource: IAdImpression
    {
        public AdFormat format { get; }
        public string provider { get; }
        public string placement { get; }
        public bool success { get; }

        public AdImpressionIronSource(AdFormat format, string placement, bool success)
        {
            this.format = format;
            provider = "IronSource";
            this.placement = placement;
            this.success = success;
        }
    }
}
#endif