using Newtonsoft.Json;

namespace commercetools.Common
{
    public class CentPrecisionMoney : Money
    {
        #region Properties

        [JsonProperty(PropertyName = "fractionDigits")]
        public int FractionDigits { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = "centPrecision";

        #endregion


        /// <summary>
        /// Constructor.
        /// </summary>
        public CentPrecisionMoney()
        {
        }

        /// <summary>
        /// Initializes this instance with JSON data from an API response.
        /// </summary>
        /// <param name="data">JSON object</param>
        public CentPrecisionMoney(dynamic data)
        {
            if (data == null)
            {
                return;
            }

            this.CurrencyCode = data.currencyCode;
            this.CentAmount = data.centAmount;
            this.FractionDigits = data.fractionDigits;
        }
    }
}
