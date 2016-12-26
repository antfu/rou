using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MaterialIcons;
using Rou.Utils;

namespace Rou.Actions
{
    public class NextTrackAction : KeyPressAction
    {
        public NextTrackAction() : base("Next Track", MaterialIconType.ic_skip_next, Keys.MediaNextTrack)
        { }
    }

    public class PrevTrackAction : KeyPressAction
    {
        public PrevTrackAction() : base("Prev Track", MaterialIconType.ic_skip_previous, Keys.MediaPreviousTrack)
        { }
    }

    public class PauseTrackAction : KeyPressAction
    {
        public PauseTrackAction() : base("Pause", MaterialIconType.ic_pause, Keys.MediaPlayPause)
        { }
    }

    public class WinAction : KeyPressAction
    {
        public WinAction() : base("Win", MaterialIconType.ic_menu, Keys.LWin)
        { }
    }
}
