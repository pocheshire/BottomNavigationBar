using System;

namespace BottomNavigationBar.Listeners
{
	public interface IOnSizeDeterminedListener
	{
		/// <summary>
		/// Called when the size of the BottomBar is determined and ready.
		/// </summary>
		/// <param name="size">height or width of the BottomBar, depending on if the current device is a phone or a tablet.</param>
		void OnSizeReady(int size);
	}
}

