using System;
using Android.Views;
using Android.Support.Design.Widget;
using Android.OS;
using BottomNavigationBar.Scrollswetness;
using System.Diagnostics.CodeAnalysis;

namespace BottomNavigationBar.Listeners
{
    public class CustomOnGlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private readonly bool _shyHeightAlreadyCalculated;
        private readonly CoordinatorLayout.LayoutParams _layoutParams;
        private readonly View _outerContainer;
        private readonly ViewTreeObserver _viewTreeObserver;

        public CustomOnGlobalLayoutListener(bool mShyHeightAlreadyCalculated, CoordinatorLayout.LayoutParams layoutParams, View outerContainer, ViewTreeObserver viewTreeObserver)
        {
            _shyHeightAlreadyCalculated = mShyHeightAlreadyCalculated;
            _layoutParams = layoutParams;
            _outerContainer = outerContainer;
            _viewTreeObserver = viewTreeObserver;
        }

		[SuppressMessage("deprecation")]
        public void OnGlobalLayout()
        {
            if (!_shyHeightAlreadyCalculated)
            {
                _layoutParams.Behavior = new BottomNavigationBehavior<View>(_outerContainer.Height, 0);
            }

            ViewTreeObserver obs = _viewTreeObserver;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
            {
                obs.RemoveOnGlobalLayoutListener(this);
            }
            else
            {
                obs.RemoveGlobalOnLayoutListener(this);
            }
        }
    }
}

