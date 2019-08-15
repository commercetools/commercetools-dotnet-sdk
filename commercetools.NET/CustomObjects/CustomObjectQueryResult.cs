using System.Collections.Generic;
using commercetools.Common;
using Newtonsoft.Json;

namespace commercetools.CustomObjects
{
    /// <summary>
    /// An implementation of PagedQueryResult that provides access to the results as a List of CustomObject objects.
    /// </summary>
    /// <see href="https://dev.commercetools.com/http-api.html#pagedqueryresult"/>
    public class CustomObjectQueryResult : PagedQueryResult
    {
        #region Properties

        /// <summary>
        /// Results
        /// </summary>
        [JsonProperty(PropertyName = "results")]
        public List<CustomObject> Results { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes this instance with JSON data from an API response.
        /// </summary>
        /// <param name="data">JSON object</param>
        public CustomObjectQueryResult(dynamic data = null)
            : base((object)data)
        {
            if (data == null)
            {
                return;
            }

            this.Results = Helper.GetListFromJsonArray<CustomObject>(data.results);
        }

        #endregion
    }
}