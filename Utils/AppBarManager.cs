using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace aydocs.NotchWin.Utils
{
    /// <summary>
    /// Registers NotchWin as a Windows AppBar so it reserves screen space at the
    /// top similar to the macOS menu bar. All other app windows are pushed down
    /// and cannot overlap the reserved strip.
    /// </summary>
    public sealed class AppBarManager : IDisposable
    {
        // ─── Win32 ────────────────────────────────────────────────────────────
        private const int ABM_NEW            = 0x00000000;
        private const int ABM_REMOVE         = 0x00000001;
        private const int ABM_QUERYPOS       = 0x00000002;
        private const int ABM_SETPOS         = 0x00000003;
        private const int ABM_GETSTATE       = 0x00000004;
        private const int ABM_WINDOWPOSCHANGED = 0x00000009;

        private const int ABE_TOP = 1;

        private const int WM_USER = 0x0400;
        private const int CallbackMessage = WM_USER + 1;   // arbitrary app-defined msg

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public int    cbSize;
            public IntPtr hWnd;
            public uint   uCallbackMessage;
            public uint   uEdge;
            public RECT   rc;
            public int    lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern IntPtr SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);

        // ─── State ────────────────────────────────────────────────────────────
        private readonly Window  _window;
        private IntPtr           _hwnd;
        private bool             _registered;
        private bool             _disposed;
        private HwndSource?      _hwndSource;

        public static AppBarManager? Instance { get; private set; }

        // Height in physical pixels that will be reserved at the top of the screen.
        // 32 px × DPI scale matches typical macOS menu-bar height on HiDPI.
        public int ReservedHeightPx { get; private set; }

        public AppBarManager(Window window, int reservedHeightPx = 32)
        {
            _window          = window ?? throw new ArgumentNullException(nameof(window));
            ReservedHeightPx = reservedHeightPx;
            Instance         = this;
        }

        /// <summary>Register the AppBar for the current monitor.</summary>
        public void Register()
        {
            if (_registered) return;

            if (!_window.IsLoaded)
            {
                _window.Loaded += (_, _) => Register();
                return;
            }

            _hwnd = new WindowInteropHelper(_window).Handle;
            if (_hwnd == IntPtr.Zero) return;

            // Hook the window procedure so we can handle the AppBar callback message
            _hwndSource = HwndSource.FromHwnd(_hwnd);
            _hwndSource?.AddHook(WndProc);

            var data = BuildData();
            SHAppBarMessage(ABM_NEW, ref data);

            _registered = true;
            SetPosition();
        }

        /// <summary>
        /// Call this whenever the window changes position/size (e.g. monitor switch).
        /// </summary>
        public void SetPosition()
        {
            if (!_registered) return;

            // Find which monitor the window is on
            var screen = GetCurrentScreen();
            if (screen == null) return;

            var bounds = screen.Bounds;

            // Query: let the shell adjust the rectangle if needed
            var data = BuildData();
            data.uEdge = ABE_TOP;
            data.rc = new RECT
            {
                Left   = bounds.Left,
                Top    = bounds.Top,
                Right  = bounds.Right,
                Bottom = bounds.Top + ReservedHeightPx
            };

            SHAppBarMessage(ABM_QUERYPOS, ref data);

            // Always snap to the true top
            data.rc.Bottom = data.rc.Top + ReservedHeightPx;

            SHAppBarMessage(ABM_SETPOS, ref data);
        }

        /// <summary>Unregister and release the reserved strip.</summary>
        public void Unregister()
        {
            if (!_registered) return;

            var data = BuildData();
            SHAppBarMessage(ABM_REMOVE, ref data);
            _registered = false;

            _hwndSource?.RemoveHook(WndProc);
        }

        // ─── Helpers ──────────────────────────────────────────────────────────
        private APPBARDATA BuildData() => new APPBARDATA
        {
            cbSize          = Marshal.SizeOf<APPBARDATA>(),
            hWnd            = _hwnd,
            uCallbackMessage = CallbackMessage,
        };

        private Screen? GetCurrentScreen()
        {
            if (_hwnd == IntPtr.Zero) return Screen.PrimaryScreen;
            return Screen.FromHandle(_hwnd);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == CallbackMessage)
            {
                // AppBar callback – re-assert position when shell asks us to
                SetPosition();
            }
            return IntPtr.Zero;
        }

        // ─── IDisposable ──────────────────────────────────────────────────────
        public void Dispose()
        {
            if (_disposed) return;
            Unregister();
            _disposed = true;
            if (Instance == this) Instance = null;
        }
    }
}
