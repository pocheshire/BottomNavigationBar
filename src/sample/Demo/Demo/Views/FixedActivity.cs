
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

namespace Demo.Views
{
    [Activity(
        Label = "Demo"
        , Icon = "@drawable/icon"
        , LaunchMode = LaunchMode.SingleInstance)]
    public class FixedActivity : Activity, IOnTabClickListener
    {
        private BottomBar _bottomBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Fixed);

            // Create your application here
            _bottomBar = BottomBar.AttachShy(FindViewById<CoordinatorLayout>(Resource.Id.myCoordinator), 
                FindViewById(Resource.Id.myScrollingContent), savedInstanceState);

            _bottomBar.UseDarkTheme();
            _bottomBar.UseFixedMode();

            _bottomBar.SetItems(new [] {
                new BottomBarTab(Resource.Drawable.ic_recents, "Recents"),
                new BottomBarTab(Resource.Drawable.ic_favorites, "Favorites"),
                new BottomBarTab(Resource.Drawable.ic_nearby, "Nearby")
            });

            _bottomBar.SetOnTabClickListener(this);

            _bottomBar.SetActiveTabColor(Resources.GetColor(Resource.Color.colorAccent, Theme));
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

