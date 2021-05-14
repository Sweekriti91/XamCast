
using System;

using Android.App;
using Android.Gms.Cast;
using Android.Gms.Cast.Framework;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Microsoft.MobCAT;
using XamCast.Models;
using Debug = System.Diagnostics.Debug;
using MediaRouteButton = Android.Support.V7.App.MediaRouteButton;

namespace XamCast.Droid
{
    [Activity(Label = "PlayerActivity")]
    public class PlayerActivity : AppCompatActivity
    {
        Lazy<IChromecastService> chromecastService = new Lazy<IChromecastService>(() => ServiceContainer.Resolve<IChromecastService>());
        XamCast.Models.MediaInfo mediaInfo;

        //controls
        VideoView videoView;
        MediaController mController;
       
        public static Button backButton;
        public static TextView assetTitle;
        public static MediaRouteButton castButton;

        public static CastContext castContext;
        public static CastSession castSession;
        public static CastSessionManagerListener castSessionManagerListener;
        //public static CustomCastMediaManager customCastMediaManager;

        //ENUMs and VARIABLES to handle Cast Playback vs local Playback
        //From Google Sample : https://github.com/googlecast/CastVideos-android/blob/master/src/com/google/sample/cast/refplayer/mediaplayer/LocalPlayerActivity.java#L111
        private PlaybackLocation mLocation;

        /**
         * indicates whether we are doing a local or a remote playback
         */
        public enum PlaybackLocation
        {
            LOCAL,
            REMOTE
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            base.OnCreate(savedInstanceState);

            castSessionManagerListener = new CastSessionManagerListener(this);
            castContext = CastContext.GetSharedInstance(this);
            castSession = castContext.SessionManager.CurrentCastSession;
            castContext.SessionManager.AddSessionManagerListener(castSessionManagerListener);

            //setup layout and video data
            SetContentView(Resource.Layout.playerPageLayout);
            mediaInfo = chromecastService.Value.GetPlaybackAsset();

            //TITLE
            assetTitle = FindViewById<TextView>(Resource.Id.assetTitle);
            assetTitle.Text = mediaInfo.DisplayName;
            assetTitle.TextSize = 20;

            //BACKBUTTON
            backButton = FindViewById<Button>(Resource.Id.backButton);
            backButton.Click += BackButton_Click;

            //Cast Button setup
            castButton = (MediaRouteButton)FindViewById(Resource.Id.media_route_button);
            CastButtonFactory.SetUpMediaRouteButton(ApplicationContext, castButton);

            //VideoPlayer Source
            videoView = FindViewById<VideoView>(Resource.Id.video_view);
            var videoURL = Android.Net.Uri.Parse(mediaInfo.SourceURL);

            mController = new Android.Widget.MediaController(this);
            mController.SetAnchorView(videoView);
            videoView.SetVideoURI(videoURL);
            videoView.SetMediaController(mController);

            if(mLocation == PlaybackLocation.LOCAL)
                videoView.Start();
            else
            {
                castSession = castContext.SessionManager.CurrentCastSession;
                if((castSession != null) && (castSession.IsConnected == true))
                {
                    //setup media to send to cast receiver
                    mLocation = PlaybackLocation.REMOTE;
                    var test = castContext.SessionManager.CurrentCastSession;

                    //call/initialize customCastMediaManager if needed. this sample uses default things
                }
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            //add app center tracking etc shared between player closes normally from back button
            videoView.StopPlayback();
            this.Finish();
        }

        public void CastClosePlayerActivity()
        {
            //things to happen when player page closes because connected to cast
            mLocation = PlaybackLocation.REMOTE;
            videoView.StopPlayback();
            this.Finish();
        }

        public void LoadRemoteMedia()
        {
            castSession = castContext.SessionManager.CurrentCastSession;
            if (castSession == null)
            {
                Debug.WriteLine("NO CAST SESSION");
                return;
            }

            var remoteClient = castSession.RemoteMediaClient;
            if (remoteClient != null)
            {
                remoteClient.Load(CreateMediaInfo(), true);
                remoteClient.Seek(videoView.CurrentPosition);
                remoteClient.Play();

                //CastClosePlayerActivity();
            }

        }

        public Android.Gms.Cast.MediaInfo CreateMediaInfo()
        {
            MediaMetadata metadata = new MediaMetadata(MediaMetadata.MediaTypeMovie);
            metadata.PutString(MediaMetadata.KeySubtitle, "Hello World Fridays!");
            metadata.PutString(MediaMetadata.KeyTitle, mediaInfo.DisplayName);

            var castableMedia = new Android.Gms.Cast.MediaInfo.Builder(mediaInfo.SourceURL).SetMetadata(metadata).Build();

            return castableMedia;
        }

        //Setup based on Google Sample Code : https://github.com/googlecast/CastVideos-android/blob/master/src/com/google/sample/cast/refplayer/mediaplayer/LocalPlayerActivity.java#L172
        public partial class CastSessionManagerListener : Java.Lang.Object, ISessionManagerListener
        {
            PlayerActivity playeractivity;
            public CastSessionManagerListener(PlayerActivity player)
            {
                playeractivity = player;
            }

            public void OnSessionEnded(Java.Lang.Object session, int error)
            {
                //Update Tracking session?
                Debug.WriteLine("CAST SESSION ENDED");

                OnApplicationDisconnected();
            }

            public void OnSessionResumed(Java.Lang.Object session, bool wasSuspended)
            {
                //Cast connected successfully
                playeractivity.mLocation = PlaybackLocation.REMOTE;
                castSession = session as CastSession;

                OnApplicationConnected(castSession);

                //playeractivity.CastClosePlayerActivity();
            }

            public void OnSessionResumeFailed(Java.Lang.Object session, int error)
            {
                //Update Tracking session?
                Debug.WriteLine("CAST SESSION RESUME FAILED");

                OnApplicationDisconnected();
            }


            public void OnSessionStarted(Java.Lang.Object session, string sessionId)
            {
                //Cast connected successfully
                playeractivity.mLocation = PlaybackLocation.REMOTE;
                castSession = session as CastSession;

                OnApplicationConnected(castSession);

                //playeractivity.CastClosePlayerActivity();
            }

            public void OnSessionStartFailed(Java.Lang.Object session, int error)
            {
                //Update Tracking session?
                Debug.WriteLine("CAST SESSION START FAILED");

                OnApplicationDisconnected();
            }

            public void OnSessionStarting(Java.Lang.Object session)
            {
            }

            public void OnSessionEnding(Java.Lang.Object session)
            {
            }

            public void OnSessionResuming(Java.Lang.Object session, string sessionId)
            {
            }

            public void OnSessionSuspended(Java.Lang.Object session, int reason)
            {
            }

            private void OnApplicationConnected(CastSession castSession)
            {
                Debug.WriteLine("CAST SESSION :: " + castSession);
                if(playeractivity.videoView.IsPlaying)
                    playeractivity.videoView.Pause();
                playeractivity.mLocation = PlaybackLocation.REMOTE;

                playeractivity.LoadRemoteMedia();

                //do anything else here like navigate back to main page
                playeractivity.CastClosePlayerActivity();
            }

            private void OnApplicationDisconnected()
            {
                Debug.WriteLine("OOPSY Happened!");

                //playeractivity.CastClosePlayerActivity();
            }
        }
    }
}
