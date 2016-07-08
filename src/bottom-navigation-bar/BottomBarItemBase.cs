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
using Android.Graphics.Drawables;
using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;

namespace BottomNavigationBar
{
    public class BottomBarItemBase
    {
        protected int _iconResource;
        protected Drawable _icon;
        protected int _titleResource;
        protected String _title;
        protected int _color;
        protected bool _isEnabled;
        protected bool _isVisible;

        public Drawable GetIcon(Context context)
        {
			return this._iconResource != 0 ? AppCompatDrawableManager.Get ().GetDrawable(context, this._iconResource) : this._icon;
        }

        public String GetTitle(Context context)
        {
            return this._titleResource != 0 ? context.GetString(this._titleResource) : this._title;
        }

        public bool IsEnabled
        {
            get
            {
                return this._isEnabled;
            }
            set
            {
                this._isEnabled = value;
            }
        }

        public bool IsVisible
        {
            get
            {
                return this._isVisible;
            }
            set
            {
                this._isVisible = value;
            }
        }
    }
}

