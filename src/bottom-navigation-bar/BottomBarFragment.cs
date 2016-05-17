using System;
using Android.Graphics.Drawables;

namespace BottomNavigationBar
{
	[Obsolete("Deprecated")]
    public class BottomBarFragment : BottomBarItemBase
    {
        public Android.App.Fragment Fragment
        {
            get;
            private set;
        }

        public Android.Support.V4.App.Fragment SupportFragment
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new Tab for the BottomBar.
        /// </summary>
        /// <param name="fragment">a Fragment to be shown when this Tab is selected.</param>
        /// <param name="iconResource"> a resource for the Tab icon.</param>
        /// <param name="title">title for the Tab.</param>
        public BottomBarFragment(Android.App.Fragment fragment, int iconResource, String title)
        {
            this.Fragment = fragment;
            this._iconResource = iconResource;
            this._title = title;
        }

        /// <summary>
        /// Creates a new Tab for the BottomBar.
        /// </summary>
        /// <param name="fragment"> a Fragment to be shown when this Tab is selected.</param>
        /// <param name="icon">an icon for the Tab.</param>
        /// <param name="title">title for the Tab.</param>
        public BottomBarFragment(Android.App.Fragment fragment, Drawable icon, String title)
        {
            this.Fragment = fragment;
            this._icon = icon;
            this._title = title;
        }

        /// <summary>
        /// Creates a new Tab for the BottomBar.
        /// </summary>
        /// <param name="fragment"> a Fragment to be shown when this Tab is selected.</param>
        /// <param name="icon"> an icon for the Tab.</param>
        /// <param name="titleResource">resource for the title.</param>
        public BottomBarFragment(Android.App.Fragment fragment, Drawable icon, int titleResource)
        {
            this.Fragment = fragment;
            this._icon = icon;
            this._titleResource = titleResource;
        }

        /// <summary>
        /// Creates a new Tab for the BottomBar.
        /// </summary>
        /// <param name="fragment"> a Fragment to be shown when this Tab is selected.</param>
        /// <param name="iconResource"> a resource for the Tab icon.</param>
        /// <param name="titleResource"> resource for the title.</param>
        public BottomBarFragment(Android.App.Fragment fragment, int iconResource, int titleResource)
        {
            this.Fragment = fragment;
            this._iconResource = iconResource;
            this._titleResource = titleResource;
        }

        /// <summary>
        /// Creates a new Tab for the BottomBar.
        /// </summary>
        /// <param name="fragment"> a Fragment to be shown when this Tab is selected.</param>
        /// <param name="iconResource">a resource for the Tab icon.</param>
        /// <param name="title"> title for the Tab.</param>
        public BottomBarFragment(Android.Support.V4.App.Fragment fragment, int iconResource, String title)
        {
            this.SupportFragment = fragment;
            this._iconResource = iconResource;
            this._title = title;
        }

        /// <summary>
        /// Creates a new Tab for the BottomBar.
        /// </summary>
        /// <param name="fragment"> a Fragment to be shown when this Tab is selected.</param>
        /// <param name="icon"> an icon for the Tab.</param>
        /// <param name="title"> title for the Tab.</param>
        public BottomBarFragment(Android.Support.V4.App.Fragment fragment, Drawable icon, String title)
        {
            this.SupportFragment = fragment;
            this._icon = icon;
            this._title = title;
        }

        /// <summary>
        /// Creates a new Tab for the BottomBar.
        /// </summary>
        /// <param name="fragment">a Fragment to be shown when this Tab is selected.</param>
        /// <param name="icon">an icon for the Tab.</param>
        /// <param name="titleResource">resource for the title.</param>
        public BottomBarFragment(Android.Support.V4.App.Fragment fragment, Drawable icon, int titleResource)
        {
            this.SupportFragment = fragment;
            this._icon = icon;
            this._titleResource = titleResource;
        }

        /// <summary>
        /// Creates a new Tab for the BottomBar.
        /// </summary>
        /// <param name="fragment">a Fragment to be shown when this Tab is selected.</param>
        /// <param name="iconResource">a resource for the Tab icon.</param>
        /// <param name="titleResource">resource for the title.</param>
        public BottomBarFragment(Android.Support.V4.App.Fragment fragment, int iconResource, int titleResource)
        {
            this.SupportFragment = fragment;
            this._iconResource = iconResource;
            this._titleResource = titleResource;
        }
    }
}

