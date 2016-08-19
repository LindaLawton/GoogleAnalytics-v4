/*
Copyright 2016 Linda Lawton

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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

            // Example of Page streaing using the default page streamer only recomended if you have a single report and if you can edit the AnalyticsReportingService.cs 
            StandardStreamer.getData(service, ConfigurationManager.AppSettings["GoogleAnaltyicsViewId"]);
            Console.ReadLine();

            // Example using my own pagestreamer that will only work with the Google Analytics Reporting V4 API
            // Works with batching and does not require any changes to the client library.  
            AnalyticsStreamer.getData(service, ConfigurationManager.AppSettings["GoogleAnaltyicsViewId"]);
            Console.ReadLine();



        }
    }
}
