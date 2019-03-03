using System;
using System.Text;
using Crestron.SimplSharp;
using System.Collections.Generic;
using Crestron.SimplSharp.Net.Http;    // For Basic SIMPL# Classes
using System.Collections;
using System.Linq;
using Crestron.SimplSharp.Net.Https;

namespace ElegantIFTTT
{
    public class Manager : IDisposable
    {
        private static List<Manager> managers = new List<Manager>();

        private List<ModuleBase> modules = new List<ModuleBase>();

        private Crestron.SimplSharp.Net.Http.HttpServer server;
        private Crestron.SimplSharp.Net.Https.HttpsClient client;

        public int Port { get; set; }
        public string ID { get; set; }
        public string Key { get; set; }
        public string Separator { get; set; }

        public delegate void GenericEvent(SimplSharpString eventName, SimplSharpString Data1, SimplSharpString Data2,
            SimplSharpString Data3, SimplSharpString Data4, SimplSharpString Data5, SimplSharpString Data6,
            SimplSharpString Data7, SimplSharpString Data8, SimplSharpString Data9, SimplSharpString Data10);
        public GenericEvent ProcessGenericEvent { get; set; }

        /// <summary>
        /// SIMPL+ can only execute the default constructor. If you have variables that require initialization, please
        /// use an Initialize method
        /// </summary>
        public Manager()
        {
        }

        public static bool IsManagerRegistered(string id)
        {
            return (managers.Where((m) => m.ID == id).Count() > 0);
        }

        public void Register(string managerID, int port, string key, string separator)
        {
            ID = managerID;
            Port = port;
            Key = key;
            Separator = separator;
            if (managers.Where((m) => m.ID == ID).Count() == 0)
            {
                managers.Add(this);
                CrestronConsole.PrintLine("Manager: " + managerID + " registered.");
            }

        }

        public static void RegisterModule(ModuleBase module, string managerID)
        {
            using (var secure = new CCriticalSection())
            {
                var manager = managers.Where((m) => m.ID == managerID).First();
                if (manager != null)
                {
                    manager.modules.Add(module);
                }
            }
        }

        public void OpenServer()
        {
            try
            {
                server = new Crestron.SimplSharp.Net.Http.HttpServer(EthernetAdapterType.EthernetUnknownAdapter);
                server.Port = Port;
                CrestronConsole.PrintLine("IFTTT: Port #: " + Port);
                server.ServerName = "IFTTT Server";
                server.OnHttpRequest += OnServerRequest;
                server.Open();
                CrestronConsole.PrintLine("IFTTT: Server started on port " + Port);
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("IFTTT: Error occurred while starting server.");
                CrestronConsole.PrintLine("IFTTT: " + ex.Message);
            }
            try
            {
                client = new HttpsClient();
                client.KeepAlive = false;
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("IFTTT: Error occurred while creating client.");
                CrestronConsole.PrintLine("IFTTT: " + ex.Message);
            }
        }

        public void CloseServer()
        {
            if (server != null)
            {
                server.Close();
                server.OnHttpRequest -= OnServerRequest;
                server.Dispose();
                server = null;
                CrestronConsole.PrintLine("IFTTT: Disposed of the server.");
            }
            if (client != null)
            {
                client.Dispose();
                client = null;
            }
        }

        public void SendGenericCommand(string name, string value1, string value2, string value3)
        {
            try
            {
                string address = "https://maker.ifttt.com/trigger/" + name + "/with/key/" + Key;
                if (value1 != "" || value2 != "" || value3 != "")
                {
                    address += "?";
                }
                if (value1 != "")
                {
                    address += "value1=" + value1;
                    if (value2 != "" || value3 != "")
                    {
                        address += "&";
                    }
                }
                if (value2 != "")
                {
                    address += "value2=" + value2;
                    if (value3 != "")
                    {
                        address += "&";
                    }
                }
                if (value3 != "")
                {
                    address += "value3=" + value3;
                }

                var clientRequest = new HttpsClientRequest();
                clientRequest.RequestType = Crestron.SimplSharp.Net.Https.RequestType.Post;
                clientRequest.Url.Parse(address);

                var response = client.Dispatch(clientRequest);

                CrestronConsole.PrintLine("IFTTT: " + response.ContentString);
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("IFTTT: Error encountered when sending data to IFTTT webhooks service.");
                CrestronConsole.PrintLine("IFTTT: " + ex.Message);
            }
        }

        private void OnServerRequest(object sender, OnHttpRequestArgs args)
        {
            CrestronConsole.PrintLine("IFTTT: Received incoming message.");
            CrestronConsole.PrintLine(args.Request.ContentString);
            try
            {
                var eventData = EventData.Parse(args.Request.ContentString.Split(Separator.ToCharArray()));

                var mods = modules.Where((m) => m.IsEnabled && m.EventName == eventData.Name);
                if (mods != null && mods.Count() > 0)
                {
                    foreach (var m in mods)
                    {
                        if (m != null)
                        {
                            m.ProcessEventData(eventData);
                        }
                    }
                }
                else
                {
                    CrestronConsole.PrintLine("IFTTT: Trying to process generic event.");
                    ProcessGenericEvent(eventData.Name, eventData.Data1, eventData.Data2,
                        eventData.Data3, eventData.Data4, eventData.Data5, eventData.Data6, eventData.Data7,
                        eventData.Data8, eventData.Data9, eventData.Data10);
                }
                eventData = null;
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("IFTTT: Error occured while processing event from IFTTT.");
                CrestronConsole.PrintLine("Message: " + ex.Message);
            }

        }

        public void Dispose()
        {
            Dispose(true);
            CrestronEnvironment.GC.SuppressFinalize(true);
        }

        public virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (server != null)
                {
                    CloseServer();
                }
            }
        }
    }
}
