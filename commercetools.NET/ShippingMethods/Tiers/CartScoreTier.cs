using commercetools.Common;
using Newtonsoft.Json;

namespace commercetools.ShippingMethods.Tiers
{
    /// <inheritdoc />
    public sealed class CartScoreTier : Tier
    {
        [JsonProperty(PropertyName = "score")]
        public int Score { get; private set; }

        /// <summary>
        /// Initializes this instance with JSON data from an API response.
        /// </summary>
        /// <param name="data">JSON object</param>
        public CartScoreTier(dynamic data)
        {
            if (data == null)
            {
                return;
            }

            this.Type = data.type;
            this.Price = Helper.GetMoneyBasedOnType(data.price);
            this.Score = data.score;
        }
    }
}
