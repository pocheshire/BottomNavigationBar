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
using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;

namespace BottomNavigationBar
{
    public class BottomBarTab
    {
		private int _titleResource;
		private Drawable _icon;
		private int _iconResource;
		private string _title;

		internal int Id = -1;

        /// <summary>
        /// Creates a new Tab for the BottomBar
        /// </summary>
        /// <param name="iconResource">a resource for the Tab icon.</param>
        /// <param name="title">title for the Tab.</param>
        public BottomBarTab(int iconResource, String title)
        {
            this._iconResource = iconResource;
            this._title = title;
        }

        /// <summary>
        /// Creates a new Tab for the BottomBar.
        /// </summary>
        /// <param name="icon">an icon for the Tab.</param>
        /// <param name="title">title title for the Tab.</param>
        public BottomBarTab(Drawable icon, String title)
        {
            this._icon = icon;
            this._title = title;
        }

        /// <summary>
        /// Creates a new Tab for the BottomBar.
        /// </summary>
        /// <param name="icon">an icon for the Tab.</param>
        /// <param name="titleResource">resource for the title.</param>
        public BottomBarTab(Drawable icon, int titleResource)
        {
            this._icon = icon;
            this._titleResource = titleResource;
        }

        /// <summary>
        /// Creates a new Tab for the BottomBar.
        /// </summary>
        /// <param name="iconResource">a resource for the Tab icon.</param>
        /// <param name="titleResource">resource for the title.</param>
        public BottomBarTab(int iconResource, int titleResource)
        {
            this._iconResource = iconResource;
            this._titleResource = titleResource;
        }

		internal Drawable GetIcon (Context context)
		{
			if (_iconResource != 0)
				return AppCompatDrawableManager.Get ().GetDrawable (context, _iconResource);
			else
				return _icon;
		}

		internal string GetTitle (Context context)
		{
			if (_titleResource != 0)
				return context.GetString (_titleResource);
			else
				return _title;
		}
    }
}

