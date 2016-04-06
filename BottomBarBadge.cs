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
using Android.Support.V4.View;
using Android.Content;
using Android.Views;
using Android.OS;
using Android.Graphics.Drawables;
using Android.Graphics;
using System.Diagnostics.CodeAnalysis;

namespace BottomNavigationBar
{
    public class BottomBarBadge : TextView, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private readonly View _tabToAddTo;

        /// <summary>
        /// Gets or sets the unread / new item / whatever count for this Badge.
        /// </summary>
        /// <value>the value this Badge should show</value>
        private int _count;
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
                SetText(_count.ToString(), BufferType.Normal);
            }
        }

        /// <summary>
        /// Controls whether you want this Badge to be shown automatically when the BottomBar tab containing it is unselected.
        /// un selection.
        /// </summary>
        /// <value><c>true</c>, if this Badge is automatically shown after unselection, otherwise <c>false</c>.</value>
        public bool AutoShowAfterUnSelection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the scale animation duration in milliseconds.
        /// </summary>
        /// <value>animation duration in milliseconds.</value>
        private long _animationDuration = 150;
        public long AnimationDuration
        {
            get { return _animationDuration; }
            set { _animationDuration = value; }
        }

        /// <summary>
        /// Is this badge currently visible?
        /// </summary>
        /// <returns><c>true</c>, is this badge is visible, <c>false</c> otherwise.</returns>
        public bool IsVisible
        {
            get;
            private set;
        }

        /// <summary>
        /// Shows the badge with a neat little scale animation.
        /// </summary>
        public void Show()
        {
            IsVisible = true;
            ViewCompat.Animate(this)
                .SetDuration(_animationDuration)
                .ScaleX(1)
                .ScaleY(1)
                .Start();
        }

        /// <summary>
        /// Hides the badge with a neat little scale animation.
        /// </summary>
        public void Hide()
        {
            IsVisible = false;
            ViewCompat.Animate(this)
                .SetDuration(_animationDuration)
                .ScaleX(0)
                .ScaleY(0)
                .Start();
        }

        public BottomBarBadge(Context context, int position, View tabToAddTo, // Rhyming accidentally! That's a Smoove Move!
                                 Color backgroundColor)
            : base(context)
        {
            _tabToAddTo = tabToAddTo;

            var lp = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            LayoutParameters = lp;
            Gravity = GravityFlags.Center;
            SetTextAppearance(context, Resource.Style.BB_BottomBarBadge_Text);

            int three = MiscUtils.DpToPixel(context, 3);
            ShapeDrawable backgroundCircle = BadgeCircle.Make(three * 3, backgroundColor);
            SetPadding(three, three, three, three);
            SetBackgroundCompat(backgroundCircle);

            var container = new FrameLayout(context);
            container.LayoutParameters = lp;

            var parent = (ViewGroup)tabToAddTo.Parent;
            parent.RemoveView(tabToAddTo);

            container.Tag = tabToAddTo.Tag;
            container.AddView(tabToAddTo);
            container.AddView(this);

            parent.AddView(container, position);

            container.ViewTreeObserver.AddOnGlobalLayoutListener(this);
        }

        protected void AdjustPosition(View tabToAddTo) 
        {
            SetX((float)(tabToAddTo.GetX() + (tabToAddTo.Width / 1.75)));
        }

        private void AdjustPositionAndSize(View tabToAddTo)
        {
            AdjustPosition(tabToAddTo);

            TranslationY = 10;

            int size = Math.Max(Width, Height);
            LayoutParameters.Width = size;
            LayoutParameters.Height = size;
        }

        private void SetBackgroundCompat(Drawable background)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
                Background = background;
            else
                SetBackgroundDrawable(background);
        }

        public void OnGlobalLayout()
        {
            AdjustPositionAndSize(_tabToAddTo);
            var obs = ViewTreeObserver;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
                obs.RemoveOnGlobalLayoutListener(this);
            else
                obs.RemoveGlobalOnLayoutListener(this);
        }
    }
}

