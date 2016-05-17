using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using BottomNavigationBar;
using BottomNavigationBar.Listeners;
using Android.Support.V4.Content;

namespace Demo.Views
{
    [Activity(
        Label = "Demo"
        , Icon = "@drawable/icon"
        , LaunchMode = LaunchMode.SingleInstance)]
    public class ShiftingActivity : Activity, IOnMenuTabClickListener
    {
        private BottomBar _bottomBar;
        private TextView _messageView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Shifting);

            _messageView = FindViewById<TextView>(Resource.Id.messageView);

            _bottomBar = BottomBar.Attach(this, savedInstanceState);
            _bottomBar.SetItemsFromMenu(Resource.Menu.bottombar_menu, this);

            // Setting colors for different tabs when there's more than three of them.
            // You can set colors for tabs in two different ways as shown below.
            _bottomBar.MapColorForTab(0, Resources.GetColor(Resource.Color.colorAccent, Theme));
            _bottomBar.MapColorForTab(1, "#5D4037");
            _bottomBar.MapColorForTab(2, "#7B1FA2");
            _bottomBar.MapColorForTab(3, "#FF5252");
            _bottomBar.MapColorForTab(4, "#FF9800");
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            // Necessary to restore the BottomBar's state, otherwise we would
            // lose the current tab on orientation change.
            _bottomBar.OnSaveInstanceState(outState);
        }

        private string GetMessage(int menuItemId, bool isReselection)
        {
            var message = "Content for ";

            switch (menuItemId) {
                case Resource.Id.bb_menu_recents:
                    message += "recents";
                    break;
                case Resource.Id.bb_menu_favorites:
                    message += "favorites";
                    break;
                case Resource.Id.bb_menu_nearby:
                    message += "nearby";
                    break;
                case Resource.Id.bb_menu_friends:
                    message += "friends";
                    break;
                case Resource.Id.bb_menu_food:
                    message += "food";
                    break;
            }

            if (isReselection)
                message += " WAS RESELECTED! YAY!";

            return message;
        }

        #region IOnMenuTabClickListener implementation

        public void OnMenuTabSelected(int menuItemId)
        {
            _messageView.Text = GetMessage(menuItemId, false);
        }

        public void OnMenuTabReSelected(int menuItemId)
        {
            Toast.MakeText(ApplicationContext, GetMessage(menuItemId, true), ToastLength.Short).Show();
        }

        #endregion
    }
}


