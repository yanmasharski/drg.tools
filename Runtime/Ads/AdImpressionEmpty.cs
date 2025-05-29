namespace DRG.Ads
{
    public class AdImpressionEmpty : IAdImpression
    {
        public AdFormat format { get; }
        public string provider => "unknown";
        public string placement { get; }
        public bool success => false;

        public AdImpressionEmpty(AdFormat format, string placement)
        {
            this.format = format;
            this.placement = placement;
        }
    }
}