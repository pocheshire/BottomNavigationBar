using System;
using BottomNavigationBar;
using Android.Content;
using Android.Graphics;

namespace Demo.Controls
{
    public class CustomBottomBarBadge : BottomBarBadge
    {
        public CustomBottomBarBadge(Context context, int position, Color backgroundColor)
            : base (context, position, backgroundColor)
        {
            AutoShowAfterUnSelection = true;
        }

        protected override void AdjustPosition(Android.Views.View tabToAddTo)
        {
            SetX((float)((tabToAddTo.Width - Width) / 2));
        }
    }
}

