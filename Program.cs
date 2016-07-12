using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using DangerousPanel_Server.PanelServer;
using WebSocketSharp.Server;

namespace DangerousPanel_Server
{
    class Program
    {
        public static bool Debug = true;

        public static string Token = null;

        public static Config config;

        public static KeyBindingHelper keybindingHelper;

        public static void Log(string text, ConsoleColor color = ConsoleColor.White, bool debug = false)
        {
            if (Debug || !debug)
            {
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(text);
                Console.ForegroundColor = oldColor;
            }
        }

        /// <summary>
        /// Generate a random string.
        /// </summary>
        /// <returns>Cryptographically secure random string</returns>
        public static string GenerateToken()
        {
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenBytes = new byte[5];
                rng.GetBytes(tokenBytes);
                return BitConverter.ToString(tokenBytes);
            }
        }

        public static void LoadConfig()
        {
            // Configuration
            string configPath = Environment.GetEnvironmentVariable("appdata") + "\\edpanel\\config.json";
            Config.GenerateConfigFile(configPath, new Dictionary<string, object>()
            {
                {"debug", false },
                {"useAccessTokens", true },
                {"cmdrName", "Steve" },
                { "keybindsFile", "%appdata%\\edpanel\\keybinds.json" }
            });
            config = new Config(configPath);
        }

        static void Main(string[] args)
        {
            // Load config
            LoadConfig();

            Debug = (bool)config.ReadConfig("debug", false);

            Console.Title = "Dangerous Panel Server";
            Log("Dangerous Panel Server (by Mitchfizz05)", ConsoleColor.Cyan);
            Log("Debug mode active.", ConsoleColor.Green, true);
            Console.WriteLine();

            Log("Welcome back " + config.ReadConfig("cmdrName"));

            if ((bool)config.ReadConfig("useAccessTokens"))
            {
                // Generate access token
                Token = GenerateToken();
                Log("Access Token: " + Token, ConsoleColor.Yellow);
            }
            else
            {
                // Access tokens disabled
                Log("WARNING!! Access tokens are disabled! This means anyone who can connect to the server may be able to take control of your computer!\nChange in %appdata%\\edpanel\\config.json", ConsoleColor.Red);

            }
            
            Log("Loading keybinds...");
            keybindingHelper = new KeyBindingHelper(Environment.ExpandEnvironmentVariables((string)config.ReadConfig("keybindsFile")));
            keybindingHelper.LoadKeybinds();

            Log("Starting websocket server...");
            WebSocketServer wsServer = new WebSocketServer(7751);
            wsServer.AddWebSocketService<PanelWebsocketService>("/panel");
            wsServer.Start();
            Log("Websocket server running!");

            Console.ReadKey();
        }
    }
}
