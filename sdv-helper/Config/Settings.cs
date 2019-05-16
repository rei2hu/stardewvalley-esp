﻿using Newtonsoft.Json;
using StardewModdingAPI;
using System.Collections.Generic;
using System.IO;

namespace sdv_helper.Config
{
    class Settings
    {
        private static readonly string defaultContent = "{}";
        public Dictionary<string, int> DSettings { get; } = new Dictionary<string, int>();
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
            DSettings.Clear();
            foreach (var entry in json)
            {
                DSettings.Add(entry.Name, entry.Value.ToObject<int>());
            }
        }

        public void SaveSettings()
        {
            string text = JsonConvert.SerializeObject(DSettings);
            File.WriteAllText(path, text);
        }

        public void SetDefaultsFor(string name)
        {
            DSettings.Add(name, 19);
            SaveSettings();
        }

        public void SetColorFor(string name, int color)
        {
            DSettings[name] = color;
            SaveSettings();
        }

        public int GetColorFor(string name)
        {
            if (!DSettings.ContainsKey(name)) SetDefaultsFor(name);
            return DSettings[name];
        }
    }
}
