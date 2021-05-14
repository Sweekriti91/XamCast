using System;
using System.Collections.Generic;
using System.Linq;
using AVFoundation;
using Foundation;
using Microsoft.MobCAT;
using UIKit;
using XamCast.iOS.Services;

namespace XamCast.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            var errOut = new NSError();
            AVAudioSession.SharedInstance().SetCategory(AVAudioSession.CategoryPlayback, AVAudioSession.ModeMoviePlayback, AVAudioSessionRouteSharingPolicy.LongForm, options: 0, out errOut);


            global::Xamarin.Forms.Forms.Init();
            ServiceContainer.Register<IChromecastService>(() => new ChromecastService());
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
