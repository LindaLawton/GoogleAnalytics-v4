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
using Google.Apis.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GoogleAnalytics.StreamingV4
{
    class StandardStreamer
    {

        /// <summary>
        /// Makes a request to the Google Analytics Reporting API v4.  Using the Default Google .Net Client library PageStreamer.
        /// 
        /// NOTE: It will only work with one report.
        /// 
        /// Note2: For this to work the AnaltyicsReportingService.cs must be changed to allow body to be public
        /// 
        /// ->>  public Google.Apis.AnalyticsReporting.v4.Data.GetReportsRequest Body { get; set; }
        /// 
        /// 
        /// Serously Dont send more then one:  It is a very bad idea to do what I am doing in this example.   
        ///                                    If you do this its still going to send the second report with every request
        ///                                    However there will be no pagaintation so you wil just get the first page mulitple times.
        /// </summary>
        /// <param name="service">Authentcated AnalyticsReportingService</param>
        /// <param name="requests">List of report Requests</param>
        public static void getData(AnalyticsReportingService service, List<ReportRequest> requests)
        {
            if (service == null)
                throw new ArgumentNullException("Service is required");
            if (requests == null)
                throw new ArgumentNullException("Requests are required");

            if (requests.Count == 0)
                return;  // Nothing to do

            // If you try and send more then one report we are going to delete the extras. 
            // This just wont work with more then one report.
            if (requests.Count > 1)
            {
                requests.RemoveRange(1, requests.Count - 1);
            }

            // Create the GetReportsRequest object.
            GetReportsRequest getReport = new GetReportsRequest() { ReportRequests = requests };
            var test = service.Reports.BatchGet(getReport);


            var orignal = new PageStreamer<Report, ReportsResource.BatchGetRequest, GetReportsResponse, string>(
                                                   (request, token) => request.Body.ReportRequests[0].PageToken = token,
                                                   response => response.Reports[0].NextPageToken,
                                                   response => response.Reports);          


            foreach (var report in orignal.Fetch(test))
            {

                // Defaulting dateranges to that of the first report request until I figure out why they arent returning with the response.
                DisplayData.ShowData(report, (List<DateRange>)requests[0].DateRanges);
            }
        }


        /// <summary>
        /// Makes an Async request to the Google Analytics Reporting API v4.  Using the Default Google .Net Client library PageStreamer.
        /// 
        /// NOTE: It will only work with one report.
        /// 
        /// Note2: For this to work the AnaltyicsReportingService.cs must be changed to allow body to be public
        /// 
        /// ->>  public Google.Apis.AnalyticsReporting.v4.Data.GetReportsRequest Body { get; set; }
        /// 
        /// 
        /// Serously Dont send more then one:  It is a very bad idea to do what I am doing in this example.   
        ///                                    If you do this its still going to send the second report with every request
        ///                                    However there will be no pagaintation so you wil just get the first page mulitple times.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="requests">List of report Requests</param>
        public static void getDataAsync(AnalyticsReportingService service, List<ReportRequest> requests)
        {
            if (service == null)
                throw new ArgumentNullException("Service is required");
            if (requests == null)
                throw new ArgumentNullException("Requests are required");

            if (requests.Count == 0)
                return;  // Nothing to do

            // If you try and send more then one report we are going to delete the extras. 
            // This just wont work with more then one report.
            if (requests.Count > 1)
            {
                requests.RemoveRange(1, requests.Count - 1);
            }


            // Create the GetReportsRequest object.
            GetReportsRequest getReport = new GetReportsRequest() { ReportRequests = requests };
            var test = service.Reports.BatchGet(getReport);


            var orignal = new PageStreamer<Report, ReportsResource.BatchGetRequest, GetReportsResponse, string>(
                                                   (request, token) => request.Body.ReportRequests[0].PageToken = token,
                                                   response => response.Reports[0].NextPageToken,
                                                   response => response.Reports);
            
            var allData = orignal.FetchAllAsync(test, CancellationToken.None);
            allData.Wait();

            foreach (var report in allData.Result)
            {
                // Defaulting dateranges to that of the first report request until I figure out why they arent returning with the response.
                DisplayData.ShowData(report, (List<DateRange>)requests[0].DateRanges);
            }
        }


    }

}
