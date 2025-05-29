namespace DRG.Ads
{
    using System;

    /// <summary>
    /// Interface for fullscreen ads like interstitials and rewarded videos
    /// </summary>
    public interface IFullscreenAd
    {
        /// <summary>
        /// Whether the ad is loaded and ready to be shown
        /// </summary>
        bool isReady { get; }

        /// <summary>
        /// The bid price for this ad, used to determine which ad network to show
        /// </summary>
        float bid { get; }

        /// <summary>
        /// Shows the fullscreen ad
        /// </summary>
        /// <param name="onClose">Callback triggered when the ad closes</param>
        /// <param name="placement">Optional placement identifier for the ad</param>
        void Show(Action<IAdImpression> onClose, string placement = "");
    }
}
