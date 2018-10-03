using commercetools.Common;
using Newtonsoft.Json;

namespace commercetools.ShippingMethods
{
    /// <summary>
    /// The representation to be sent to the server when creating a new shipping rate price tier.
    /// </summary>
    /// <see href="https://docs.commercetools.com/http-api-projects-shippingMethods.html#shippingratepricetier"/>
    public class Tier
    {
        #region Properties

        [JsonProperty(PropertyName = "type")]
        public string Type { get; private set; }

        [JsonProperty(PropertyName = "minimumCentAmount")]
        public long? MinimumCentAmount { get; private set; }

        [JsonProperty(PropertyName = "price")]
        public Money Price { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes this instance with JSON data from an API response.
        /// </summary>
        /// <param name="data">JSON object</param>
        public Tier(dynamic data)
        {
            if (data == null)
            {
                return;
            }

            this.Type = data.type;
            this.Price = new Money(data.price);
            this.MinimumCentAmount = data.minimumCentAmount;
        }

        #endregion
    }
}
