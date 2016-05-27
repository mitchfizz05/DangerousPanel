using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace DangerousPanel_Server.PanelServer
{
    public class PanelWebsocketService : WebSocketBehavior
    {
        dynamic wscript; // https://msdn.microsoft.com/en-us/library/8c6yea83(v=vs.84).aspx

        public PanelWebsocketService()
        {
            Program.Log("Initialising WScript COM object...", debug: true);
            Type wscriptType = Type.GetTypeFromProgID("WScript.Shell");
            wscript = System.Activator.CreateInstance(wscriptType);
            Program.Log("WScript COM objecet initalised!", debug: true);
        }

        /// <summary>
        /// Handles "key:" requests
        /// </summary>
        /// <param name="data">Message received from client</param>
        public void OnKeyRequest(string data)
        {
            string key = data.Substring("key:".Length);
            Program.Log("Key request: " + key, debug: true);
            wscript.SendKeys(key);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Program.Log("Received message: " + e.Data, ConsoleColor.Gray, true);

            if (e.IsText)
            {
                if (e.Data.StartsWith("key:"))
                {
                    OnKeyRequest(e.Data);
                }
            }

            base.OnMessage(e);
        }

        protected override void OnOpen()
        {
            Program.Log("Connected with " + Context.UserEndPoint.Address, ConsoleColor.Green);

            base.OnOpen();
        }
    }
}
