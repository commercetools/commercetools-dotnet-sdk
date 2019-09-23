﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace commercetools.Common
{
    /// <summary>
    /// A set of configuration variables needed for making requests with the client.
    /// </summary>
    public class Configuration
    {
        private ProjectScope _projectScope;
        private string _projectKey;

        #region Properties

        public string OAuthUrl { get; set; }
        public string ApiUrl { get; set; }

        public string ProjectKey
        {
            get => _projectKey;
            set {
                _projectKey = value;
                if (ScopeString == null)
                {
                    ScopeString = ToScopeString(_projectScope, value);
                }
            }
        }

        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public ProjectScope Scope
        {
            get { return _projectScope; }
            set
            {
                _projectScope = value;
                if (_projectKey != null)
                {
                    ScopeString = ToScopeString(value, _projectKey);
                }
            }
        }

        public static string ToScopeString(ProjectScope value, string projectKey)
        {
            return string.Concat(value.ToEnumMemberString(), ":", projectKey);
        }

        public string ScopeString { get; set; }

        public int InternalServerErrorRetries { get; set; }
        public int InternalServerErrorRetryInterval { get; set; }
        public TimeSpan HttpClientPoolItemLifetime { get; set; }
        #endregion

        #region Constructors

        public Configuration()
        {
            this.InternalServerErrorRetries = 1;
            this.InternalServerErrorRetryInterval = 100;
            this.HttpClientPoolItemLifetime = TimeSpan.FromHours(1);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oAuthUrl"></param>
        /// <param name="apiUrl"></param>
        /// <param name="projectKey"></param>
        /// <param name="clientID"></param>
        /// <param name="clientSecret"></param>
        /// <param name="scope"></param>
        /// <param name="internalServerErrorRetries">Used to specify amount of retries when an internal server error occurs</param>
        /// <param name="internalServerErrorRetryInterval">Used to specify the amount of time in milliseconds to wait between retries when an internal server error occurs</param>
        /// <param name="httpClientPoolItemLifetime">Used to specify the timespan to wait before disposing an HttpClient LimitedPoolItem</param>
        public Configuration(string oAuthUrl, string apiUrl, string projectKey, string clientID, string clientSecret, ProjectScope scope, int internalServerErrorRetries = 1, int internalServerErrorRetryInterval = 100, TimeSpan? httpClientPoolItemLifetime = null)
        {
            this.OAuthUrl = oAuthUrl;
            this.ApiUrl = apiUrl;
            this.ProjectKey = projectKey;
            this.ClientID = clientID;
            this.ClientSecret = clientSecret;
            this.Scope = scope;
            this.InternalServerErrorRetries = internalServerErrorRetries;
            this.InternalServerErrorRetryInterval = internalServerErrorRetryInterval;
            this.HttpClientPoolItemLifetime = httpClientPoolItemLifetime ?? TimeSpan.FromHours(1);

            if (this.OAuthUrl.EndsWith("/"))
            {
                this.OAuthUrl = this.OAuthUrl.Remove(this.OAuthUrl.Length - 1);
            }

            if (this.ApiUrl.EndsWith("/"))
            {
                this.ApiUrl = apiUrl.Remove(this.ApiUrl.Length - 1);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oAuthUrl"></param>
        /// <param name="apiUrl"></param>
        /// <param name="projectKey"></param>
        /// <param name="clientID"></param>
        /// <param name="clientSecret"></param>
        /// <param name="scope"></param>
        /// <param name="internalServerErrorRetries">Used to specify amount of retries when an internal server error occurs</param>
        /// <param name="internalServerErrorRetryInterval">Used to specify the amount of time in milliseconds to wait between retries when an internal server error occurs</param>
        /// <param name="httpClientPoolItemLifetime">Used to specify the timespan to wait before disposing an HttpClient LimitedPoolItem</param>
        public Configuration(string oAuthUrl, string apiUrl, string projectKey, string clientID, string clientSecret, string scope, int internalServerErrorRetries = 1, int internalServerErrorRetryInterval = 100, TimeSpan? httpClientPoolItemLifetime = null)
        {
            this.OAuthUrl = oAuthUrl;
            this.ApiUrl = apiUrl;
            this.ProjectKey = projectKey;
            this.ClientID = clientID;
            this.ClientSecret = clientSecret;
            this.ScopeString = scope;
            this.InternalServerErrorRetries = internalServerErrorRetries;
            this.InternalServerErrorRetryInterval = internalServerErrorRetryInterval;
            this.HttpClientPoolItemLifetime = httpClientPoolItemLifetime ?? TimeSpan.FromHours(1);

            if (this.OAuthUrl.EndsWith("/"))
            {
                this.OAuthUrl = this.OAuthUrl.Remove(this.OAuthUrl.Length - 1);
            }

            if (this.ApiUrl.EndsWith("/"))
            {
                this.ApiUrl = apiUrl.Remove(this.ApiUrl.Length - 1);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oAuthUrl"></param>
        /// <param name="apiUrl"></param>
        /// <param name="projectKey"></param>
        /// <param name="clientID"></param>
        /// <param name="clientSecret"></param>
        /// <param name="scopes"></param>
        /// <param name="internalServerErrorRetries">Used to specify amount of retries when an internal server error occurs</param>
        /// <param name="internalServerErrorRetryInterval">Used to specify the amount of time in milliseconds to wait between retries when an internal server error occurs</param>
        /// <param name="httpClientPoolItemLifetime">Used to specify the timespan to wait before disposing an HttpClient LimitedPoolItem</param>
        public Configuration(string oAuthUrl, string apiUrl, string projectKey, string clientID, string clientSecret, List<ProjectScope> scopes, int internalServerErrorRetries = 1, int internalServerErrorRetryInterval = 100, TimeSpan? httpClientPoolItemLifetime = null)
        {
            this.OAuthUrl = oAuthUrl;
            this.ApiUrl = apiUrl;
            this.ProjectKey = projectKey;
            this.ClientID = clientID;
            this.ClientSecret = clientSecret;
            this.ScopeString = string.Join(" ",scopes.Select(scope => ToScopeString(scope, projectKey)));
            this.InternalServerErrorRetries = internalServerErrorRetries;
            this.InternalServerErrorRetryInterval = internalServerErrorRetryInterval;
            this.HttpClientPoolItemLifetime = httpClientPoolItemLifetime ?? TimeSpan.FromHours(1);

            if (this.OAuthUrl.EndsWith("/"))
            {
                this.OAuthUrl = this.OAuthUrl.Remove(this.OAuthUrl.Length - 1);
            }

            if (this.ApiUrl.EndsWith("/"))
            {
                this.ApiUrl = apiUrl.Remove(this.ApiUrl.Length - 1);
            }
        }

        #endregion

    }
}
