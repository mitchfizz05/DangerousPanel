using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace DangerousPanel_Server
{
    class KeyBindingHelper
    {
        protected string keybindingsFilePath;

        // Default keybindings to write to file if they're missing.
        protected Dictionary<string, string> defaultKeybindings = new Dictionary<string, string>
        {
            { "Landing_Gear", "INSERT" },
            { "Hardpoints", "U" },
            { "Cargo_Scoop", "HOME" },
            { "Lights", "L" },
            { "Silent_Running", "K" },
            { "Flight_Assist", "O" }
        };

        // User configured keybindings.
        protected IDictionary<string, string> keybindings;

        /// <summary>
        /// Load keybinds from file
        /// </summary>
        /// <param name="generateFile">If the keybinds file doesn't exist, should we just generate it with default values?</param>
        public void LoadKeybinds(bool generateFile = true)
        {
            // Check if the keybinds file should be generated.
            if (!File.Exists(keybindingsFilePath))
            {
                if (!generateFile)
                    throw new FileNotFoundException("Keybinding file doesn't exist!");

                keybindings = new Dictionary<string, string>();
            }
            else
            {
                // Keybinds file exists - read file.
                string rawJson = File.ReadAllText(keybindingsFilePath);
                keybindings = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawJson);
            }

            // Bring in any additional default values that have not been set in the file.
            bool modified = false;
            foreach (KeyValuePair<string, string> keybind in defaultKeybindings)
            {
                if (!keybindings.ContainsKey(keybind.Key))
                {
                    // Missing key. Add it in with default value.
                    keybindings[keybind.Key] = keybind.Value;
                    modified = true;
                }
            }
            
            // If we had to add in new values, save the keybinds to disk.
            if (modified)
                SaveKeybinds();
        }

        /// <summary>
        /// Save keybinds to file
        /// </summary>
        public void SaveKeybinds()
        {
            string rawJson = JsonConvert.SerializeObject(keybindings, Formatting.Indented);
            File.WriteAllText(keybindingsFilePath, rawJson);
        }

        /// <summary>
        /// Get a keybind for an action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public string GetKeybind(string action, string defaultValue = null)
        {
            if (keybindings[action] != null)
                return keybindings[action];
            else
                return defaultValue;
        }

        /// <summary>
        /// Update a keybind.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="key"></param>
        /// <param name="autosave">Should the keybinds be saved to disk after the change?</param>
        public void WriteKeybind(string action, string key, bool autosave = true)
        {
            keybindings[action] = key;
            if (autosave)
                SaveKeybinds();
        }
        
        /// <param name="configPath">Path to JSON configuration file</param>
        /// <param name="autoload">Should the configuration file be automatically loaded?</param>
        public KeyBindingHelper(string configPath, bool autoload = true)
        {
            this.keybindingsFilePath = configPath;
            if (autoload)
                LoadKeybinds();
        }
    }
}
