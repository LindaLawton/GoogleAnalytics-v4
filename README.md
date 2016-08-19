# GoogleAnalytics-v4-Streamer

How to use pagination with the [Google Analytics Reporting API V4](https://developers.google.com/analytics/devguides/reporting/core/v4/).

This project uses the Google .Net Client library.   The library supports pagination in standard APIs using the pagestreamer class.   
After some testing I found that this was not going to work as well with the New Google Analytics reporting API due to its batching nature. 

I have a issue request with the Google .net Client library team trying to address this issue.  [Issues with PageStreamer and the Google Analytics reporting api v4 #818](https://github.com/google/google-api-dotnet-client/issues/818)

Include in this project are two options for getting it to work.

#Using Standard streamer:

This works but requires a change to the AnalyticsReportingService.cs to expose the body paramater.  I have included the file in the project.  It also only supports pagination over the first report.  Any other reports are just going to request the same page again and again.  This is bad becouse it will cause you to use up quota to needlessly.

#AnalyticsReportingPageStreamer.cs

I have created a new version of the PageStreamer class which supports the batch nature of the Google Analytics Reporting API v4.   It will work with multiple reports and does not require any changes to the  AnalyticsReportingService.cs.

However becouse of the fact that this will be API speciffic I dont think it will ever be released along with the client library itself.
