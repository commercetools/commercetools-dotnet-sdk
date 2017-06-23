using Newtonsoft.Json;

namespace commercetools.Carts
{
    /// <summary>
    /// DiscountedLineItemPriceForQuantity
    /// </summary>
    /// <see href="https://dev.commercetools.com/http-api-projects-carts.html#discountedlineitempriceforquantity"/>
    public class DiscountedLineItemPriceForQuantity
    {
        #region Properties

        /// <summary>
        /// Quantity
        /// </summary>
        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; private set; }

        /// <summary>
        /// Discounted Line Item Price
        /// </summary>
        [JsonProperty(PropertyName = "discountedPrice")]
        public DiscountedLineItemPrice DiscountedPrice { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes this instance with JSON data from an API response.
        /// </summary>
        /// <param name="data">JSON object</param>
        public DiscountedLineItemPriceForQuantity(dynamic data)
        {
            if (data == null)
            {
                return;
            }

            this.Quantity = data.quantity;
            this.DiscountedPrice = new DiscountedLineItemPrice(data.discountedPrice);
        }

        #endregion
    }
}
