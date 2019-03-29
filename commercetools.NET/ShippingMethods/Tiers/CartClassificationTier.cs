using commercetools.Common;
using Newtonsoft.Json;

namespace commercetools.ShippingMethods.Tiers
{
    /// <inheritdoc />
    public sealed class CartClassificationTier : Tier
    {
        [JsonProperty(PropertyName = "value")]
        public string Value { get; private set; }

        /// <summary>
        /// Initializes this instance with JSON data from an API response.
        /// </summary>
        /// <param name="data">JSON object</param>
        public CartClassificationTier(dynamic data)
        {
            if (data == null)
            {
                return;
            }

            this.Type = data.type;
            this.Price = Helper.GetMoneyBasedOnType(data.price);
            this.Value = data.value;
        }
    }
}
