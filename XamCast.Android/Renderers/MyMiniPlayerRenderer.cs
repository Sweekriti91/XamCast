using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XamCast;
using XamCast.Droid.Renderers;

[assembly: ExportRenderer(typeof(MyMiniPlayer), typeof(MyMiniPlayerRenderer))]
namespace XamCast.Droid.Renderers
{
    public class MyMiniPlayerRenderer : ViewRenderer<MyMiniPlayer, global::Android.Views.View>
    {
        LinearLayout linearLayout;
        global::Android.Views.View view;
        Activity activity;

        public MyMiniPlayerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<MyMiniPlayer> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                activity = this.Context as Activity;
                view.Dispose();
                linearLayout.Dispose();
            }


            if (e.NewElement != null)
            {
                if (Control == null)
                {

                    activity = this.Context as Activity;

                    view = activity.LayoutInflater.Inflate(Resource.Layout.miniplayerLayout, null);

                    linearLayout = view.FindViewById<LinearLayout>(Resource.Layout.miniplayerLayout);
                    SetNativeControl(view);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {

            var fm = Control.Context.GetFragmentManager();
            var xmlFragment = fm.FindFragmentById(Resource.Id.castMiniController);
            if (xmlFragment != null)
            {
                fm.BeginTransaction().Remove(xmlFragment).Commit();
            }

            base.Dispose(disposing);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
        }
    }
}
