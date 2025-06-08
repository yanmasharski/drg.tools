#if DRG_IRONSOURCE
namespace DRG.Ads
{
    using Logs;
    using Utils;
    using Unity.Services.LevelPlay;

    public class IronSourceAdFactory
    {
        private readonly string appKey;
        private readonly ILogger logger;
        private readonly IDebouncedExecutor debouncedExecutor;

        private RewardedIronSource rewarded;
        private InterstitialIronSource interstitial;

        private bool initCalled = false;

        public IronSourceAdFactory(string appKey, ILogger logger, IDebouncedExecutor debouncedExecutor)
        {
            this.appKey = appKey;
            this.logger = logger;
            this.debouncedExecutor = debouncedExecutor;

            IronSource.Agent.validateIntegration();
            LevelPlay.OnInitSuccess += SdkInitializedEvent;
            LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
        }

        ~IronSourceAdFactory()
        {
            LevelPlay.OnInitSuccess -= SdkInitializedEvent;
            LevelPlay.OnInitFailed -= SdkInitializationFailedEvent;
        }

        public void Initialize()
        {
            if (initCalled)
            {
                return;
            }

            initCalled = true;
            LevelPlay.Init(appKey);
        }

        public IFullscreenAd GetRewardedVideo(string adUnitId)
        {
            Initialize();
            return rewarded ??= new RewardedIronSource(adUnitId, debouncedExecutor);
        }

        public IFullscreenAd GetInterstitial(string adUnitId)
        {
            Initialize();
            return interstitial ??= new InterstitialIronSource(adUnitId, debouncedExecutor);
        }

        private void SdkInitializedEvent(LevelPlayConfiguration config)
        {
        }

        private void SdkInitializationFailedEvent(LevelPlayInitError error)
        {
            logger.Log("unity-script: I got SdkInitializationFailedEvent with error: " + error);
        }
    }
}
#endif