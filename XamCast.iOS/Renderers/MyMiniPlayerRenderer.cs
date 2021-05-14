using System;
using System.Diagnostics;
using CoreGraphics;
using Google.Cast;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamCast;
using XamCast.iOS.Renderers;

[assembly: ExportRenderer(typeof(MyMiniPlayer), typeof(MyMiniPlayerRenderer))]
namespace XamCast.iOS.Renderers
{
    public class MyMiniPlayerRenderer : ViewRenderer<MyMiniPlayer, UIView>
    {
        UIMiniMediaControlsViewController miniMediaControlsViewController;
        UIView miniMediaControlsContainerView;

        protected override void OnElementChanged(ElementChangedEventArgs<MyMiniPlayer> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var castContext = CastContext.SharedInstance;
                    miniMediaControlsViewController = castContext.CreateMiniMediaControlsViewController();
                    //miniMediaControlsViewController.View.TintColor = Colors.Gold.ToUIColor();

                    //var castStyle = UIStyle.SharedInstance;
                    //castStyle.CastViews.MediaControl.MiniController.SliderProgressColor = Colors.Gold.ToUIColor();
                    //castStyle.CastViews.MediaControl.MiniController.SliderSecondaryProgressColor = Colors.Gold.ToUIColor();
                    //castStyle.CastViews.MediaControl.MiniController.SliderUnseekableProgressColor = Colors.Gold.ToUIColor();
                    //castStyle.CastViews.MediaControl.MiniController.IconTintColor = Colors.Gold.ToUIColor();
                    //castStyle.CastViews.MediaControl.MiniController.HeadingTextShadowColor = UIColor.Clear;

                    //VisualTheme theme = AppInfo.RequestedTheme == AppTheme.Dark ? VisualTheme.Dark : VisualTheme.Light;

                    //switch (theme)
                    //{
                    //    case VisualTheme.Light:
                    //        {
                    //            castStyle.CastViews.MediaControl.MiniController.BackgroundColor = Colors.LightBackground.ToUIColor();
                    //            castStyle.CastViews.MediaControl.MiniController.HeadingTextColor = Colors.LightText.ToUIColor();

                    //            break;
                    //        }
                    //    case VisualTheme.Dark:
                    //        {
                    //            castStyle.CastViews.MediaControl.MiniController.BackgroundColor = Colors.DarkBackground.ToUIColor();
                    //            castStyle.CastViews.MediaControl.MiniController.HeadingTextColor = Colors.DarkText.ToUIColor();

                    //            break;
                    //        }
                    //    default:
                    //        {
                    //            castStyle.CastViews.MediaControl.MiniController.BackgroundColor = Colors.LightBackground.ToUIColor();
                    //            castStyle.CastViews.MediaControl.MiniController.HeadingTextColor = Colors.LightText.ToUIColor();

                    //            break;
                    //        }
                    //}
                    //castStyle.ApplyStyle();

                    miniMediaControlsViewController.Delegate = new XamGoogleCastMiniControllerDelegate(this);
                    miniMediaControlsContainerView = new UIView();
                    miniMediaControlsContainerView.Frame = new CGRect(0, 0, 400, 45);

                    miniMediaControlsViewController.View.Frame = miniMediaControlsContainerView.Bounds;
                    miniMediaControlsContainerView.AddSubview(miniMediaControlsViewController.View);
                    UpdateControlBarsVisibility();
                    SetNativeControl(miniMediaControlsContainerView);
                }
            }
        }

        public class XamGoogleCastMiniControllerDelegate : UIMiniMediaControlsViewControllerDelegate
        {
            MyMiniPlayerRenderer playerRenderer;

            public XamGoogleCastMiniControllerDelegate(MyMiniPlayerRenderer pageR)
            {
                this.playerRenderer = pageR;
            }

            public override void ShouldAppear(UIMiniMediaControlsViewController miniMediaControlsViewController, bool shouldItAppear)
            {
                playerRenderer.UpdateControlBarsVisibility();
            }
        }

        public void UpdateControlBarsVisibility()
        {
            Debug.WriteLine("miniMediaControlsViewController Active : " + miniMediaControlsViewController.Active);
            if (miniMediaControlsViewController.Active)
            {
                Debug.WriteLine("TRUE YES MINI");
                miniMediaControlsContainerView.Hidden = false;
            }
            else
            {
                Debug.WriteLine("FALSE NO MINI");
                miniMediaControlsContainerView.Hidden = true;
            }

        }
    }
}
