using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;
using VideoOS.Platform;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;

namespace ControllerMonitor.Background
{
    /// <summary>
    /// A background plugin will be started during application start and be running until the user logs off or application terminates.<br/>
    /// The Environment will call the methods Init() and Close() when the user login and logout, 
    /// so the background task can flush any cached information.<br/>
    /// The base class implementation of the LoadProperties can get a set of configuration, 
    /// e.g. the configuration saved by the Options Dialog in the Smart Client or a configuration set saved in one of the administrators.  
    /// Identification of which configuration to get is done via the GUID.<br/>
    /// The SaveProperties method can be used if updating of configuration is relevant.
    /// <br/>
    /// The configuration is stored on the server the application is logged into, and should be refreshed when the ApplicationLoggedOn method is called.
    /// Configuration can be user private or shared with all users.<br/>
    /// <br/>
    /// This plugin could be listening to the Message with MessageId == Server.ConfigurationChangedIndication to when when to reload its configuration.  
    /// This event is send by the environment within 60 second after the administrator has changed the configuration.
    /// </summary>
    public class ControllerMonitorBackgroundPlugin : BackgroundPlugin
    {
        private bool _stop = false;
        private Thread _thread;
        private object _controllerConfigChangedObject;

        AutoResetEvent _stopEvent = new AutoResetEvent(false);
        private bool _stopThread = false;
        private Item _connectedController;
        private TcpClient _client;

        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get { return ControllerMonitorDefinition.ControllerMonitorBackgroundPlugin; }
        }

        /// <summary>
        /// The name of this background plugin
        /// </summary>
        public override String Name
        {
            get { return "ControllerMonitor BackgroundPlugin"; }
        }

        /// <summary>
        /// Called by the Environment when the user has logged in.
        /// </summary>
        public override void Init()
        {
            _controllerConfigChangedObject = EnvironmentManager.Instance.RegisterReceiver(ControllerConfigChangedHandler, new MessageIdAndRelatedKindFilter(MessageId.Server.ConfigurationChangedIndication, ControllerMonitorDefinition.ControllerMonitorKind));

        }

        /// <summary>
        /// Called by the Environment when the user log's out.
        /// You should close all remote sessions and flush cache information, as the
        /// user might logon to another server next time.
        /// </summary>
        public override void Close()
        {
            _stopEvent.Set();
            _stopThread = true;

            if (_thread != null)
            {
                _thread.Join();
            }

            _stopEvent.Dispose();
            _stopEvent = null;
            EnvironmentManager.Instance.UnRegisterReceiver(_controllerConfigChangedObject);
            _controllerConfigChangedObject = null;
        }

        /// <summary>
        /// Define in what Environments the current background task should be started.
        /// </summary>
        public override List<EnvironmentType> TargetEnvironments
        {
            get { return new List<EnvironmentType>() { EnvironmentType.Service }; } // Default will run in the Event Server
        }


        private bool Connect(string host, int port)
        {
            if (_client != null)
            {
                _client.Close();
                _client = null;
            }
            try
            {
                _client = new TcpClient(host, port);
                _client.ReceiveTimeout = 1000;
            }
            catch (Exception ex)
            {
                EnvironmentManager.Instance.Log(false, "ControllerMonitorBackgroundplugin", string.Format("Failed connecting to {0}:{1}. Exception: {2}", host, port, ex.Message));
                return false;
            }
            return true;
        }

        internal void SendCommand(Item destinationItem, string command)
        {
            // should use destinationItem to find right controller and send to that one, but here we only have the one
            if (_client != null)
            {
                _client.Client.Send(ASCIIEncoding.ASCII.GetBytes(command));
            }
        }



        private void ControllerComm()
        {
            while (!_stopThread)
            {
                if (!Connect(_connectedController.Properties["IPAddress"], 4567))
                {
                    _stopEvent.WaitOne(TimeSpan.FromSeconds(10));
                    continue;
                }
                while (!_stopThread)
                {
                    try
                    {
                        byte[] buffer = new byte[256];
                        int bytesReceived = _client.Client.Receive(buffer);
                        if (bytesReceived > 0)
                        {
                            //HandleEvent(ASCIIEncoding.ASCII.GetString(buffer, 0, bytesReceived));
                            EnvironmentManager.Instance.Log(false, "ControllerMonitorBackgroundPlugin", string.Format("Received this data from the controller {0}: {1}", _client.Client.RemoteEndPoint.ToString(), ASCIIEncoding.ASCII.GetString(buffer, 0, bytesReceived)));

                        }
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.TimedOut)
                        {
                            continue;
                        }
                        else
                        {
                            _client.Close();
                            break;
                        }
                    }
                    catch (Exception)
                    {
                        _client.Close();
                        break;
                    }
                }
            }
        }

        private object ControllerConfigChangedHandler(Message message, FQID dest, FQID sender)
        {
            List<Item> items = Configuration.Instance.GetItemConfigurations(ControllerMonitorDefinition.ControllerMonitorPluginId, null, ControllerMonitorDefinition.ControllerMonitorKind);
            // in real solution we should connect to all controllers, but here we only connect to the first
            if (items.Any())
            {
                if (items[0].Enabled && items[0].Properties.ContainsKey("IPAddress") && !string.IsNullOrWhiteSpace(items[0].Properties["IPAddress"]))
                {
                    _stopEvent.Set();
                    _stopThread = true;

                    if (_thread != null)
                    {
                        _thread.Join();
                    }
                    _stopThread = false;
                    _connectedController = items[0];
                    _thread = new System.Threading.Thread(ControllerComm);
                    _thread.Name = "Controller Monitor listen thread";
                    _thread.Start();
                }
            }
            EnvironmentManager.Instance.Log(false, "ControllerMonitorBackgroundplugin", "Controller configuration changed");
            return null;
        }
    }
}
