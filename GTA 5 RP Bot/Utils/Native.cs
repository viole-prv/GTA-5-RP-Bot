using System.Runtime.InteropServices;

namespace GTA_5_RP_Bot
{
    public partial class Native
    {
        public const uint WM_SETICON = 0x0080;

        public const int ICON_SMALL = 0;
        public const int ICON_BIG = 1;

#pragma warning disable CA1401 // P/Invokes should not be visible

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);

        public const int SWP_NOSIZE = 0x0001;

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

#pragma warning restore CA1401 // P/Invokes should not be visible
    }
}
