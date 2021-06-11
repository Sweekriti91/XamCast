using System;
using System.Diagnostics;
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
        NSUrl url;

        public SessionManager sessionManager;
        public SessionManagerListener xamaSessionManagerListener;
        public UIMediaController castMediaController;
        public MediaInformation castMediaInfo;
        public VisualElement currentPage;

        // track state as sample does here https://github.com/googlecast/CastVideos-ios/blob/master/CastVideos-swift/Classes/MediaViewController.swift#L18
        private PlaybackLocation mLocation;
        public enum PlaybackLocation
        {
            LOCAL,
            REMOTE
        }

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
                SetupChromecastThings();
                SetupUserInterface();
                StartVideoPlayback();
                currentPage = Element;
                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"\t\t\tERROR: {ex.Message}");
            }

            void StartVideoPlayback()
            {
                var hasConnection = sessionManager.HasConnectedSession;
                if (hasConnection)
                {
                    mLocation = PlaybackLocation.REMOTE;
                    SwitchToRemotePlayback();
                    ClosePage();
                }

                if (mLocation == PlaybackLocation.LOCAL)
                    avp.Play();
            }

            void SetupUserInterface()
            {
                url = NSUrl.FromString(mediaInfo.SourceURL);
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

            void SetupChromecastThings()
            {
                //setup Cast session manager
                sessionManager = CastContext.SharedInstance.SessionManager;
                xamaSessionManagerListener = new XamSessionManagerListener(this);
                sessionManager.AddListener(xamaSessionManagerListener);

                //castMediaController 
                castMediaController = new UIMediaController();
                castMediaController.Delegate = new XamMediaControllerDelegate();
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            avp.Pause();
            avp.Dispose();
            avpvc.Dispose();
        }

        public void ClosePage()
        {
            currentPage.Navigation.PopAsync();
        }

        //CreateCastMedia
        public void CreateMediaInfo()
        {
            var videoUrl = mediaInfo.SourceURL;
            var vname = mediaInfo.DisplayName;
            var extra = new string[] { "{", "}" };
            

            var metaData = new MediaMetadata(MediaMetadataType.Generic);
            metaData.SetString(mediaInfo.DisplayName, MetadataKey.Title);
            metaData.SetString("Hello World on Fridays!", MetadataKey.Subtitle);

           
            var builder = new MediaInformationBuilder();
            builder.ContentId = videoUrl;
            builder.StreamType = MediaStreamType.Buffered;
            builder.Metadata = metaData;

            castMediaInfo = builder.Build();

        }

        public void SwitchToLocalPlayback(NSError withError)
        {
            Debug.WriteLine("OOPSY Happened to Cast Connect! :: " + withError);
            // set remotemediaclient to null
            //add logic to resume local play, update tracking 
        }

        public void SwitchToRemotePlayback()
        {
            avp.Pause();
            CreateMediaInfo();
            mLocation = PlaybackLocation.REMOTE;

            //ios sample does it via queue, we'll straight up load
            var castSession = CastContext.SharedInstance.SessionManager.CurrentCastSession;
            RemoteMediaClient remoteMediaClient = null;
            var options = new MediaLoadOptions();
            //options.PlayPosition = currentprogress;
            options.Autoplay = true;

            if (castSession != null)
            {
                remoteMediaClient = castSession.RemoteMediaClient;
                remoteMediaClient.MediaQueue?.Clear();
            }

            if (castSession != null && remoteMediaClient != null && castMediaInfo != null)
                remoteMediaClient.LoadMedia(castMediaInfo, options);
            else
                Console.WriteLine("ERROR in SwitchToRemotePlayback");

            ClosePage();
        }

    }

    // MARK: - GCKSessionManagerListener
    public class XamSessionManagerListener : SessionManagerListener
    {
        PlayerPageRenderer playerPage;
        public XamSessionManagerListener(PlayerPageRenderer pageRenderer)
        {
            playerPage = pageRenderer;
        }

        public override void DidStartSession(SessionManager sessionManager, Session session)
        {
            playerPage.SwitchToRemotePlayback();

        }

        public override void DidResumeSession(SessionManager sessionManager, Session session)
        {
            playerPage.SwitchToRemotePlayback();
        }

        public override void DidEndSession(SessionManager sessionManager, Session session, NSError error)
        {
            playerPage.SwitchToLocalPlayback(error);
        }

        public override void DidFailToStartSession(SessionManager sessionManager, Session session, NSError error)
        {
            playerPage.SwitchToLocalPlayback(error);
        }
    }

    // MARK: - GCKUIMediaControllerDelegate
    public class XamMediaControllerDelegate : UIMediaControllerDelegate
    {
        public XamMediaControllerDelegate()
        {
        }

        public override void DidUpdateMediaStatus(UIMediaController mediaController, MediaStatus mediaStatus)
        {
            // Once the video has finished, let the delegate know
            // and attempt to proceed to the next session, if autoAdvance
            // is enabled

            if (mediaStatus != null)
            {
                if (mediaStatus.IdleReason == MediaPlayerIdleReason.Finished)
                {
                    //add own logic to resume local play, update tracking etc as needed
                        return;

                
                }

                if (mediaStatus.IdleReason == MediaPlayerIdleReason.Error)
                {
                    Debug.WriteLine("OOPSY Happened to Cast Playback! :: " + MediaPlayerIdleReason.Error);

                    return;
                }
            }
            
        }
    }

}
