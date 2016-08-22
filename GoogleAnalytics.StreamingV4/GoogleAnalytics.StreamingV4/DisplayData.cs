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
using System.Threading.Tasks;

namespace GoogleAnalytics.StreamingV4
{
  public class DisplayData
    {
        /// <summary>
        /// Just used to display the data returned.
        /// </summary>
        /// <param name="report"></param>
        /// <param name="dateRanges"></param>
        public static void ShowData(Report report, List<DateRange> dateRanges) {

            // Displaying a header so we know which report this data is for
            var reportDimensions = string.Join(",", report.ColumnHeader.Dimensions);
            var reportMetrics = string.Join(",", report.ColumnHeader.MetricHeader.MetricHeaderEntries.Select(m => m.Name).ToList());
            Console.WriteLine(string.Format("Results for Report: Dimensons: {0} :Metrics: {1}", reportDimensions, reportMetrics));

            ColumnHeader header = report.ColumnHeader;
            List<String> dimensionHeaders = (List<String>)header.Dimensions;
            List<MetricHeaderEntry> metricHeaders = (List<MetricHeaderEntry>)header.MetricHeader.MetricHeaderEntries;

            // Looping though row data
            foreach (var row in report.Data.Rows)
            {
                List<String> dimensions = (List<String>)row.Dimensions;
                List<DateRangeValues> metrics = (List<DateRangeValues>)row.Metrics;

                // Looping though the date range to match the data with the range.
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
