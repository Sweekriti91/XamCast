
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Microsoft.MobCAT;
using XamCast.Models;

namespace XamCast.Droid
{
    [Activity(Label = "PlayerActivity")]
    public class PlayerActivity : AppCompatActivity
    {
        Lazy<IChromecastService> chromecastService = new Lazy<IChromecastService>(() => ServiceContainer.Resolve<IChromecastService>());

        MediaInfo mediaInfo;

        //controls
        VideoView videoView;
        public static Button backButton;
        public static TextView assetTitle;
        public static MediaRouteButton castButton;

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

            //VideoPlayer Source
            videoView = FindViewById<VideoView>(Resource.Id.video_view);
            var videoURL = Android.Net.Uri.Parse(mediaInfo.SourceURL);

            MediaController mController = new Android.Widget.MediaController(this);
            mController.SetAnchorView(videoView);
            videoView.SetVideoURI(videoURL);
            videoView.SetMediaController(mController);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            //add app center tracking etc shared between player closes normally from back button
            videoView.StopPlayback();
            this.Finish();
        }
    }
}
