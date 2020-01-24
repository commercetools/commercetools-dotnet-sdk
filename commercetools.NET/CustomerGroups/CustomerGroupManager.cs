using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using commercetools.Categories;
using commercetools.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace commercetools.CustomerGroups
{
    /// <summary>
    /// Provides access to the functions in the CustomerGroup section of the API.
    /// </summary>
    public class CustomerGroupManager
    {
        #region Constants

        private const string ENDPOINT_PREFIX = "/customer-groups";

        #endregion

        #region Member Variables

        private readonly IClient _client;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client">Client</param>
        public CustomerGroupManager(IClient client)
        {
            _client = client;
        }

        #endregion

        #region API Methods

        /// <summary>
        /// Gets a customerGroup by its ID.
        /// </summary>
        /// <param name="customerGroupId">CustomerGroup ID</param>
        /// <returns>CustomerGroup</returns>
        public Task<Response<CustomerGroup>> GetCustomerGroupByIdAsync(string customerGroupId)
        {
            if (string.IsNullOrWhiteSpace(customerGroupId))
            {
                throw new ArgumentException("customerGroupId is required");
            }

            var endpoint = string.Concat(ENDPOINT_PREFIX, "/", customerGroupId);
            return _client.GetAsync<CustomerGroup>(endpoint);
        }

        /// <summary>
        /// Queries customerGroups.
        /// </summary>
        /// <param name="where">Where</param>
        /// <param name="sort">Sort</param>
        /// <param name="limit">Limit</param>
        /// <param name="offset">Offset</param>
        /// <returns>CustomerGroupQueryResult</returns>
        public Task<Response<CustomerGroupQueryResult>> QueryCustomerGroupsAsync(string where = null, string sort = null,
            int limit = -1, int offset = -1)
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

            return _client.GetAsync<CustomerGroupQueryResult>(ENDPOINT_PREFIX, values);
        }

        /// <summary>
        /// Creates a new customerGroup.
        /// </summary>
        /// <param name="customerGroupDraft">CustomerGroupDraft object</param>
        /// <returns>CustomerGroup</returns>
        public Task<Response<CustomerGroup>> CreateCustomerGroupAsync(CustomerGroupDraft customerGroupDraft)
        {
            if (customerGroupDraft == null)
            {
                throw new ArgumentException("customerGroupDraft cannot be null");
            }

            if (string.IsNullOrEmpty(customerGroupDraft.GroupName))
            {
                throw new ArgumentException("Group name is required");
            }

            string payload = JsonConvert.SerializeObject(customerGroupDraft,
                new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
            return _client.PostAsync<CustomerGroup>(ENDPOINT_PREFIX, payload);
        }

        /// <summary>
        /// Updates a customerGroup.
        /// </summary>
        /// <param name="customerGroup">CustomerGroup</param>
        /// <param name="action">The update action to be performed on the customerGroup.</param>
        /// <returns>CustomerGroup</returns>
        public Task<Response<CustomerGroup>> UpdateCustomerGroupAsync(CustomerGroup customerGroup, UpdateAction action)
        {
            return UpdateCustomerGroupAsync(customerGroup.Id, customerGroup.Version, new List<UpdateAction> {action});
        }

        /// <summary>
        /// Updates a customerGroup.
        /// </summary>
        /// <param name="customerGroup">CustomerGroup</param>
        /// <param name="actions">The list of update actions to be performed on the customerGroup.</param>
        /// <returns>CustomerGroup</returns>
        public Task<Response<CustomerGroup>> UpdateCustomerGroupAsync(CustomerGroup customerGroup,
            List<UpdateAction> actions)
        {
            return UpdateCustomerGroupAsync(customerGroup.Id, customerGroup.Version, actions);
        }

        /// <summary>
        /// Updates a customerGroup.
        /// </summary>
        /// <param name="customerGroupId">ID of the customerGroup</param>
        /// <param name="version">The expected version of the customerGroup on which the changes should be applied.</param>
        /// <param name="actions">The list of update actions to be performed on
        /// the customerGroup.</param>
        /// <returns>CustomerGroup</returns>
        public Task<Response<CustomerGroup>> UpdateCustomerGroupAsync(string customerGroupId, int version,
            List<UpdateAction> actions)
        {
            if (string.IsNullOrWhiteSpace(customerGroupId))
            {
                throw new ArgumentException("CustomerGroup ID is required");
            }

            if (version < 1)
            {
                throw new ArgumentException("Version is required");
            }

            if (actions == null || actions.Count < 1)
            {
                throw new ArgumentException("One or more update actions is required");
            }

            JObject data = JObject.FromObject(new
            {
                version = version,
                actions = JArray.FromObject(actions, new JsonSerializer {NullValueHandling = NullValueHandling.Ignore})
            });

            string endpoint = string.Concat(ENDPOINT_PREFIX, "/", customerGroupId);
            return _client.PostAsync<CustomerGroup>(endpoint, data.ToString());
        }

        /// <summary>
        /// Deletes a customerGroup.
        /// </summary>
        /// <param name="customerGroup">CustomerGroup</param>
        /// <returns>CustomerGroup</returns>
        public Task<Response<CustomerGroup>> DeleteCustomerGroupAsync(CustomerGroup customerGroup)
        {
            return DeleteCustomerGroupAsync(customerGroup.Id, customerGroup.Version);
        }

        /// <summary>
        /// Deletes a customerGroup.
        /// </summary>
        /// <param name="customerGroupId">Caregory ID</param>
        /// <param name="version">Caregory version</param>
        /// <returns>CustomerGroup</returns>
        public Task<Response<CustomerGroup>> DeleteCustomerGroupAsync(string customerGroupId, int version)
        {
            if (string.IsNullOrWhiteSpace(customerGroupId))
            {
                throw new ArgumentException("CustomerGroup ID is required");
            }

            if (version < 1)
            {
                throw new ArgumentException("Version is required");
            }

            var values = new NameValueCollection
            {
                {"version", version.ToString()}
            };

            string endpoint = string.Concat(ENDPOINT_PREFIX, "/", customerGroupId);
            return _client.DeleteAsync<CustomerGroup>(endpoint, values);
        }

        #endregion
    }
}