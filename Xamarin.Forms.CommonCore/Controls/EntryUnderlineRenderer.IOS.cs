﻿#if __IOS__
using System;
using Xamarin.Forms.CommonCore;
using CoreAnimation;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(EntryUnderline), typeof(EntryUnderlineRenderer))]
namespace Xamarin.Forms.CommonCore
{
	public class EntryUnderlineRenderer : EntryRenderer
	{
		private CALayer bottomBorder;
		private CGColor controlColor;
		private EntryUnderline formControl;
		private UIImageView imgView;

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (Control != null && Element != null)
			{
				Control.BorderStyle = UITextBorderStyle.None;

				formControl = (Element as EntryUnderline);
				if ((Control != null) & (formControl != null))
				{
					var returnType = formControl.ReturnKeyType;

					if (this.Element.Keyboard == Keyboard.Numeric || this.Element.Keyboard == Keyboard.Telephone)
					{
						this.AddAccessoryButton(returnType);
					}
					else
					{
						Control.ReturnKeyType = returnType.GetValueFromDescription();
					}
				}

				((UITextField)Control).ShouldReturn = (textField) =>
				{
					formControl?.NextFocus?.Invoke();
					return true;
				};

				controlColor = formControl.EntryColor.ToCGColor();

				var ctrl = (UITextField)Control;
				ctrl.TextColor = formControl.EntryColor.ToUIColor();
				ctrl.TintColor = formControl.EntryColor.ToUIColor();

				var fontSize = ctrl.Font.PointSize;
				var iconHeight = fontSize + 9;  // UIScreen.MainScreen.Bounds.Height * .03; 
				var passwordIconAdder = 2;
				var passwordIconPaddingHeight = 2;
				var passwordIconHeight = iconHeight + passwordIconAdder;
				var passwordIconPaddingWidth = 10;
				var iconPadding = passwordIconAdder / 2 + passwordIconPaddingHeight;
				nfloat iconFrameHeight = 0;
				nfloat iconPasswordFrameHeight = 0;

				if (formControl.Icon != null)
				{
					if (formControl.Icon.IndexOf(".png", StringComparison.InvariantCultureIgnoreCase) == -1)
						formControl.Icon = formControl.Icon + ".png";

					imgView = new UIImageView(new CGRect(0, 0, (iconHeight), (iconHeight)));
                    imgView.Image = new UIImage(formControl.Icon).ChangeImageColor(formControl.PlaceholderColor.ToUIColor());

					Resize(imgView, fontSize, fontSize);

					var paddingView = new UIView(new CGRect(0, 0, (iconHeight + 4), (iconHeight + iconPadding)));
					iconFrameHeight = paddingView.Frame.Height;

					paddingView.AddSubview(imgView);
					ctrl.LeftViewMode = UITextFieldViewMode.Always;
					ctrl.LeftView = paddingView;
				}
				if (formControl.IsPassword && formControl.PasswordRevealEnabled)
				{
					var imgView = new UIImageView(new CGRect(0, 0, (passwordIconHeight + 10), (passwordIconHeight + 10)));
                    imgView.Image = new UIImage("passwordeye.png").ChangeImageColor(formControl.EntryColor.ToUIColor());

					Resize(imgView, passwordIconHeight + passwordIconPaddingWidth, passwordIconHeight);

					var paddingView = new UIView(new CGRect(0, 0, (passwordIconHeight + passwordIconPaddingWidth), (passwordIconHeight + passwordIconPaddingHeight + 10)));
					iconPasswordFrameHeight = paddingView.Frame.Height;

					var btn = new UIButton(paddingView.Frame);
					btn.AddSubview(imgView);
					btn.TouchUpInside += (ee, aa) =>
					{
						formControl.IsPassword = !formControl.IsPassword;
						var fileName = formControl.IsPassword ? "passwordeye.png" : "passwordeyeslash.png";
                        imgView.Image = new UIImage(fileName).ChangeImageColor(formControl.EntryColor.ToUIColor());
					};
					paddingView.AddSubview(btn);
					ctrl.RightViewMode = UITextFieldViewMode.Always;
					ctrl.RightView = paddingView;
					ctrl.RightView.UserInteractionEnabled = true;

					Control.SpellCheckingType = UITextSpellCheckingType.No;             // No Spellchecking
					Control.AutocorrectionType = UITextAutocorrectionType.No;           // No Autocorrection
					Control.AutocapitalizationType = UITextAutocapitalizationType.None; // No Autocapitalization
				}
				formControl.HeightRequest = iconFrameHeight > iconPasswordFrameHeight ? iconFrameHeight : iconPasswordFrameHeight;
			}
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == EntryUnderline.WidthProperty.PropertyName)
			{
				var width = ((Entry)sender).Width;
				var height = ((Entry)sender).Height;
				bottomBorder?.RemoveFromSuperLayer();
				if (width > 0 && height > 0)
					CreateUnderline((nfloat)height, (nfloat)width);
			}
			if (e.PropertyName == EntryUnderline.HeightProperty.PropertyName)
			{
				var width = ((Entry)sender).Width;
				var height = ((Entry)sender).Height;
				bottomBorder?.RemoveFromSuperLayer();
				if (width > 0 && height > 0)
					CreateUnderline((nfloat)height, (nfloat)width);
			}
			if (e.PropertyName == EntryUnderline.HasErrorProperty.PropertyName)
			{
				if (bottomBorder != null)
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						if (formControl.HasError)
							bottomBorder.BackgroundColor = UIColor.Red.CGColor;
						else
							bottomBorder.BackgroundColor = controlColor;

					});

				}
			}

			if (e.PropertyName == EntryUnderline.PlaceholderColorProperty.PropertyName)
			{
                imgView.Image = new UIImage(formControl.Icon).ChangeImageColor(formControl.PlaceholderColor.ToUIColor());
			}

			if (e.PropertyName == EntryUnderline.ReturnKeyTypeProperty.PropertyName)
			{
				var returnType = (sender as EntryUnderline).ReturnKeyType;

				if (this.Element.Keyboard == Keyboard.Numeric || this.Element.Keyboard == Keyboard.Telephone)
				{
					this.AddAccessoryButton(returnType);
				}
				else
				{
					Control.ReturnKeyType = returnType.GetValueFromDescription();
				}
			}

			base.OnElementPropertyChanged(sender, e);
		}

		private void CreateUnderline(nfloat height, nfloat width)
		{
			bottomBorder = new CALayer();
			bottomBorder.Frame = new CoreGraphics.CGRect(0, height - 1, width, 1);
			bottomBorder.BackgroundColor = controlColor;
			Control.Layer.AddSublayer(bottomBorder);

		}


		private void Resize(UIImageView imgView, nfloat width, nfloat height)
		{
			var newSize = new CGSize(width, height);
			UIGraphics.BeginImageContextWithOptions(newSize, false, UIScreen.MainScreen.Scale);
			imgView.Image.Draw(new CGRect(0, 0, newSize.Width, newSize.Height));
			imgView.Image = UIGraphics.GetImageFromCurrentImageContext();
			imgView.ContentMode = UIViewContentMode.ScaleAspectFit;
		}


		private void AddAccessoryButton(ReturnKeyTypes returnType)
		{
			if (returnType == ReturnKeyTypes.Done || returnType == ReturnKeyTypes.Next)
			{
				UIToolbar toolbar = new UIToolbar(new CGRect(0.0f, 0.0f, 50.0f, 44.0f));

				UIBarButtonItem accessoryButton = null;

				if (returnType == ReturnKeyTypes.Done)
				{
					accessoryButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
					{
						this.Control.ResignFirstResponder();
						formControl?.NextFocus?.Invoke();
					});
				}
				else
				{
					accessoryButton = new UIBarButtonItem("Next", UIBarButtonItemStyle.Bordered, delegate
					{
						this.Control.ResignFirstResponder();
						formControl?.NextFocus?.Invoke();
					});

				}

				toolbar.Items = new UIBarButtonItem[]
				{
					new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
					accessoryButton
				};
				this.Control.InputAccessoryView = toolbar;
			}
		}
	}

}

#endif