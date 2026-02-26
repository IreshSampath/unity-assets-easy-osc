using extOSC;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace GAG.EasyOSC
{
    public class OSCHandler : MonoBehaviour
    {
        public static OSCHandler Instance;
        public OSCSettingsModel OSCSettingsModel = new OSCSettingsModel();

        string _path => Path.Combine(Application.persistentDataPath, "osc_settings.json");

        #region Fields Transmitter

        [Header("OSC Settings")]
        public OSCTransmitter Transmitter;
        public string SourceOSCAddress = "/Message";

        #endregion


        #region Fields Receiver

        [Header("OSC Settings")]
        public OSCReceiver Receiver;
        public string DestinationOSCAddress = "/Message";

        #endregion


        #region Unity Methods

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void OnEnable()
        {
            AppEvents.OSCDashboardOpened += LoadOSCSettings;
            AppEvents.OSCMessageSent += SendMessage;
        }

        void OnDisable()
        {
            AppEvents.OSCDashboardOpened -= LoadOSCSettings;
            AppEvents.OSCMessageSent -= SendMessage;
        }

        void Start()
        {
            LoadOSCSettings();
            Receiver.Bind(SourceOSCAddress, MessageReceived);
        }

        #endregion


        #region Other Methods

        public string LoadSourceIP()
        {
            return GetWiFiIPAddress();
        }

        string GetWiFiIPAddress()
        {
            foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Check if the interface is Wi-Fi
                if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 &&
                    netInterface.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ipInfo in netInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (ipInfo.Address.AddressFamily == AddressFamily.InterNetwork) // IPv4 only
                        {
                            return ipInfo.Address.ToString();
                        }
                    }
                }
            }

            //return "Wi-Fi IPv4 Not Found";
            return GetLocalIPAddress();
        }

        string GetLocalIPAddress()
        {
            string ipAddress = "Not Found";

            foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInterface.OperationalStatus == OperationalStatus.Up)  // Check if network is active
                {
                    foreach (UnicastIPAddressInformation ipInfo in netInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (ipInfo.Address.AddressFamily == AddressFamily.InterNetwork) // IPv4 only
                        {
                            return ipInfo.Address.ToString();
                        }
                    }
                }
            }

            return ipAddress;
        }

        void SendMessage(string msg)
        {
            //var message = new OSCMessage(DestinationOSCAddress);

            //switch (msgType)
            //{
            //    case int i:
            //        message.AddValue(OSCValue.Int(int.Parse(msg)));
            //        break;

            //    case float f:
            //        message.AddValue(OSCValue.Float(float.Parse(msg)));
            //        break;

            //    case bool b:
            //        message.AddValue(OSCValue.Bool(bool.Parse(msg)));
            //        break;

            //    default:
            //        message.AddValue(OSCValue.String(msg));
            //        break;
            //}

            //Transmitter.Send(message);

            var message = new OSCMessage(DestinationOSCAddress);
            //message.AddValue(OSCValue.String(msg));
            //message.AddValue(OSCValue.Int(1));
            //message.AddValue(OSCValue.Int(int.Parse(msg)));
            //message.AddValue(OSCValue.Int(int.Parse(msg)));
            message.AddValue(OSCValue.String(msg));
            Transmitter.Send(message);
        }

        void MessageReceived(OSCMessage message)
        {
            if (message.Values.Count == 0) return;

            var oscValue = message.Values[0];

            if (oscValue.Type == OSCValueType.Int)
            {
                int value = oscValue.IntValue;
                AppEvents.RaiseOSCMsgReceived(value.ToString());
                Debug.Log("Received INT: " + value);
            }
            else if (oscValue.Type == OSCValueType.String)
            {
                string value = oscValue.StringValue;
                AppEvents.RaiseOSCMsgReceived(value);
                Debug.Log("Received STRING: " + value);
            }
        }

        #endregion


        public void SaveOSCSettings()
        {
            string json = JsonUtility.ToJson(OSCSettingsModel, true);
            File.WriteAllText(_path, json);

            UpdateSettings();
        }

        public void LoadOSCSettings()
        {
            AppEvents.RaiseLocalIPLoaded(GetWiFiIPAddress());

            if (File.Exists(_path))
            {
                string json = File.ReadAllText(_path);
                OSCSettingsModel = JsonUtility.FromJson<OSCSettingsModel>(json);

                UpdateSettings();
            }
        }

        void UpdateSettings()
        {
            Receiver.LocalHost = OSCSettingsModel.SourceIP;
            Receiver.LocalPort = OSCSettingsModel.SourcePort;
            //SourceOSCAddress = OSCSettingsModel.SourceOSCAddress;
            RebindReceiver(OSCSettingsModel.SourceOSCAddress);
            Transmitter.RemoteHost = OSCSettingsModel.DestinationIP;
            Transmitter.RemotePort = OSCSettingsModel.DestinationPort;
            DestinationOSCAddress = OSCSettingsModel.DestinationOSCAddress;
        }

        void RebindReceiver(string newPath)
        {
            // Remove ALL previous bindings (safe approach)
            Receiver.UnbindAll();

            SourceOSCAddress = newPath;

            // Bind again with the new path
            Receiver.Bind(SourceOSCAddress, MessageReceived);

            Debug.Log($"OSC Receiver rebound to: {SourceOSCAddress}");
        }

        void OnApplicationQuit()
        {
#if !UNITY_EDITOR
            Process.GetCurrentProcess().Kill();
#endif
        }
    }
}
