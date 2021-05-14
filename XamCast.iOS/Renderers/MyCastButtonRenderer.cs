using System;
using Google.Cast;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamCast;
using XamCast.iOS.Renderers;

[assembly: ExportRenderer(typeof(MyCastButton), typeof(MyCastButtonRenderer))]
namespace XamCast.iOS.Renderers
{
    public class MyCastButtonRenderer : ViewRenderer<MyCastButton, UICastButton>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<MyCastButton> e)
        {
            UICastButton castButton;

            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    castButton = new UICastButton();
                    SetNativeControl(castButton);
                }
            }
        }
    }
}
