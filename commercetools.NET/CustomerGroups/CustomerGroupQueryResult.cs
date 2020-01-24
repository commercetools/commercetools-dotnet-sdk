using System.Collections.Generic;

using commercetools.Common;
using commercetools.CustomerGroups;
using Newtonsoft.Json;

namespace commercetools.CustomerGroups
{
    /// <summary>
    /// An implementation of PagedQueryResult that provides access to the results as a List of CustomerGroups objects.
    /// </summary>
    public class CustomerGroupQueryResult : PagedQueryResult
    {
        #region Properties

        /// <summary>
        /// Results
        /// </summary>
        [JsonProperty(PropertyName = "results")]
        public List<CustomerGroup> Results { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes this instance with JSON data from an API response.
        /// </summary>
        /// <param name="data">JSON object</param>
        public CustomerGroupQueryResult(dynamic data = null)
            : base((object)data)
        {
            if (data == null)
            {
                return;
            }

            this.Results = Helper.GetListFromJsonArray<CustomerGroup>(data.results);
        }

        #endregion
    }
}
