using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MaterialIcons;
using Rou.Actions;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Rou.Utils
{
    class JsonLoader
    {
        public string Path { get; private set; }
        public List<Action> Actions { get; private set; }
        public Configuration Configs { get; private set; }

        public JsonLoader(string path)
        {
            Path = path;
        }

        #region Static Methods
        public static dynamic ParseFile(String path)
        {
            return JObject.Parse(File.ReadAllText(path));
        }
        private static Keys StringToKey(string str)
        {
            return (Keys)Enum.Parse(typeof(Keys), str, true);
        }
        private static MaterialIconType StringToIcon(string str)
        {
            return (MaterialIconType)Enum.Parse(typeof(MaterialIconType), str, true);
        }
        #endregion

        public void Load()
        {
            dynamic obj = ParseFile(Path);

            // Load configs
            Configs = new Configuration();
            if (obj.configs != null && obj.configs.hotkeys != null)
                Configs.Hotkey = StringToKey(obj.configs.hotkeys.ToString());

            // Load actions
            Actions = new List<Action>();
            foreach (dynamic objAction in obj.actions)
            {
                Action action = null;
                try
                {
                    if (objAction.type == "keyboard")
                    {

                        string text = objAction.text.ToString();
                        string icon_text = objAction.icon.ToString();
                        MaterialIconType icon = StringToIcon(icon_text);
                        dynamic keys = objAction.keys;

                        if (keys is JArray)
                        {
                            var keyactions = new List<KeyAction>();
                            foreach (dynamic keyaction in keys)
                            {
                                var p = keyaction.ToString().Split(' ');
                                var key = StringToKey(p[1]);
                                KeyOperation op = KeyOperation.Press;
                                if (p[0] == "P")
                                    op = KeyOperation.Press;
                                else if (p[0] == "D")
                                    op = KeyOperation.Down;
                                else if (p[0] == "U")
                                    op = KeyOperation.Up;
                                keyactions.Add(new KeyAction(key, op));
                            }
                            action = new KeyboardAction(text, icon, keyactions);
                        }
                        else
                        {
                            action = new KeyboardAction(text, icon, StringToKey(keys.ToString()));
                        }

                    }
                    else
                    {
                        //TODO
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error parsing json: " + e.Message);
                    action = null;
                }

                if (action != null)
                    Actions.Add(action);
            }
        }
    }
}
