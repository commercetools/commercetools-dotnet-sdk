using commercetools.Common;
using commercetools.Common.Converters;
using Newtonsoft.Json;

namespace commercetools.ShippingMethods.Tiers
{
    /// <summary>
    /// The representation to be sent to the server when creating a new shipping rate price tier.
    /// </summary>
    /// <see href="https://docs.commercetools.com/http-api-projects-shippingMethods.html#shippingratepricetier"/>
    [JsonConverter(typeof(TierJsonConverter))]
    public abstract class Tier
    {
        #region Properties

        [JsonProperty(PropertyName = "type")]
        public string Type { get; protected set; }

        [JsonProperty(PropertyName = "price")]
        public Money Price { get; protected set; }

        #endregion
    }
}
