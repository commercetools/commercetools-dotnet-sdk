using commercetools.Core.Common;
using Newtonsoft.Json;

namespace commercetools.Core.ShippingMethods
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

            this.Price = new Money(data.price);
            this.FreeAbove = new Money(data.freeAbove);
        }

        #endregion
    }
}
