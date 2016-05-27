using System;
using Android.Animation;
using Android.Views;

namespace BottomNavigationBar.Listeners
{
	public class ResizePaddingTopAnimatorUpdateListener : Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
	{
		private readonly View _icon;

		public ResizePaddingTopAnimatorUpdateListener (View icon)
		{
			_icon = icon;
		}

		public void OnAnimationUpdate (ValueAnimator animation)
		{
			_icon.SetPadding (_icon.PaddingLeft, (Int32)animation.AnimatedValue, _icon.PaddingRight, _icon.PaddingBottom);
		}
	}
}

