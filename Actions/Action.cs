using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MaterialIcons;

namespace Rou
{
    public delegate void ActionChanged(Action sender);

    public abstract class Action
    {
        private string _text;
        private MaterialIconType _icon;

        public string Text
        {
            get
            {
                return _text;
            }
            protected set
            {
                _text = value;
                onActionChanged?.Invoke(this);
            }
        }
        public MaterialIconType IconType
        {
            get
            {
                return _icon;
            }
            protected set
            {
                _icon = value;
                onActionChanged?.Invoke(this);
            }
        }

        public MaterialIcon Icon
        {
            get
            {
                var icon = new MaterialIcon();
                icon.Icon = _icon;
                return icon;
            }
        }

        public Action(string text, MaterialIconType icon)
        {
            _text = text;
            _icon = icon;
        }

        public abstract bool Click();
        public event ActionChanged onActionChanged;
    }
}
