using System;

[Serializable]
public class OSCSettingsModel
{
    // --- Source (Listen From) ---
    public string SourceIP = "0.0.0.0";   // Auto-filled from local device
    public int SourcePort = 7000;
    public string SourceOSCAddress = "/message";

    // --- Destination (Send To) ---
    public string DestinationIP = "127.0.0.1";
    public int DestinationPort = 7000;
    public string DestinationOSCAddress = "/message";
}
