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
                miniMediaControlsContainerView.Frame = new CGRect(0, 0, 400, 55);
                miniMediaControlsContainerView.SetNeedsDisplay();
                miniMediaControlsContainerView.LayoutIfNeeded();
            }
            else
            {
                Debug.WriteLine("FALSE NO MINI");
                miniMediaControlsContainerView.Frame = new CGRect(0, 0, 400, 0);
                miniMediaControlsContainerView.SetNeedsDisplay();
                miniMediaControlsContainerView.LayoutIfNeeded();
            }

        }
    }
}
