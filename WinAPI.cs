using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ClickOnTime
{
    public class WinAPI
    {
        public struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT point);

        [DllImport("user32")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static readonly double ScreenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
        public static readonly double ScreenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;

        private static readonly object ClickLock = new object();
        public static void Click(POINT point)
        {
            lock (ClickLock)
                mouse_event(32775, (int)(65536.0 / ScreenWidth * point.x), (int)(65536.0 / ScreenHeight * point.y), 0, 0);
        }
    }
}
