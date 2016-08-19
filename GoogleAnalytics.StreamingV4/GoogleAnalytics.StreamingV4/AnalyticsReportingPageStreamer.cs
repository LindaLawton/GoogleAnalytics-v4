/*
Copyright 2016 Google Inc

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
using System.Threading;
using System.Threading.Tasks;

namespace GoogleAnalytics.StreamingV4
{
    // TODO(jskeet): Make sure one of our samples uses this.

    /// <summary>
    /// A page streamer is a helper to provide both synchronous and asynchronous page streaming
    /// of a listable or queryable resource.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The expected usage pattern is to create a single paginator for a resource collection,
    /// and then use the instance methods to obtain paginated results.
    /// </para>
    /// </remarks>
    /// <example>
    /// To construct a page streamer to return snippets from the YouTube v3 Data API, you might use code
    /// such as the following. The pattern for other APIs would be very similar, with the <c>request.PageToken</c>,
    /// <c>response.NextPageToken</c> and <c>response.Items</c> properties potentially having different names. Constructing
    /// the page streamer doesn't require any service references or authentication, so it's completely safe to perform this
    /// in a type initializer.
    /// <code><![CDATA[
    /// using Google.Apis.YouTube.v3;
    /// using Google.Apis.YouTube.v3.Data;
    /// ...
    /// private static readonly snippetPageStreamer = new PageStreamer<SearchResult, SearchResource.ListRequest, SearchListResponse, string>(
    ///     (request, token) => request.PageToken = token,
    ///     response => response.NextPageToken,
    ///     response => response.Items);
    /// ]]></code>
    /// </example>
    /// <typeparam name="TResource">The type of resource being paginated</typeparam>
    /// <typeparam name="TRequest">The type of request used to fetch pages</typeparam>
    /// <typeparam name="TResponse">The type of response obtained when fetching pages</typeparam>
    /// <typeparam name="TToken">The type of the "next page token", which must be a reference type;
    /// a null reference for a token indicates the end of a stream of pages.</typeparam>
    public sealed class AnalyticsReportingPageStreamer<TResource, TRequest, TResponse, TToken>
        where TToken : class
        where TRequest : IClientServiceRequest<TResponse>
    {
        // Simple way of avoiding NullReferenceException if the response extractor returns null.
        private static readonly TResource[] emptyResources = new TResource[0];

        private readonly Action<ReportRequest, TToken> requestModifier;
        private readonly Func<Report, TToken> tokenExtractor;

        private readonly Func<GetReportsResponse, IEnumerable<TResource>> resourceExtractor;

        /// <summary>
        /// Creates a paginator for later use.
        /// </summary>
        /// <param name="requestModifier">Action to modify a request to include the specified page token.
        /// Must not be null.</param>
        /// <param name="tokenExtractor">Function to extract the next page token from a response.
        /// Must not be null.</param>
        /// <param name="resourceExtractor">Function to extract a sequence of resources from a response.
        /// Must not be null, although it can return null if it is passed a response which contains no
        /// resources.</param>
        public AnalyticsReportingPageStreamer(
            Action<ReportRequest, TToken> requestModifier,
            Func<Report, TToken> tokenExtractor,
            Func<GetReportsResponse, IEnumerable<TResource>> resourceExtractor)
        {
            if (requestModifier == null)
            {
                throw new ArgumentNullException("requestProvider");
            }
            if (tokenExtractor == null)
            {
                throw new ArgumentNullException("tokenExtractor");
            }
            if (resourceExtractor == null)
            {
                throw new ArgumentNullException("resourceExtractor");
            }
            this.requestModifier = requestModifier;
            this.tokenExtractor = tokenExtractor;
            this.resourceExtractor = resourceExtractor;
        }

        /// <summary>
        /// Lazily fetches resources a page at a time.
        /// </summary>
        /// <param name="request">The initial request to send. If this contains a page token,
        /// that token is maintained. This will be modified with new page tokens over time, and should not
        /// be changed by the caller. (The caller should clone the request if they want an independent object
        /// to use in other calls or to modify.) Must not be null.</param>
        /// <returns>A sequence of resources, which are fetched a page at a time. Must not be null.</returns>
        public IEnumerable<Report> Fetch(ReportsResource resource, GetReportsRequest reports)
        {        
            if (resource == null)
            {
                throw new ArgumentNullException("resource");
            }
            if (reports == null)
            {
                throw new ArgumentNullException("reports");
            }
            bool moredata = false;

            do
            {

                // If there are reports to process
                if (reports.ReportRequests.Count != 0)
                {

                    // Make the request for the reports.
                    GetReportsResponse response = resource.BatchGet(reports).Execute();

                    // Return the data.
                    foreach (var item in response.Reports)
                    {
                        yield return item;
                    }


                    // Loop though each of the reports Looking for Next links
                    for (int i = 0; i < reports.ReportRequests.Count; i++)
                    {
                        var token = tokenExtractor(response.Reports[i]);
                        if (token != null)
                        {
                            requestModifier(reports.ReportRequests[i], token);
                            moredata = true;
                        }
                        else
                        {
                            Console.WriteLine("Removed a completed report.");
                            reports.ReportRequests.Remove(reports.ReportRequests[i]);
                        }

                    }
                }
                else {
                    moredata = false;
                }
            } while (moredata);
        }

        /// <summary>
        /// Asynchronously (but eagerly) fetches a complete set of resources, potentially making multiple requests.
        /// </summary>
        /// <param name="request">The initial request to send. If this contains a page token,
        /// that token is maintained. This will be modified with new page tokens over time, and should not
        /// be changed by the caller. (The caller should clone the request if they want an independent object
        /// to use in other calls or to modify.) Must not be null.</param>
        /// <returns>A sequence of resources, which are fetched asynchronously and a page at a time.</returns>
        /// <param name="cancellationToken"></param>
        /// <returns>A task whose result (when complete) is the complete set of results fetched starting with the given
        /// request, and continuing to make further requests until a response has no "next page" token.</returns>
        public async Task<IList<TResource>> FetchAllAsync(ReportsResource resource, GetReportsRequest reports,
            CancellationToken cancellationToken)
        {
            if (resource == null)
            {
                throw new ArgumentNullException("resource");
            }
            if (reports == null)
            {
                throw new ArgumentNullException("reports");
            }
            var results = new List<TResource>();
            //TToken token;
            bool moredata = false;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                //GetReportsResponse response = resource.BatchGet(reports).Execute();

                GetReportsResponse response = await resource.BatchGet(reports).ExecuteAsync(cancellationToken).ConfigureAwait(false);

                // Loop though each of the reports
                for (int i = 0; i < reports.ReportRequests.Count; i++)
                {

                    var tt = tokenExtractor(response.Reports[i]);
                    //requestModifier(response.Reports[i], tt);
                    // Is there a next page for this report
                    var token = response.Reports[i].NextPageToken;

                    if (token != null)
                    {
                        // add the next page to the report for the next request
                        reports.ReportRequests[i].PageToken = token;
                        moredata = true;
                        // Note remove requests that no longer have more rows??
                    }
                }

                results.AddRange(resourceExtractor(response) ?? emptyResources);

            } while (moredata);
            return results;
        }
    }
}