using System;
using System.Collections.Generic;
using Android.Content;
using Android.Gms.Cast;
using Android.Gms.Cast.Framework;
using Android.Gms.Cast.Framework.Media;
using Android.Gms.Common.Images;
using Android.Runtime;

namespace XamCast.Droid
{
    [Register("XamCast/Android/CastOptionsProvider")]
    public class CastOptionsProvider : Java.Lang.Object, IOptionsProvider
    {
        public IList<SessionProvider> GetAdditionalSessionProviders(Context appContext)
        {
            return null;
        }

        public CastOptions GetCastOptions(Context appContext)
        {

            //var notificationOptions = new NotificationOptions.Builder()
            //    .SetActions(new List<string>() { MediaIntentReceiver.ActionSkipNext, MediaIntentReceiver.ActionTogglePlayback, MediaIntentReceiver.ActionStopCasting }, new int[] { 1, 2 })
            //    .SetTargetActivityClassName("com.brightcove.cast.DefaultExpandedControllerActivity")
            //    .Build();

            //var mediaOptions = new CastMediaOptions.Builder()
            //    .SetImagePicker(new ImagePickerImpl())
            //    .SetNotificationOptions(notificationOptions)
            //    .SetExpandedControllerActivityClassName("com.brightcove.cast.DefaultExpandedControllerActivity")
            //    .Build();

            var launchOptions = new LaunchOptions.Builder()
                .Build();


            var castOptions = new CastOptions.Builder()
                .SetLaunchOptions(launchOptions)
                .SetReceiverApplicationId("0A6928D1")
                .Build();

            return castOptions;
        }
    }

    public partial class ImagePickerImpl : ImagePicker
    {
        public override WebImage OnPickImage(MediaMetadata mediaMetadata, ImageHints hints)
        {
            var type = hints.Type;
            if ((mediaMetadata == null) || !mediaMetadata.HasImages)
            {
                return null;
            }
            var images = mediaMetadata.Images;
            if (images.Count == 1)
                return images[0];
            else
            {
                if (type == ImagePicker.ImageTypeMediaRouteControllerDialogBackground)
                    return images[0];
                else
                    return images[1];
            }
        }
    }
}
