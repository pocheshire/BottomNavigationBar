
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
using Demo.Views;

namespace Demo
{
    [Activity(
        Label = "Demo"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        , LaunchMode = LaunchMode.SingleInstance)]           
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            FindViewById<Button>(Resource.Id.shiftingBtn).Click += ShowShiftingActivity;
            FindViewById<Button>(Resource.Id.fixedBtn).Click += ShowFixedActivity;
        }

        private void ShowShiftingActivity(object sender, EventArgs e)
        {
            StartActivity(typeof(ShiftingActivity));
        }

        private void ShowFixedActivity(object sender, EventArgs e)
        {
            StartActivity(typeof(FixedActivity));
        }
    }
}

