using commercetools.Common;
using Newtonsoft.Json;

namespace commercetools.ShippingMethods.Tiers
{
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
            this.Price = new Money(data.price);
            this.Score = data.score;
        }
    }
}
