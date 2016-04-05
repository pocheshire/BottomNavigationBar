using System;
using Android.Support.V4.View;
using Android.Views;
using Android.Graphics;

namespace BottomNavigationBar.Adapters
{
    internal class CustomViewPropertyAnimatorListenerAdapter : ViewPropertyAnimatorListenerAdapter
    {
        private readonly View _backgroundView;
        private readonly View _bgOverlay;
        private readonly Color _newColor;

        public CustomViewPropertyAnimatorListenerAdapter(View backgroundView, int newColor, View bgOverlay)
        {
            _backgroundView = backgroundView;
            _newColor = new Color(newColor);
            _bgOverlay = bgOverlay;
        }

        private void OnCancel()
        {
            _backgroundView.SetBackgroundColor(_newColor);
            _bgOverlay.Visibility = ViewStates.Invisible;
            ViewCompat.SetAlpha(_bgOverlay, 1);
        }

        public override void OnAnimationEnd(Android.Views.View view)
        {
            OnCancel();
        }

        public override void OnAnimationCancel(View view)
        {
            OnCancel();
        }
    }
}

