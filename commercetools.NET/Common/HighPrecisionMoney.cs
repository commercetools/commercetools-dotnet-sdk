using Newtonsoft.Json;

namespace commercetools.Common
{
    public class HighPrecisionMoney : Money
    {
        #region Properties

        [JsonProperty(PropertyName = "fractionDigits")]
        public int FractionDigits { get; set; }

        [JsonProperty(PropertyName = "preciseAmount")]
        public int PreciseAmount { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = "highPrecision";

        #endregion


        /// <summary>
        /// Constructor.
        /// </summary>
        public HighPrecisionMoney()
        {
        }

        /// <summary>
        /// Initializes this instance with JSON data from an API response.
        /// </summary>
        /// <param name="data">JSON object</param>
        public HighPrecisionMoney(dynamic data)
        {
            if (data == null)
            {
                return;
            }

            this.CurrencyCode = data.currencyCode;
            this.CentAmount = data.centAmount;
            this.FractionDigits = data.fractionDigits;
            this.PreciseAmount = data.preciseAmount;
        }
    }
}
