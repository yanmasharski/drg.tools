namespace DRG.Ads
{
    using System;

    /// <summary>
    /// Controller interface for managing different types of ads (interstitial, rewarded, banner)
    /// </summary>
    public interface IAdsController
    {
        /// <summary>
        /// Event triggered when an ad finishes showing, whether completed successfully or not
        /// </summary>
        public event Action<IAdImpression> onAdCompleted;

        /// <summary>
        /// Event triggered when an ad starts showing
        /// </summary>
        public event Action<IAdImpression> onAdStarted;
        
        /// <summary>
        /// Whether an interstitial ad is ready to be shown
        /// </summary>
        bool isReadyInterstitial { get; }

        /// <summary>
        /// Whether a rewarded ad is ready to be shown
        /// </summary>
        bool isReadyRewarded { get; }

        /// <summary>
        /// Whether a banner ad is ready to be shown
        /// </summary>
        bool isReadyBanner { get; }

        /// <summary>
        /// Shows an interstitial ad
        /// </summary>
        /// <param name="onClose">Callback triggered when the ad closes</param>
        /// <param name="placement">Optional placement identifier for the ad</param>
        void ShowInterstitial(Action<IAdImpression> onClose, string placement = "");

        /// <summary>
        /// Shows a rewarded ad
        /// </summary>
        /// <param name="onClose">Callback triggered when the ad closes</param>
        /// <param name="placement">Optional placement identifier for the ad</param>
        void ShowRewarded(Action<IAdImpression> onClose, string placement = "");

        /// <summary>
        /// Shows a banner ad at the specified placement
        /// </summary>
        /// <param name="placement">Placement identifier for the banner ad</param>
        void ShowBanner(string placement);

        /// <summary>
        /// Hides the banner ad at the specified placement
        /// </summary>
        /// <param name="placement">Placement identifier for the banner ad to hide</param>
        void HideBanner(string placement);
    }
}