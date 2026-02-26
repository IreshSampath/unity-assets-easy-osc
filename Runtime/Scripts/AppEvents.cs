using System;

namespace GAG.EasyOSC 
{
    public class AppEvents
    {
        public static event Action OSCDashboardOpened;

        public static event Action<string> LocalIPLoaded;
        public static event Action<string> PortLoaded;
        public static event Action<string> RemoteIPUpdated;

        public static event Action<string> OSCMessageSent;
        public static event Action<string> OSCMsgReceived;

        public static void RaiseOSCDashboardOpened()
        {
            OSCDashboardOpened?.Invoke();
        }

        public static void RaiseLocalIPLoaded(string ip)
        {
            LocalIPLoaded?.Invoke(ip);
        }

        public static void RaiseRemoteIPUpdated(string ip)
        {
            RemoteIPUpdated?.Invoke(ip);
        }

        public static void RaisePortLoaded(string port)
        {
            PortLoaded?.Invoke(port);
        }

        public static void RaiseOSCMessageSent(string msg)
        {
            OSCMessageSent?.Invoke(msg);
        }

        public static void RaiseOSCMsgReceived(string msg)
        {
            OSCMsgReceived?.Invoke(msg);
        }
    }
}
