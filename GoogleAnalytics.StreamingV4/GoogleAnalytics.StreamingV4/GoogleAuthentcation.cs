﻿/*
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
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Threading;

namespace GoogleAnalytics.StreamingV4
{
  
    public class GoogleAuthentcation
    {
        /// <summary>
        /// Authenticate to Google Using Oauth2
        /// Documentation https://developers.google.com/accounts/docs/OAuth2
        /// Credentials are stored in System.Environment.SpecialFolder.Personal
        /// </summary>
        /// <param name="clientId">From Google Developer console https://console.developers.google.com</param>
        /// <param name="clientSecret">From Google Developer console https://console.developers.google.com</param>
        /// <param name="userName">Identifying string for the user who is being authentcated.</param>
        /// <returns>SheetsService used to make requests against the Sheets API</returns>
        public static AnalyticsReportingService AuthenticateOauth(string clientId, string clientSecret, string userName)
        {
            try
            {

                if (string.IsNullOrEmpty(clientId))
                    throw new Exception("clientId is required.");
                if (string.IsNullOrEmpty(clientSecret))
                    throw new Exception("clientSecret is required.");
                if (string.IsNullOrEmpty(userName))
                    throw new Exception("userName is required for datastore.");

                // These are the scopes of permissions you need. It is best to request only what you need and not all of them
                string[] scopes = new string[] { AnalyticsReportingService.Scope.AnalyticsReadonly };          // View and manage your spreadsheets in Google Drive
                string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                credPath = System.IO.Path.Combine(credPath, ".credentials/apiName");

                // Requesting Authentication or loading previously stored authentication for userName
                var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets { ClientId = clientId, ClientSecret = clientSecret }
                                                                                             , scopes
                                                                                             , userName
                                                                                             , CancellationToken.None
                                                                                             , new FileDataStore(credPath, true)).Result;
                // Returning the SheetsService
                return new AnalyticsReportingService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = string.Format("{0} Authentication", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("AuthenticateOauth failed: {0}",ex.Message));
                throw new Exception("RequestAuthentcationFailed", ex);
            }

        }
    }
}
