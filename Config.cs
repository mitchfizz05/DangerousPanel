using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace DangerousPanel_Server
{
    public class Config
    {
        protected string configPath;

        protected IDictionary<string, object> config;

        /// <summary>
        /// Load configuration from file
        /// </summary>
        public void LoadConfig()
        {
            string rawJson = File.ReadAllText(configPath);
            config = JsonConvert.DeserializeObject<Dictionary<string, object>>(rawJson);
        }

        /// <summary>
        /// Save configuration to file
        /// </summary>
        public void SaveConfig()
        {
            string rawJson = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(configPath, rawJson);
        }

        /// <summary>
        /// Get a configuration value
        /// </summary>
        /// <param name="key">Config key</param>
        /// <returns></returns>
        public object ReadConfig(string key, object defaultValue = null)
        {
            if (config[key] != null)
                return config[key];
            else
                return defaultValue;
        }

        /// <summary>
        /// Write a value to the configuration file
        /// </summary>
        /// <param name="key">Config key</param>
        /// <param name="value">Config value</param>
        /// <param name="autosave">Should the config be saved to disk after the change?</param>
        public void WriteConfig(string key, object value, bool autosave = true)
        {
            config[key] = value;
            if (autosave)
                SaveConfig();
        }

        /// <summary>
        /// Create a new configuration object.
        /// </summary>
        /// <param name="configPath">Path to JSON configuration file</param>
        /// <param name="autoload">Should the configuration file be automatically loaded?</param>
        public Config(string configPath, bool autoload = true)
        {
            this.configPath = configPath;
            if (autoload)
                LoadConfig();
        }

        /// <summary>
        /// Generate a configuration file with the specified values.
        /// </summary>
        /// <param name="configPath"></param>
        /// <param name="configValues"></param>
        public static void GenerateConfigFile(string configPath, IDictionary<string, object> configValues)
        {
            if (!File.Exists(configPath))
            {
                // Check if parent directory exists
                if (!Directory.Exists(Path.GetDirectoryName(configPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(configPath));

                // Serialise dictonary and save
                string rawJson = JsonConvert.SerializeObject(configValues, Formatting.Indented);
                File.WriteAllText(configPath, rawJson);
            }
        }
    }
}
