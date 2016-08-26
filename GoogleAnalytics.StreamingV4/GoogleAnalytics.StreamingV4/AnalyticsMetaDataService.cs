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

using Google.Apis.Discovery;
using Google.Apis.Requests;
using Google.Apis.Services;
using Google.Apis.Util;

namespace GoogleAnalytics.StreamingV4
{
    /// <summary>The Analytics MetaData Service.
    /// 
    /// The majority of this code was extracted from the Google Analytics V3 class 
    /// 
    /// </summary>
    public class AnalyticsMetaDataService : BaseClientService
    {
        /// <summary>The API version.</summary>
        public const string Version = "v3";

        /// <summary>The discovery version used to generate this service.</summary>
        public static Google.Apis.Discovery.DiscoveryVersion DiscoveryVersionUsed = Google.Apis.Discovery.DiscoveryVersion.Version_1_0;

        /// <summary>Constructs a new service.</summary>
        public AnalyticsMetaDataService() :
            this(new Initializer())
        { }

        /// <summary>Constructs a new service.</summary>
        /// <param name="initializer">The service initializer.</param>
        public AnalyticsMetaDataService(BaseClientService.Initializer initializer)
            : base(initializer)
        {
            metadata = new MetadataResource(this);
        }

        /// <summary>Gets the service supported features.</summary>
        public override System.Collections.Generic.IList<string> Features
        {
            get { return new string[0]; }
        }

        /// <summary>Gets the service name.</summary>
        public override string Name
        {
            get { return "analyticsmetadata"; }
        }

        /// <summary>Gets the service base URI.</summary>
        public override string BaseUri
        {
            get { return "https://www.googleapis.com/analytics/v3/"; }
        }

        /// <summary>Gets the service base path.</summary>
        public override string BasePath
        {
            get { return "analytics/v3/"; }
        }

        private readonly MetadataResource metadata;

        /// <summary>Gets the Metadata resource.</summary>
        public virtual MetadataResource Metadata
        {
            get { return metadata; }
        }

    }

    ///<summary>A base abstract class for Analytics requests.</summary>
    public abstract class AnalyticsBaseServiceRequest<TResponse> : ClientServiceRequest<TResponse>
    {
        ///<summary>Constructs a new AnalyticsBaseServiceRequest instance.</summary>
        protected AnalyticsBaseServiceRequest(IClientService service)
            : base(service)
        {
        }

        /// <summary>Data format for the response.</summary>
        /// [default: json]
        [RequestParameterAttribute("alt", RequestParameterType.Query)]
        public virtual System.Nullable<AltEnum> Alt { get; set; }

        /// <summary>Data format for the response.</summary>
        public enum AltEnum
        {
            /// <summary>Responses with Content-Type of application/json</summary>
            [StringValueAttribute("json")]
            Json,
        }

        /// <summary>Selector specifying which fields to include in a partial response.</summary>
        [RequestParameterAttribute("fields", RequestParameterType.Query)]
        public virtual string Fields { get; set; }

        /// <summary>API key. Your API key identifies your project and provides you with API access, quota, and reports.
        /// Required unless you provide an OAuth 2.0 token.</summary>
        [RequestParameterAttribute("key", RequestParameterType.Query)]
        public virtual string Key { get; set; }

        /// <summary>OAuth 2.0 token for the current user.</summary>
        [RequestParameterAttribute("oauth_token", RequestParameterType.Query)]
        public virtual string OauthToken { get; set; }

        /// <summary>Returns response with indentations and line breaks.</summary>
        /// [default: false]
        [RequestParameterAttribute("prettyPrint", RequestParameterType.Query)]
        public virtual System.Nullable<bool> PrettyPrint { get; set; }

        /// <summary>Available to use for quota purposes for server-side applications. Can be any arbitrary string
        /// assigned to a user, but should not exceed 40 characters. Overrides userIp if both are provided.</summary>
        [RequestParameterAttribute("quotaUser", RequestParameterType.Query)]
        public virtual string QuotaUser { get; set; }

        /// <summary>IP address of the site where the request originates. Use this if you want to enforce per-user
        /// limits.</summary>
        [RequestParameterAttribute("userIp", RequestParameterType.Query)]
        public virtual string UserIp { get; set; }

        /// <summary>Initializes Analytics parameter list.</summary>
        protected override void InitParameters()
        {
            base.InitParameters();

            RequestParameters.Add(
                "alt", new Parameter
                {
                    Name = "alt",
                    IsRequired = false,
                    ParameterType = "query",
                    DefaultValue = "json",
                    Pattern = null,
                });
            RequestParameters.Add(
                "fields", new Parameter
                {
                    Name = "fields",
                    IsRequired = false,
                    ParameterType = "query",
                    DefaultValue = null,
                    Pattern = null,
                });
            RequestParameters.Add(
                "key", new Parameter
                {
                    Name = "key",
                    IsRequired = false,
                    ParameterType = "query",
                    DefaultValue = null,
                    Pattern = null,
                });
            RequestParameters.Add(
                "oauth_token", new Parameter
                {
                    Name = "oauth_token",
                    IsRequired = false,
                    ParameterType = "query",
                    DefaultValue = null,
                    Pattern = null,
                });
            RequestParameters.Add(
                "prettyPrint", new Parameter
                {
                    Name = "prettyPrint",
                    IsRequired = false,
                    ParameterType = "query",
                    DefaultValue = "false",
                    Pattern = null,
                });
            RequestParameters.Add(
                "quotaUser", new Parameter
                {
                    Name = "quotaUser",
                    IsRequired = false,
                    ParameterType = "query",
                    DefaultValue = null,
                    Pattern = null,
                });
            RequestParameters.Add(
                "userIp", new Parameter
                {
                    Name = "userIp",
                    IsRequired = false,
                    ParameterType = "query",
                    DefaultValue = null,
                    Pattern = null,
                });
        }
    }


