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
        dynamic wscript;

        public PanelWebsocketService()
        {
            Type wscriptType = Type.GetTypeFromProgID("WScript.Shell");
            wscript = System.Activator.CreateInstance(wscriptType);
        }

        /// <summary>
        /// Handles "key:" requests
        /// </summary>
        /// <param name="data">Message received from client</param>
        public void OnKeyRequest(string data)
        {
            string key = data.Substring("key:".Length, 1);
            Program.Log("Key request: " + key, debug: true);
            wscript.SendKeys(key);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsText)
            {
                if (e.Data.StartsWith("key:"))
                {
                    OnKeyRequest(e.Data);
                }
            }

            base.OnMessage(e);
        }
    }
}
