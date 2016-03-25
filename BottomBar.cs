/*
 * C# port BottomBar library for Android
 * Copyright (c) 2016 Iiro Krankka (http://github.com/roughike).
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Android.Widget;
using Android.Views;
using Android.Content;
using BottomNavigationBar.Listeners;
using Android.Graphics;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Util;
using Android.Annotation;
using Android.Support.V4.Content;
using Android.Content.Res;

namespace BottomNavigationBar
{
    public class BottomBar : FrameLayout, View.IOnClickListener, View.IOnLongClickListener
    {
        private const long ANIMATION_DURATION = 150;
        private const int MAX_FIXED_TAB_COUNT = 3;

        private const String STATE_CURRENT_SELECTED_TAB = "STATE_CURRENT_SELECTED_TAB";
        private const String STATE_BADGE_STATES_BUNDLE = "STATE_BADGE_STATES_BUNDLE";
        private const String TAG_BOTTOM_BAR_VIEW_INACTIVE = "BOTTOM_BAR_VIEW_INACTIVE";
        private const String TAG_BOTTOM_BAR_VIEW_ACTIVE = "BOTTOM_BAR_VIEW_ACTIVE";
        private const String TAG_BADGE = "BOTTOMBAR_BADGE_";

        private Context _context;
        private bool _isComingFromRestoredState;
        private bool _ignoreTabletLayout;
        private bool _isTabletMode;

        private View _backgroundView;
        private View _backgroundOverlay;
        private View _shadowView;
        private View _tabletRightBorder;

        private Color _primaryColor;
        private Color _inActiveColor;
        private Color _darkBackgroundColor;
        private Color _whiteColor;

        private int _screenWidth;
        private int _twoDp;
        private int _tenDp;
        private int _maxFixedItemWidth;

		private Object _listener;
		private Object _menuListener;

        private bool _isShiftingMode;

        private Java.Lang.Object _fragmentManager;
        private int _fragmentContainer;

        private BottomBarItemBase[] _items;
        private Dictionary<int, Color> _colorMap;
        private Dictionary<int, Java.Lang.Object> _badgeMap;
        private Dictionary<int, bool> _badgeStateMap;

        private Color _currentBackgroundColor;
        private Color _defaultBackgroundColor;

        private bool _isDarkTheme;
		private bool _ignoreNightMode;
        private int _customActiveTabColor;

        private int _pendingTextAppearance = -1;
        private Typeface _pendingTypeface;

        // For fragment state restoration
        private bool _shouldUpdateFragmentInitially;

        private bool _useTopOffset = true;
        protected bool UseTopOffset
        {
            get { return _useTopOffset; }
            set { _useTopOffset = value; }
        }

        protected View PendingUserContentView { get; set; }
        protected ViewGroup UserContainer { get; set; }
        
        protected bool DrawBehindNavBar { get; set; }
        protected bool UseOnlyStatusbarOffset { get; set; }

		public ViewGroup ItemContainer { get; private set; }

		/// <summary>
		/// Get the actual BottomBar that has the tabs inside it for whatever what you may want
		/// to do with it.
		/// </summary>
		/// <value>The BottomBar</value>
		public ViewGroup OuterContainer { get; protected set; }

		/// <summary>
		/// Gets the current tab position.
		/// </summary>
		/// <value>the position of currently selected tab.</value>
		public int CurrentTabPosition { get; private set; }

        public bool UseExtraOffset { get; private set; }

        public bool IsShy { get; internal set; }
        public bool ShyHeightAlreadyCalculated { get; internal set; }

        /// <summary>
        /// Bind the BottomBar to your Activity, and inflate your layout here.
        /// Remember to also call <seealso cref="OnRestoreInstanceState(Bundle)"/> inside
        /// of your <seealso cref="Activity.OnRestoreInstanceState(Bundle)"/> to restore the state.
        /// </summary>
        /// <param name="activity">an Activity to attach to.</param>
        /// <param name="savedInstanceState">a Bundle for restoring the state on configuration change.</param>
        /// <returns>a BottomBar at the bottom of the screen.</returns>
        public static BottomBar Attach(Activity activity, Bundle savedInstanceState)
        {
            BottomBar bottomBar = new BottomBar(activity);
            bottomBar.OnRestoreInstanceState(savedInstanceState);

            ViewGroup contentView = (ViewGroup)activity.FindViewById(Android.Resource.Id.Content);
            View oldLayout = contentView.GetChildAt(0);
            contentView.RemoveView(oldLayout);

            bottomBar.PendingUserContentView = oldLayout;
            contentView.AddView(bottomBar, 0);

            return bottomBar;
        }

        // <summary>
        /// Bind the BottomBar to your Activity, and inflate your layout here.
        /// Remember to also call <seealso cref="OnRestoreInstanceState(Bundle)"/> inside
        /// of your <seealso cref="Activity.OnRestoreInstanceState(Bundle)"/> to restore the state.
        /// </summary>
        /// <param name="view">a View, which parent we're going to attach to.</param>
        /// <param name="savedInstanceState">a Bundle for restoring the state on configuration change.</param>
        /// <returns>a BottomBar at the bottom of the screen.</returns>
        public static BottomBar Attach(View view, Bundle savedInstanceState)
        {
            BottomBar bottomBar = new BottomBar(view.Context);
            bottomBar.OnRestoreInstanceState(savedInstanceState);

            ViewGroup contentView = (ViewGroup)view.Parent;

            if (contentView != null)
            {
                View oldLayout = contentView.GetChildAt(0);
                contentView.RemoveView(oldLayout);

                bottomBar.PendingUserContentView = oldLayout;
                contentView.AddView(bottomBar, 0);
            }
            else
            {
                bottomBar.PendingUserContentView = view;
            }

            return bottomBar;
        }

        /// <summary>
        /// Deprecated. Breaks support for tablets.
        /// Use <seealso cref="attachShy(CoordinatorLayout, View, Bundle)"/> instead.
        /// </summary>
        [Obsolete("deprecated")]
        public static BottomBar AttachShy(CoordinatorLayout coordinatorLayout, Bundle savedInstanceState)
        {
            return AttachShy(coordinatorLayout, null, savedInstanceState);
        }

        /// <summary>
        /// Adds the BottomBar inside of your CoordinatorLayout and shows / hides it according to scroll state changes.
        /// Remember to also call <seealso cref="OnRestoreInstanceState(Bundle)"/> inside
        /// of your <seealso cref="Activity.OnRestoreInstanceState(Bundle)"/> to restore the state.
        /// </summary>
        /// <returns>The shy.</returns>
        /// <param name="coordinatorLayout">a CoordinatorLayout for the BottomBar to add itself into.</param>
        /// <param name="userContentView">the view (usually a NestedScrollView) that has your scrolling content. Needed for tablet support.</param>
        /// <param name="savedInstanceState">a Bundle for restoring the state on configuration change.</param>
        /// <returns>a BottomBar at the bottom of the screen.</returns>
        public static BottomBar AttachShy(CoordinatorLayout coordinatorLayout, View userContentView, Bundle savedInstanceState)
        {
            BottomBar bottomBar = new BottomBar(coordinatorLayout.Context);
            bottomBar.ToughChildHood(ViewCompat.GetFitsSystemWindows(coordinatorLayout));
            bottomBar.OnRestoreInstanceState(savedInstanceState);

            if (userContentView != null && coordinatorLayout.Context.Resources.GetBoolean(Resource.Boolean.bb_bottom_bar_is_tablet_mode))
            {
                bottomBar.PendingUserContentView = userContentView;
            }

            coordinatorLayout.AddView(bottomBar);
            return bottomBar;
        }

        /// <summary>
		/// Deprecated
		/// 
		/// Use either <see cref="SetItems(BottomBarTab...)"/> or
		/// <see cref="SetItemsFromMenu(int, IOnMenuTabClickListener)"/> and add a listener using
		/// <see cref="SetOnTabClickListener(IOnTabClickListener) to handle tab changes by yourself."/>
        /// </summary>
		[Obsolete("Deprecated")]
        public void SetFragmentItems(Android.App.FragmentManager fragmentManager, int containerResource, BottomBarFragment[] fragmentItems)
        {
            if (fragmentItems.Length > 0)
            {
                int index = 0;

                foreach (var fragmentItem in fragmentItems)
                {
                    if (fragmentItem.Fragment == null
                        && fragmentItem.SupportFragment != null)
                    {
                        throw new ArgumentException("Conflict: cannot use android.app.FragmentManager " +
                            "to handle a android.support.v4.app.Fragment object at position " + index +
                            ". If you want BottomBar to handle support Fragments, use getSupportFragment" +
                            "Manager() instead of getFragmentManager().");
                    }

                    index++;
                }
            }

            ClearItems();
            _fragmentManager = fragmentManager;
            _fragmentContainer = containerResource;
            _items = fragmentItems;
            UpdateItems(_items);
        }

		/// <summary>
		/// Deprecated
		/// 
		/// Use either <see cref="SetItems(BottomBarTab...)"/> or
		/// <see cref="SetItemsFromMenu(int, IOnMenuTabClickListener)"/> and add a listener using
		/// <see cref="SetOnTabClickListener(IOnTabClickListener) to handle tab changes by yourself."/>
		/// </summary>
		[Obsolete("Deprecated")]
        public void SetFragmentItems(Android.Support.V4.App.FragmentManager fragmentManager, int containerResource, BottomBarFragment[] fragmentItems)
        {
            if (fragmentItems.Length > 0)
            {
                int index = 0;

                foreach (var fragmentItem in fragmentItems)
                {
                    if (fragmentItem.SupportFragment == null
                        && fragmentItem.Fragment != null)
                    {
                        throw new ArgumentException("Conflict: cannot use android.support.v4.app.FragmentManager " +
                            "to handle a android.app.Fragment object at position " + index +
                            ". If you want BottomBar to handle normal Fragments, use getFragment" +
                            "Manager() instead of getSupportFragmentManager().");
                    }

                    index++;
                }
            }
            ClearItems();
            _fragmentManager = fragmentManager;
            _fragmentContainer = containerResource;
            _items = fragmentItems;
            UpdateItems(_items);
        }

        /// <summary>
        /// Set tabs and fragments for this BottomBar. When setting more than 3 items,
        /// only the icons will show by default, but the selected item will have the text visible.
        /// </summary>
        /// <param name="bottomBarTabs">an array of <see cref="BottomBarTab"/> objects.</param>
        public void SetItems(BottomBarTab[] bottomBarTabs)
        {
            ClearItems();
            _items = bottomBarTabs;
            UpdateItems(_items);
        }

        /// <summary>
		/// Deprecated, use <see cref="SetItemsFromMenu(int menuRes, IOnMenuTabClickListener listener)"/> instead
        /// </summary>
		[Obsolete("Deprecated")]
        public void SetItemsFromMenu(int menuRes, IOnMenuTabSelectedListener listener)
        {
            ClearItems();
            _items = MiscUtils.InflateMenuFromResource((Activity)Context, menuRes);
            _menuListener = listener;
            UpdateItems(_items);
        }

		/// <summary>
		/// Set items from an XML menu resource file.
		/// </summary>
		/// <param name="menuRes">the menu resource to inflate items from.</param>
		/// <param name="listener">listener for tab change events.</param>
		public void SetItemsFromMenu(int menuRes, IOnMenuTabClickListener listener)
		{
			ClearItems();
			_items = MiscUtils.InflateMenuFromResource((Activity)Context, menuRes);
			_menuListener = listener;
			UpdateItems(_items);

			if (_items != null && _items.Length > 0 && _items is BottomBarTab[])
				listener.OnMenuTabSelected (((BottomBarTab)_items [CurrentTabPosition]).Id);
		}

		/// <summary>
		/// Deprecated, use <see cref="SetOnItemSelectedListener(IOnTabClickListener listener)"/> instead
		/// </summary>
		[Obsolete("Deprecated")]
        public void SetOnItemSelectedListener(IOnTabSelectedListener listener)
        {
            _listener = listener;
        }

		/// <summary>
		/// Set a listener that gets fired when the selected tab changes.
		/// </summary>
		/// <param name="listener">a listener for monitoring changes in tab selection.</param>
		public void SetOnTabClickListener(IOnTabClickListener listener)
		{
			_listener = listener;

			if (_items != null && _items.Length > 0)
				listener.OnTabSelected (CurrentTabPosition);
		}

        /// <summary>
        /// Select a tab at the specified position.
        /// </summary>
        /// <param name="position">the position to select.</param>
        /// <param name="animate">If set to <c>true</c> animate.</param>
        public void SelectTabAtPosition(int position, bool animate)
        {
            if (_items == null || _items.Length == 0)
            {
                throw new InvalidOperationException("Can't select tab at " +
                    "position " + position + ". This BottomBar has no items set yet.");
            }
            else if (position > _items.Length - 1 || position < 0)
            {
                throw new ArgumentOutOfRangeException("Can't select tab at position " +
                    position + ". This BottomBar has no items at that position.");
            }

            UnselectTab(ItemContainer.FindViewWithTag(TAG_BOTTOM_BAR_VIEW_ACTIVE), animate);
            SelectTab(ItemContainer.GetChildAt(position), animate);

            UpdateSelectedTab(position);
        }

		/// <summary>
		/// Sets the default tab for this BottomBar that is shown until the user changes
		/// the selection.
		/// </summary>
		/// <param name="defaultTabPosition">the default tab position.</param>
		public void SetDefaultTabPosition(int defaultTabPosition)
		{
			if (_items == null || _items.Length == 0)
				throw new InvalidOperationException("Can't set default tab at " +
					"position " + defaultTabPosition + ". This BottomBar has no items set yet.");
			else if (defaultTabPosition > _items.Length - 1 || defaultTabPosition < 0)
				throw new ArgumentOutOfRangeException("Can't set default tab at position " +
					defaultTabPosition + ". This BottomBar has no items at that position.");

			if (!_isComingFromRestoredState)
				SelectTabAtPosition (defaultTabPosition, false);
		}

		/// <summary>
		/// Hide the BottomBar.
		/// </summary>
		public void Hide() 
		{
			SetBarVisibility(ViewStates.Gone);
		}

		/// <summary>
		/// Show the BottomBar.
		/// </summary>
		public void Show() 
		{
			SetBarVisibility(ViewStates.Visible);
		}

		protected void SetBarVisibility(ViewStates visibility)
		{
			if (OuterContainer != null)
				OuterContainer.Visibility = visibility;

			if (_backgroundView != null)
				_backgroundView.Visibility = visibility;

			if (_backgroundOverlay != null)
				_backgroundOverlay.Visibility = visibility;
		}


        /// <summary>
        /// Call this method in your Activity's onSaveInstanceState to keep the BottomBar's state on configuration change.
        /// </summary>
        /// <param name="outState">the Bundle to save data to.</param>
        public void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt(STATE_CURRENT_SELECTED_TAB, CurrentTabPosition);

            if (_badgeMap != null && _badgeMap.Count > 0)
            {
                if (_badgeStateMap == null)
                {
                    _badgeStateMap = new Dictionary<int, bool>();
                }

                foreach (var key in _badgeMap.Keys)
                {
                    BottomBarBadge badgeCandidate = (BottomBarBadge)OuterContainer.FindViewWithTag(_badgeMap[key]);

                    if (badgeCandidate != null)
                    {
                        _badgeStateMap.Add(key, badgeCandidate.IsVisible);
                    }
                }

                outState.PutString(STATE_BADGE_STATES_BUNDLE, Newtonsoft.Json.JsonConvert.SerializeObject(_badgeStateMap));
            }

            if (_fragmentManager != null
                && _fragmentContainer != 0
                && _items != null
                && _items is BottomBarFragment[])
            {
                BottomBarFragment bottomBarFragment = (BottomBarFragment)_items[CurrentTabPosition];

                if (bottomBarFragment.Fragment != null)
                {
                    bottomBarFragment.Fragment.OnSaveInstanceState(outState);
                }
                else if (bottomBarFragment.SupportFragment != null)
                {
                    bottomBarFragment.SupportFragment.OnSaveInstanceState(outState);
                }
            }
        }

        /// <summary>
        /// Map a background color for a Tab, that changes the whole BottomBar background color when the Tab is selected.
        /// </summary>
        /// <param name="tabPosition">zero-based index for the tab.</param>
        /// <param name="color">a hex color for the tab, such as 0xFF00FF00.</param>
        public void MapColorForTab(int tabPosition, Color color)
        {
            if (_items == null || _items.Length == 0)
            {
                throw new Java.Lang.UnsupportedOperationException("You have no BottomBar Tabs set yet. " +
                    "Please set them first before calling the mapColorForTab method.");
            }
            else if (tabPosition > _items.Length - 1 || tabPosition < 0)
            {
                throw new Java.Lang.IndexOutOfBoundsException("Cant map color for Tab " +
                    "index " + tabPosition + ". You have no BottomBar Tabs at that position.");
            }

            if (!_isShiftingMode || _isTabletMode)
                return;

            if (_colorMap == null)
            {
                _colorMap = new Dictionary<int, Color>();
            }

            if (tabPosition == CurrentTabPosition
                && _currentBackgroundColor != color)
            {
                _currentBackgroundColor = color;
                _backgroundView.SetBackgroundColor(color);
            }

            _colorMap.Add(tabPosition, color);
        }

        /// <summary>
        /// Map a background color for a Tab, that changes the whole BottomBar background color when the Tab is selected.
        /// </summary>
        /// <param name="tabPosition">zero-based index for the tab.</param>
        /// <param name="color">a hex color for the tab, such as "#00FF000".</param>
        public void MapColorForTab(int tabPosition, String color)
        {
            MapColorForTab(tabPosition, Color.ParseColor(color));
        }

        /// <summary>
        /// Use dark theme instead of the light one.
        /// NOTE: You might want to change your active tab color to something else
        /// using <see cref="SetActiveTabColor(int)"/>, as the default primary color might
        /// not have enough contrast for the dark background.
        /// </summary>
        /// <param name="darkThemeEnabled">whether the dark the should be enabled or not.</param>
        public void UseDarkTheme(bool darkThemeEnabled)
        {
            if (!_isDarkTheme && darkThemeEnabled
                && _items != null && _items.Length > 0)
            {
                DarkThemeMagic();

                for (int i = 0; i < ItemContainer.ChildCount; i++)
                {
                    View bottomBarTab = ItemContainer.GetChildAt(i);
                    ((ImageView)bottomBarTab.FindViewById(Resource.Id.bb_bottom_bar_icon)).SetColorFilter(_whiteColor);

                    if (i == CurrentTabPosition)
                    {
                        SelectTab(bottomBarTab, false);
                    }
                    else
                    {
                        UnselectTab(bottomBarTab, false);
                    }
                }
            }

            _isDarkTheme = darkThemeEnabled;
        }

		/// <summary>
		/// Ignore the automatic Night Mode detection and use a light theme by default,
		/// even if the Night Mode is on.
		/// </summary>
		public void IgnoreNightMode()
		{
			if (_items != null && _items.Length > 0) {
				throw new InvalidOperationException("This BottomBar " +
					"already has items! You must call ignoreNightMode() " +
					"before setting any items.");
			}

			_ignoreNightMode = true;
		}

        /// <summary>
        /// Set a custom color for an active tab when there's three or less items.
        /// NOTE: This value is ignored on mobile devices if you have more than three items.
        /// </summary>
        /// <param name="activeTabColor">a hex color used for active tabs, such as "#00FF000".</param>
        public void SetActiveTabColor(String activeTabColor)
        {
            SetActiveTabColor(Color.ParseColor(activeTabColor));
        }

        /// <summary>
        /// Set a custom color for an active tab when there's three or less items.
        /// NOTE: This value is ignored on mobile devices if you have more than three items.
        /// </summary>
        /// <param name="activeTabColor">a hex color used for active tabs, such as "#00FF000".</param>
        public void SetActiveTabColor(Color activeTabColor)
        {
            _customActiveTabColor = activeTabColor;

			if (_items != null && _items.Length > 0)
				SelectTabAtPosition (CurrentTabPosition, false);
        }

        /// <summary>
        /// Creates a new Badge (for example, an indicator for unread messages) for a Tab at the specified position.
        /// </summary>
        /// <returns>The <see cref="BottomBar.BottomBarBadge"/> object.</returns>
        /// <param name="tabPosition">zero-based index for the tab.</param>
        /// <param name="backgroundColor">a color for this badge, such as "#FF0000".</param>
        /// <param name="initialCount">text displayed initially for this Badge.</param>
        public BottomBarBadge MakeBadgeForTabAt(int tabPosition, String backgroundColor, int initialCount)
        {
            return MakeBadgeForTabAt(tabPosition, Color.ParseColor(backgroundColor), initialCount);
        }

        /// <summary>
        /// Creates a new Badge (for example, an indicator for unread messages) for a Tab at the specified position.
        /// </summary>
        /// <returns>The <see cref="BottomBar.BottomBarBadge"/> object.</returns>
        /// <param name="tabPosition">zero-based index for the tab.</param>
        /// <param name="backgroundColor">a color for this badge, such as "#FF0000".</param>
        /// <param name="initialCount">text displayed initially for this Badge.</param>
        public BottomBarBadge MakeBadgeForTabAt(int tabPosition, Color backgroundColor, int initialCount)
        {
            if (_items == null || _items.Length == 0)
            {
                throw new InvalidOperationException("You have no BottomBar Tabs set yet. " +
                    "Please set them first before calling the makeBadgeForTabAt() method.");
            }
            else if (tabPosition > _items.Length - 1 || tabPosition < 0)
            {
                throw new ArgumentOutOfRangeException("Cant make a Badge for Tab " +
                    "index " + tabPosition + ". You have no BottomBar Tabs at that position.");
            }

            BottomBarBadge badge = new BottomBarBadge(_context, ItemContainer.GetChildAt(tabPosition), backgroundColor);
            badge.Tag = (TAG_BADGE + tabPosition);
            badge.Count = initialCount;

            if (_badgeMap == null)
            {
                _badgeMap = new Dictionary<int, Java.Lang.Object>();
            }

            _badgeMap.Add(tabPosition, badge.Tag);
            OuterContainer.AddView(badge);

            bool canShow = true;

            if (_isComingFromRestoredState && _badgeStateMap != null
                && _badgeStateMap.ContainsKey(tabPosition))
            {
                canShow = _badgeStateMap[tabPosition];
            }

            if (canShow && CurrentTabPosition != tabPosition
                && initialCount != 0)
            {
                badge.Show();
            }
            else
            {
                badge.Hide();
            }

            return badge;
        }

        /// <summary>
        /// Set a custom TypeFace for the tab titles. The .ttf file should be located at "/src/main/assets".
        /// </summary>
        /// <param name="typeFacePath">path for the custom typeface in the assets directory.</param>
        public void SetTypeFace(String typeFacePath)
        {
            Typeface typeface = Typeface.CreateFromAsset(_context.Assets, typeFacePath);

            if (ItemContainer != null && ItemContainer.ChildCount > 0)
            {
                for (int i = 0; i < ItemContainer.ChildCount; i++)
                {
                    View bottomBarTab = ItemContainer.GetChildAt(i);
                    TextView title = (TextView)bottomBarTab.FindViewById(Resource.Id.bb_bottom_bar_title);
                    if (title != null)
                        title.Typeface = typeface;
                }
            }
            else
            {
                _pendingTypeface = typeface;
            }
        }

        /// <summary>
        /// Set a custom TypeFace for the tab titles. The .ttf file should be located at "/src/main/assets".
        /// </summary>
        /// <param name="typeface">custom typeface in the assets directory.</param>
        public void SetTypeFace(Typeface typeface)
        {
            if (ItemContainer != null && ItemContainer.ChildCount > 0)
            {
                for (int i = 0; i < ItemContainer.ChildCount; i++)
                {
                    View bottomBarTab = ItemContainer.GetChildAt(i);
                    TextView title = (TextView)bottomBarTab.FindViewById(Resource.Id.bb_bottom_bar_title);
                    if (title != null)
                        title.Typeface = typeface;
                }
            }
            else
            {
                _pendingTypeface = typeface;
            }
        }

        /// <summary>
        /// Set a custom text appearance for the tab title.
        /// </summary>
        /// <param name="resId">path to the custom text appearance.</param>
        public void SetTextAppearance(int resId)
        {
            if (ItemContainer != null && ItemContainer.ChildCount > 0)
            {
                for (int i = 0; i < ItemContainer.ChildCount; i++)
                {
                    View bottomBarTab = ItemContainer.GetChildAt(i);
                    TextView title = (TextView)bottomBarTab.FindViewById(Resource.Id.bb_bottom_bar_title);
                    MiscUtils.SetTextAppearance(title, resId);
                }
            }
            else
            {
                _pendingTextAppearance = resId;
            }
        }

        /// <summary>
        /// Hide the shadow that's normally above the BottomBar.
        /// </summary>
        public void HideShadow()
        {
            if (_shadowView != null)
            {
                _shadowView.Visibility = ViewStates.Gone;
            }
        }

        /// <summary>
        /// Prevent the BottomBar drawing behind the Navigation Bar and making it transparent. 
        /// Must be called before setting items.
        /// </summary>
        public void NoNavBarGoodness()
        {
            if (_items != null)
            {
                throw new Java.Lang.UnsupportedOperationException("This BottomBar already has items! " +
                    "You must call noNavBarGoodness() before setting the items, preferably " +
                    "right after attaching it to your layout.");
            }

            DrawBehindNavBar = false;
        }

        /// <summary>
        /// Force the BottomBar to behave exactly same on tablets and phones,
        /// instead of showing a left menu on tablets.
        /// </summary>
        public void NoTabletGoodness()
        {
            if (_items != null)
            {
                throw new Java.Lang.UnsupportedOperationException("This BottomBar already has items! " +
                    "You must call noTabletGoodness() before setting the items, preferably " +
                    "right after attaching it to your layout.");
            }

            _ignoreTabletLayout = true;
        }

		/// <summary>
		/// Get this BottomBar's height (or width), depending if the BottomBar
		/// is on the bottom (phones) or the left (tablets) of the screen.
		/// </summary>
		/// <param name="listener">listener <see cref="IOnSizeDeterminedListener"/> to get the size when it's ready.</param>
		public void GetBarSize(IOnSizeDeterminedListener listener)
		{
			int sizeCandidate = _isTabletMode ? OuterContainer.Width : OuterContainer.Height;

			if (sizeCandidate == 0) {
				OuterContainer.ViewTreeObserver.AddOnGlobalLayoutListener(new BarSizeOnGlobalLayoutListener(listener, _isTabletMode, OuterContainer));
				return;
			}

			listener.OnSizeReady(sizeCandidate);
		}


        /* ------------------ Super ugly hacks ------------------------- */

        /// <summary>
        /// If you get some unwanted extra padding in the top (such as when using CoordinatorLayout), this fixes it.
        /// </summary>
        public void NoTopOffset()
        {
            _useTopOffset = false;
        }

        /// <summary>
        /// If your ActionBar gets inside the status bar for some reason, this fixes it.
        /// </summary>
        public void UseOnlyStatusBarTopOffset()
        {
            UseOnlyStatusbarOffset = true;
        }

        /* ---------------------- End --------------------- */

        public BottomBar(Context context)
            : base(context)
        {
            Init(context, null, 0, 0);
        }

        public BottomBar(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Init(context, attrs, 0, 0);
        }

        public BottomBar(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            
            Init(context, attrs, defStyleAttr, 0);
        }

        [TargetApiAttribute(Value = (int)BuildVersionCodes.Lollipop)]
        public BottomBar(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init(context, attrs, defStyleAttr, defStyleRes);
        }

        private void Init(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
        {
            _context = context;

            _darkBackgroundColor = new Color(ContextCompat.GetColor(Context, Resource.Color.bb_darkBackgroundColor));
            _whiteColor = new Color(ContextCompat.GetColor(Context, Resource.Color.white));
            _primaryColor = new Color(MiscUtils.GetColor(Context, Resource.Attribute.colorPrimary));
            _inActiveColor = new Color(ContextCompat.GetColor(Context, Resource.Color.bb_inActiveBottomBarItemColor));

            _screenWidth = MiscUtils.GetScreenWidth(_context);
            _twoDp = MiscUtils.DpToPixel(_context, 2);
            _tenDp = MiscUtils.DpToPixel(_context, 10);
            _maxFixedItemWidth = MiscUtils.DpToPixel(_context, 168);
        }

        private void InitializeViews()
        {
            _isTabletMode = !_ignoreTabletLayout && _context.Resources.GetBoolean(Resource.Boolean.bb_bottom_bar_is_tablet_mode);

            View rootView = View.Inflate(_context, _isTabletMode ? Resource.Layout.bb_bottom_bar_item_container_tablet : Resource.Layout.bb_bottom_bar_item_container, null);
            _tabletRightBorder = rootView.FindViewById(Resource.Id.bb_tablet_right_border);

            UserContainer = (ViewGroup)rootView.FindViewById(Resource.Id.bb_user_content_container);
            _shadowView = rootView.FindViewById(Resource.Id.bb_bottom_bar_shadow);

            OuterContainer = (ViewGroup)rootView.FindViewById(Resource.Id.bb_bottom_bar_outer_container);
            ItemContainer = (ViewGroup)rootView.FindViewById(Resource.Id.bb_bottom_bar_item_container);

            _backgroundView = rootView.FindViewById(Resource.Id.bb_bottom_bar_background_view);
            _backgroundOverlay = rootView.FindViewById(Resource.Id.bb_bottom_bar_background_overlay);

            if (IsShy && _ignoreTabletLayout)
            {
                PendingUserContentView = null;
            }

            if (PendingUserContentView != null)
            {
                var param = PendingUserContentView.LayoutParameters;

                if (param == null)
                {
                    param = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                }

                if (_isTabletMode && IsShy)
                {
                    ((ViewGroup)PendingUserContentView.Parent).RemoveView(PendingUserContentView);
                }

                UserContainer.AddView(PendingUserContentView, 0, param);
                PendingUserContentView = null;
            }

            if (IsShy && !_isTabletMode)
            {
                ViewTreeObserver.AddOnGlobalLayoutListener(new InitializeViewsOnGlobalLayoutListener(ShyHeightAlreadyCalculated, ((CoordinatorLayout.LayoutParams)LayoutParameters), OuterContainer, ViewTreeObserver));
            }

            AddView(rootView);
        }

        /// <summary>
        /// Makes this BottomBar "shy". In other words, it hides on scroll.
        /// </summary>
        /// <param name="useExtraOffsets">Use extra offset.</param>
        private void ToughChildHood(bool useExtraOffsets)
        {
            IsShy = true;
            UseExtraOffset = useExtraOffsets;
        }

        public void OnClick(View v)
        {
            if (v.Tag.Equals(TAG_BOTTOM_BAR_VIEW_INACTIVE))
            {
                UnselectTab(FindViewWithTag(TAG_BOTTOM_BAR_VIEW_ACTIVE), true);
                SelectTab(v, true);
            }
			UpdateSelectedTab(FindItemPosition(v));
        }

        private void UpdateSelectedTab(int newPosition)
        {
			var notifyMenuListener = _menuListener != null && _items is BottomBarTab[];
			var notifyRegularListener = _listener != null;

			if (newPosition != CurrentTabPosition)
            {
                HandleBadgeVisibility(CurrentTabPosition, newPosition);
                CurrentTabPosition = newPosition;

				if (notifyRegularListener)
					NotifyRegularListener (_listener, false, CurrentTabPosition);

				if (notifyMenuListener)
					NotifyMenuListener (_menuListener, false, ((BottomBarTab)_items [CurrentTabPosition]).Id);

				UpdateCurrentFragment();
			}
			else
			{
				if (notifyRegularListener)
					NotifyRegularListener (_listener, true, CurrentTabPosition);

				if (notifyMenuListener && _menuListener is IOnMenuTabClickListener)
					NotifyMenuListener (_menuListener, true, ((BottomBarTab)_items [CurrentTabPosition]).Id);
			}
        }

		private void NotifyRegularListener(Object listener, bool isReselection, int position)
		{
			if (listener is IOnTabClickListener)
			{
				var onTabClickListener = (IOnTabClickListener) listener;
				if (!isReselection) 
					onTabClickListener.OnTabSelected(position);
				else
					onTabClickListener.OnTabReSelected(position);
			}
			else if (_listener is IOnTabSelectedListener)
			{
				var onTabSelectedListener = (IOnTabSelectedListener) listener;
				if (!isReselection)
					onTabSelectedListener.OnItemSelected(position);
			}
		}

		private void NotifyMenuListener(Object listener, bool isReselection, int menuItemId)
		{
			if (listener is IOnMenuTabClickListener)
			{
				var onMenuTabClickListener = (IOnMenuTabClickListener) listener;
				if (!isReselection) 
					onMenuTabClickListener.OnMenuTabSelected(menuItemId);
				else
					onMenuTabClickListener.OnMenuTabReSelected(menuItemId);
			}
			else if (_listener is IOnMenuTabSelectedListener)
			{
				var onMenuTabSelectedListener = (IOnMenuTabSelectedListener) listener;
				if (!isReselection)
					onMenuTabSelectedListener.OnMenuItemSelected(menuItemId);
			}
		}


        private void HandleBadgeVisibility(int oldPosition, int newPosition)
        {
            if (_badgeMap == null)
            {
                return;
            }

            if (_badgeMap.ContainsKey(oldPosition))
            {
                BottomBarBadge oldBadge = (BottomBarBadge)OuterContainer.FindViewWithTag(_badgeMap[oldPosition]);

                if (oldBadge.AutoShowAfterUnSelection)
                {
                    oldBadge.Show();
                }
                else
                {
                    oldBadge.Hide();
                }
            }

            if (_badgeMap.ContainsKey(newPosition))
            {
                BottomBarBadge newBadge = (BottomBarBadge)OuterContainer.FindViewWithTag(_badgeMap[newPosition]);
                newBadge.Hide();
            }
        }

        public bool OnLongClick(View v)
        {
            if ((_isShiftingMode || _isTabletMode) && v.Tag.Equals(TAG_BOTTOM_BAR_VIEW_INACTIVE))
            {
                Toast.MakeText(_context, _items[FindItemPosition(v)].GetTitle(_context), ToastLength.Short).Show();
            }

            return true;
        }

        private void UpdateItems(BottomBarItemBase[] bottomBarItems)
        {
			if (ItemContainer == null)
				InitializeViews ();

            int index = 0;
            int biggestWidth = 0;
            _isShiftingMode = MAX_FIXED_TAB_COUNT < bottomBarItems.Length;

			if (!_isDarkTheme && !_ignoreNightMode && MiscUtils.IsNightMode (_context))
				_isDarkTheme = true;

            if (!_isTabletMode && _isShiftingMode)
            {
                _defaultBackgroundColor = _currentBackgroundColor = _primaryColor;
                _backgroundView.SetBackgroundColor(_defaultBackgroundColor);

                if (_context is Activity)
                {
                    NavBarMagic((Activity)_context, this);
                }
            }
            else if (_isDarkTheme)
            {
                DarkThemeMagic();
            }

            View[] viewsToAdd = new View[bottomBarItems.Length];

            foreach (var bottomBarItemBase in bottomBarItems)
            {
                int layoutResource;

                if (_isShiftingMode && !_isTabletMode)
                {
                    layoutResource = Resource.Layout.bb_bottom_bar_item_shifting;
                }
                else
                {
                    layoutResource = _isTabletMode ? Resource.Layout.bb_bottom_bar_item_fixed_tablet : Resource.Layout.bb_bottom_bar_item_fixed;
                }

                View bottomBarTab = View.Inflate(_context, layoutResource, null);
                ImageView icon = (ImageView)bottomBarTab.FindViewById(Resource.Id.bb_bottom_bar_icon);

                icon.SetImageDrawable(bottomBarItemBase.GetIcon(_context));

                if (!_isTabletMode)
                {
                    TextView title = (TextView)bottomBarTab.FindViewById(Resource.Id.bb_bottom_bar_title);
                    title.Text = bottomBarItemBase.GetTitle(_context);

                    if (_pendingTextAppearance != -1)
                    {
                        MiscUtils.SetTextAppearance(title, _pendingTextAppearance);
                    }

                    if (_pendingTypeface != null)
                    {
                        title.Typeface = (_pendingTypeface);
                    }
                }

                if (_isDarkTheme || (!_isTabletMode && _isShiftingMode))
                {
                    icon.SetColorFilter(_whiteColor);
                }

                if (bottomBarItemBase is BottomBarTab)
                {
                    bottomBarTab.Id = (((BottomBarTab)bottomBarItemBase).Id);
                }

                if (index == CurrentTabPosition)
                {
                    SelectTab(bottomBarTab, false);
                }
                else
                {
                    UnselectTab(bottomBarTab, false);
                }

                if (!_isTabletMode)
                {
                    if (bottomBarTab.Width > biggestWidth)
                    {
                        biggestWidth = bottomBarTab.Width;
                    }

                    viewsToAdd[index] = bottomBarTab;
                }
                else
                {
                    ItemContainer.AddView(bottomBarTab);
                }

                bottomBarTab.SetOnClickListener(this);
                bottomBarTab.SetOnLongClickListener(this);
                index++;
            }

            if (!_isTabletMode)
            {
                int proposedItemWidth = Math.Min(
                                            MiscUtils.DpToPixel(_context, _screenWidth / bottomBarItems.Length),
                                            _maxFixedItemWidth
                                        );

                var param = new LinearLayout.LayoutParams(proposedItemWidth, LinearLayout.LayoutParams.WrapContent);

                foreach (var bottomBarView in viewsToAdd)
                {
                    bottomBarView.LayoutParameters = param;
                    ItemContainer.AddView(bottomBarView);
                }
            }

            if (_pendingTextAppearance != -1)
            {
                _pendingTextAppearance = -1;
            }

            if (_pendingTypeface != null)
            {
                _pendingTypeface = null;
            }
        }

        private void DarkThemeMagic()
        {
            if (!_isTabletMode)
            {
                _backgroundView.SetBackgroundColor(_darkBackgroundColor);
            }
            else
            {
                ItemContainer.SetBackgroundColor(_darkBackgroundColor);
                _tabletRightBorder.SetBackgroundColor(new Color(ContextCompat.GetColor(_context, Resource.Color.bb_tabletRightBorderDark)));
            }
        }

        private void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            if (savedInstanceState != null)
            {
                CurrentTabPosition = savedInstanceState.GetInt(STATE_CURRENT_SELECTED_TAB, -1);
                _badgeStateMap = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, bool>>(savedInstanceState.GetString(STATE_BADGE_STATES_BUNDLE));

                if (CurrentTabPosition == -1)
                {
                    CurrentTabPosition = 0;
                    Log.Error("BottomBar", "You must override the Activity's onSave" +
                        "InstanceState(Bundle outState) and call BottomBar.onSaveInstanc" +
                        "eState(outState) there to restore the state properly.");
                }

                _isComingFromRestoredState = true;
                _shouldUpdateFragmentInitially = true;
            }
        }

        private void SelectTab(View tab, bool animate)
        {
            tab.Tag = TAG_BOTTOM_BAR_VIEW_ACTIVE;
            ImageView icon = (ImageView)tab.FindViewById(Resource.Id.bb_bottom_bar_icon);
            TextView title = (TextView)tab.FindViewById(Resource.Id.bb_bottom_bar_title);

            int tabPosition = FindItemPosition(tab);

            if (!_isShiftingMode || _isTabletMode)
            {
                var activeColor = _customActiveTabColor != 0 ? new Color(_customActiveTabColor) : _primaryColor;
                icon.SetColorFilter(activeColor);

                if (title != null)
                {
                    title.SetTextColor(activeColor);
                }
            }

            if (_isDarkTheme)
            {
                if (title != null)
                {
                    ViewCompat.SetAlpha(title, 1.0f);
                }

                ViewCompat.SetAlpha(icon, 1.0f);
            }

            if (title == null)
            {
                return;
            }

            int translationY = _isShiftingMode ? _tenDp : _twoDp;

            if (animate)
            {
                ViewCompat.Animate(title)
                    .SetDuration(ANIMATION_DURATION)
                    .ScaleX(1)
                    .ScaleY(1)
                    .Start();
                ViewCompat.Animate(tab)
                    .SetDuration(ANIMATION_DURATION)
                    .TranslationY(-translationY)
                    .Start();

                if (_isShiftingMode)
                {
                    ViewCompat.Animate(icon)
                        .SetDuration(ANIMATION_DURATION)
                        .Alpha(1.0f)
                        .Start();
                }

                HandleBackgroundColorChange(tabPosition, tab);
            }
            else
            {
                ViewCompat.SetScaleX(title, 1);
                ViewCompat.SetScaleY(title, 1);
                ViewCompat.SetTranslationY(tab, -translationY);

                if (_isShiftingMode)
                {
                    ViewCompat.SetAlpha(icon, 1.0f);
                }
            }
        }

        private void UnselectTab(View tab, bool animate)
        {
            tab.Tag = (TAG_BOTTOM_BAR_VIEW_INACTIVE);

            ImageView icon = (ImageView)tab.FindViewById(Resource.Id.bb_bottom_bar_icon);
            TextView title = (TextView)tab.FindViewById(Resource.Id.bb_bottom_bar_title);

            if (!_isShiftingMode || _isTabletMode)
            {
                var inActiveColor = _isDarkTheme ? _whiteColor : _inActiveColor;
                icon.SetColorFilter(inActiveColor);

                if (title != null)
                {
                    title.SetTextColor(inActiveColor);
                }
            }

            if (_isDarkTheme)
            {
                if (title != null)
                {
                    ViewCompat.SetAlpha(title, 0.6f);
                }

                ViewCompat.SetAlpha(icon, 0.6f);
            }

            if (title == null)
            {
                return;
            }

            float scale = _isShiftingMode ? 0 : 0.86f;

            if (animate)
            {
                ViewCompat.Animate(title)
                    .SetDuration(ANIMATION_DURATION)
                    .ScaleX(scale)
                    .ScaleY(scale)
                    .Start();
                ViewCompat.Animate(tab)
                    .SetDuration(ANIMATION_DURATION)
                    .TranslationY(0)
                    .Start();

                if (_isShiftingMode)
                {
                    ViewCompat.Animate(icon)
                        .SetDuration(ANIMATION_DURATION)
                        .Alpha(0.6f)
                        .Start();
                }
            }
            else
            {
                ViewCompat.SetScaleX(title, scale);
                ViewCompat.SetScaleY(title, scale);
                ViewCompat.SetTranslationY(tab, 0);

                if (_isShiftingMode)
                {
                    ViewCompat.SetAlpha(icon, 0.6f);
                }
            }
        }

        private void HandleBackgroundColorChange(int tabPosition, View tab)
        {
            if (!_isShiftingMode || _isTabletMode)
                return;

            if (_colorMap != null && _colorMap.ContainsKey(tabPosition))
            {
                HandleBackgroundColorChange(tab, _colorMap[tabPosition]);
            }
            else
            {
                HandleBackgroundColorChange(tab, _defaultBackgroundColor);
            }
        }

        private void HandleBackgroundColorChange(View tab, Color color)
        {
            MiscUtils.AnimateBGColorChange(tab,
                _backgroundView,
                _backgroundOverlay,
                color);
            _currentBackgroundColor = color;
        }

        private int FindItemPosition(View viewToFind)
        {
            int position = 0;

            for (int i = 0; i < ItemContainer.ChildCount; i++)
            {
                View candidate = ItemContainer.GetChildAt(i);

                if (candidate.Equals(viewToFind))
                {
                    position = i;
                    break;
                }
            }

            return position;
        }

        private void UpdateCurrentFragment()
        {
            if (!_shouldUpdateFragmentInitially && _fragmentManager != null
                && _fragmentContainer != 0
                && _items != null
                && _items is BottomBarFragment[])
            {
                var newFragment = ((BottomBarFragment)_items[CurrentTabPosition]);

                if (_fragmentManager is Android.Support.V4.App.FragmentManager
                    && newFragment.SupportFragment != null)
                {
                    ((Android.Support.V4.App.FragmentManager)_fragmentManager).BeginTransaction()
                        .Replace(_fragmentContainer, newFragment.SupportFragment)
                        .Commit();
                }
                else if (_fragmentManager is Android.App.FragmentManager
                         && newFragment.Fragment != null)
                {
                    ((Android.App.FragmentManager)_fragmentManager).BeginTransaction()
                        .Replace(_fragmentContainer, newFragment.Fragment)
                        .Commit();
                }
            }

            _shouldUpdateFragmentInitially = false;
        }

        private void ClearItems()
        {
            if (ItemContainer != null)
            {
                int childCount = ItemContainer.ChildCount;

                if (childCount > 0)
                {
                    for (int i = 0; i < childCount; i++)
                    {
                        ItemContainer.RemoveView(ItemContainer.GetChildAt(i));
                    }
                }
            }

            if (_fragmentManager != null)
            {
                _fragmentManager = null;
            }

            if (_fragmentContainer != 0)
            {
                _fragmentContainer = 0;
            }

            if (_items != null)
            {
                _items = null;
            }
        }

        private static void NavBarMagic(Activity activity, BottomBar bottomBar)
        {
            var res = activity.Resources;
            int softMenuIdentifier = res.GetIdentifier("config_showNavigationBar", "bool", "android");
            int navBarIdentifier = res.GetIdentifier("navigation_bar_height", "dimen", "android");
            int navBarHeight = 0;

            if (navBarIdentifier > 0)
            {
                navBarHeight = res.GetDimensionPixelSize(navBarIdentifier);
            }

            if (!bottomBar.DrawBehindNavBar
                || navBarHeight == 0
                || (!(softMenuIdentifier > 0 && res.GetBoolean(softMenuIdentifier))))
            {
                return;
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich
                && ViewConfiguration.Get(activity).HasPermanentMenuKey)
            {
                return;
            }

            /* Copy-paste coding made possible by: http://stackoverflow.com/a/14871974/940036 */
            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1)
            {
                var d = activity.WindowManager.DefaultDisplay;

                DisplayMetrics realDisplayMetrics = new DisplayMetrics();
                d.GetRealMetrics(realDisplayMetrics);

                int realHeight = realDisplayMetrics.HeightPixels;
                int realWidth = realDisplayMetrics.WidthPixels;

                DisplayMetrics displayMetrics = new DisplayMetrics();
                d.GetMetrics(displayMetrics);

                int displayHeight = displayMetrics.HeightPixels;
                int displayWidth = displayMetrics.WidthPixels;

                var hasSoftwareKeys = (realWidth - displayWidth) > 0
                                      || (realHeight - displayHeight) > 0;

                if (!hasSoftwareKeys)
                {
                    return;
                }
            }
            /* End of delicious copy-paste code */

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat
                && res.Configuration.Orientation == Android.Content.Res.Orientation.Portrait)
            {
                activity.Window.Attributes.Flags |= WindowManagerFlags.TranslucentNavigation;

                if (bottomBar.UseTopOffset)
                {
                    int offset;
                    int statusBarResource = res.GetIdentifier("status_bar_height", "dimen", "android");

                    if (statusBarResource > 0)
                    {
                        offset = res.GetDimensionPixelSize(statusBarResource);
                    }
                    else
                    {
                        offset = MiscUtils.DpToPixel(activity, 25);
                    }
                  
                    if (!bottomBar.UseOnlyStatusbarOffset)
                    {
                        TypedValue tv = new TypedValue();
                        if (activity.Theme.ResolveAttribute(Android.Resource.Attribute.ActionBarSize, tv, true))
                        {
                            offset += TypedValue.ComplexToDimensionPixelSize(tv.Data,
                                res.DisplayMetrics);
                        }
                        else
                        {
                            offset += MiscUtils.DpToPixel(activity, 56);
                        }
                    }

                    bottomBar.UserContainer.SetPadding(0, offset, 0, 0);
                }

                View outerContainer = bottomBar.OuterContainer;
                int navBarHeightCopy = navBarHeight;
                bottomBar.ViewTreeObserver.AddOnGlobalLayoutListener(new NavBarMagicOnGlobalLayoutListener(bottomBar, outerContainer, navBarHeightCopy));
            }
        }
    }
}

