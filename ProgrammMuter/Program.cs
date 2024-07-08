using CSCore.CoreAudioAPI;
using System.Data;
using System.Diagnostics;

public class MixerTest
{
    static void Main(string[] args)
    {
        foreach (AudioSessionManager2 sessionManager in GetDefaultAudioSessionManager2(DataFlow.Render))
        {
            using (sessionManager)
            {
                using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
                {
                    foreach (var session in sessionEnumerator)
                    {
                        using var simpleVolume = session.QueryInterface<SimpleAudioVolume>();
                        using var sessionControl = session.QueryInterface<AudioSessionControl2>();
                        Console.WriteLine((sessionControl.Process.ProcessName, sessionControl.SessionIdentifier));
                        if (Process.GetProcessById(sessionControl.ProcessID).ProcessName.Equals("chrome"))
                        {
                            if(simpleVolume.IsMuted)
                            {
                                simpleVolume.IsMuted = false;
                            }
                            else
                            {
                                simpleVolume.IsMuted = true;
                            }
                        }
                    }
                }
            }
        }

        Console.ReadKey();
    }
    private static IEnumerable<AudioSessionManager2> GetDefaultAudioSessionManager2(DataFlow dataFlow)
    {
        using var enumerator = new MMDeviceEnumerator();
        using var devices = enumerator.EnumAudioEndpoints(dataFlow, DeviceState.Active);
        foreach (var device in devices)
        {
            Console.WriteLine("Device: " + device.FriendlyName);
            var sessionManager = AudioSessionManager2.FromMMDevice(device);
            yield return sessionManager;
        }
    }
}