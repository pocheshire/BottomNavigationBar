
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BottomNavigationBar;
using Android.Support.Design.Widget;
using BottomNavigationBar.Listeners;
using Android.Graphics;
using Demo.Controls;

namespace Demo.Views
{
    [Activity(
        Label = "Demo"
        , Icon = "@drawable/icon"
        , LaunchMode = LaunchMode.SingleInstance)]
    public class FixedActivity : Activity, IOnTabClickListener
    {
        private BottomBar _bottomBar;

        BottomBarBadge _badge0;
        BottomBarBadge _badge1;
        BottomBarBadge _badge2;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Fixed);

            // Create your application here
            _bottomBar = BottomBar.AttachShy(FindViewById<CoordinatorLayout>(Resource.Id.myCoordinator), 
                FindViewById(Resource.Id.myScrollingContent), savedInstanceState);

            _bottomBar.UseFixedMode();
            _bottomBar.UseDarkThemeWithAlpha();

            _bottomBar.SetItems(new [] {
                new BottomBarTab(Resource.Drawable.ic_recents, "Recents"),
                new BottomBarTab(Resource.Drawable.ic_favorites, "Favorites"),
                new BottomBarTab(Resource.Drawable.ic_nearby, "Nearby")
            });
            _badge0 = _bottomBar.MakeBadgeForTabAt(0, Color.Green, 100);
            _badge0.AutoShowAfterUnSelection = true;

            _badge1 = _bottomBar.MakeBadgeForTabAt(1, Color.Green, 100);
            _badge1.AutoShowAfterUnSelection = true;
            _badge1.Position = BottomNavigationBar.Enums.BadgePosition.Left;

            _badge2 = new CustomBottomBarBadge(this, 2, Color.Green);
            _badge2.Count = 100;
            _bottomBar.MakeBadgeForTab(_badge2);

            _bottomBar.SetOnTabClickListener(this);

            _bottomBar.SetActiveTabColor(Color.Red);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            // Necessary to restore the BottomBar's state, otherwise we would
            // lose the current tab on orientation change.
            _bottomBar.OnSaveInstanceState(outState);
        }

        #region IOnTabClickListener implementation

        public void OnTabSelected(int position)
        {            
            Toast.MakeText(ApplicationContext, "Tab selected!", ToastLength.Short).Show();
        }

        public void OnTabReSelected(int position)
        {
            Toast.MakeText(ApplicationContext, "Tab reselected!", ToastLength.Short).Show();
        }

        #endregion
    }
}

