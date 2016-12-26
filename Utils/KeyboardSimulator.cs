using System;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Rou.Utils
{
	/// <summary>
	/// Simulate keyboard key presses
	/// </summary>
	public static class KeyboardSimulator
	{

        #region Windows API Code
        const byte VK_BROWSER_BACK = 0xA6;
        const byte VK_BROWSER_FORWARD = 0xA7;
        const byte VK_BROWSER_REFRESH = 0xA8;
        const byte VK_BROWSER_STOP = 0xA9;
        const byte VK_BROWSER_SEARCH = 0xAA;
        const byte VK_BROWSER_FAVORITES = 0xAB;
        const byte VK_BROWSER_HOME = 0xAC;
        const byte VK_VOLUME_MUTE = 0xAD;
        const byte VK_VOLUME_DOWN = 0xAE;
        const byte VK_VOLUME_UP = 0xAF;
        const byte VK_MEDIA_NEXT_TRACK = 0xB0;
        const byte VK_MEDIA_PREV_TRACK = 0xB1;
        const byte VK_MEDIA_STOP = 0xB2;
        const byte VK_MEDIA_PLAY_PAUSE = 0xB3;
        const byte VK_LAUNCH_MAIL = 0xB4;
        const byte VK_LAUNCH_MEDIA_SELECT = 0xB5;
        const byte VK_LAUNCH_APP1 = 0xB6;
        const byte VK_LAUNCH_APP2 = 0xB7;
        const int KEYEVENTF_EXTENDEDKEY = 0x1;
		const int KEYEVENTF_KEYUP = 0x2;
		[DllImport("user32.dll")]
		static extern void keybd_event(byte key, byte scan, int flags, int extraInfo);
		#endregion
		#region Methods
		public static void KeyDown(Keys key)
		{
			keybd_event(ParseKey(key), 0x45, 0, 0);
		}
		public static void KeyUp(Keys key)
		{
			keybd_event(ParseKey(key), 0x45, KEYEVENTF_KEYUP, 0);
		}
		public static void KeyPress(Keys key)
		{
			KeyDown(key);
			KeyUp(key);
		}
	
		static byte ParseKey(Keys key)
		{
			// Alt, Shift, and Control need to be changed for API function to work with them
			switch (key)
			{
				case Keys.Alt:
					return (byte)18;
				case Keys.Control:
					return (byte)17;
				case Keys.Shift:
					return (byte)16;
                case Keys.MediaNextTrack:
                    return VK_MEDIA_NEXT_TRACK;
                case Keys.MediaPreviousTrack:
                    return VK_MEDIA_PREV_TRACK;
                case Keys.MediaPlayPause:
                    return VK_MEDIA_PLAY_PAUSE;
                default:
					return (byte)key;
			}
		}
		#endregion
	}
}