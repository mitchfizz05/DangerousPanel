using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DangerousPanel_Server
{
    class Program
    {
        public static bool Debug = true;

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

        static void Main(string[] args)
        {
            Console.Title = "Dangerous Panel Server";
            Log("Dangerous Panel Server (by Mitchfizz05)", ConsoleColor.Cyan);
            Log("Debug mode active.", ConsoleColor.Green, true);

            Console.ReadKey();
        }
    }
}
