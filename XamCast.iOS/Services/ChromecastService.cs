using System;
using Google.Cast;
using XamCast.Models;

namespace XamCast.iOS.Services
{
    public class ChromecastService : IChromecastService
    {
        MediaInfo media { get; set; }

        public MediaInfo GetPlaybackAsset()
        {
            return media;
        }

        public void OpenPlayerPage()
        {
            throw new NotImplementedException();
        }

        public void SetPlaybackAsset(MediaInfo asset)
        {
            media = asset;
        }

        public void SetupChromecast()
        {
            var discoveryCriteria = new DiscoveryCriteria("0A6928D1");
            var castOptions = new CastOptions(discoveryCriteria);
            CastContext.SetSharedInstance(castOptions);
            CastContext.SharedInstance.UseDefaultExpandedMediaControls = true;
        }
    }
}
