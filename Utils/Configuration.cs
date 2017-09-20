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
    class Configuration
    {
        public string FolderDir { get; private set; }
        public Keys Hotkey { get; set; }
        public ActionsCollection DefaultActions { get; private set; }

        private Dictionary<string, ActionsCollection> _actionCache;

        public Configuration(string path)
        {
            FolderDir = path;
            _actionCache = new Dictionary<string, ActionsCollection>();
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

        public bool HasApp(String appname) {
            var path = Path.Combine(FolderDir, appname + ".json");
            return _actionCache.ContainsKey(appname) || File.Exists(path);
        }

        public ActionsCollection LoadActionsForApp(string appname) {
            if (_actionCache.ContainsKey(appname))
                return _actionCache[appname];
            else
            {
                var path = Path.Combine(FolderDir, appname + ".json");
                if (File.Exists(path))
                {
                    dynamic obj = ParseFile(path);
                    var actions = LoadActions(obj.actions);
                    foreach (Action action in actions)
                        action.Active = true;
                    _actionCache.Add(appname, actions);
                    return actions;
                }
                else {
                    return DefaultActions;
                }
            }
        }

        private static ActionsCollection LoadActions(dynamic actionobj)
        {
            var actions = new ActionsCollection();
            foreach (dynamic objAction in actionobj)
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
                    actions.Add(action);
            }

            return actions;
        }

        public void Load()
        {
            var path = Path.Combine(FolderDir, "default.json");
            dynamic obj = ParseFile(path);

            // Load configs
            if (obj.configs != null && obj.configs.hotkeys != null)
                Hotkey = StringToKey(obj.configs.hotkeys.ToString());

            // Load actions
            DefaultActions = LoadActions(obj.actions);
        }

        public void Save(ActionsCollection actions, String filename) {

        }
    }
}
