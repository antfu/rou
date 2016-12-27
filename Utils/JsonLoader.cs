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
    static class JsonLoader
    {
        public static dynamic ParseFile(String path)
        {
            return JObject.Parse(File.ReadAllText(path));
        }

        public static List<Action> LoadAction(String path = "default.json")
        {
            dynamic obj = ParseFile(path);
            var objActions = obj.actions;
            var actions = new List<Action>();

            foreach (dynamic objAction in objActions)
            {
                Action action = null;
                try
                {
                    if (objAction.type == "keyboard")
                    {

                        string text = objAction.text.ToString();
                        string icon_text = objAction.icon.ToString();
                        MaterialIconType icon = (MaterialIconType)Enum.Parse(typeof(MaterialIconType), icon_text, true);
                        dynamic keys = objAction.keys;

                        if (keys is JArray)
                        {
                            var keyactions = new List<KeyAction>();
                            foreach (dynamic keyaction in keys)
                            {
                                var p = keyaction.ToString().Split(' ');
                                var key = (Keys)Enum.Parse(typeof(Keys), p[1], true);
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
                            action = new KeyboardAction(text, icon, (Keys)Enum.Parse(typeof(Keys), keys.ToString(), true));
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
    }
}
