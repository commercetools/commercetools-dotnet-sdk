using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace commercetools.Common
{
    /// <summary>
    /// A client for executing requests against the commercetools web service.
    /// </summary>
    public class Client : IClient
    {
        #region Properties

        /// <summary>
        /// Configuration
        /// </summary>
        public Configuration Configuration { get; private set; }

        /// <summary>
        /// Token
        /// </summary>
        public Token Token { get; private set; }

        /// <summary>
        /// The identifiying user agent that is included in all API requests.
        /// </summary>
        public string UserAgent { get; private set; }

        /// <summary>
        /// The HttpClient to utilize in all API requests.
        /// </summary>
        private RestClient RestClientInstance { get; set; }

        public HttpClient HttpClientInstance {
            get { return RestClientInstance.Client; }
            set { RestClientInstance = new RestClient(value); }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public Client(Configuration configuration, HttpClient httpClientInstance = null)
        {
            this.Configuration = configuration;
            this.HttpClientInstance = httpClientInstance;
            Assembly assembly = Assembly.GetExecutingAssembly();
            string assemblyVersion = assembly.GetName().Version.ToString();
            string dotNetVersion = Environment.Version.ToString();
            this.UserAgent = string.Format("commercetools-dotnet-sdk/{0} .NET/{1}", assemblyVersion, dotNetVersion);
        }

        #endregion

        #region Web Service Methods

        /// <summary>
        /// Executes a GET request.
        /// </summary>
        /// <param name="endpoint">API endpoint, excluding the project key</param>
        /// <param name="values">Values</param>
        /// <returns>JSON object</returns>
        public async Task<Response<T>> GetAsync<T>(string endpoint, NameValueCollection values = null)
        {
            Response<T> response = new Response<T>();

            await EnsureToken();

            if (this.Token == null)
            {
                response.Success = false;
                response.Errors.Add(new ErrorMessage("no_token", "Could not retrieve token"));
                return response;
            }

            if (!string.IsNullOrWhiteSpace(endpoint) && !endpoint.StartsWith("/"))
            {
                endpoint = string.Concat("/", endpoint);
            }

            string url = string.Concat(this.Configuration.ApiUrl, "/", this.Configuration.ProjectKey, endpoint, values.ToQueryString());

            HttpRequestMessage httpRequestMessage = CreateRequestMessage(url, HttpMethod.Get);
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(this.Token.TokenType, this.Token.AccessToken);
            response = await SendAsync<T>(httpRequestMessage);
            return response;
        }

        /// <summary>
        /// Executes a POST request.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="payload">Body of the request</param>
        /// <returns>JSON object</returns>
        public async Task<Response<T>> PostAsync<T>(string endpoint, string payload)
        {
            Response<T> response = new Response<T>();

            await EnsureToken();

            if (this.Token == null)
            {
                response.Success = false;
                response.Errors.Add(new ErrorMessage("no_token", "Could not retrieve token"));
                return response;
            }

            if (!string.IsNullOrWhiteSpace(endpoint) && !endpoint.StartsWith("/"))
            {
                endpoint = string.Concat("/", endpoint);
            }

            string url = string.Concat(this.Configuration.ApiUrl, "/", this.Configuration.ProjectKey, endpoint);

            HttpRequestMessage httpRequestMessage = CreateRequestMessage(url, HttpMethod.Post, payload);
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(this.Token.TokenType, this.Token.AccessToken);
            response = await SendAsync<T>(httpRequestMessage);
            return response;
        }

        /// <summary>
        /// Executes a DELETE request.
        /// </summary>
        /// <param name="endpoint">API endpoint, excluding the project key</param>
        /// <param name="values">Values</param>
        /// <returns>JSON object</returns>
        public async Task<Response<T>> DeleteAsync<T>(string endpoint, NameValueCollection values = null)
        {
            Response<T> response = new Response<T>();

            await EnsureToken();

            if (this.Token == null)
            {
                response.Success = false;
                response.Errors.Add(new ErrorMessage("no_token", "Could not retrieve token"));
                return response;
            }

            if (!string.IsNullOrWhiteSpace(endpoint) && !endpoint.StartsWith("/"))
            {
                endpoint = string.Concat("/", endpoint);
            }

            string url = string.Concat(this.Configuration.ApiUrl, "/", this.Configuration.ProjectKey, endpoint, values.ToQueryString());

            HttpRequestMessage httpRequestMessage = CreateRequestMessage(url, HttpMethod.Delete);
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(this.Token.TokenType, this.Token.AccessToken);
            response = await SendAsync<T>(httpRequestMessage);
            return response;
        }

        private HttpRequestMessage CreateRequestMessage(string url, HttpMethod method, string payload = null)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, new Uri(url));
            if (payload != null)
            {
                httpRequestMessage.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            }
            httpRequestMessage.Headers.Accept.Clear();
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpRequestMessage.Headers.UserAgent.Clear();
            httpRequestMessage.Headers.UserAgent.ParseAdd(this.UserAgent);
            return httpRequestMessage;
        }

        /// <summary>
        /// Executes a request.
        /// </summary>
        /// <param name="endpoint">API endpoint, excluding the project key</param>
        /// <param name="values">Values</param>
        /// <returns>JSON object</returns>
        private async Task<Response<T>> SendAsync<T>(HttpRequestMessage httpRequestMessage)
        {
            Response<T> response = new Response<T>();

            for (int internalServerErrorRetries = -1; internalServerErrorRetries < this.Configuration.InternalServerErrorRetries; internalServerErrorRetries++)
            {
                HttpResponseMessage httpResponseMessage = await RestClientInstance.SendAsync(httpRequestMessage);
                response = await GetResponse<T>(httpResponseMessage);
                if (response.StatusCode != 503)
                {
                    return response;
                }
                else if (this.Configuration.InternalServerErrorRetryInterval > 0)
                {
                    await Task.Delay(this.Configuration.InternalServerErrorRetryInterval);
                }
            }
            return response;
        }

        #endregion

        #region Token Methods

        /// <summary>
        /// Ensures that the token for this instance has been retrieved and that it has not expired.
        /// </summary>
        private async Task EnsureToken()
        {
            if (this.Token == null || this.Token.IsExpired())
            {
                this.Token = null;
                Response<Token> tokenResponse = await GetTokenAsync();

                if (tokenResponse.Success)
                {
                    this.Token = tokenResponse.Result;
                }
            }
            /*
             * The refresh token flow is currently only available for the password flow, which is currently not supported by the SDK.
             * More info: https://dev.commercetools.com/http-api-authorization.html#password-flow
             *
                else if (this.Token.IsExpired())
                {
                    this.Token = RefreshTokenAsync(this.Token.RefreshToken);
                }
             */
        }

        /// <summary>
        /// Retrieves a token from the authorization API using the client credentials flow.
        /// </summary>
        /// <returns>Token</returns>
        /// <see href="http://dev.commercetools.com/http-api-authorization.html#authorization-flows"/>
        public async Task<Response<Token>> GetTokenAsync()
        {
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", string.Concat(this.Configuration.Scope.ToEnumMemberString(), ":", this.Configuration.ProjectKey))
            };

            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Concat(this.Configuration.ClientID, ":", this.Configuration.ClientSecret)));
            HttpRequestMessage httpRequestMessage = CreateRequestMessage(this.Configuration.OAuthUrl, HttpMethod.Post);
            httpRequestMessage.Content = new FormUrlEncodedContent(pairs);
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            Response<Token> response = await SendAsync<Token>(httpRequestMessage);
            return response;
        }

        /// <summary>
        /// Refreshes a token from the authorization API using the refresh token flow.
        /// </summary>
        /// <param name="refreshToken">Refresh token value from the current token</param>
        /// <returns>Token</returns>
        /// <see href="http://dev.commercetools.com/http-api-authorization.html#authorization-flows"/>
        public async Task<Response<Token>> RefreshTokenAsync(string refreshToken)
        {
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken)
            };

            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Concat(this.Configuration.ClientID, ":", this.Configuration.ClientSecret)));


            HttpRequestMessage httpRequestMessage = CreateRequestMessage(this.Configuration.OAuthUrl, HttpMethod.Post);
            httpRequestMessage.Content = new FormUrlEncodedContent(pairs);
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            Response<Token> response = await SendAsync<Token>(httpRequestMessage);

            return response;
        }

        #endregion

        #region Utility

        /// <summary>
        /// Gets a response object from the API response.
        /// </summary>
        /// <typeparam name="T">Type of result</typeparam>
        /// <param name="httpResponseMessage">HttpResponseMessage</param>
        /// <returns>Response</returns>
        private async Task<Response<T>> GetResponse<T>(HttpResponseMessage httpResponseMessage)
        {
            Response<T> response = new Response<T>();
            Type resultType = typeof(T);

            response.StatusCode = (int)httpResponseMessage.StatusCode;
            response.ReasonPhrase = httpResponseMessage.ReasonPhrase;

            if (response.StatusCode >= 200 && response.StatusCode < 300)
            {
                response.Success = true;
                if (resultType == typeof(JObject) || resultType == typeof(JArray) || resultType.IsArray || (resultType.IsGenericType && resultType.Name.Equals(typeof(List<>).Name)))
                {
                    response.Result = JsonConvert.DeserializeObject<T>(await httpResponseMessage.Content.ReadAsStringAsync());
                }
                else
                {
                    dynamic data = JsonConvert.DeserializeObject(await httpResponseMessage.Content.ReadAsStringAsync());
                    ConstructorInfo constructor = Helper.GetConstructorWithDataParameter(resultType);

                    if (constructor != null)
                    {
                        Helper.ObjectActivator<T> activator = Helper.GetActivator<T>(constructor);
                        response.Result = activator(data);
                    }
                }
            }
            else
            {
                JObject data = JsonConvert.DeserializeObject<JObject>(await httpResponseMessage.Content.ReadAsStringAsync());

                response.Success = false;
                response.Errors = new List<ErrorMessage>();

                if (data != null && (data["errors"] != null))
                {
                    foreach (JObject error in data["errors"])
                    {
                        if (error.HasValues)
                        {
                            string code = error.Value<string>("code");
                            string message = error.Value<string>("message");
                            response.Errors.Add(new ErrorMessage(code, message));
                        }
                    }
                }
            }

            return response;
        }

        #endregion
    }
}
