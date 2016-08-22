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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleAnalytics.StreamingV4
{
    public class AnalyticsStreamer
    {
        /// <summary>
        /// Make a request to the Google Analytics reporting api v4 
        /// 
        /// Note recomend that the reports NOT have diffrent dateRanges at this time.   I can find no way to match things up
        /// The response does not contain any information about the report that was requested we have to ASSUME a lot here
        /// 
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

            // Create the GetReportsRequest object.
            GetReportsRequest getReport = new GetReportsRequest() { ReportRequests = requests };
            var test = service.Reports.BatchGet(getReport);

            // Create the page streamer
            var orignal = new AnalyticsReportingPageStreamer<Report, ReportsResource.BatchGetRequest, GetReportsResponse, string>(
                                                  (request, token) => request.PageToken = token,
                                                  response => response.NextPageToken,
                                                  response => response.Reports);
                       
            foreach (var report in orignal.Fetch(service.Reports, getReport))
            {
                // Defaulting dateranges to that of the first report request until I figure out why they arent returning with the response.
                DisplayData.ShowData(report, (List<DateRange>)requests[0].DateRanges);
            }
        }


        /// <summary>
        /// Make a request to the Google Analytics reporting api v4 Async
        /// 
        /// Note recomend that the reports NOT have diffrent dateRanges at this time.   I can find no way to match things up
        /// The response does not contain any information about the report that was requested we have to ASSUME a lot here
        /// </summary>
        /// <param name="service">Authentcated AnalyticsReportingService</param>
        /// <param name="requests">List of report Requests</param>
        public static void getdataAsync(AnalyticsReportingService service, List<ReportRequest> requests)
        {

            if (service == null)
                throw new ArgumentNullException("Service is required");
            if (requests == null)
                throw new ArgumentNullException("Requests are required");

            if (requests.Count == 0)
                return;  // Nothing to do


            // Create the GetReportsRequest object.
            GetReportsRequest getReport = new GetReportsRequest() { ReportRequests = requests };
            

            var orignal = new AnalyticsReportingPageStreamer<Report, ReportsResource.BatchGetRequest, GetReportsResponse, string>(
                                                (request, token) => request.PageToken = token,
                                                response => response.NextPageToken,
                                                response => response.Reports);


            var allData = orignal.FetchAllAsync(service.Reports, getReport, CancellationToken.None);
            allData.Wait();

            foreach (var report in allData.Result)
            {
                // Defaulting dateranges to that of the first report request until I figure out why they arent returning with the response.
                DisplayData.ShowData(report, (List<DateRange>)requests[0].DateRanges);
            }
        }
    }
}
