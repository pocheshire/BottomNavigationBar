using System;
using Android.Views;
using Android.Support.Design.Widget;
using BottomNavigationBar.Scrollswetness;
using Android.OS;
using System.Diagnostics.CodeAnalysis;

namespace BottomNavigationBar.Listeners
{
	internal class NavBarMagicOnGlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private readonly BottomBar _bottomBar;
        private readonly View _outerContainer;
        private readonly int _navBarHeightCopy;

        public NavBarMagicOnGlobalLayoutListener(BottomBar bottomBar, View outerContainer, int navBarHeightCopy)
        {
            _bottomBar = bottomBar;
            _outerContainer = outerContainer;
            _navBarHeightCopy = navBarHeightCopy;
        }

        public void OnGlobalLayout()
        {
            _bottomBar.ShyHeightAlreadyCalculated = true;
            
            int newHeight = _outerContainer.Height + _navBarHeightCopy;
            _outerContainer.LayoutParameters.Height = newHeight;

            if (_bottomBar.IsShy)
            {
                int defaultOffset = _bottomBar.UseExtraOffset ? _navBarHeightCopy : 0;
                _bottomBar.TranslationY = defaultOffset;
                ((CoordinatorLayout.LayoutParams)_bottomBar.LayoutParameters).Behavior = new BottomNavigationBehavior<View>(newHeight, defaultOffset);
            }

            ViewTreeObserver obs = _outerContainer.ViewTreeObserver;

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

