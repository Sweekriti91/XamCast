using System;
using System.Diagnostics;
using Android.Content;
using Xamarin.Essentials;
using XamCast.Models;

namespace XamCast.Droid.Services
{
    public class ChromecastService : IChromecastService
    {
        MediaInfo media { get; set; }

        public void OpenPlayerPage()
        {

            Intent intent = new Intent(Platform.CurrentActivity,typeof(PlayerActivity));
            intent.AddFlags(ActivityFlags.NewTask);
            Platform.AppContext.StartActivity(intent);
        }

        public void SetupChromecast()
        {
            Debug.WriteLine("Leave empty");
        }

        public MediaInfo GetPlaybackAsset()
        {
            return media;
        }

        public void SetPlaybackAsset(MediaInfo asset)
        {
            media = asset;
        }
    }
}
