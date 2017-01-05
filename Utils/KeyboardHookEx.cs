using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace Rou.Utils
{
    public delegate int keyboardHookProc(int code, int wParam, ref keyboardHookStruct lParam);
    public delegate int mouseHookProc(int code, int wParam, ref mouseHookStruct lParam);

    public struct keyboardHookStruct
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public int dwExtraInfo;
    }


    public struct mouseHookStruct
    {
        public int X;
        public int Y;
        public int flags;
        public int data;
        public int dwExtraInfo;
    }

    /// <summary>
    /// A class that manages a global low level keyboard hook
    /// </summary>
    class KeyboardHookEx
    {
        #region Constant, Structure and Delegate Definitions
        /// <summary>
        /// defines the callback type for the hook
        /// </summary>

        const int WH_KEYBOARD_LL = 13;
        const int WH_MOUSE_LL = 14;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;
        const int WM_MOUSEMOVE = 0x0200;
        const int MK_LBUTTON = 0x001;
        const int MK_MBUTTON = 0x010;
        const int MK_RBUTTON = 0x002;
        const int MK_XBUTTON1 = 0x020;
        const int MK_XBUTTON2 = 0x040;
        const int WM_MBUTTONDOWN = 0x0207;
        const int WM_MBUTTONUP = 0x0208;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;
        #endregion

        #region Instance Variables
        /// <summary>
        /// The collections of keys to watch for
        /// </summary>
        public List<Keys> HookedKeys = new List<Keys>();
        /// <summary>
        /// Handle to the hook, need this to unhook and call the next hook
        /// </summary>
        IntPtr hhook = IntPtr.Zero;
        IntPtr mhook = IntPtr.Zero;
        private keyboardHookProc hookProcDelegate;
        private mouseHookProc mhookProcDelegate;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when one of the hooked keys is pressed
        /// </summary>
        public event KeyEventHandler KeyDown;
        /// <summary>
        /// Occurs when one of the hooked keys is released
        /// </summary>
        public event KeyEventHandler KeyUp;
        #endregion

        #region Constructors and Destructors
        /// <summary>
        /// Initializes a new instance of the <see cref="globalKeyboardHook"/> class and installs the keyboard hook.
        /// </summary>
        public KeyboardHookEx()
        {
            hookProcDelegate = hookProc;
            mhookProcDelegate = mhookProc;
            hook();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="globalKeyboardHook"/> is reclaimed by garbage collection and uninstalls the keyboard hook.
        /// </summary>
        ~KeyboardHookEx()
        {
            unhook();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Installs the global hook
        /// </summary>
        public void hook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, hookProcDelegate, hInstance, 0);
            mhook = SetWindowsHookExMouse(WH_MOUSE_LL, mhookProcDelegate, hInstance, 0);
        }

        /// <summary>
        /// Uninstalls the global hook
        /// </summary>
        public void unhook()
        {
            UnhookWindowsHookEx(hhook);
            UnhookWindowsHookEx(mhook);
        }

        /// <summary>
        /// The callback for the keyboard hook
        /// </summary>
        /// <param name="code">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The keyhook event information</param>
        /// <returns></returns>
        public int hookProc(int code, int wParam, ref keyboardHookStruct lParam)
        {
            if (code >= 0)
            {
#if DEBUG
                //MessageBox.Show(lParam.vkCode.ToString());
#endif
                Keys key = (Keys)lParam.vkCode;
                if (HookedKeys.Contains(key))
                {
                    KeyEventArgs kea = new KeyEventArgs(key);
                    if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && (KeyDown != null))
                    {
                        KeyDown(this, kea);
                    }
                    else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && (KeyUp != null))
                    {
                        KeyUp(this, kea);
                    }
                    if (kea.Handled)
                        return 1;
                }
            }
            return CallNextHookEx(hhook, code, wParam, ref lParam);
        }

        public int mhookProc(int code, int wParam, ref mouseHookStruct lParam)
        {
            if (code >= 0)
            {
#if DEBUG
                //Debug.WriteLine("X:" + lParam.X + "  Y:" + lParam.Y + " 1:" + lParam.flags + " w:" + wParam);
#endif
   
                switch (wParam)
                {
                    case WM_MBUTTONDOWN:
                        KeyDown(this, new KeyEventArgs(Keys.MButton));
                        break;
                    case WM_MBUTTONUP:
                        KeyUp(this, new KeyEventArgs(Keys.MButton));
                        break;
                    case WM_LBUTTONDOWN:
                        KeyDown(this, new KeyEventArgs(Keys.LButton));
                        break;
                    case WM_LBUTTONUP:
                        KeyUp(this, new KeyEventArgs(Keys.LButton));
                        break;

                    // TODO: Add more mouse keys
                }

            }
            return CallNextHookExMouse(mhook, code, wParam, ref lParam);
        }
        #endregion

        #region DLL imports
        /// <summary>
        /// Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
        /// </summary>
        /// <param name="idHook">The id of the event you want to hook</param>
        /// <param name="callback">The callback.</param>
        /// <param name="hInstance">The handle you want to attach the event to, can be null</param>
        /// <param name="threadId">The thread you want to attach the event to, can be null</param>
        /// <returns>a handle to the desired hook</returns>
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll", EntryPoint = "SetWindowsHookEx")]
        static extern IntPtr SetWindowsHookExMouse(int idHook, mouseHookProc callback, IntPtr hInstance, uint threadId);

        /// <summary>
        /// Unhooks the windows hook.
        /// </summary>
        /// <param name="hInstance">The hook handle that was returned from SetWindowsHookEx</param>
        /// <returns>True if successful, false otherwise</returns>
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        /// <summary>
        /// Calls the next hook.
        /// </summary>
        /// <param name="idHook">The hook id</param>
        /// <param name="nCode">The hook code</param>
        /// <param name="wParam">The wparam.</param>
        /// <param name="lParam">The lparam.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref keyboardHookStruct lParam);
        [DllImport("user32.dll", EntryPoint = "CallNextHookEx")]
        static extern int CallNextHookExMouse(IntPtr idHook, int nCode, int wParam, ref mouseHookStruct lParam);

        /// <summary>
        /// Loads the library.
        /// </summary>
        /// <param name="lpFileName">Name of the library</param>
        /// <returns>A handle to the library</returns>
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);
        #endregion

    }
}
