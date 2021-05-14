using System;
using Google.Cast;
using XamCast.Models;

namespace XamCast.iOS.Services
{
    public class ChromecastService : IChromecastService
    {
        public MediaInfo GetPlaybackAsset()
        {
            throw new NotImplementedException();
        }

        public void OpenPlayerPage(MediaInfo asset)
        {
            throw new NotImplementedException();
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
