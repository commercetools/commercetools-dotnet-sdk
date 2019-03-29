﻿using commercetools.Common;
using Newtonsoft.Json;

namespace commercetools.ShippingMethods.Tiers
{
    /// <inheritdoc />
    public sealed class CartValueTier : Tier
    {
        [JsonProperty(PropertyName = "minimumCentAmount")]
        public long? MinimumCentAmount { get; private set; }

        /// <summary>
        /// Initializes this instance with JSON data from an API response.
        /// </summary>
        /// <param name="data">JSON object</param>
        public CartValueTier(dynamic data)
        {
            if (data == null)
            {
                return;
            }

            this.Type = data.type;
            this.Price = Helper.GetMoneyBasedOnType(data.price);
            this.MinimumCentAmount = data.minimumCentAmount;
        }
    }
}
