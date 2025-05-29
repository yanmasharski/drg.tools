namespace DRG.Ads
{
    /// <summary>
    /// Represents an ad impression with information about the ad that was shown
    /// </summary>
    public interface IAdImpression
    {
        /// <summary>
        /// The format of the ad
        /// </summary>
        AdFormat format { get; }

        /// <summary>
        /// The ad network provider that served this ad
        /// </summary>
        string provider { get; }

        /// <summary>
        /// The placement identifier where this ad was shown
        /// </summary>
        string placement { get; }

        /// <summary>
        /// Whether the ad was successfully completed (e.g. user watched rewarded ad to completion)
        /// </summary>
        bool success { get; }
    }
}