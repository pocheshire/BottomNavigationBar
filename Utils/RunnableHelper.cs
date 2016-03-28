using System;

namespace BottomNavigationBar.Utils
{
    internal class RunnableHelper : Java.Lang.Object, Java.Lang.IRunnable
    {
        private readonly Action _actionToRun;

        public RunnableHelper(Action toRun)
        {
            _actionToRun = toRun;
        }

        public void Run()
        {
            _actionToRun?.Invoke();
        }
    }
}

