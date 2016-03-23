using System;
using Android.Views;
using System.Diagnostics.CodeAnalysis;

namespace BottomNavigationBar.Listeners
{
	public class BarSizeOnGlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
	{
		private readonly IOnSizeDeterminedListener _listener;
		private readonly bool _isTabletMode;
		private readonly ViewGroup _itemContainer;

		public BarSizeOnGlobalLayoutListener (IOnSizeDeterminedListener listener, bool isTabletMode, ViewGroup itemContainer)
		{
			_listener = listener;
			_isTabletMode = isTabletMode;
			_itemContainer = itemContainer;
		}

		[SuppressMessage("deprecation")]
		public void OnGlobalLayout ()
		{
			_listener.OnSizeReady(_isTabletMode ? _itemContainer.Width : _itemContainer.Height);

			var obs = _itemContainer.ViewTreeObserver;

			if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBean)
				obs.RemoveOnGlobalLayoutListener (this);
			else
				obs.RemoveGlobalOnLayoutListener (this);
		}
	}
}

