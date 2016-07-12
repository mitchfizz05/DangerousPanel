using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;

namespace DangerousPanel_Server.PanelServer
{
    public class PanelWebsocketService : WebSocketBehavior
    {
        public bool Authenticated = false; // has the user been authenticated yet?

        public PanelWebsocketService()
        {
        }

        /// <summary>
        /// Structure for metadata sent to client so it can update it's UI.
        /// </summary>
        struct PanelMetadata
        {
            public string CmdrName;
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
        /// Handles "action:" requests.
        /// Similar to key requests except the actual keycode is loaded from a file based on the action name passed.
        /// </summary>
        /// <param name="data"></param>
        public void OnActionRequest(string data)
        {
            string action = data.Substring("action:".Length);
            Program.Log("Action request: " + action, debug: true);

            string keycode = Program.keybindingHelper.GetKeybind(action);
            if (keycode != null)
            {
                try
                {
                    KeySender.SendKey(ScancodeConvert.GetScancode(keycode));
                }
                catch
                {
                    Program.Log("Failed to send key: " + keycode, ConsoleColor.Red);
                }
            }
            else
            {
                Program.Log("Attempt to trigger action that doesn't have an associated keybind! (" + action + ")", ConsoleColor.Red);
            }
        }

        /// <summary>
        /// Handles request from the client to send out metadata.
        /// </summary>
        /// <param name="data"></param>
        public void OnMetaRequest(string data)
        {
            PanelMetadata meta = new PanelMetadata()
            {
                CmdrName = (string)Program.config.ReadConfig("cmdrName", "Commander")
            };

            // Serialise and send the panel metadata
            string rawJson = JsonConvert.SerializeObject(meta);
            Send("meta:" + rawJson);
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
                else if (e.Data.StartsWith("action:"))
                {
                    OnActionRequest(e.Data);
                }
                else if (e.Data == "fetchmeta")
                {
                    OnMetaRequest(e.Data);
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
