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

namespace GoogleAnalytics.StreamingV4.GoogleDeveloper
{
    /// <summary>
    /// These samples were created for the Google Analytics Reporting API V4 Google Developer site.
    /// 
    /// Once copied up there they will be under Googles licenicng.
    /// 
    /// Samples for https://developers.google.com/analytics/devguides/reporting/core/v4/samples
    /// </summary>    
    public class GoogleDevSiteSamples
    {

        /// <summary>
        /// Dimensions and Metrics
        /// 
        /// Below is a simple request with just a few dimensions and metrics. See the Dimensions and
        /// Metrics Explorer For the complete set of dimensions and metrics available. The dimensions and
        /// metrics are configurable repeated objects passed in the post body.
        /// </summary>
        public static void DimensonsAndMetrics(AnalyticsReportingService analyticsreporting)
        {
            if (analyticsreporting == null)
                throw new ArgumentException("Reporting service required");     

            // Create the DateRange object.
            DateRange dateRange = new DateRange() { StartDate = "2015-06-15", EndDate = "2015-06-30" };

            // Create the Metrics object.
            Metric sessions = new Metric { Expression = "ga:sessions", Alias = "Sessions" };


            //Create the Dimensions object.
            Dimension browser = new Dimension { Name = "ga:browser" };

            // Create the ReportRequest object.
            // Create the ReportRequest object.
            ReportRequest reportRequest = new ReportRequest
            {
                ViewId = "XXXX",
                DateRanges = new List<DateRange>() { dateRange },
                Dimensions = new List<Dimension>() { browser },
                Metrics = new List<Metric>() { sessions }
            };


            List<ReportRequest> requests = new List<ReportRequest>();
            requests.Add(reportRequest);

            // Create the GetReportsRequest object.
            GetReportsRequest getReport = new GetReportsRequest() { ReportRequests = requests };

            // Call the batchGet method.
            GetReportsResponse response = analyticsreporting.Reports.BatchGet(getReport).Execute();

        }

        /// <summary>
        /// Multiple Date Ranges
        /// Below is an example with multiple date ranges:
        /// </summary>
        public static void MultipleDateRanges(AnalyticsReportingService analyticsreporting)
        {
            if (analyticsreporting == null)
                throw new ArgumentException("Reporting service required");

            // Create the DateRange object.
            DateRange march = new DateRange() { StartDate = "2015-03-01", EndDate = "2015-03-31" };

            DateRange january = new DateRange() { StartDate = "2015-01-01", EndDate = "2015-01-31" };

            // Create the Metrics object.
            Metric sessions = new Metric { Expression = "ga:sessions", Alias = "Sessions" };


            //Create the Dimensions object.
            Dimension browser = new Dimension { Name = "ga:browser" };

            // Create the ReportRequest object.
            // Create the ReportRequest object.
            ReportRequest reportRequest = new ReportRequest
            {
                ViewId = "XXXX",
                DateRanges = new List<DateRange>() { march, january },
                Dimensions = new List<Dimension>() { browser },
                Metrics = new List<Metric>() { sessions }
            };


            List<ReportRequest> requests = new List<ReportRequest>();
            requests.Add(reportRequest);

            // Create the GetReportsRequest object.
            GetReportsRequest getReport = new GetReportsRequest() { ReportRequests = requests };

            // Call the batchGet method.
            GetReportsResponse response = analyticsreporting.Reports.BatchGet(getReport).Execute();

        }

