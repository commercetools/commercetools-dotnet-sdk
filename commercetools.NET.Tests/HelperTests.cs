using commercetools.Common;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace commercetools.Tests
{
    [TestFixture]
    public class HelperTests
    {
        private string _HighPrecisionPriceJson = "{ \"type\": \"highPrecision\", \"currencyCode\": \"EUR\", \"centAmount\": 100, \"preciseAmount\": 1000, \"fractionDigits\": 3}";
        private string _CentPrecisionPriceJson = "{ \"type\": \"centPrecision\", \"currencyCode\": \"EUR\", \"centAmount\": 100, \"preciseAmount\": 1000, \"fractionDigits\": 2}";
        private string _otherPriceJson = "{ \"type\": \"other\", \"currencyCode\": \"EUR\", \"centAmount\": 100}";
        private string _withoutTypePriceJson = "{ \"currencyCode\": \"EUR\", \"centAmount\": 100}";

        [Test]
        public void ShouldGetMoneyBasedOnType()
        {
            JObject highPrecisionPriceAsJson = JObject.Parse(_HighPrecisionPriceJson);
            JObject centPrecisionPriceAsJson = JObject.Parse(_CentPrecisionPriceJson);

            Assert.NotNull(highPrecisionPriceAsJson);
            Assert.NotNull(centPrecisionPriceAsJson);

            var highPrecision = commercetools.Common.Helper.GetMoneyBasedOnType(highPrecisionPriceAsJson);
            var centPrecision = commercetools.Common.Helper.GetMoneyBasedOnType(centPrecisionPriceAsJson);

            //HighPrecisionPrice
            Assert.IsNotNull(highPrecision);
            Assert.IsInstanceOf(typeof(HighPrecisionMoney), highPrecision);
            var highPrecisionPrice = highPrecision as HighPrecisionMoney;
            Assert.AreEqual(3, highPrecisionPrice.FractionDigits);
            Assert.AreEqual("EUR", highPrecisionPrice.CurrencyCode);
            Assert.AreEqual(100, highPrecisionPrice.CentAmount);
            Assert.AreEqual(1000, highPrecisionPrice.PreciseAmount);

            //CentPrecisionPrice
            Assert.IsNotNull(centPrecision);
            Assert.IsInstanceOf(typeof(CentPrecisionMoney), centPrecision);
            var centPrecisionPrice = centPrecision as CentPrecisionMoney;
            Assert.AreEqual(2, centPrecisionPrice.FractionDigits);
            Assert.AreEqual("EUR", centPrecisionPrice.CurrencyCode);
            Assert.AreEqual(100, centPrecisionPrice.CentAmount);

        }

        /// <summary>
        /// Make sure that it will parse it to money if there is new type added
        /// </summary>
        [Test]
        public void ShouldGetMoneyBasedOnOtherType()
        {
            JObject otherPriceAsJson = JObject.Parse(_otherPriceJson);

            Assert.NotNull(otherPriceAsJson);

            var otherPrice = commercetools.Common.Helper.GetMoneyBasedOnType(otherPriceAsJson);

            Assert.IsNotNull(otherPrice);
            Assert.IsInstanceOf(typeof(Money), otherPrice);
            Assert.AreEqual("EUR", otherPrice.CurrencyCode);
            Assert.AreEqual(100, otherPrice.CentAmount);

        }

        /// <summary>
        /// Make sure that it will parse it to money even if there is no type provided in json (but have at least currency and centAmount)
        /// </summary>
        [Test]
        public void ShouldGetMoneyBasedWithoutType()
        {
            JObject priceAsJson = JObject.Parse(_withoutTypePriceJson);

            Assert.NotNull(priceAsJson);

            var otherPrice = commercetools.Common.Helper.GetMoneyBasedOnType(priceAsJson);

            Assert.IsNotNull(priceAsJson);
            Assert.IsInstanceOf(typeof(Money), otherPrice);
            Assert.AreEqual("EUR", otherPrice.CurrencyCode);
            Assert.AreEqual(100, otherPrice.CentAmount);

        }
    }
}
