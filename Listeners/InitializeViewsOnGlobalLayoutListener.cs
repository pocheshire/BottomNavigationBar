using System;
using Android.Views;
using Android.Support.Design.Widget;
using Android.OS;
using BottomNavigationBar.Scrollswetness;
using System.Diagnostics.CodeAnalysis;

namespace BottomNavigationBar.Listeners
{
	internal class InitializeViewsOnGlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private readonly bool _shyHeightAlreadyCalculated;
        private readonly CoordinatorLayout.LayoutParams _layoutParams;
        private readonly View _outerContainer;
        private readonly ViewTreeObserver _viewTreeObserver;
		private readonly bool _isShy;
		private readonly bool _isTabletMode;

		public InitializeViewsOnGlobalLayoutListener(bool mShyHeightAlreadyCalculated, CoordinatorLayout.LayoutParams layoutParams, View outerContainer, ViewTreeObserver viewTreeObserver, bool isShy, bool isTabletMode)
        {
            _shyHeightAlreadyCalculated = mShyHeightAlreadyCalculated;
            _layoutParams = layoutParams;
            _outerContainer = outerContainer;
            _viewTreeObserver = viewTreeObserver;
			_isShy = isShy;
			_isTabletMode = isTabletMode;
        }

        public void OnGlobalLayout()
        {
            if (!_shyHeightAlreadyCalculated)
            {
                _layoutParams.Behavior = new BottomNavigationBehavior<View>(_outerContainer.Height, 0, _isShy, _isTabletMode);
            }

            var obs = _viewTreeObserver;

            if (obs.IsAlive)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
                    obs.RemoveOnGlobalLayoutListener(this);
                else
                    obs.RemoveGlobalOnLayoutListener(this);
            }
        }
    }
}

