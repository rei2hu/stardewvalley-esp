using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdv_helper.Settings
{
    class Settings
    {
        private static readonly string defaultContent = "{}";
        private readonly Dictionary<string, int[]> settings = new Dictionary<string, int[]>();
        private readonly IModHelper helper;
        private string path;
        public Settings(IModHelper helper)
        {
            this.helper = helper;
            LoadSettings();
        }

        public void LoadSettings()
        {
            path = Path.Combine(helper.DirectoryPath, "settings.json");
            if (!File.Exists(path))
            {
                File.WriteAllText(path, defaultContent);
            }
            string text = File.ReadAllText(path);
            dynamic json = JsonConvert.DeserializeObject(text);
            settings.Clear();
            foreach (var entry in json)
            {
                settings.Add(entry.Name, entry.Value.ToObject<int[]>());
            }
        }

        public void SaveSettings()
        {
            string text = JsonConvert.SerializeObject(settings);
            File.WriteAllText(path, text);
        }

        public void SetDefaultsFor(string name, Color color)
        {
            settings.Add(name, new int[] { 1, 255, 255, 255, 255 });
            SetColorFor(name, color);
        }

        public void SetColorFor(string name, Color color)
        {
            settings[name][1] = color.R;
            settings[name][2] = color.G;
            settings[name][3] = color.B;
            settings[name][4] = color.A;
            SaveSettings();
        }

        public void SetEnabledFor(string name, bool enable)
        {
            settings[name][0] = enable ? 1 : 0;
            SaveSettings();
        }

        public Color GetColorFor(string name, Color color)
        {
            if (!settings.ContainsKey(name)) SetDefaultsFor(name, color);
            return Color.FromNonPremultiplied(settings[name][1], settings[name][2], settings[name][3], settings[name][4]);
        }

        public bool IsEnabled(string name, Color color)
        {
            if (!settings.ContainsKey(name)) SetDefaultsFor(name, color);
            return settings[name][0] > 0;
        }
    }
}
