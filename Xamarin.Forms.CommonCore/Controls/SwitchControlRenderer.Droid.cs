﻿#if __ANDROID__
using System;
using Android.Graphics;
using Android.Widget;
using Xamarin.Forms.CommonCore;
using Xamarin.Forms.CommonCore;
using Xamarin.Forms.Platform.Android;
using FormColor = Xamarin.Forms.Color;

[assembly: Xamarin.Forms.ExportRenderer(typeof(SwitchControl), typeof(SwitchControlRenderer))]
namespace Xamarin.Forms.CommonCore
{
    public class SwitchControlRenderer : SwitchRenderer
    {
		private FormColor falseColor;
		private FormColor trueColor;
        private SwitchControl ctrl;
		protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
		{
			base.OnElementChanged(e);

			if (this.Control != null)
			{
                ctrl = (SwitchControl)e.NewElement;
                trueColor = ctrl.TrueColor;
                falseColor = ctrl.FalseColor;

				if (this.Control.Checked)
				{
					this.Control.TrackDrawable.SetColorFilter(trueColor.ToAndroid(), PorterDuff.Mode.Multiply);
					this.Control.ThumbDrawable.SetColorFilter(trueColor.ToAndroid(), PorterDuff.Mode.Multiply);
				}
				else
				{
					this.Control.TrackDrawable.SetColorFilter(falseColor.ToAndroid(), PorterDuff.Mode.Multiply);
					this.Control.ThumbDrawable.SetColorFilter(falseColor.ToAndroid(), PorterDuff.Mode.Multiply);
				}

				this.Control.CheckedChange += this.OnCheckedChange;
			}
		}

		protected override void Dispose(bool disposing)
		{
			this.Control.CheckedChange -= this.OnCheckedChange;
			base.Dispose(disposing);
		}

		private void OnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
		{

			if (this.Control.Checked)
			{
                this.Element.IsToggled = true;
				this.Control.TrackDrawable.SetColorFilter(trueColor.ToAndroid(), PorterDuff.Mode.Multiply);
				this.Control.ThumbDrawable.SetColorFilter(trueColor.ToAndroid(), PorterDuff.Mode.Multiply);
			}
			else
			{
                this.Element.IsToggled = false;
				this.Control.TrackDrawable.SetColorFilter(falseColor.ToAndroid(), PorterDuff.Mode.Multiply);
				this.Control.ThumbDrawable.SetColorFilter(falseColor.ToAndroid(), PorterDuff.Mode.Multiply);
			}
		}
    }
}
#endif
