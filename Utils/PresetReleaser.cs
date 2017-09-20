using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Rou.Utils
{
    class PresetReleaser
    {
        public const string DEFAULT_DEST = "./configs";
        public const string ASSEMBLY_NAMESPACE = "Rou.Preset.";

        public string GetResourceTextFile(string filename)
        {
            string result = string.Empty;

            using (Stream stream = this.GetType().Assembly.
                       GetManifestResourceStream(ASSEMBLY_NAMESPACE + filename))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }

        public void Release(string filename, string dest = DEFAULT_DEST)
        {
            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);
            string data = GetResourceTextFile(filename);
            File.WriteAllText(Path.Combine(dest, filename), data);
        }

        public bool RelaseIfNotExists(string filename, string dest = DEFAULT_DEST)
        {
            bool exists = File.Exists(Path.Combine(dest, filename));
            if (!exists)
                Release(filename, dest);
            return exists;
        }

        public void ReleaseAll(string dest = DEFAULT_DEST)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var names = executingAssembly.GetManifestResourceNames();
            foreach (var name in names)
                if (name.StartsWith(ASSEMBLY_NAMESPACE))
                    Release(name.Remove(0, ASSEMBLY_NAMESPACE.Length));
        }
    }
}
