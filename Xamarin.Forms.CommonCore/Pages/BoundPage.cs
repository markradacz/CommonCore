﻿﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.CommonCore;
namespace Xamarin.Forms.CommonCore
{
	public abstract class BoundPage<T> : BasePages
		where T : ObservableViewModel
	{
        private long appearingUTC;

        private WeakReference<T> _vm;
        public T VM 
        {
            get
            {
                return _vm.ToObject<T>();
            }
            set
            {
                _vm = new WeakReference<T>(value);
            } 
        }

		public BoundPage()
		{
			VM = InjectionManager.GetViewModel<T>();
			this.BindingContext = VM;
            if (!string.IsNullOrEmpty(VM.PageTitle))
                VM.PageTitle = this.Title;
            this.SetBinding(ContentPage.TitleProperty, "PageTitle");

		}

		protected override void OnAppearing()
		{
            appearingUTC = DateTime.UtcNow.Ticks;

			if (Navigation != null)
				CoreSettings.AppNav = Navigation;
			base.OnAppearing();
		}

        protected override void OnDisappearing()
        {
			if (CoreSettings.AppData.Instance.Settings.AnalyticsEnabled)
			{
                VM.Log.LogAnalytics(this.GetType().FullName, new TrackingMetatData()
                {
                    StartUtc = appearingUTC,
                    EndUtc = DateTime.UtcNow.Ticks
                });
			}
            base.OnDisappearing();
        }

	}
}

