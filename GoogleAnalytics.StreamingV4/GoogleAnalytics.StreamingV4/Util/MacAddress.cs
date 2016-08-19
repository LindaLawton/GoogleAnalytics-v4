using System.Linq;
using System.Net.NetworkInformation;

namespace GoogleAnalytics.StreamingV4.Util
{
    public class MacAddress
    {
        // Fetching name for Auth user
        public static string getMacAddress() {

            return NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up).Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();
        }


    }
}
