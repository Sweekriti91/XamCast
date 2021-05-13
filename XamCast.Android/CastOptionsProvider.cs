using System;
using System.Collections.Generic;
using Android.Content;
using Android.Gms.Cast;
using Android.Gms.Cast.Framework;
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

            var launchOptions = new LaunchOptions.Builder()
                .Build();


            var castOptions = new CastOptions.Builder()
                .SetLaunchOptions(launchOptions)
                .SetReceiverApplicationId("0A6928D1")
                .Build();

            return castOptions;

            //var castOptions = new CastOptions.Builder()
            //    .SetReceiverApplicationId(AppConstants.ReceiverID)
            //    .Build();
        }
    }
}
