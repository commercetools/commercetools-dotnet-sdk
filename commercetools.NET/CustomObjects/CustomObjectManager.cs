using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using commercetools.Common;
using Newtonsoft.Json;

namespace commercetools.CustomObjects
{
    /// <summary>
    /// Provides access to the functions in the CustomObjects section of the API.
    /// </summary>
    /// <see href="https://docs.commercetools.com/http-api-projects-custom-objects.html"/>
    public class CustomObjectManager
    {
        #region Constants
        
        private const string ENDPOINT_PREFIX = "/custom-objects";
        
        #endregion

        #region Member Variables

        private readonly IClient _client;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client">Client</param>
        public CustomObjectManager(IClient client)
        {
            _client = client;
        }
        
        #endregion

        #region API Methods

        /// <summary>
        /// Gets a CustomObject by its ID
        /// </summary>
        /// <param name="customObjectId">ID of the custom object</param>
        /// <returns>CustomObject</returns>
        /// <see href="https://docs.commercetools.com/http-api-projects-custom-objects.html#get-customobject-by-id"/>
        public Task<Response<CustomObject>> GetCustomObjectByIdAsync(string customObjectId)
        {
            if (string.IsNullOrWhiteSpace(customObjectId))
            {
                throw new ArgumentException("CustomObject ID is required");
            }

            string endpoint = string.Concat(ENDPOINT_PREFIX, "/", customObjectId);
            return _client.GetAsync<CustomObject>(endpoint);
        }
        
        /// <summary>
        /// Gets a CustomObject by its container and key
        /// </summary>
        /// <param name="container">CustomObject container</param>
        /// <param name="key">CustomObject key</param>
        /// <returns>CustomObject</returns>
        /// <see href="https://docs.commercetools.com/http-api-projects-custom-objects.html#get-customobject-by-container-and-key" />
        public Task<Response<CustomObject>> GetCustomObjectByContainerAndKeyAsync(string container, string key)
        {
            if (string.IsNullOrWhiteSpace(container))
            {
                throw new ArgumentException("Container is required");
            }
            
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key is required");
            }

            string endpoint = string.Concat(ENDPOINT_PREFIX, "/", container, "/", key);
            return _client.GetAsync<CustomObject>(endpoint);
        }

        /// <summary>
        /// Queries for CustomObjects
        /// </summary>
        /// <param name="where">Where</param>
        /// <param name="sort">Sort</param>
        /// <param name="limit">Limit</param>
        /// <param name="offset">Offset</param>
        /// <returns>CustomObjectQueryResult</returns>
        /// <see href="https://docs.commercetools.com/http-api-projects-custom-objects.html#query-customobjects" />
        public Task<Response<CustomObjectQueryResult>> QueryCustomObjectsAsync(string where = null, string sort = null, int limit = -1, int offset = -1)
        {
            NameValueCollection values = new NameValueCollection();

            if (!string.IsNullOrWhiteSpace(where))
            {
                values.Add("where", where);
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                values.Add("sort", sort);
            }

            if (limit > 0)
            {
                values.Add("limit", limit.ToString());
            }

            if (offset >= 0)
            {
                values.Add("offset", offset.ToString());
            }

            return _client.GetAsync<CustomObjectQueryResult>(ENDPOINT_PREFIX, values);
        }

        /// <summary>
        /// Creates or updates a CustomObject.
        ///
        /// If a CustomObject with the same key already exists, it is updated. Otherwise, it is created.
        /// </summary>
        /// <param name="customObjectDraft">CustomObjectDraft</param>
        /// <returns>CustomObject</returns>
        /// <see href="https://docs.commercetools.com/http-api-projects-custom-objects.html#create-or-update-a-customobject" />
        public Task<Response<CustomObject>> CreateOrUpdateCustomObjectAsync(CustomObjectDraft customObjectDraft)
        {
            if (string.IsNullOrWhiteSpace(customObjectDraft.Container))
            {
                throw new ArgumentException("Container is required");
            }
            if (string.IsNullOrWhiteSpace(customObjectDraft.Key))
            {
                throw new ArgumentException("Key is required");
            }
            if (customObjectDraft.Value == null)
            {
                throw new ArgumentException("Value is required");
            }
            
            string payload = JsonConvert.SerializeObject(customObjectDraft, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return _client.PostAsync<CustomObject>(ENDPOINT_PREFIX, payload);
        }

        /// <summary>
        /// Deletes a CustomObject
        /// </summary>
        /// <param name="customObject">CustomObject to delete</param>
        /// <param name="dataErasure">Whether or not to erase data</param>
        /// <returns>CustomObject</returns>
        /// <see href="https://docs.commercetools.com/http-api-projects-custom-objects.html#delete-customobject-by-id" />
        public Task<Response<CustomObject>> DeleteCustomObjectAsync(CustomObject customObject, bool dataErasure = false)
        {
            return DeleteCustomObjectAsync(customObject.Id, customObject.Version, dataErasure);
        }

        /// <summary>
        /// Deletes a CustomObject
        /// </summary>
        /// <param name="customObjectId">CustomObject ID</param>
        /// <param name="version">CustomObject version</param>
        /// <param name="dataErasure">Whether or not to erase data</param>
        /// <returns>CustomObject</returns>
        /// <see href="https://docs.commercetools.com/http-api-projects-custom-objects.html#delete-customobject-by-id" />
        public Task<Response<CustomObject>> DeleteCustomObjectAsync(string customObjectId, int? version = null, bool dataErasure = false)
        {
            if (string.IsNullOrWhiteSpace(customObjectId))
            {
                throw new ArgumentException("CustomObject ID is required");
            }

            NameValueCollection values = new NameValueCollection();
            
            if (version != null && version >= 1)
            {
                values.Add(nameof(version), version.ToString());
            }
            
            if (dataErasure)
            {
                values.Add(nameof(dataErasure), dataErasure.ToString());
            }
            
            string endpoint = string.Concat(ENDPOINT_PREFIX, "/", customObjectId);
            return _client.DeleteAsync<CustomObject>(endpoint, values);
        }

        /// <summary>
        /// Deletes a CustomObject
        /// </summary>
        /// <param name="container">CustomObject container</param>
        /// <param name="key">CustomObject key</param>
        /// <param name="dataErasure">Whether or not to erase data</param>
        /// <returns>CustomObject</returns>
        /// <see href="https://docs.commercetools.com/http-api-projects-custom-objects.html#delete-customobject-by-container-and-key" />
        public Task<Response<CustomObject>> DeleteCustomObjectByContainerAndKeyAsync(string container, string key, bool dataErasure = false)
        {
            if (string.IsNullOrWhiteSpace(container))
            {
                throw new ArgumentException("Container is required");
            }
            
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key is required");
            }
            
            NameValueCollection values = new NameValueCollection();
            if (dataErasure)
            {
                values.Add(nameof(dataErasure), dataErasure.ToString());
            }
            
            string endpoint = string.Concat(ENDPOINT_PREFIX, "/", container, "/", key);
            return _client.DeleteAsync<CustomObject>(endpoint, values);
        }
        
        #endregion
    }
}