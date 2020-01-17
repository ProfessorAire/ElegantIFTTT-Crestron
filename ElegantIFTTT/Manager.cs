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
    /// <summary>
    /// Provides the core module for communication to/from the IFTTT service.
    /// Multiple can be used, but require unique Port numbers and IDs.
    /// </summary>
    public class Manager : IDisposable
    {
        /// <summary>
        /// A list of Manager objects. Used to provide access to all managers, as well as ensure no duplicates are created.
        /// </summary>
        private static List<Manager> managers = new List<Manager>();

        /// <summary>
        /// A list of modules that are registered to this manager.
        /// </summary>
        private List<ModuleBase> modules = new List<ModuleBase>();

        /// <summary>
        /// The server object, used to listen to the IFTTT service.
        /// </summary>
        private Crestron.SimplSharp.Net.Http.HttpServer server;

        /// <summary>
        /// The client object, used to send messages to the IFTTT service.
        /// </summary>
        private Crestron.SimplSharp.Net.Https.HttpsClient client;

        /// <summary>
        /// The port to listen on the local network for messages.
        /// External communication from the IFTTT service should be forwarded to this port for all messages intended for this module.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The unique ID of the manager. Used by modules to register with this manager.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The private key value used to provide communication to the IFTTT service.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The separator used to break-up incoming data sets from the IFTTT service.
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// Used to provide bulk event data back to Simpl+. Only called if no modules are registered for a specific event.
        /// </summary>
        public delegate void GenericEvent(SimplSharpString eventName, SimplSharpString Data1, SimplSharpString Data2,
            SimplSharpString Data3, SimplSharpString Data4, SimplSharpString Data5, SimplSharpString Data6,
            SimplSharpString Data7, SimplSharpString Data8, SimplSharpString Data9, SimplSharpString Data10);
        public GenericEvent ProcessGenericEvent { get; set; }

        public event EventHandler<EventDataReceivedEventArgs> GenericEventReceived;

        /// <summary>
        /// Provided for Simpl+ compatibility.
        /// </summary>
        [Obsolete("Provided for Simpl+ compatibility.")]
        public Manager() { }

        /// <summary>
        /// Creates a new instance of the Manager, registering it in the process.
        /// </summary>
        /// <param name="managerID">The unique ID for the manager.</param>
        /// <param name="port">The port to listen on for IFTTT communication.</param>
        /// <param name="key">The IFTTT key to use for communicating to the service.</param>
        /// <param name="separator">The separator used to break-up incoming data sets from the IFTTT service.</param>
        public Manager(string managerID, int port, string key, string separator)
        {
            Register(managerID, port, key, separator);
        }

        /// <summary>
        /// Returns a boolean if the manager with the given ID is registered and ready.
        /// </summary>
        public static bool IsManagerRegistered(string id)
        {
            return (managers.Where((m) => m.ID == id).Count() > 0);
        }

        /// <summary>
        /// Registers this manager object. Will fail if a manager with the same ID already exists.
        /// </summary>
        /// <param name="managerID">The unique ID for the manager.</param>
        /// <param name="port">The port to listen on for IFTTT communication.</param>
        /// <param name="key">The IFTTT key to use for communicating to the service.</param>
        /// <param name="separator">The separator used to break-up incoming data sets from the IFTTT service.</param>
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

        /// <summary>
        /// Used by modules to register with a manager, so as to be placed in its list of modules.
        /// </summary>
        /// <param name="module">The module being registered.</param>
        /// <param name="managerID">The ID of the manager to register with.</param>
        internal static void RegisterModule(ModuleBase module, string managerID)
        {
            try
            {
                var manager = managers.Where((m) => m.ID == managerID).First();
                if (manager != null)
                {
                    manager.modules.Add(module);
                }
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("Unable to register a module with the manager: " + managerID);
                CrestronConsole.PrintLine(ex.Message);
            }
        }

        /// <summary>
        /// Call to open the server and start listening for communication from IFTTT.
        /// </summary>
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
                client.HostVerification = false;
                client.PeerVerification = false;
                client.KeepAlive = false;
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("IFTTT: Error occurred while creating client.");
                CrestronConsole.PrintLine("IFTTT: " + ex.Message);
            }
        }

        /// <summary>
        /// Call to close the server and stop listening for communication from IFTTT.
        /// </summary>
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

        public void SendGenericCommand(string commandName, params string[] values)
        {
            try
            {
                if (client == null)
                {
                    return;
                }
                var address = new StringBuilder();
                address.AppendFormat("https://maker.ifttt.com/trigger/{0}/with/key/{1}", commandName, Key);
                if (values != null && values.Length > 0)
                {
                    address.Append("?");
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i > 0)
                        {
                            address.Append("&");
                        }
                        address.AppendFormat("value{0}={1}", i + 1, values[i] ?? "");
                    }
                }

                var clientRequest = new HttpsClientRequest();
                clientRequest.RequestType = Crestron.SimplSharp.Net.Https.RequestType.Post;
                clientRequest.Url.Parse(address.ToString());

                var response = client.Dispatch(clientRequest);
                if (response != null)
                {
                    CrestronConsole.PrintLine("IFTTT: " + response.ContentString);
                }
            }
            catch (HttpsException ex)
            {
                CrestronConsole.PrintLine("IFTTT: HTTPS Exception encoutered when sending data to IFTTT webhooks serice.");
                CrestronConsole.PrintLine("IFTTT: " + ex.Message);
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("IFTTT: Error encountered when sending data to IFTTT webhooks service.");
                CrestronConsole.PrintLine("IFTTT: " + ex.Message);
            }
        }

        /// <summary>
        /// Used to send a command to the IFTTT service.
        /// </summary>
        /// <param name="name">The name of the command/event.</param>
        /// <param name="value1">The first value to provide IFTTT. Can be an empty string.</param>
        /// <param name="value2">The second value to provide IFTTT. Can be an empty string.</param>
        /// <param name="value3">The third value to provide IFTTT. Can be an empty string.</param>
        public void SendGenericCommand(string name, string value1, string value2, string value3)
        {
            try
            {
                if (client == null)
                {
                    CrestronConsole.PrintLine("IFTTT: Communication not enabled. Can't send message to service.");
                    return;
                }
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
                if (response != null)
                {
                    CrestronConsole.PrintLine("IFTTT: " + response.ContentString);
                }

            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("IFTTT: Error encountered when sending data to IFTTT webhooks service.");
                CrestronConsole.PrintLine("IFTTT: " + ex.Message);
            }
        }

        /// <summary>
        /// Handles data received from IFTTT. Passes to the correct module, or provides the data back to Simpl+ if no matching modules exist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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
                    if (ProcessGenericEvent != null)
                    {
                        ProcessGenericEvent(eventData.Name, eventData.Data1, eventData.Data2,
                            eventData.Data3, eventData.Data4, eventData.Data5, eventData.Data6, eventData.Data7,
                            eventData.Data8, eventData.Data9, eventData.Data10);
                    }
                    if (GenericEventReceived != null)
                    {
                        GenericEventReceived(this, new EventDataReceivedEventArgs(eventData));
                    }
                }
                eventData = null;
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("IFTTT: Error occured while processing event from IFTTT.");
                CrestronConsole.PrintLine("Message: " + ex.Message);
            }

        }

        /// <summary>
        /// Call to dispose the class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            CrestronEnvironment.GC.SuppressFinalize(true);
        }

        /// <summary>
        /// Calls clean up code.
        /// </summary>
        public virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                CloseServer();
            }
        }
    }
}