    /// <summary>The "metadata" collection of methods.</summary>
    public class MetadataResource
    {
        private const string Resource = "metadata";

        /// <summary>The service which this resource belongs to.</summary>
        private readonly IClientService service;

        /// <summary>Constructs a new resource.</summary>
        public MetadataResource(IClientService service)
        {
            this.service = service;
            columns = new ColumnsResource(service);

        }

        private readonly ColumnsResource columns;

        /// <summary>Gets the Columns resource.</summary>
        public virtual ColumnsResource Columns
        {
            get { return columns; }
        }

        /// <summary>The "columns" collection of methods.</summary>
        public class ColumnsResource
        {
            private const string Resource = "columns";

            /// <summary>The service which this resource belongs to.</summary>
            private readonly IClientService service;

            /// <summary>Constructs a new resource.</summary>
            public ColumnsResource(IClientService service)
            {
                this.service = service;

            }

            /// <summary>Lists all columns for a report type</summary>
            /// <param name="reportType">Report type. Allowed Values: 'ga'. Where 'ga' corresponds to the Core Reporting
            /// API</param>
            public virtual ListRequest List(string reportType)
            {
                return new ListRequest(service, reportType);
            }

            /// <summary>Lists all columns for a report type</summary>
            public class ListRequest : AnalyticsBaseServiceRequest<Columns>
            {
                /// <summary>Constructs a new List request.</summary>
                public ListRequest(IClientService service, string reportType)
                    : base(service)
                {
                    ReportType = reportType;
                    InitParameters();
                }


                /// <summary>Report type. Allowed Values: 'ga'. Where 'ga' corresponds to the Core Reporting
                /// API</summary>
                [RequestParameterAttribute("reportType", RequestParameterType.Path)]
                public virtual string ReportType { get; private set; }


                ///<summary>Gets the method name.</summary>
                public override string MethodName
                {
                    get { return "list"; }
                }

                ///<summary>Gets the HTTP method.</summary>
                public override string HttpMethod
                {
                    get { return "GET"; }
                }

                ///<summary>Gets the REST path.</summary>
                public override string RestPath
                {
                    get { return "metadata/{reportType}/columns"; }
                }

                /// <summary>Initializes List parameter list.</summary>
                protected override void InitParameters()
                {
                    base.InitParameters();

                    RequestParameters.Add(
                        "reportType", new Parameter
                        {
                            Name = "reportType",
                            IsRequired = true,
                            ParameterType = "path",
                            DefaultValue = null,
                            Pattern = @"ga",
                        });
                }

            }
        }
    }

    #region MetaData Response

    /// <summary>JSON template for a metadata column.</summary>
    public class Column : IDirectResponseSchema
    {
        /// <summary>Map of attribute name and value for this column.</summary>
        [Newtonsoft.Json.JsonPropertyAttribute("attributes")]
        public virtual System.Collections.Generic.IDictionary<string, string> Attributes { get; set; }

        /// <summary>Column id.</summary>
        [Newtonsoft.Json.JsonPropertyAttribute("id")]
        public virtual string Id { get; set; }

        /// <summary>Resource type for Analytics column.</summary>
        [Newtonsoft.Json.JsonPropertyAttribute("kind")]
        public virtual string Kind { get; set; }

        /// <summary>The ETag of the item.</summary>
        public virtual string ETag { get; set; }
    }

    /// <summary>Lists columns (dimensions and metrics) for a particular report type.</summary>
    public class Columns : IDirectResponseSchema
    {
        /// <summary>List of attributes names returned by columns.</summary>
        [Newtonsoft.Json.JsonPropertyAttribute("attributeNames")]
        public virtual System.Collections.Generic.IList<string> AttributeNames { get; set; }

        /// <summary>Etag of collection. This etag can be compared with the last response etag to check if response has
        /// changed.</summary>
        [Newtonsoft.Json.JsonPropertyAttribute("etag")]
        public virtual string ETag { get; set; }

        /// <summary>List of columns for a report type.</summary>
        [Newtonsoft.Json.JsonPropertyAttribute("items")]
        public virtual System.Collections.Generic.IList<Column> Items { get; set; }

        /// <summary>Collection type.</summary>
        [Newtonsoft.Json.JsonPropertyAttribute("kind")]
        public virtual string Kind { get; set; }

        /// <summary>Total number of columns returned in the response.</summary>
        [Newtonsoft.Json.JsonPropertyAttribute("totalResults")]
        public virtual System.Nullable<int> TotalResults { get; set; }

    }
    #endregion

}