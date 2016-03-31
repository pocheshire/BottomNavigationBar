/**
* C# port created by Nikola D. on 3/15/2016.
*
* Credit goes to Nikola Despotoski:
* https://github.com/NikolaDespotoski
*/

using System;
using Android.Support.Design.Widget;
using Android.Content;
using Android.Support.V4.View;
using Android.Util;
using Java.Lang.Annotation;
using Android.Views;
using Android.Support.V4.View.Animation;
using Android.Views.Animations;
using Android.OS;

namespace BottomNavigationBar.Scrollswetness
{
    public class BottomNavigationBehavior<V> : VerticalScrollingBehavior<V>
        where V : View
    {
        private static readonly IInterpolator INTERPOLATOR = new LinearOutSlowInInterpolator();
		private readonly int _bottomNavHeight;
		private readonly int _defaultOffset;
		private readonly bool _isShy = false;
		private readonly bool _isTablet = false;

        private ViewPropertyAnimatorCompat mTranslationAnimator;
        private bool hidden = false;
		private int _snackbarHeight;
		private readonly IBottomNavigationWithSnackbar _withSnackBarImpl = Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop ? new LollipopBottomNavWithSnackBarImpl() : new PreLollipopBottomNavWithSnackBarImpl();
		private bool _scrollingEnabled = true;

		public BottomNavigationBehavior(int bottomNavHeight, int defaultOffset, bool shy, bool tablet)
        {
            _bottomNavHeight = bottomNavHeight;
            _defaultOffset = defaultOffset;
			_isShy = shy;
			_isTablet = tablet;
        }

        #region implemented abstract members of VerticalScrollingBehavior

        private void HandleDirection(V child, ScrollDirection scrollDirection)
        {
			if (!_scrollingEnabled)
				return;
            if (scrollDirection == ScrollDirection.SCROLL_DIRECTION_DOWN && hidden)
            {
                hidden = false;
                AnimateOffset(child, _defaultOffset);
            }
            else if (scrollDirection == ScrollDirection.SCROLL_DIRECTION_UP && !hidden)
            {
                hidden = true;
                AnimateOffset(child, _bottomNavHeight + _defaultOffset);
            }
        }

        private void AnimateOffset(V child, int offset)
        {
            EnsureOrCancelAnimator(child);
            mTranslationAnimator.TranslationY(offset).Start();
        }

        private void EnsureOrCancelAnimator(V child)
        {
            if (mTranslationAnimator == null)
            {
                mTranslationAnimator = ViewCompat.Animate(child);
                mTranslationAnimator.SetDuration(300);
                mTranslationAnimator.SetInterpolator(INTERPOLATOR);
            }
            else
            {
                mTranslationAnimator.Cancel();
            }
        }

        protected override bool OnNestedDirectionFling(CoordinatorLayout coordinatorLayout, V child, View target, float velocityX, float velocityY, ScrollDirection scrollDirection)
        {
            HandleDirection(child, scrollDirection);
            return true;
        }

        public override void onNestedVerticalOverScroll(CoordinatorLayout coordinatorLayout, V child, ScrollDirection direction, int currentOverScroll, int totalOverScroll)
        {
            
        }

        public override void onDirectionNestedPreScroll(CoordinatorLayout coordinatorLayout, V child, View target, int dx, int dy, int[] consumed, ScrollDirection scrollDirection)
        {
            HandleDirection(child, scrollDirection);
        }

		public override bool LayoutDependsOn (CoordinatorLayout parent, Java.Lang.Object child, View dependency)
		{
			_withSnackBarImpl.UpdateSnackbar(parent, dependency, child);
			return dependency is Snackbar.SnackbarLayout;
		}

		public override void OnDependentViewRemoved (CoordinatorLayout parent, Java.Lang.Object child, View dependency)
		{
			UpdateScrollingForSnackbar(dependency, true);
			base.OnDependentViewRemoved (parent, child, dependency);
		}

		private void UpdateScrollingForSnackbar(View dependency, bool enabled) 
		{
			if (!_isTablet && dependency is Snackbar.SnackbarLayout)
				_scrollingEnabled = enabled;
		}


		public override bool OnDependentViewChanged (CoordinatorLayout parent, Java.Lang.Object child, View dependency)
		{
			UpdateScrollingForSnackbar(dependency, false);
			return base.OnDependentViewChanged (parent, child, dependency);
		}

        #endregion

		private interface IBottomNavigationWithSnackbar 
		{
			void UpdateSnackbar(CoordinatorLayout parent, View dependency, View child);
		}

		private class PreLollipopBottomNavWithSnackBarImpl : IBottomNavigationWithSnackbar 
		{
			public void UpdateSnackbar(CoordinatorLayout parent, View dependency, View child)
			{
				if (!_isTablet && _isShy && dependency is Snackbar.SnackbarLayout)
				{
					if (_snackbarHeight == -1)
						_snackbarHeight = dependency.Height;
					
					int targetPadding = _bottomNavHeight + _snackbarHeight;

					ViewGroup.MarginLayoutParams layoutParams = (ViewGroup.MarginLayoutParams) dependency.LayoutParameters;
					layoutParams.BottomMargin = targetPadding;

					child.BringToFront();
					child.Parent.RequestLayout();

					if (Build.VERSION.SdkInt < BuildVersionCodes.Kitkat)
						((View)child.Parent).Invalidate ();

				}
			}
		}

		private class LollipopBottomNavWithSnackBarImpl : IBottomNavigationWithSnackbar 
		{	
			public void UpdateSnackbar(CoordinatorLayout parent, View dependency, View child)
			{
				if (!_isTablet && _isShy && dependency is Snackbar.SnackbarLayout)
				{
					if (_snackbarHeight == -1)
						_snackbarHeight = dependency.Height;
					
					int targetPadding = (_snackbarHeight + _bottomNavHeight);
					dependency.SetPadding (
						dependency.PaddingLeft, dependency.PaddingTop, dependency.PaddingRight, targetPadding
					);
				}
			}
		}
    }
}

