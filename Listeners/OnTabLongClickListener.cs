using System;
using Android.Views;

namespace BottomNavigationBar.Listeners
{
    internal class OnTabLongClickListener : Java.Lang.Object, View.IOnLongClickListener
    {
        private readonly Func<bool> _callback;

        public OnTabLongClickListener(Func<bool> callback)
        {
            _callback = callback;
        }

        public bool OnLongClick(View v)
        {
            return _callback();
        }
    }
}

