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
using Android.Graphics.Drawables.Shapes;
using Android.Graphics;
using Android.Content;
using Android.Support.V4.Content.Res;

namespace BottomNavigationBar
{
    public static class BadgeCircle
    {
        internal static ShapeDrawable Make(int size, Color color) 
        {
            var indicator = new ShapeDrawable(new OvalShape());

            indicator.SetIntrinsicWidth(size);
            indicator.SetIntrinsicHeight(size);
            indicator.Paint.Color = color;

            return indicator;
        }


        internal static Drawable drawRoundCornerRectange(Context _context, Color color)
        {
            Drawable mybadge = Android.Support.V4.Content.ContextCompat.GetDrawable(_context, Resource.Drawable.bb_bottom_bar_Round_Rectangle_Shape);
            mybadge.SetColorFilter(color, PorterDuff.Mode.Multiply);
            return mybadge;
        }
    }
}

