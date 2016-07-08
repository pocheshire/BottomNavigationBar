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
using BottomNavigationBar.Enums;
using System.Threading.Tasks;

namespace BottomNavigationBar
{
    public class BottomBarBadge : TextView, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private View _tabToAddTo;

        private static bool _needUpdateLayout;

        private Action OnLayoutUpdated { get; set; }

        internal int TabPosition { get; private set; }

        private int _count;
        /// <summary>
        /// Gets or sets the unread / new item / whatever count for this Badge.
        /// </summary>
        /// <value>the value this Badge should show</value>
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

        private long _animationDuration = 150;
        /// <summary>
        /// Gets or sets the scale animation duration in milliseconds.
        /// </summary>
        /// <value>animation duration in milliseconds.</value>
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
		/// Controls whether you want this Badge to be hidden automatically when the BottomBar tab containing it is selected.
		/// </summary>
		/// <value><c>false</c> false if you don't want this Badge to hide every time the BottomBar tab containing it is selected; otherwise, <c>true</c>.</value>
		public bool AutoHideWhenSelection { get; set; }

        /// <summary>
        /// Gets or sets the position of badge in tab.
        /// </summary>
        /// <value>Left or Right (default – Right)</value>
        public BadgePosition Position { get; set; }

        /// <summary>
        /// Shows the badge with a neat little scale animation.
        /// </summary>
		public void Show(bool animated = true)
        {
            if (_needUpdateLayout)
            {
				OnLayoutUpdated = () => Show(false);
                return;
            }

            IsVisible = true;

            ViewCompat.Animate(this)
	            .SetDuration(animated ? _animationDuration : 0)
	            .ScaleX(1)
	            .ScaleY(1)
	            .Start();
        }

        /// <summary>
        /// Hides the badge with a neat little scale animation.
        /// </summary>
		public void Hide(bool animated = true)
        {
            IsVisible = false;
            ViewCompat.Animate(this)
                .SetDuration(animated ? _animationDuration : 0)
                .ScaleX(0)
                .ScaleY(0)
                .Start();
        }

        public BottomBarBadge(Context context, int position, Color backgroundColor)
            : base(context)
        {
            _needUpdateLayout = true;

            AutoHideWhenSelection = true;
            Position = BadgePosition.Right;

            TabPosition = position;

            LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            Gravity = GravityFlags.Center;
            SetTextAppearance(context, Resource.Style.BB_BottomBarBadge_Text);

            int three = MiscUtils.DpToPixel(context, 3);
            ShapeDrawable backgroundCircle = BadgeCircle.Make(three * 3, backgroundColor);
            SetPadding(three, three, three, three);
            SetBackgroundCompat(backgroundCircle);
        }

        internal BottomBarBadge(Context context, int position, View tabToAddTo, // Rhyming accidentally! That's a Smoove Move!
                                 Color backgroundColor)
            : this(context, position, backgroundColor)
        {
            AddBadgeToTab(context, tabToAddTo);
        }

        internal void AddBadgeToTab(Context context, View tabToAddTo)
        {
            _tabToAddTo = tabToAddTo;

            var container = new FrameLayout(context);
            container.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            var parent = (ViewGroup)tabToAddTo.Parent;
            parent.RemoveView(tabToAddTo);

            container.Tag = tabToAddTo.Tag;
            tabToAddTo.Tag = null;
            container.AddView(tabToAddTo);
            container.AddView(this);

            parent.AddView(container, TabPosition);

            container.ViewTreeObserver.AddOnGlobalLayoutListener(this);
        }

        protected virtual void AdjustPosition(View tabToAddTo) 
        {
            SetX( (float)(tabToAddTo.GetX() + (tabToAddTo.Width / (Position == BadgePosition.Right ? 1.75 : 5.25))) );
        }

        private void AdjustPositionAndSize(View tabToAddTo)
        {
            AdjustPosition(tabToAddTo);

            TranslationY = 10;

            int size = Math.Max(Width, Height);

			if (LayoutParameters.Width != size || LayoutParameters.Height != size) 
			{
				var lp = LayoutParameters;
				lp.Width = size;
				lp.Height = size;

				LayoutParameters = lp;
			}
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
            var obs = ViewTreeObserver;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
                obs.RemoveOnGlobalLayoutListener(this);
            else
                obs.RemoveGlobalOnLayoutListener(this);

			AdjustPositionAndSize (_tabToAddTo);

            _needUpdateLayout = false;

            OnLayoutUpdated?.Invoke();
            OnLayoutUpdated = null;
        }
    }
}

