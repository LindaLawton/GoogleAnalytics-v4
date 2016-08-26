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
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoogleAnalytics.StreamingV4
{
    /// <summary>
    /// This class should make it easer to access the metaData api.
    /// </summary>
    public class MetaDataHelper
    {
        enum reporttype { ga };
        enum attributes { type, uiName };
        enum metadataTypes { DIMENSION, METRIC };
        private string apiKey { get; set; }
        private Columns columns;

        /// <summary>
        /// List of all of the columns returned by the metaData api
        /// </summary>
        public Columns Columns
        {
            get
            {
                if (this.columns == null)
                {
                    this.getAllMetadata();
                }
                return this.columns;
            }
        }


        /// <summary>
        /// List of all the dimensions returned by the metaData api
        /// </summary>
        /// <returns></returns>
        public List<Column> getDimensions()
        {
            return ListDimensions(this.Columns).Items.ToList();
        }

        /// <summary>
        /// List of all the dimensions returned by the metaData api as Dimensions for easier use of the V4 api
        /// </summary>
        /// <returns></returns>
        public List<Dimension> getReportingDimensions()
        {

            List<Dimension> result = new List<Dimension>();
            foreach (var dim in ListDimensions(this.Columns).Items.ToList())
            {

                result.Add(new Dimension { Name = dim.Id });
            }

            return result;
        }

        /// <summary>
        /// List of all the metrics returned by the metaData api
        /// </summary>
        /// <returns></returns>
        public List<Column> getMetrics()
        {
            return ListMetrics(this.Columns).Items.ToList();
        }

        /// <summary>
        /// List of all the Metrics returned by the metaData api as Dimensions for easier use of the V4 api
        /// </summary>
        /// <returns></returns>
        public List<Metric> getReportingMetrics()
        {
            List<Metric> result = new List<Metric>();
            foreach (var dim in ListDimensions(this.Columns).Items.ToList())
            {
                result.Add(new Metric { Expression = dim.Id, Alias = dim.Attributes.Where(a => a.Key == attributes.uiName.ToString()).FirstOrDefault().Value } ) ;
            }

            return result;
        }

       

        /// <summary>
        /// Authentcated Analytics Service.
        /// </summary>
        private AnalyticsMetaDataService service;
        public AnalyticsMetaDataService Service
        {
            get
            {
                if (this.service == null)
                {
                    this.setService();
                }
                return service;
            }
        }

        /// <summary>
        /// Constructor requires public api key to access the api.  
        /// Created on Google Developers console https://console.developers.google.com
        /// Enable Analytics API.
        /// </summary>
        /// <param name="apiKey"></param>
        public MetaDataHelper(string apiKey)
        {
            this.apiKey = apiKey;
        }

        /// <summary>
        /// Creates the Analytics MetaData service used to connect to Google Analytics.
        /// </summary>
        public void setService()
        {
            var metadataService = new AnalyticsMetaDataService(new BaseClientService.Initializer()
            {
                ApiKey = this.apiKey,
                ApplicationName = "Metadata api",
            });

            this.service = metadataService;
        }

        #region metaData

        /// <summary>
        /// Gets all of the metaData columns.
        /// </summary>
        private void getAllMetadata()
        {
            try
            {
                this.columns = Service.Metadata.Columns.List(reporttype.ga.ToString()).Execute();
            }
            catch (Exception ex) {
                throw new Exception("Failed to retrieve MetaData", ex);
            }
        }
        #endregion metaData

        #region Dimensions

        /// <summary>
        /// Returns a list of only the dimensions from the metadata api.  A new request is sent to the metaData API every time this
        /// method is called.
        /// </summary>
        /// <param name="service">Authencated AnalyticsMetaDataService</param>
        /// <returns>columns</returns>
        public Columns ListDimensions(AnalyticsMetaDataService service)
        {
            // Get the list of all of the Metadata
            var result = service.Metadata.Columns.List(reporttype.ga.ToString()).Execute();

            return MetaDataHelper.ListDimensions(result);
        }

        /// <summary>
        /// Returns a list of the dimensions from the metaData api.  A new request is only sent to the metaData api the first time a request
        /// is made against the helper class.   (Helps to save on quota)
        /// </summary>
        /// <param name="result">a Columns object response from the metaData API</param>
        /// <returns>columns</returns>
        private static Columns ListDimensions(Columns result)
        {
            // remove the items we dont want
            var list = result.Items.ToList();
            list.RemoveAll(a => !a.Attributes.Any(b => b.Key == attributes.type.ToString() && b.Value == metadataTypes.DIMENSION.ToString()));

            // apply the results.
            result.Items = list;
            return result;
        }

        #endregion Dimensions

        #region Metrics

        /// <summary>
        /// Returns a list of only the metrics from the metadata api.  A new request is sent to the metaData API every time this
        /// method is called.
        /// </summary>
        /// <param name="service">Authencated AnalyticsMetaDataService</param>
        /// <returns>columns</returns>
        public static Columns ListMetrics(AnalyticsMetaDataService service)
        {
            // Get the list of all of the Metadata
            var result = service.Metadata.Columns.List(reporttype.ga.ToString()).Execute();

            return MetaDataHelper.ListMetrics(result);
        }

        /// <summary>
        /// Returns a list of the dimensions from the metaData api.  A new request is only sent to the metaData api the first time a request
        /// is made against the helper class.   (Helps to save on quota)
        /// </summary>
        /// <param name="result">a Columns object response from the metaData API</param>
        /// <returns>columns</returns>
        private static Columns ListMetrics(Columns result)
        {
            // remove the items we dont want
            var list = result.Items.ToList();
            list.RemoveAll(a => !a.Attributes.Any(b => b.Key == attributes.type.ToString() && b.Value == metadataTypes.METRIC.ToString()));

            // apply the results.
            result.Items = list;
            return result;
        }

        #endregion metrics

        #region search

        /// <summary>
        /// Returns a list of dimensions and metrics from the metadata api. Searching on uiName and Id. A new request is sent to the metaData API every time this
        /// method is called.
        /// </summary>
        /// <param name="service">Authencated AnalyticsMetaDataService</param>
        /// <param name="name">String you want to search for</param>
        /// <returns>columns</returns>
        public static Columns FindMetadataByName(AnalyticsMetaDataService service, string name)
        {
            // Get the list of all of the Metadata
            var result = service.Metadata.Columns.List(reporttype.ga.ToString()).Execute();

            return MetaDataHelper.FindMetadataByName(result, name);
        }

        /// <summary>
        /// Returns a list of dimensions and metrics from the metadata api. Searching on uiName and Id. A new request is sent to the metaData API every time this
        /// method is called.
        /// </summary>
        /// <param name="result">a Columns object response from the metaData API</param>
        /// <param name="name">String you want to search for</param>
        /// <returns>columns</returns>
        private static Columns FindMetadataByName(Columns data, string name)
        {
            // remove the items we dont want
            var list = data.Items.ToList();
            var items = list.Where(a => a.Id.ToLower().Contains(name.ToLower()) || a.Attributes.Any(b => b.Key == attributes.uiName.ToString() && b.Value.ToLower().Contains(name.ToLower()))).ToList();

            // apply the results.
            data.Items = items;
            return data;
        }

        /// <summary>
        /// Returns a list of dimensions and metrics from the metadata api. Searching on uiName and Id.  A new request is only sent to the metaData api the first time a request
        /// is made against the helper class.   (Helps to save on quota)
        /// </summary>
        /// <param name="name">String you want to search for</param>
        /// <returns>columns</returns>
        public Columns FindMetadataByName(string name)
        {
            return MetaDataHelper.FindMetadataByName(this.Columns, name);
        }
        #endregion search
    }
}
