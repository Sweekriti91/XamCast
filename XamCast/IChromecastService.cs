using System;
using XamCast.Models;

namespace XamCast
{
    public interface IChromecastService
    {
        void SetupChromecast();
        void OpenPlayerPage();
        MediaInfo GetPlaybackAsset();
        void SetPlaybackAsset(MediaInfo asset);
    }
}
