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

        static void Main(string[] args)
        {
            Console.Title = "Dangerous Panel Server";
            Log("Dangerous Panel Server (by Mitchfizz05)", ConsoleColor.Cyan);
            Log("Debug mode active.", ConsoleColor.Green, true);
            Console.WriteLine();

            // Generate access token
            //Token = GenerateToken();
            //Log("Access Token: " + Token, ConsoleColor.Yellow);

            Log("Starting websocket server...");
            WebSocketServer wsServer = new WebSocketServer(7751);
            wsServer.AddWebSocketService<PanelWebsocketService>("/panel");
            wsServer.Start();
            Log("Websocket server running!");

            Console.ReadKey();
        }
    }
}
