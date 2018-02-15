using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using commercetools.Core.Common;

using Newtonsoft.Json;

namespace commercetools.Core.DiscountCodes
{
    public class DiscountCodeExpandedQueryResult
        : PagedQueryResult
    {
        #region Properties

        /// <summary>
        /// Results
        /// </summary>
        [JsonProperty(PropertyName = "results")]
        public List<DiscountCodeExpanded> Results { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes this instance with JSON data from an API response.
        /// </summary>
        /// <param name="data">JSON object</param>
        public DiscountCodeExpandedQueryResult(dynamic data)
            : base((object)data)
        {
            if (data == null)
            {
                return;
            }

            this.Results = Helper.GetListFromJsonArray<DiscountCodeExpanded>(data.results);
        }

        #endregion
    }
}
