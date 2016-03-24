using System;
using Android.Views;
using System.Diagnostics.CodeAnalysis;

namespace BottomNavigationBar.Listeners
{
	internal class BarSizeOnGlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
	{
		private readonly IOnSizeDeterminedListener _listener;
		private readonly bool _isTabletMode;
		private readonly ViewGroup _outerContainer;

		public BarSizeOnGlobalLayoutListener (IOnSizeDeterminedListener listener, bool isTabletMode, ViewGroup outerContainer)
		{
			_listener = listener;
			_isTabletMode = isTabletMode;
			_outerContainer = outerContainer;
		}

		public void OnGlobalLayout ()
		{
			_listener.OnSizeReady(_isTabletMode ? _outerContainer.Width : _outerContainer.Height);

			var obs = _outerContainer.ViewTreeObserver;

            if (obs.IsAlive)
            {
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBean)
                    obs.RemoveOnGlobalLayoutListener(this);
                else
                    obs.RemoveGlobalOnLayoutListener(this);
            }
		}
	}
}

