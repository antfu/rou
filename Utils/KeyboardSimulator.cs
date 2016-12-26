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
		const int KEYEVENTF_EXTENDEDKEY = 0x1;
		const int KEYEVENTF_KEYUP = 0x2;
		[DllImport("user32.dll")]
		static extern void keybd_event(byte key, byte scan, int flags, int extraInfo);
		#endregion
		#region Methods
		public static void KeyDown(Keys key)
		{
			keybd_event(ParseKey(key), 0, 0, 0);
		}
		public static void KeyUp(Keys key)
		{
			keybd_event(ParseKey(key), 0, KEYEVENTF_KEYUP, 0);
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
				default:
					return (byte)key;
			}
		}
		#endregion
	}
}