        /// <summary>
        /// When parsing the response of a request with multiple date ranges the results are returned as an array of dateRangeValues.
        /// </summary>
        /// <param name="reports"></param>
        public static void printResults(List<Report> reports)
        {
            foreach (Report report in reports)
            {
                ColumnHeader header = report.ColumnHeader;
                List<string> dimensionHeaders = (List<string>)header.Dimensions;

                List<MetricHeaderEntry> metricHeaders = (List<MetricHeaderEntry>)header.MetricHeader.MetricHeaderEntries;
                List<ReportRow> rows = (List<ReportRow>)report.Data.Rows;

                foreach (ReportRow row in rows)
                {
                    List<string> dimensions = (List<string>)row.Dimensions;
                    List<DateRangeValues> metrics = (List<DateRangeValues>)row.Metrics;

                    for (int i = 0; i < dimensionHeaders.Count() && i < dimensions.Count(); i++)
                    {
                        Console.WriteLine(dimensionHeaders[i] + ": " + dimensions[i]);
                    }

                    for (int j = 0; j < metrics.Count(); j++)
                    {
                        Console.WriteLine("Date Range (" + j + "): ");
                        DateRangeValues values = metrics[j];
                        for (int k = 0; k < values.Values.Count() && k < metricHeaders.Count(); k++)
                        {
                            Console.WriteLine(metricHeaders[k].Name + ": " + values.Values[k]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Metric Expressions
        /// 
        /// The repeated metrics parameters can take any of the existing metrics but you can also create a custom calculated metric,
        /// by combining existing metrics into a new metrics expression. Notice because the below sample is a division operation I also 
        /// need to set the formatingType to be FLOAT; I am also making use of the alias parameter:
        /// </summary>
        public static void MetricExpressions()
        {
            // Create the Metrics object.
            Metric metricExpression = new Metric
            {
                Expression = "ga:goal1Completions/ga:goal1Starts",
                FormattingType = "FLOAT",
                Alias = "Metric Expression"
            };
        }

        /// <summary>
        /// Histogram Buckets
        /// 
        /// The API V4 also allows you to define your own set of custom histogram buckets saving you from doing expensive data processing 
        /// on the client side. Below is an example of bucketed dimensions. Note there is also orderBy parameter which will sort the bucketed dimensions in the correct order:
        /// </summary>
        public static void HistogramBuckets()
        {

            // Create the Dimensions object.
            Dimension buckets = new Dimension
            {
                Name = "ga:sessionCount",
                HistogramBuckets = (IList<long?>)new List<long> { 1L, 10L, 100L, 200L, 300L, 400L }
            };

            // Create the Ordering.
            OrderBy ordering = new OrderBy()
            {
                OrderType = "HISTOGRAM_BUCKET",
                FieldName = "ga:sessionCount"
            };
        }

        /// <summary>
        /// Segments 
        /// The segments are defined by combining logical operators of segment filter objects.
        /// You also you notice that it is necessary to add ga:segment to the list of dimensions. 
        /// Segment definitions can be either constructed dynamically inside the query, or you can specify an id of an existing built-in/custom segment.
        /// Below is an example of using a dynamic segment definition:
        /// </summary>
        /// <param name="analyticsreporting"></param>
        public static void Segments(AnalyticsReportingService analyticsreporting)
        {
            if (analyticsreporting == null)
                throw new ArgumentException("Reporting service required");


            // Create the DateRange object.
            DateRange dateRange = new DateRange() { StartDate = "2015-06-15", EndDate = "2015-06-30" };

            // Create the Metrics object.
            Metric sessions = new Metric() { Expression = "ga:sessions", Alias = "sessions" };

            //Create the browser dimension.
            Dimension browser = new Dimension() { Name = "ga:browser" };

            // Create the segment dimension.
            Dimension segmentDimensions = new Dimension() { Name = "ga:segment" };

            // Create Dimension Filter.
            SegmentDimensionFilter dimensionFilter = new SegmentDimensionFilter() { DimensionName = "ga:browser", Operator__ = "EXACT", Expressions = new List<string> { "Safari" } };


            // Create Segment Filter Clause.
            SegmentFilterClause segmentFilterClause = new SegmentFilterClause() { DimensionFilter = dimensionFilter };


            // Create the Or Filters for Segment.
            OrFiltersForSegment orFiltersForSegment = new OrFiltersForSegment() { SegmentFilterClauses = new List<SegmentFilterClause> { segmentFilterClause } };


            // Create the Simple Segment.
            SimpleSegment simpleSegment = new SimpleSegment() { OrFiltersForSegment = new List<OrFiltersForSegment> { orFiltersForSegment } };


            // Create the Segment Filters.
            SegmentFilter segmentFilter = new SegmentFilter() { SimpleSegment = simpleSegment };

            // Create the Segment Definition.
            SegmentDefinition segmentDefinition = new SegmentDefinition() { SegmentFilters = new List<SegmentFilter> { segmentFilter } };

            // Create the Dynamic Segment.
            DynamicSegment dynamicSegment = new DynamicSegment() { SessionSegment = segmentDefinition, Name = "Sessions with Safari browser" };

            // Create the Segments object.
            Segment segment = new Segment() { DynamicSegment = dynamicSegment };

            // Create the ReportRequest object.
            ReportRequest request = new ReportRequest()
            {
                ViewId = "XXXX",
                DateRanges = new List<DateRange> { dateRange },
                Dimensions = new List<Dimension> { browser, segmentDimensions },
                Segments = new List<Segment> { segment },
                Metrics = new List<Metric> { sessions }
            };


            // Create the GetReportsRequest object.
            GetReportsRequest getReport = new GetReportsRequest() { ReportRequests = new List<ReportRequest> { request } };

            // Call the batchGet method.
            GetReportsResponse response = analyticsreporting.Reports.BatchGet(getReport).Execute();

            printResults((List<Report>)response.Reports);
        }

        /// <summary>
        /// Segments 
        /// As mentioned above, instead of constructing a dynamic segment definition, it is possible 
        /// to specify a predefined segment id using segmentId field of Segment definition. The example below creates a segment for returning users.
        /// </summary>
        /// <param name="analyticsreporting"></param>
        public static void SegmentsPreDefined(AnalyticsReportingService analyticsreporting)
        {
            if (analyticsreporting == null)
                throw new ArgumentException("Reporting service required");


            // Create the Segments object for returning users.
            Segment segment = new Segment() { SegmentId = "gaid::-3" };

            // Create the DateRange object.
            DateRange dateRange = new DateRange() { StartDate = "2015-06-15", EndDate = "2015-06-30" };

            // Create the Metrics object.
            Metric sessions = new Metric() { Expression = "ga:sessions", Alias = "sessions" };

            //Create the browser dimension.
            Dimension browser = new Dimension() { Name = "ga:browser" };

            // Create the segment dimension.
            Dimension segmentDimensions = new Dimension() { Name = "ga:segment" };

            // Create the ReportRequest object.
            ReportRequest request = new ReportRequest()
            {
                ViewId = "XXXX",
                DateRanges = new List<DateRange> { dateRange },
                Dimensions = new List<Dimension> { browser, segmentDimensions },
                Segments = new List<Segment> { segment },
                Metrics = new List<Metric> { sessions }
            };


            // Create the GetReportsRequest object.
            GetReportsRequest getReport = new GetReportsRequest() { ReportRequests = new List<ReportRequest> { request } };

            // Call the batchGet method.
            GetReportsResponse response = analyticsreporting.Reports.BatchGet(getReport).Execute();

            printResults((List<Report>)response.Reports);

        }


        #region Multiple Segments

        /// <summary>
        /// The Reporting API V4 also supports multiple segments inside the ReportRequest definition. Below is a simple query with multiple segments:
        /// </summary>
        /// <param name="segmentName"></param>
        /// <param name="dimension"></param>
        /// <param name="dimensionFilterExpression"></param>
        /// <returns></returns>
        public static Segment buildSimpleSegment(String segmentName, String dimension, String dimensionFilterExpression)
        {
            // Create Dimension Filter.
            SegmentDimensionFilter dimensionFilter = new SegmentDimensionFilter() { DimensionName = dimension, Operator__ = "EXACT", Expressions = new List<string> { dimensionFilterExpression } };

            // Create Segment Filter Clause.
            SegmentFilterClause segmentFilterClause = new SegmentFilterClause() { DimensionFilter = dimensionFilter };

            // Create the Or Filters for Segment.
            OrFiltersForSegment orFiltersForSegment = new OrFiltersForSegment() { SegmentFilterClauses = new List<SegmentFilterClause> { segmentFilterClause } };

            // Create the Simple Segment.
            SimpleSegment simpleSegment = new SimpleSegment() { OrFiltersForSegment = new List<OrFiltersForSegment> { orFiltersForSegment } };

            // Create the Segment Filters.
            SegmentFilter segmentFilter = new SegmentFilter() { SimpleSegment = simpleSegment };

            // Create the Segment Definition.
            SegmentDefinition segmentDefinition = new SegmentDefinition() { SegmentFilters = new List<SegmentFilter> { segmentFilter } };

            // Create the Dynamic Segment.
            DynamicSegment dynamicSegment = new DynamicSegment() { SessionSegment = segmentDefinition, Name = segmentName };

            // Create the Segments object.
            Segment segment = new Segment() { DynamicSegment = dynamicSegment };

            return segment;
        }

        public static void multipleSegmentsRequest(AnalyticsReportingService analyticsreporting)
        {
            if (analyticsreporting == null)
                throw new ArgumentException("Reporting service required");

            // Create the DateRange object.
            DateRange dateRange = new DateRange() { StartDate = "2015-06-15", EndDate = "2015-06-30" };

            // Create the Metrics object.
            Metric sessions = new Metric() { Expression = "ga:sessions", Alias = "sessions" };

            Dimension browser = new Dimension() { Name = "ga:browser" };

            Dimension segmentDimensions = new Dimension() { Name = "ga:segment" };

            Segment browserSegment = buildSimpleSegment("Sessions with Safari browser", "ga:browser", "Safari");

            Segment countrySegment = buildSimpleSegment("Sessions from United States", "ga:country", "United States");

            // Create the ReportRequest object.            
            ReportRequest request = new ReportRequest()
            {
                ViewId = "XXXX",
                DateRanges = new List<DateRange> { dateRange },
                Dimensions = new List<Dimension> { browser, segmentDimensions },
                Segments = new List<Segment> { browserSegment, countrySegment },
                Metrics = new List<Metric> { sessions }
            };


            // Create the GetReportsRequest object.
            GetReportsRequest getReport = new GetReportsRequest() { ReportRequests = new List<ReportRequest> { request } };

            // Call the batchGet method.
            GetReportsResponse response = analyticsreporting.Reports.BatchGet(getReport).Execute();

            printResults((List<Report>)response.Reports);
        }

        #endregion Multiple Segments

        #region Pivots 

        public static void pivotRequest(AnalyticsReportingService analyticsreporting)
        {
            if (analyticsreporting == null)
                throw new ArgumentException("Reporting service required");


            // Create the DateRange object.
            DateRange dateRange = new DateRange() { StartDate = "2015-06-15", EndDate = "2015-06-30" };

            // Create the Metric objects.
            Metric sessions = new Metric() { Expression = "ga:sessions", Alias = "sessions" };

            Metric pageviews = new Metric() { Expression = "ga:pageviews", Alias = "pageviews" };

            // Create the Dimension objects.
            Dimension browser = new Dimension() { Name = "ga:browser" };
            Dimension campaign = new Dimension() { Name = "ga:campaign" };
            Dimension age = new Dimension() { Name = "ga:userAgeBracket" };

            // Create the Pivot object.
            Pivot pivot = new Pivot() { Dimensions = new List<Dimension> { age }, MaxGroupCount = 3, StartGroup = 0, Metrics = new List<Metric> { sessions, pageviews } };

            // Create the ReportRequest object.       
            ReportRequest request = new ReportRequest()
            {
                ViewId = "XXXX",
                DateRanges = new List<DateRange> { dateRange },
                Dimensions = new List<Dimension> { browser, campaign },
                Pivots = new List<Pivot> { pivot },
                Metrics = new List<Metric> { sessions }
            };


            // Create the GetReportsRequest object.
            GetReportsRequest getReport = new GetReportsRequest() { ReportRequests = new List<ReportRequest> { request } };

            // Call the batchGet method.
            GetReportsResponse response = analyticsreporting.Reports.BatchGet(getReport).Execute();

            printResults((List<Report>)response.Reports);
        }

        #endregion Pivots

        #region Cohorts



        private static void cohortRequest(AnalyticsReportingService analyticsreporting)
        {
            if (analyticsreporting == null)
                throw new ArgumentException("Reporting service required");


            // Create the ReportRequest object.
            ReportRequest request = new ReportRequest() { ViewId = "XXXX" };

            // Set the cohort dimensions
            request.Dimensions  = new List<Dimension> { new Dimension() { Name = "ga:cohort" }, new Dimension() { Name = "ga:cohortNthWeek" } } ;

            // Set the cohort metrics
            request.Metrics = new List<Metric> { new Metric() { Expression = "ga:cohortTotalUsersWithLifetimeCriteria" }, new Metric() { Expression = "ga:cohortRevenuePerUser" } };

            // Create the first cohort
            Cohort cohort1 =  new Cohort() { Name = "cohort_1", Type = "FIRST_VISIT_DATE", DateRange = new DateRange() { StartDate = "2015-08-01", EndDate = "2015-09-01" } };

            // Create the second cohort which only differs from the first one by the date range
            Cohort cohort2 = new Cohort() { Name = "cohort21", Type = "FIRST_VISIT_DATE", DateRange = new DateRange() { StartDate = "2015-07-01", EndDate = "2015-08-01" } };

            // Create the cohort group
            CohortGroup cohortGroup = new CohortGroup();
            cohortGroup.Cohorts = new List<Cohort> { cohort1, cohort2 }; 
            cohortGroup.LifetimeValue = true;
            request.CohortGroup = cohortGroup;

            // Create the GetReportsRequest object.
            GetReportsRequest getReport = new GetReportsRequest() { ReportRequests = new List<ReportRequest> { request };

            // Call the batchGet method.
            GetReportsResponse response = analyticsreporting.Reports.BatchGet(getReport).Execute();

            printResults((List<Report>)response.Reports);
        }

        #endregion


    }
}
