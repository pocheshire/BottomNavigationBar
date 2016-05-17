using System;
using Android.Views;

namespace BottomNavigationBar.Listeners
{
    internal class OnTabClickListener : Java.Lang.Object, View.IOnClickListener
    {
        private readonly Action _callback;

        public OnTabClickListener(Action callback)
        {
            _callback = callback;
        }

        public void OnClick(View v)
        {
            _callback?.Invoke();
        }
    }
}

