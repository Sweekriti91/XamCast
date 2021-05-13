using System;
using Android.Content;
using Android.Widget;
using Android.Support.V7.App;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XamCast;
using XamCast.Droid.Renderers;

[assembly: ExportRenderer(typeof(MyCastButton), typeof(MyCastButtonRenderer))]
namespace XamCast.Droid.Renderers
{
    public class MyCastButtonRenderer : ViewRenderer<MyCastButton, LinearLayout>
    {
        MediaRouteButton mediaRouteButton;
        LinearLayout linearLayout;

        public MyCastButtonRenderer(Context context) : base(context)
        {
        }
    }
}
