using System;
using Android.Animation;
using Android.Widget;
using Android.Views;

namespace BottomNavigationBar.Listeners
{
	public class ResizeTabAnimatorUpdateListener : Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
	{
		private readonly View _tab;
		private readonly ValueAnimator _animator;

		public ResizeTabAnimatorUpdateListener (View tab, ValueAnimator animator)
		{
			_tab = tab;
			_animator = animator;
		}

		public void OnAnimationUpdate (ValueAnimator animation)
		{
            ViewGroup.LayoutParams pars = _tab.LayoutParameters;
			if (pars == null) return;

            pars.Width = (int)Math.Round((float)_animator.AnimatedValue);
			_tab.LayoutParameters = pars;
		}
	}
}

