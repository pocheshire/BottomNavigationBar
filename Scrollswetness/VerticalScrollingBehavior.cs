/**
* C# port created by Nikola D. on 11/22/2015.
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

namespace BottomNavigationBar.Scrollswetness
{
    public enum ScrollDirection
    {
        SCROLL_DIRECTION_UP = 1,
        SCROLL_DIRECTION_DOWN = -1,
        SCROLL_NONE = 0
    }

    public abstract class VerticalScrollingBehavior<V> : CoordinatorLayout.Behavior
        where V : View
    {
		private int _totalDyUnconsumed = 0;
		private int totalDy = 0;

		private ScrollDirection _overScrollDirection = ScrollDirection.SCROLL_NONE;
		private ScrollDirection _scrollDirection = ScrollDirection.SCROLL_NONE;

        public VerticalScrollingBehavior()
            : base()
        {
            
        }

        public VerticalScrollingBehavior(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        /// <summary>
        /// Gets the over scroll direction.
        /// </summary>
        /// <returns>SCROLL_DIRECTION_UP, CROLL_DIRECTION_DOWN, SCROLL_NONE</returns>
        public ScrollDirection GetOverScrollDirection()
        {
            return _overScrollDirection;
        }

        /// <summary>
        /// Gets the scroll direction.
        /// </summary>
        /// <returns>SCROLL_DIRECTION_UP, SCROLL_DIRECTION_DOWN, SCROLL_NONE</returns>
        public ScrollDirection GetScrollDirection()
        {
            return _scrollDirection;
        }

        #region abstract methods

        protected abstract bool OnNestedDirectionFling(CoordinatorLayout coordinatorLayout, V child, View target, float velocityX, float velocityY, ScrollDirection scrollDirection);

        /// <summary>
        /// Ons the direction nested pre scroll.
        /// </summary>
        /// <param name="coordinatorLayout">
        /// <param name="child"></param>
        /// <param name="direction">Direction of the overscroll: SCROLL_DIRECTION_UP, SCROLL_DIRECTION_DOWN</param>
        /// <param name="currentOverScroll">Unconsumed value, negative or positive based on the direction</param>
        /// <param name="totalOverScroll">Cumulative value for current direction</param>
        public abstract void onNestedVerticalOverScroll(CoordinatorLayout coordinatorLayout, V child, ScrollDirection direction, int currentOverScroll, int totalOverScroll);

        /// <summary>
        /// Ons the direction nested pre scroll.
        /// </summary>
        /// <param name="scrollDirection">Direction of the overscroll: SCROLL_DIRECTION_UP, SCROLL_DIRECTION_DOWN</param>
        public abstract void onDirectionNestedPreScroll(CoordinatorLayout coordinatorLayout, V child, View target, int dx, int dy, int[] consumed, ScrollDirection scrollDirection);

        #endregion

        #region overrides

        public override bool OnStartNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target, int nestedScrollAxes)
        {
            return (nestedScrollAxes & (int)ScrollAxis.Vertical) != 0;
        }

        public override void OnNestedScrollAccepted(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target, int nestedScrollAxes)
        {
            base.OnNestedScrollAccepted(coordinatorLayout, child, directTargetChild, target, nestedScrollAxes);
        }

        public override void OnStopNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target)
        {
            base.OnStopNestedScroll(coordinatorLayout, child, target);
        }

        public override void OnNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dxConsumed, int dyConsumed, int dxUnconsumed, int dyUnconsumed)
        {
            base.OnNestedScroll(coordinatorLayout, child, target, dxConsumed, dyConsumed, dxUnconsumed, dyUnconsumed);
            if (dyUnconsumed > 0 && _totalDyUnconsumed < 0)
            {
                _totalDyUnconsumed = 0;
                _overScrollDirection = ScrollDirection.SCROLL_DIRECTION_UP;
            }
            else if (dyUnconsumed < 0 && _totalDyUnconsumed > 0)
            {
                _totalDyUnconsumed = 0;
                _overScrollDirection = ScrollDirection.SCROLL_DIRECTION_DOWN;
            }
            _totalDyUnconsumed += dyUnconsumed;
            onNestedVerticalOverScroll(coordinatorLayout, (V)child, _overScrollDirection, dyConsumed, _totalDyUnconsumed);
        }

        public override void OnNestedPreScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dx, int dy, int[] consumed)
        {
            base.OnNestedPreScroll(coordinatorLayout, child, target, dx, dy, consumed);
            if (dy > 0 && totalDy < 0)
            {
                totalDy = 0;
                _scrollDirection = ScrollDirection.SCROLL_DIRECTION_UP;
            }
            else if (dy < 0 && totalDy >= 0)
            {
                totalDy = 0;
                _scrollDirection = ScrollDirection.SCROLL_DIRECTION_DOWN;
            }
            totalDy += dy;
            onDirectionNestedPreScroll(coordinatorLayout, (V)child, target, dx, dy, consumed, _scrollDirection);
        }

        public override bool OnNestedFling(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, float velocityX, float velocityY, bool consumed)
        {
            base.OnNestedFling(coordinatorLayout, child, target, velocityX, velocityY, consumed);
            _scrollDirection = velocityY > 0 ? ScrollDirection.SCROLL_DIRECTION_UP : ScrollDirection.SCROLL_DIRECTION_DOWN;
            return OnNestedDirectionFling(coordinatorLayout, (V)child, target, velocityX, velocityY, _scrollDirection);
        }

        public override bool OnNestedPreFling(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, float velocityX, float velocityY)
        {
            return base.OnNestedPreFling(coordinatorLayout, child, target, velocityX, velocityY);
        }

        public override WindowInsetsCompat OnApplyWindowInsets(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, WindowInsetsCompat insets)
        {
            return base.OnApplyWindowInsets(coordinatorLayout, child, insets);
        }

        public override Android.OS.IParcelable OnSaveInstanceState(CoordinatorLayout parent, Java.Lang.Object child)
        {
            return base.OnSaveInstanceState(parent, child);
        }

        #endregion
    }
}

