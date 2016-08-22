/*
Copyright 2016 Linda Lawton
http://www.daimto.com/

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
using Google.Apis.AnalyticsReporting.v4.Data;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace GoogleAnalytics.StreamingV4
{
    class Program
    {
        static void Main(string[] args)
        {
            // Required varables can be found in App.config.
            var service = GoogleAuthentcation.AuthenticateOauth(ConfigurationManager.AppSettings["clientSecretJsonPath"],
                                                                Util.MacAddress.getMacAddress());

            // Create the DateRange object.
            DateRange June2015 = new DateRange() { StartDate = "2015-01-01", EndDate = "2015-06-30" };
            DateRange June2016 = new DateRange() { StartDate = "2016-01-01", EndDate = "2016-06-30" };
            List<DateRange> dateRanges = new List<DateRange>() { June2016, June2015 };

            //Create the Dimensions object.
            Dimension browser = new Dimension { Name = "ga:browser" };

            // Create the ReportRequest object.
            ReportRequest reportRequest = new ReportRequest
            {
                ViewId = ConfigurationManager.AppSettings["GoogleAnaltyicsViewId"],
                DateRanges = dateRanges,
                Dimensions = new List<Dimension>() { browser },
                Metrics = new List<Metric>() { new Metric { Expression = "ga:sessions" } },
                PageSize = 20,
            };

            // Create a second ReportRequest object.
            ReportRequest reportRequest2 = new ReportRequest
            {
                ViewId = ConfigurationManager.AppSettings["GoogleAnaltyicsViewId"],
                DateRanges = dateRanges,
                Dimensions = new List<Dimension>() { new Dimension { Name = "ga:userType" } },
                Metrics = new List<Metric>() { new Metric { Expression = "ga:users" } },
                PageSize = 10,
            };


            List<ReportRequest> requests = new List<ReportRequest>();
            requests.Add(reportRequest);
            requests.Add(reportRequest2);

            // Example using my own pagestreamer that will only work with the Google Analytics Reporting V4 API
            // Works with batching and does not require any changes to the client library.  
            AnalyticsStreamer.getData(service, requests);

            // Gets data Async
            AnalyticsStreamer.getdataAsync(service,requests);



            // Example of Page streaing using the default page streamer only recomended if you have a single report.  Requires a patch of the AnalyticsReportingService.cs 
            StandardStreamer.getData(service, requests);

            // Example of Page streaing  async using the default page streamer only recomended if you have a single report.  Requires a patch of the AnalyticsReportingService.cs 
            StandardStreamer.getDataAsync(service, requests);

            Console.ReadLine();

        





            Console.ReadLine();



        }
    }
}
