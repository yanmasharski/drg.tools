#if DRG_APPLOVIN
namespace DRG.Ads
{
    using Utils;
    
    public class ApplovinAdFactory
    {
        private readonly string appKey;
        private readonly string[] adUnits;
        private readonly IDebouncedExecutor debouncedExecutor;

        private RewardedApplovin rewardedApplovin;
        private InterstitialApplovin interstitialApplovin;

        public bool isInitialized { get; private set; }

        public ApplovinAdFactory(string appKey, IDebouncedExecutor debouncedExecutor, string[] adUnits = null)
        {
            this.appKey = appKey;
            this.adUnits = adUnits;
            this.debouncedExecutor = debouncedExecutor;

            MaxSdkCallbacks.OnSdkInitializedEvent += SdkInitializedEvent;
        }

        ~ApplovinAdFactory()
        {
            MaxSdkCallbacks.OnSdkInitializedEvent -= SdkInitializedEvent;
        }

        public void Initialize()
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;

            MaxSdk.SetSdkKey(appKey);
            MaxSdk.InitializeSdk(adUnits);
            isInitialized = true;
        }

        public IFullscreenAd GetRewardedVideo(string adUnitId)
        {
            Initialize();
            return rewardedApplovin ??= new RewardedApplovin(adUnitId, debouncedExecutor);
        }

        public IFullscreenAd GetInterstitial(string adUnitId)
        {
            Initialize();
            return interstitialApplovin ??= new InterstitialApplovin(adUnitId, debouncedExecutor);
        }

        private void SdkInitializedEvent(MaxSdkBase.SdkConfiguration sdkConfiguration)
        {
            // AppLovin SDK is initialized, start loading ads
        }
    }
}
#endif