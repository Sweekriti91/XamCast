using System;
using Android.Content;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XamCast;
using XamCast.Droid.Renderers;
using Android.Gms.Cast.Framework;
using Android.Support.V7.App;
using Android.Support.V7.Media;
using Android.Gms.Cast;

[assembly: ExportRenderer(typeof(MyCastButton), typeof(MyCastButtonRenderer))]
namespace XamCast.Droid.Renderers
{
    public class MyCastButtonRenderer : ViewRenderer<MyCastButton, LinearLayout>
    {
        MediaRouteButton mediaRouteButton;
        MediaRouter mediaRouter;
        MediaRouteSelector mediaRouteSelector;
        MediaRouter.Callback mediaRouterCallback;

        LinearLayout linearLayout;

        public MyCastButtonRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<MyCastButton> e)
        {
            base.OnElementChanged(e);

            if(e.NewElement != null)
            {
                if(Control == null)
                {
                    mediaRouteButton = new MediaRouteButton(Context);

                    mediaRouter = MediaRouter.GetInstance(Context);
                    mediaRouteSelector = new MediaRouteSelector
                        .Builder()
                        .AddControlCategory(CastMediaControlIntent.CategoryForCast("0A6928D1"))
                        .Build();

                    mediaRouterCallback = new CustomMediaRouterCallBack();
                    mediaRouter.AddCallback(mediaRouteSelector, mediaRouterCallback, MediaRouter.CallbackFlagPerformActiveScan);
                    mediaRouteButton.RouteSelector = mediaRouteSelector;

                    //CastButtonFactory.SetUpMediaRouteButton(Context, mediaRouteButton);
                    //var castContext = CastContext.SharedInstance;
                    //castContext.
                    linearLayout = new LinearLayout(Context);
                    linearLayout.AddView(mediaRouteButton);

                    SetNativeControl(linearLayout);
                }
            }
        }


        public class CustomMediaRouterCallBack : MediaRouter.Callback
        {
            CastDevice castDevice;

            public override void OnRouteSelected(MediaRouter router, MediaRouter.RouteInfo route)
            {
                //base.OnRouteSelected(router, route);
                castDevice = CastDevice.GetFromBundle(route.Extras);
                System.Diagnostics.Debug.WriteLine("SELECTED; Connected to: " + route.Name);



            }

            public override void OnRouteUnselected(MediaRouter router, MediaRouter.RouteInfo route)
            {
                //base.OnRouteUnselected(router, route);
                System.Diagnostics.Debug.WriteLine("UNSELECTED");
                castDevice = null;
            }
        }
    }
}
