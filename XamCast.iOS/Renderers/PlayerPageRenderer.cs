using System;
using AVFoundation;
using AVKit;
using CoreGraphics;
using Foundation;
using Google.Cast;
using Microsoft.MobCAT;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamCast;
using XamCast.iOS.Renderers;

[assembly: ExportRenderer(typeof(PlayerPage), typeof(PlayerPageRenderer))]
namespace XamCast.iOS.Renderers
{
    public class PlayerPageRenderer : PageRenderer
    {
        Lazy<IChromecastService> chromecastService = new Lazy<IChromecastService>(() => ServiceContainer.Resolve<IChromecastService>());
        XamCast.Models.MediaInfo mediaInfo;

        AVPlayer avp;
        AVPlayerViewController avpvc;

        public PlayerPageRenderer()
        {
            mediaInfo = chromecastService.Value.GetPlaybackAsset();
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);



            if (e.OldElement != null || Element == null)
            {
                return;
            }

            try
            {
                SetupEventsAndHooks();
                SetupUserInterface();
                StartVideoPlayback();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"\t\t\tERROR: {ex.Message}");
            }

            void StartVideoPlayback()
            {
                avp.Play();
            }

            void SetupUserInterface()
            {

                var url = NSUrl.FromString(mediaInfo.SourceURL);
                avp = new AVPlayer(url);
                avpvc = new AVPlayerViewController();
                avpvc.Player = avp;
                AddChildViewController(avpvc);
                avpvc.View.Frame = new CGRect(0, 100, 375, 300);
                avpvc.ShowsPlaybackControls = true;
                View.AddSubview(avpvc.View);

                var castButton = new UICastButton(new CGRect(50, 20, 24, 24));
                View.AddSubview(castButton);
            }

            void SetupEventsAndHooks()
            {
                //var fairPlayAuthProxy = new BCOVFPSBrightcoveAuthProxy(null, null);
                //var fps = sDKManager.CreateFairPlaySessionProviderWithAuthorizationProxy(fairPlayAuthProxy, null);

                ////Create the playback controller
                //playbackController = sDKManager.CreateFairPlayPlaybackControllerWithAuthorizationProxy(fairPlayAuthProxy);
                //playbackController.SetAutoPlay(true);
                //playbackController.SetAutoAdvance(false);
                //playbackController.Delegate = new BCPlaybackControllerDelegate();

                ////USING CUSTOM GoogleCastManager
                //GoogleCastManager googleCastManager = new GoogleCastManager();
                //var gcmPlaybackSession = new XamBCPlaybackSessionConsumer(googleCastManager);
                //googleCastManager.gcmDelegate = new XamGoogleCastManagerDelegate(playbackController);
                //playbackController.AddSessionConsumer(gcmPlaybackSession);

                //// Set up our player view. Create with a standard VOD layout.
                //var options = new BCOVPUIPlayerViewOptions() { ShowPictureInPictureButton = true };
                //playerView = new BCOVPUIPlayerView(playbackController, options, BCOVPUIBasicControlView.BasicControlViewWithVODLayout());
                //playerView.Delegate = new BCUIPlaybackViewController();
                //playerView.PlaybackController = playbackController;
            }
        }
    }
}
