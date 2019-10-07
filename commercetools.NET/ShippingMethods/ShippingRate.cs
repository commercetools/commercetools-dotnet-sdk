using System;
using System.Collections.Generic;
using commercetools.Common;
using commercetools.ShippingMethods.Tiers;
using Newtonsoft.Json;

namespace commercetools.ShippingMethods
{
    /// <summary>
    /// ShippingRate
    /// </summary>
    /// <see href="https://dev.commercetools.com/http-api-projects-shippingMethods.html#shippingrate"/>
    public class ShippingRate
    {
        #region Properties

        [JsonProperty(PropertyName = "price")]
        public Money Price { get; set; }

        [JsonProperty(PropertyName = "freeAbove")]
        public Money FreeAbove { get; set; }

        [JsonProperty(PropertyName = "tiers")]
        public List<Tier> Tiers { get; set; }

        [JsonProperty(PropertyName = "isMatching")]
        public Boolean IsMatching { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public ShippingRate()
        {
        }

        /// <summary>
        /// Initializes this instance with JSON data from an API response.
        /// </summary>
        /// <param name="data">JSON object</param>
        public ShippingRate(dynamic data)
        {
            if (data == null)
            {
                return;
            }

            this.Price = Helper.GetMoneyBasedOnType(data.price);
            this.FreeAbove = Helper.GetMoneyBasedOnType(data.freeAbove);

            // We do not use Helper.GetListFromJsonArray here, due to the JsonConverter property on Tier class.
            // Using GetListFromJsonArray ignores the JsonConverter property and fails to deserialize properly.
            this.Tiers = JsonConvert.DeserializeObject<List<Tier>>(data.tiers.ToString());
            this.IsMatching = data.isMatching;
        }

        #endregion
    }
}
