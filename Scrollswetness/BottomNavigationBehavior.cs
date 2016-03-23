/**
* C# port created by Nikola D. on 3/15/2016.
*
* Credit goes to Nikola Despotoski:
* https://github.com/NikolaDespotoski
*/

using System;
using Android.Support.Design.Widget;
using Android.Content;
using Android.Support.V4.View;
using Android.Util;
using Java.Lang.Annotation;
using Android.Views;
using Android.Support.V4.View.Animation;
using Android.Views.Animations;

namespace BottomNavigationBar.Scrollswetness
{
    public class BottomNavigationBehavior<V> : VerticalScrollingBehavior<V>
        where V : View
    {
        private static readonly IInterpolator INTERPOLATOR = new LinearOutSlowInInterpolator();
        private readonly int mBottomNavHeight;
        private readonly int mDefaultOffset;

        private ViewPropertyAnimatorCompat mTranslationAnimator;
        private bool hidden = false;

        public BottomNavigationBehavior(int bottomNavHeight, int defaultOffset)
        {
            mBottomNavHeight = bottomNavHeight;
            mDefaultOffset = defaultOffset;
        }

        #region implemented abstract members of VerticalScrollingBehavior

        private void handleDirection(V child, ScrollDirection scrollDirection)
        {
            if (scrollDirection == ScrollDirection.SCROLL_DIRECTION_DOWN && hidden)
            {
                hidden = false;
                animateOffset(child, mDefaultOffset);
            }
            else if (scrollDirection == ScrollDirection.SCROLL_DIRECTION_UP && !hidden)
            {
                hidden = true;
                animateOffset(child, mBottomNavHeight + mDefaultOffset);
            }
        }

        private void animateOffset(V child, int offset)
        {
            ensureOrCancelAnimator(child);
            mTranslationAnimator.TranslationY(offset).Start();
        }

        private void ensureOrCancelAnimator(V child)
        {
            if (mTranslationAnimator == null)
            {
                mTranslationAnimator = ViewCompat.Animate(child);
                mTranslationAnimator.SetDuration(300);
                mTranslationAnimator.SetInterpolator(INTERPOLATOR);
            }
            else
            {
                mTranslationAnimator.Cancel();
            }
        }

        protected override bool onNestedDirectionFling(CoordinatorLayout coordinatorLayout, V child, View target, float velocityX, float velocityY, ScrollDirection scrollDirection)
        {
            handleDirection(child, scrollDirection);
            return true;
        }

        public override void onNestedVerticalOverScroll(CoordinatorLayout coordinatorLayout, V child, ScrollDirection direction, int currentOverScroll, int totalOverScroll)
        {
            
        }

        public override void onDirectionNestedPreScroll(CoordinatorLayout coordinatorLayout, V child, View target, int dx, int dy, int[] consumed, ScrollDirection scrollDirection)
        {
            handleDirection(child, scrollDirection);
        }

        #endregion
    }
}

