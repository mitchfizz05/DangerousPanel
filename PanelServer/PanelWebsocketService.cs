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
        public bool Authenticated = false; // has the user been authenticated yet?

        public PanelWebsocketService()
        {
        }

        /// <summary>
        /// Handles "key:" requests
        /// </summary>
        /// <param name="data">Message received from client</param>
        public void OnKeyRequest(string data)
        {
            string key = data.Substring("key:".Length);
            Program.Log("Key request: " + key, debug: true);
            
            try
            {
                KeySender.SendKey(ScancodeConvert.GetScancode(key));
            }
            catch
            {
                Program.Log("Failed to send key: " + key, ConsoleColor.Red);
            }
        }

        /// <summary>
        /// Handles authentication requests.
        /// </summary>
        /// <param name="data">Message received from client</param>
        public void OnAuthRequest(string data)
        {
            if (data.Replace("-", "").ToLower() == Program.Token.Replace("-", "").ToLower())
            {
                Authenticated = true;
                Send("authenticated");
                Program.Log("Client authenticated: " + Context.UserEndPoint.Address, ConsoleColor.Green);
            }
            else
            {
                Authenticated = false;
                Send("authfail");
                Program.Log("Authentication failure (incorrect token)", ConsoleColor.Red);
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Program.Log("Received message: " + e.Data, ConsoleColor.Gray, true);

            if (e.IsText)
            {
                if (!Authenticated)
                {
                    // Not authenticated yet - receiving authcode.
                    OnAuthRequest(e.Data);
                }
                else if (e.Data.StartsWith("key:"))
                {
                    OnKeyRequest(e.Data);
                }
            }

            base.OnMessage(e);
        }

        protected override void OnOpen()
        {
            Program.Log("Connection from " + Context.UserEndPoint.Address, ConsoleColor.Green);

            if (Program.Token != null)
            {
                // We have an access token, ask client to authenticate.
                Send("authenticate");
            }
            else
            {
                // No access token - automatically authenticated
                Authenticated = true;
                Send("authenticated");
                Program.Log("Automatically authenticated " + Context.UserEndPoint.Address + "!", ConsoleColor.Green);
            }

            base.OnOpen();
        }
    }
}
