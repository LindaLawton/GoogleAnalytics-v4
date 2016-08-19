using System;
using System.Configuration;

namespace GoogleAnalytics.StreamingV4
{
    class Program
    {
        static void Main(string[] args)
        {
            // Required varables can be found in App.config.

            // Request authentcation
            var service = GoogleAuthentcation.AuthenticateOauth(ConfigurationManager.AppSettings["clientId"],
                                                                ConfigurationManager.AppSettings["clientSecret"],
                                                                Util.MacAddress.getMacAddress());


            //StandardStreamer.getData(service, ConfigurationManager.AppSettings["GoogleAnaltyicsViewId"]);
            //Console.ReadLine();


            AnalyticsStreamer.getData(service, ConfigurationManager.AppSettings["GoogleAnaltyicsViewId"]);
            Console.ReadLine();



        }
    }
}
