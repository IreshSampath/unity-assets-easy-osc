using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;

namespace GAG.EasyOSC
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] GameObject _oscDashboardPanel;

        [SerializeField] TMP_Text _sourceIPTxt;
        [SerializeField] TMP_InputField _sourcePortInput;
        [SerializeField] TMP_InputField _sourceOscAddressInput;

        [SerializeField] TMP_InputField _destinationIPInput;
        [SerializeField] TMP_InputField _destinationPortInput;
        [SerializeField] TMP_InputField _destinationOscAddressInput;
        [SerializeField] TMP_InputField _msgInput;

        [SerializeField] TMP_Text _consolTxt;

        int _ipTapCount = 0;

        void OnEnable()
        {
            //AppEvents.OSCDashboardOpened += ShowDashboard;
            AppEvents.OSCMsgReceived += OnReceivedMessage;
            //AppEvents.LocalIPLoaded += ShowSourceIP;
        }

        void OnDisable()
        {
            //AppEvents.OSCDashboardOpened -= ShowDashboard;
            AppEvents.OSCMsgReceived -= OnReceivedMessage;
            //AppEvents.LocalIPLoaded -= ShowSourceIP;
        }

        public void OnClickOpenOSCDashboard()
        {
            _ipTapCount++;

            if (_ipTapCount == 3)
            {
                LoadToUI();
                _oscDashboardPanel.SetActive(true);

                _ipTapCount = 0;
            }
        }

        public void OnClickShowInstructions()
        {
            ShowInstructions();
        }

        void ShowInstructions()
        {
            string instructions = "" +
                "# Ensure all devices are connected to the same network.\n\n" +
                "# When changing the local network, click \"Refresh\" to update the local IP address.\n\n" +
                "# After changing anything, don't forget to click \"Save\".\n\n" +
                "# If you're not receiving messages on the Windows app, disable the firewall for public networks.";

            PrintConsole(instructions);
        }

        void PrintConsole(string msg)
        {
            DateTime now = DateTime.Now;
            string currentTime = now.ToString("hh:mm:ss");

            string newMessage = $"{currentTime} : {msg}\n\n{_consolTxt.text}"; // Prepend new message
            _consolTxt.text = newMessage;
        }

        public void LoadSourceIP()
        {
            string ipAddress = OSCHandler.Instance.LoadSourceIP();
            _sourceIPTxt.text = ipAddress;
        }

        void OnReceivedMessage(string msg)
        {
            PrintConsole($"OSC Received ({msg})" );
        }
        public void OnClickSave()
        {
            SaveFromUI();
        }

        public void OnClickSendMessages()
        {
            AppEvents.RaiseOSCMessageSent(_msgInput.text);
            PrintConsole($"OSC Sent ({ _msgInput.text})");
        }

        void LoadToUI()
        {
            LoadSourceIP();

            OSCHandler.Instance.LoadOSCSettings();

            OSCSettingsModel Model = OSCHandler.Instance.OSCSettingsModel;

            _sourceIPTxt.text = Model.SourceIP;
            _sourcePortInput.text = Model.SourcePort.ToString();
            _sourceOscAddressInput.text = Model.SourceOSCAddress;

            _destinationIPInput.text = Model.DestinationIP;
            _destinationPortInput.text = Model.DestinationPort.ToString();
            _destinationOscAddressInput.text = Model.DestinationOSCAddress;
        }

        void SaveFromUI()
        {
            OSCSettingsModel Model = OSCHandler.Instance.OSCSettingsModel;

            Model.SourceIP = _sourceIPTxt.text;
            Model.SourcePort = int.Parse(_sourcePortInput.text);
            Model.SourceOSCAddress = _sourceOscAddressInput.text;

            Model.DestinationIP = _destinationIPInput.text;
            Model.DestinationPort = int.Parse(_destinationPortInput.text);
            Model.DestinationOSCAddress = _destinationOscAddressInput.text;

            OSCHandler.Instance.SaveOSCSettings();
        }


    }
}
