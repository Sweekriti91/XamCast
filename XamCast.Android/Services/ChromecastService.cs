using System;
using Android.Content;
using Xamarin.Essentials;
using XamCast.Models;

namespace XamCast.Droid.Services
{
    public class ChromecastService : IChromecastService
    {
        MediaInfo media { get; set; }

        public void OpenPlayerPage(MediaInfo asset)
        {
            media = asset;

            Intent intent = new Intent(Platform.CurrentActivity,typeof(PlayerActivity));
            intent.AddFlags(ActivityFlags.NewTask);
            Platform.AppContext.StartActivity(intent);
        }

        public void SetupChromecast()
        {
            throw new NotImplementedException();
        }

        public MediaInfo GetPlaybackAsset()
        {
            return media;
        }
    }
}
