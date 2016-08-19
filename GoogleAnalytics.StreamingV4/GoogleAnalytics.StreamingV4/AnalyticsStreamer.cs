﻿using Google.Apis.AnalyticsReporting.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAnalytics.StreamingV4
{
    public class AnalyticsStreamer
    {
        /// <summary>
        /// make a request to the Google Analytics reporting api v4 Even though it is set up with more then one report
        /// The streamer will only stream the first.   It would be posible to create a second streamer
        /// 
        /// Note: for this to work the AnaltyicsReportingService.cs must be changed to allow body to be public
        /// 
        /// ->>  public Google.Apis.AnalyticsReporting.v4.Data.GetReportsRequest Body { get; set; }
        /// 
        /// 
        /// Note2:  It is a very bad idea to do what i am doing.   If you do this its still going to send the second report with every request
        ///         However there will be no pagaintation so you wil just get the first page mulitple times.
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
                PageSize = 20,
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


            var orignal = new AnalyticsReportingPageStreamer<Report, ReportsResource.BatchGetRequest, GetReportsResponse, string>(
                                                  (request, token) => request.PageToken = token,
                                                  response => response.NextPageToken,
                                                  response => response.Reports);


            foreach (var report in orignal.Fetch(service.Reports, getReport))
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