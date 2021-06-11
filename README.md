
 [![Build Xamarin.Android](https://github.com/Sweekriti91/XamCast/actions/workflows/android.yml/badge.svg)](https://github.com/Sweekriti91/XamCast/actions/workflows/android.yml)
     [![Build Xamarin.iOS](https://github.com/Sweekriti91/XamCast/actions/workflows/ios.yml/badge.svg)](https://github.com/Sweekriti91/XamCast/actions/workflows/ios.yml)   


# Implementing Chromecast with Xamarin.Forms app
If you are creating a video or even audio app, you can add Chromecast Support for it pretty easily. If the video/audio browsing app is built using Xamarin.Forms, this post walks you through how to add the support for video Chromecast through 2 ways, using a Xamarin.Forms Page Renderer or via a embedded native page. Before we get started, there are a few initial steps to be taken. 


## Getting Started

There are 2 main components for a Cast App :

- A Sender Application : in this post, it will be our Xamarin App
- A Receiver Application : this is what hosts the video on the Chromecast Device. 

In this post, the sample is built similar to the Google Cast [Samples](https://developers.google.com/cast/docs/sample-apps), using the default Media Receiver with the default video support and styling. For more details on customizing the Receiver itself, you can check the Google Documentation [here](https://developers.google.com/cast/docs/web_receiver).

To create and setup the default Media Receiver, the steps are very clearly explained in the Google Cast Guide [here](https://developers.google.com/cast/docs/registration). If you choose not to publish the receiver application, be sure to follow the guide to setup a device for development so you can debug the receiver application. Once the steps are completed, make a note of the Application ID, this is all we need to setup the Xamarin app to connect with the receiver.


## Setup for Sender App i.e the Xamarin App 

The Google Chromecast Framework is provided to us as nugets maintained by the Xamarin team, there is no need to create any Binding project for it. Only caveat being, these frameworks are native to each platform, there is no cross platform Xamarin.Forms implementation for it yet. In this post, we'll see two different ways to implement support into a Xamarin.Forms app. 

- Xamarin.Android [nuget](https://www.nuget.org/packages/Xamarin.GooglePlayServices.Cast/) and [source](https://github.com/xamarin/GooglePlayServicesComponents)
- Xamarin.iOS [nuget](https://www.nuget.org/packages/Xamarin.Google.iOS.Cast/) and [source](https://github.com/xamarin/GoogleApisForiOSComponents/tree/main/source/Google/Cast)


## Develop Sender App 

The Chromecast framework is developed to work with the Native Video Players or with any other third party Video Player Control. Integrating the Cast into the app invovles adding three main UI compoonents and adding Session Management between the video player and Cast Framework. The UI compoonents are: the Cast button, the Mini Player and the Expanded Player. Each of these controls are a part of the framework with default styling and can be customized as needed. For session management, the based on the Cast Connection state, the system uses the Video Player session management and the Cast Session Management to control app behavior and state of the UI components.

<img src='Images/ChromecastProcess.png'>

Each of these events are tied to the State of the Video Player and Chromecast. First, let's setup the shared UI Componenents, that appear on both the native players as well as the shared views in the Xamarin.Forms project. 

## Setup Renderers for Controls

The two UI Components that are native but we will show in our Xamarin.Forms shared app code is the 

- [MyCastButton](XamCast/Controls/MyCastButton.cs)
- [MyMiniPlayer](XamCast/Controls/MyMiniPlayer.cs)

 This is the recommended UX Design from Google, you can read more about the Design Checklist [here](https://developers.google.com/cast/docs/design_checklist/sender).
 As seen in popular Video Streaming applications as well as the UX Checklist, the option to Cast appears on many pages and when in Cast Connected State, the mini player controls are available at the bottom of most app pages, so these Controls are implemeted as Custom Renderers in this project. For a refreher in Custom Renderers in Xamarin Forms, checkout the documentation page [here](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/custom-renderer/view).


## Setup Platform Specific Implementation

## Xamarin.iOS 

<img src='Images/iOS_Cast_DEMO.gif'>

### Setup Player

For the Xamarin.iOS app, the implementation shows how to implement the player page as a full Page Renderer. [PlayerPageRender](XamCast.iOS/Renderers/PlayerPageRenderer.cs) is where we implement the Video Player for local playback , the Cast Button and setup the Session Hooks into the Cast Sessions. The Video player is using [AVPlayer](https://docs.microsoft.com/en-us/dotnet/api/avfoundation.avplayer?view=xamarin-ios-sdk-12) and the setup for that is as you would normally for video playback. 


Let's look at `SetupChromecastThings()`
```
  //setup Cast session manager
  sessionManager = CastContext.SharedInstance.SessionManager;
  xamaSessionManagerListener = new XamSessionManager(this);
  sessionManager.AddListener(xamaSessionManagerListener);

  //castMediaController 
  castMediaController = new UIMediaController();
  castMediaController.Delegate = new XamMediaControllerDelegate();
```
The `SessionManager` is the shared instance to manage Cast Sessions which is maintained by the CastContext. The SesssionManagerListener  is implemented to control how this page and the local player handles when a Cast Session is started. So based on the CastSession State, it either Switches to Local or Remote Playback and you can handle each case based on how you want the app to behave. In this sample, when the Cast Playback Sesssion is successfully, it nagivates back to the Home Page. 

```
    public class XamSessionManagerListener : SessionManagerListener
    {
        public override void DidStartSession(SessionManager sessionManager, Session session)
        {
            playerPage.SwitchToRemotePlayback();

        }

        public override void DidResumeSession(SessionManager sessionManager, Session session)
        {
            playerPage.SwitchToRemotePlayback();
        }

        public override void DidEndSession(SessionManager sessionManager, Session session, NSError error)
        {
            playerPage.SwitchToLocalPlayback(error);
        }

        public override void DidFailToStartSession(SessionManager sessionManager, Session session, NSError error)
        {
            playerPage.SwitchToLocalPlayback(error);
        }
    }
  ```

The `XamMediaControllerDelegate` is implemented to handle playback errors, and you can use it to add playback tracking and add Error Logging or track analytics as needed. 

Finally, once the connection is successful, the first step is to create a CastMedia Object with information for the Receiver app to playback. In this sample, it is implemented here as a basic video, you can add more metadata like closed caption, poster image etc : 
```

  public void CreateMediaInfo()
  {
      var videoUrl = mediaInfo.SourceURL;
      var vname = mediaInfo.DisplayName;

      var metaData = new MediaMetadata(MediaMetadataType.Generic);
      metaData.SetString(mediaInfo.DisplayName, MetadataKey.Title);
      metaData.SetString("Hello World on Fridays!", MetadataKey.Subtitle);

      var builder = new MediaInformationBuilder();
      builder.ContentId = videoUrl;
      builder.StreamType = MediaStreamType.Buffered;
      builder.Metadata = metaData;

      castMediaInfo = builder.Build();
  }
```

Second step is to load this into the Remote Client which is as simple as this :
```
remoteMediaClient.LoadMedia(castMediaInfo, options);
```
That's it! Player Page is complete! Next step, setup the Custom Renderers for Cast Button and MiniPlayer.

### Setup Custom Control Renderers

[Cast Button](XamCast.iOS/Renderers/MyCastButtonRenderer.cs) is super simple, instantiate the `UICastButton` and the Cast Framework automatically handles sharing state. That's it!

[MiniPlayer](XamCast.iOS/Renderers/MyMiniPlayerRenderer.cs) needs a Delegate to be implement to control toggle its visibility. This is very easy to implement by checking the `miniMediaControlsViewController.Active` property, which is active only when there is Cast Connected and there is playback! Simple as you can see here : 


```
var castContext = CastContext.SharedInstance;
miniMediaControlsViewController = castContext.CreateMiniMediaControlsViewController();

miniMediaControlsViewController.Delegate = new XamGoogleCastMiniControllerDelegate(this);

....
....


public class XamGoogleCastMiniControllerDelegate : UIMiniMediaControlsViewControllerDelegate
{
  public override void ShouldAppear(UIMiniMediaControlsViewController miniMediaControlsViewController, bool shouldItAppear)
  {
    UpdateControlBarsVisibility();
  }
}

public void UpdateControlBarsVisibility()
{
  if (miniMediaControlsViewController.Active)
  {
    miniMediaControlsContainerView.Hidden = false;
  }
  else
  {
    miniMediaControlsContainerView.Hidden = true;
  }
}               
```

### Initialize Chromecast Service

Last step is to initialize the Chromecast SDK, which in this sample is done via a [ChromecastService](XamCast.iOS/Services/ChromecastService.cs). This is so that we can enable the service from any page in the Xamarin.Forms layer. Using the Receiver ID that you get from the Chroemcast Dashboard as part of the intial setup, the initailiazation code very simple : 
```
var discoveryCriteria = new DiscoveryCriteria("<RECEIVER_ID>");
var castOptions = new CastOptions(discoveryCriteria);
CastContext.SetSharedInstance(castOptions);
CastContext.SharedInstance.UseDefaultExpandedMediaContrtrue;
```

## Xamarin.Android

<img src='Images/Droid_Cast_DEMO.gif'>

### Setup Player

For the Xamarin.Android app, the implementation shows how to implement the player page as an Embedded Native Activity. [PlayerActivity](XamCast.Android/PlayerActivity/PlayerActivity.cs) is where we implement the Video Player for local playback , the Cast Button and setup the Session Hooks into the Cast Sessions. Another reason needed for this approach is because of the way the Chromecast SDK is implemented for native android, it ties directly into the Activites.  The VideoPlayer is using the [VideoView](https://docs.microsoft.com/en-us/dotnet/api/android.widget.videoview?view=xamarin-android-sdk-9) and the setup for that is as you would normally for video playback.

Let's look at how to setup Chromecast, following a similar pattern to how iOS was setup :

```
castSessionManagerListener = new CastSessionManagerListener(this);
castContext = CastContext.GetSharedInstance(this);
castSession = castContext.SessionManager.CurrentCastSession;
castContext.SessionManager.AddSessionManagerListener(castSessionManagerListener);
```
The SessionManager is the shared instance to manage Cast Sessions which is maintained by the CastContext. The SesssionManagerListener is implemented to control how this page and the local player handles when a Cast Session is started. So based on the CastSession State, it either Switches to Local or Remote Playback and you can handle each case based on how you want the app to behave. In this sample, when the Cast Playback Sesssion is successfully, it nagivates back to the Home Page

```
public partial class CastSessionManagerListener : Java.Lang.Object, ISessionManagerListener
{
  public void OnSessionEnded(Java.Lang.Object session, int error)
  {
    OnApplicationDisconnected();
  }

  public void OnSessionResumed(Java.Lang.Object session, bool wasSuspended)
  {
    OnApplicationConnected(castSession);
  }

  public void OnSessionResumeFailed(Java.Lang.Object session, int error)
  {
    OnApplicationDisconnected();
  }

  public void OnSessionStarted(Java.Lang.Object session, string sessionId)
  {
    OnApplicationConnected(castSession);
  }

  public void OnSessionStartFailed(Java.Lang.Object session, int error)
  {
    OnApplicationDisconnected();
  }
}
```
As before, once the Connection is established, first create a CastMedia Object with the Video information. The code for that is straightforward for this sample and can be extended to add more data : 
```
public MediaInfo CreateMediaInfo()
{
  MediaMetadata metadata = new MediaMetadata(MediaMetadata.MediaTypeMovie);
  metadata.PutString(MediaMetadata.KeySubtitle, "Hello World Fridays!");
  metadata.PutString(MediaMetadata.KeyTitle, mediaInfo.DisplayName);

  var castableMedia = MediaInfo.Builder(mediaInfo.SourceURL).SetMetadata(metadata).Build();

  return castableMedia;
}
```

Now let's load this into the Remote Media Client, easy peasy : 
```
castSession = castContext.SessionManager.CurrentCastSession;
var remoteClient = castSession.RemoteMediaClient;
remoteClient.Load(CreateMediaInfo(), true);
```
That's it! Player Page is complete! Let's setup the Renderers, which is slightly different and needs some interesting setup. 

### Setup Custom Control Renderers

The controls in the Android Chromecast SDK are tightly coupled to native implemetation and connected to Activities and so the renderer implementation looks complex but is actually really simple. 

For the [CastButton](XamCast.Android/Renderers/MyCastButtonRenderer.cs), it is basically a MediaRouter, so we have to manually implement the MediaRouteSelector and set it's callback to listen for the Recevier ID. You can see that implementation here :
```
mediaRouteButton = new MediaRouteButton(Context);

mediaRouter = MediaRouter.GetInstance(Context);
mediaRouteSelector = new MediaRouteSelector
    .Builder()
    .AddControlCategory(CastMediaControlIntent.CategoryForCast("<RECEIVER_ID>"))
    .Build();

mediaRouterCallback = new CustomMediaRouterCallBack();
mediaRouter.AddCallback(mediaRouteSelector, mediaRouterCallback, MediaRouter.CallbackFlagPerformActiveScan);
mediaRouteButton.RouteSelector = mediaRouteSelector;
```
And the Selector Callback implementation :
```
public class CustomMediaRouterCallBack : MediaRouter.Callback
{
  public override void OnRouteSelected(MediaRouter router, MediaRouter.RouteInfo route)
  {
    castDevice = CastDevice.GetFromBundle(route.Extras);
  }

  public override void OnRouteUnselected(MediaRouter router, MediaRouter.RouteInfo route)
  {
    castDevice = null;
  }
}
```

For the [MiniPlayer](XamCast.Android/Renderers/MyMiniPlayerRenderer.cs), another interesting implemetation. The native MiniPlayer control is a Fragment so to get around this, we need to create a new layout file and add only the native MiniPlayer fragment as seen in [miniplayerLayout.xml](XamCast.Android/Resources/layout/miniplayerLayout.xml), and then we are going to manually inflate the Fragment into the View and implement FragmentManager to dispose it when Cast State is disconnected. 

```
//Layout Inflator
view = activity.LayoutInflater.Inflate(Resource.Layout.miniplayerLayout, null);
var test = view.FindViewById(Resource.Id.castMiniController);
linearLayout = view.FindViewById<LinearLayout>(Resource.Layout.miniplayerLayout);

....
....

//FragmentManagement
var fm = Control.Context.GetFragmentManager();
var xmlFragment = fm.FindFragmentById(Resource.Id.castMiniController);
if (xmlFragment != null)
{
  fm.BeginTransaction().Remove(xmlFragment).Commit();
}
```

### Initialize Chromecast Service

Last step is to initialize the Chromecast SDK, as seen so far in Android, is easy but not straight forward. The Cast Options are set via interface `IOptionsProvider` and needs to be registered in the AndroidManifest file for the app. So setup the CastOptions :

```
var castOptions = new CastOptions.Builder()
                .SetLaunchOptions(launchOptions)
                .SetReceiverApplicationId("0A6928D1")
                .SetCastMediaOptions(mediaOptions)
                .Build();
```
and here is a easy way to register Xamarin.Android files for easy access via path in the `AndroidManifest.xml` :
```
    [Register("XamCast/Android/CastOptionsProvider")]
```

A few more things to setup in the [CastOptionsProvider](XamCast.Android/CastOptionsProvider.cs) is related to the ExpandedController, which is the player view that opens when you tap on the MiniPlayer. This app uses the default implementation and to setup the default path :

```
var mediaOptions = new CastMediaOptions.Builder()
                .SetImagePicker(new ImagePickerImpl())
                .SetNotificationOptions(notificationOptions)
                .SetExpandedControllerActivityClassName("com.google.android.gms.cast.framework.media.widget.ExpandedControllerActivity")
                .Build();
```

If you implement your custom Expanded Controller, using the Register tip, you can easily refer to the class path.

Now to reference this file in the manifest, the path is `XamCast.Android.CastOptionsProvider` you don't need to look through complicated assembly locations etc to find it! Now that we have this path, let's register this in the `AndroidManifest.xml` file :

```
<meta-data android:name="com.google.android.gms.cast.framework.OPTIONS_PROVIDER_CLASS_NAME" android:value="XamCast.Android.CastOptionsProvider" />
```
That's it! Now when the Android App is launched, it gets the value set in the CastOptionsProvider and inits the Cast SDK!


## Further Customization and Tips

This sample uses all the default implementation and colors for the app. Customizing to use branded colors and styles is simple and for that the steps are exactly the same as they are for the Native Samples. Refering to the documentation and samples provided, you can create similar setup in each of the Platform implementation for the controls and adjust the colors, sizes and formatting. 

The dev loop is easy as you can use simulators to test the Cast functionality, and physical devices are not always needed, although recommended to verify it works. You do however need a Chromecast Device for casting to. 

As for Testing, unfortunately as seen in the Google [Documentation](https://developers.google.com/cast/docs/testing) it is still manual only and there is no automation support for this at the moment.

The Cast Session Management hooks into the video player events pretty agnostic to the player itself, so if you are using a custom player or any third party non-native video player, it should still be possible to connect it with the Chromecast SDK. 


## Wrap-up

You can find the [sample here](https://github.com/xamcat/XamCast) and I hope that you found a bit of inspiration in this blog post to make your Video Streaming app. Cast all the things! 

## Useful Links 

- [XamCast - Xamarin.Forms Chromecast Sample](https://github.com/xamcat/XamCast)
- [Google Chromecast Getting Started](https://developers.google.com/cast/docs/developers)
- [Google Cast SDK Developer Console](https://cast.google.com/publish) 
- [Google Chromecast Android API](https://developers.google.com/android/reference/com/google/android/gms/cast/package-summary)
- [Google Chromecast iOS API](https://developers.google.com/cast/docs/reference/ios)
- [Google Chromecast Samples Samples](https://developers.google.com/cast/docs/sample-apps)
