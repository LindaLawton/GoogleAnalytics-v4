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

using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Requests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoogleAnalytics.StreamingV4
{
    class StandardStreamer
    {

        /// <summary>
        /// Makes a request to the Google Analytics Reporting API v4.  
        /// 
        /// NOTE: It will only paginate the first report if you have more then one its just going to eat your quota.
        /// 
        /// Note2: for this to work the AnaltyicsReportingService.cs must be changed to allow body to be public
        /// 
        /// ->>  public Google.Apis.AnalyticsReporting.v4.Data.GetReportsRequest Body { get; set; }
        /// 
        /// 
        /// Serously Dont send more then one:  It is a very bad idea to do what I am doing in this example.   
        ///                                    If you do this its still going to send the second report with every request
        ///                                    However there will be no pagaintation so you wil just get the first page mulitple times.
        /// </summary>
        /// <param name="service"></param>
        public static void getData(AnalyticsReportingService service, string GoogleAnalyticsViewId)
        {

            // Create the DateRange object.
            DateRange June2015 = new DateRange() { StartDate = "2015-01-01", EndDate = "2015-06-30" };
            DateRange June2016 = new DateRange() { StartDate = "2016-01-01", EndDate = "2016-06-30" };
            List<DateRange> dateRanges = new List<DateRange>() { June2016, June2015 };

            //Create the Dimensions object.
            Dimension browser = new Dimension { Name = "ga:browser" };

            // Create the ReportRequest object.
            ReportRequest reportRequest = new ReportRequest
            {
                ViewId = GoogleAnalyticsViewId,
                DateRanges = dateRanges,
                Dimensions = new List<Dimension>() { browser },
                Metrics = new List<Metric>() { new Metric { Expression = "ga:sessions" } },
                PageSize = 5,
            };

            // Create a second ReportRequest object.
            ReportRequest reportRequest2 = new ReportRequest
            {
                ViewId = GoogleAnalyticsViewId,
                DateRanges = dateRanges,
                Dimensions = new List<Dimension>() { new Dimension { Name = "ga:userType" } },
                Metrics = new List<Metric>() { new Metric { Expression = "ga:users" } },
                PageSize = 10,
            };


            List<ReportRequest> requests = new List<ReportRequest>();
            requests.Add(reportRequest);
            requests.Add(reportRequest2);

            // Create the GetReportsRequest object.
            GetReportsRequest getReport = new GetReportsRequest() { ReportRequests = requests };
            var test = service.Reports.BatchGet(getReport);


            var orignal = new PageStreamer<Report, ReportsResource.BatchGetRequest, GetReportsResponse, string>(
                                                   (request, token) => request.Body.ReportRequests[0].PageToken = token,
                                                   response => response.Reports[0].NextPageToken,
                                                   response => response.Reports);


            foreach (var report in orignal.Fetch(test))
            {

                var reportDimensions = string.Join(",", report.ColumnHeader.Dimensions);

                var reportMetrics = string.Join(",", report.ColumnHeader.MetricHeader.MetricHeaderEntries.Select(m => m.Name).ToList());

                Console.WriteLine(string.Format("Results for Report: Dimensons: {0} :Metrics: {1}", reportDimensions, reportMetrics));

                ColumnHeader header = report.ColumnHeader;
                List<String> dimensionHeaders = (List<String>)header.Dimensions;
                List<MetricHeaderEntry> metricHeaders = (List<MetricHeaderEntry>)header.MetricHeader.MetricHeaderEntries;

                foreach (var row in report.Data.Rows)
                {

                    List<String> dimensions = (List<String>)row.Dimensions;
                    List<DateRangeValues> metrics = (List<DateRangeValues>)row.Metrics;
                    for (int d = 0; d < dateRanges.Count() && d < metrics.Count(); d++)
                    {
                        Console.Write(dateRanges[d].StartDate + " - " + dateRanges[d].EndDate + ": ");
                        List<String> metricsForDaterange = (List<String>)row.Metrics[d].Values;

                        dimensions.ForEach(delegate (String name)
                        {
                            Console.Write(name + " - ");
                        });

                        metricsForDaterange.ForEach(delegate (String name)
                        {
                            Console.Write(name + " - ");
                        });
                        Console.WriteLine("");
                    }
                }
            }
        }
    }

}
