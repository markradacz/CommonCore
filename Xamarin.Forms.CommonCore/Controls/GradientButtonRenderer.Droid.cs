#if __ANDROID__
using System;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Support.V4.View;
using Android.Util;
using Attribute = Android.Resource.Attribute;
using Xamarin.Forms.CommonCore;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(GradientButton), typeof(GradientButtonRenderer))]
namespace Xamarin.Forms.CommonCore
{
	public class GradientButtonRenderer : ButtonRenderer
	{
		GradientButton caller;

		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged(e);

			if (Control != null)
			{
				caller = e.NewElement as GradientButton;
				SetButtonDisableState();
                SetGradientAndRadius();

			}
		}

		private void SetButtonDisableState()
		{
			int[][] states = new int[][] {
				new int[] { Attribute.StateEnabled }, // enabled
                new int[] {-Attribute.StateEnabled }, // disabled
                new int[] {-Attribute.StateChecked }, // unchecked
                new int[] { Attribute.StatePressed }  // pressed
            };
			int[] colors = new int[] {
				caller.TextColor.ToAndroid(),
				caller.DisabledTextColor.ToAndroid(),
				caller.TextColor.ToAndroid(),
				caller.TextColor.ToAndroid()
			};
			var buttonStates = new ColorStateList(states, colors);
			Control.SetTextColor(buttonStates);
			//Control.ClipToOutline = true;
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == GradientButton.IsEnabledProperty.PropertyName)
			{
                SetGradientAndRadius();
			}
			base.OnElementPropertyChanged(sender, e);
		}

        private void SetGradientAndRadius()
        {
			var gradient = new GradientDrawable(GradientDrawable.Orientation.TopBottom, new[] {
					caller.StartColor.ToAndroid().ToArgb(),
					caller.EndColor.ToAndroid().ToArgb()
				});
			gradient.SetCornerRadius(caller.CornerRadius.ToDevicePixels());
            Control.SetBackground(gradient);

            var num = caller.IsEnabled ? 105f : 100f;

			Control.Elevation = num;
			Control.TranslationZ = num;
        }
	}
}
#endif

