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
using Android.Views;
using Android.OS;
using Android.Widget;
using Android.Support.V4.View;
using Android.Runtime;
using Android.Annotation;
using Android.Graphics;
using BottomNavigationBar.Adapters;
using Android.Animation;
using Android.App;
using System.Diagnostics.CodeAnalysis;
using Android.Content.Res;
using BottomNavigationBar.Listeners;

namespace BottomNavigationBar
{
    internal static class MiscUtils
    {
        public static int GetColor(Context context, int color)
        {
            var tv = new Android.Util.TypedValue();
            context.Theme.ResolveAttribute(Resource.Attribute.colorPrimary, tv, true);
            return tv.Data;
        }

        /// <summary>
        /// Converts dps to pixels nicely.
        /// </summary>
        /// <returns>dimension in pixels</returns>
        /// <param name="context">Context for getting the resources</param>
        /// <param name="dp">dimension in dps</param>
        public static int DpToPixel(Context context, float dp)
        {
            var resources = context.Resources;
            var metrics = resources.DisplayMetrics;
            return (int)(dp * ((int)metrics.DensityDpi / 160f));
        }

        /// <summary>
        /// Gets the width of the screen.
        /// </summary>
        /// <returns>The screen width.</returns>
        /// <param name="context">Context to get resources and device specific display metrics.</param>
        public static int GetScreenWidth(Context context)
        {
            var displayMetrics = context.Resources.DisplayMetrics;
            return (int)(displayMetrics.WidthPixels / displayMetrics.Density);
        }

        /// <summary>
        /// A hacky method for inflating menus from xml resources to an array of BottomBarTabs.
        /// </summary>
        /// <returns>an Array of BottomBarTabs.</returns>
        /// <param name="activity">the activity context for retrieving the MenuInflater.</param>
        /// <param name="menuRes">the xml menu resource to inflate.</param>
        public static BottomBarTab[] InflateMenuFromResource(Activity activity, int menuRes)
        {
            // A bit hacky, but hey hey what can I do
            var popupMenu = new PopupMenu(activity, null);
            var menu = popupMenu.Menu;
            activity.MenuInflater.Inflate(menuRes, menu);
        
            int menuSize = menu.Size();
            var tabs = new BottomBarTab[menuSize];
        
            for (int i = 0; i < menuSize; i++)
            {
                var item = menu.GetItem(i);
                BottomBarTab tab = new BottomBarTab(item.Icon, item.TitleFormatted.ToString());
                tab.Id = item.ItemId;
                tabs[i] = tab;
            }
        
            return tabs;
        }

        /// <summary>
        /// Animate a background color change. Uses Circular Reveal if supported, otherwise crossfades the background color in.
        /// </summary>
        /// <param name="clickedView">the view that was clicked for calculating the start position for the Circular Reveal.</param>
        /// <param name="backgroundView">the currently showing background color.</param>
        /// <param name="bgOverlay">the overlay view for the new background color that will be animated in.</param>
        /// <param name="newColor">the new color.</param>
        [TargetApiAttribute(Value = (int)BuildVersionCodes.Lollipop)]
        public static void AnimateBGColorChange(View clickedView, View backgroundView, View bgOverlay, int newColor)
        {
            int centerX = (int)(ViewCompat.GetX(clickedView) + (clickedView.MeasuredWidth / 2));
            int centerY = clickedView.MeasuredHeight / 2;
            int finalRadius = backgroundView.Width;

            backgroundView.ClearAnimation();
            bgOverlay.ClearAnimation();

            Object animator;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                if (!bgOverlay.IsAttachedToWindow)
                {
                    return;
                }

                animator = ViewAnimationUtils.CreateCircularReveal(bgOverlay, centerX, centerY, 0, finalRadius);
            }
            else
            {
                ViewCompat.SetAlpha(bgOverlay, 0);
                animator = ViewCompat.Animate(bgOverlay).Alpha(1);
            }

            if (animator is ViewPropertyAnimatorCompat)
            {
                ((ViewPropertyAnimatorCompat)animator)
                    .SetListener(new CustomViewPropertyAnimatorListenerAdapter(backgroundView, newColor, bgOverlay))
                    .Start();
            }
            else if (animator != null)
            {
                ((Animator)animator).AddListener(new CustomAnimatorListenerAdapter(backgroundView, newColor, bgOverlay));
                ((Animator)animator).Start();
            }

            bgOverlay.SetBackgroundColor(new Color(newColor)); //TODO: check this);
            bgOverlay.Visibility = ViewStates.Visible;
        }

        /// <summary>
        /// A convenience method for setting text appearance.
        /// </summary>
        /// <param name="textView">TextView which textAppearance to modify.</param>
        /// <param name="resId">style resource for the text appearance.</param>
        public static void SetTextAppearance(TextView textView, int resId)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                textView.SetTextAppearance(resId);
            else
                textView.SetTextAppearance(textView.Context, resId);
        }

		/// <summary>
		/// Determine if the current UI Mode is Night Mode.
		/// </summary>
		/// <returns><c>true</c>, if the night mode is enabled, <c>false</c> otherwise.</returns>
		/// <param name="context">Context to get the configuration.</param>
		public static bool IsNightMode(Context context)
		{
			return context.Resources.Configuration.UiMode == UiMode.NightYes;
		}

		/// <summary>
		/// A method for animating width for the tabs.
		/// </summary>
		/// <param name="tab">tab to animate.</param>
		/// <param name="start">starting width.</param>
		/// <param name="end">final width after animation.</param>
		public static void ResizeTab(View tab, float start, float end)
		{
			ValueAnimator animator = ValueAnimator.OfFloat(start, end);
			animator.SetDuration(150);
			animator.AddUpdateListener (new ResizeTabAnimatorUpdateListener (tab, animator));
			animator.Start();
		}

    }
}

