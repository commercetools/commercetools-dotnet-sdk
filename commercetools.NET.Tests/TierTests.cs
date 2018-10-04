using commercetools.ShippingMethods;
using commercetools.ShippingMethods.Tiers;
using Newtonsoft.Json;
using NUnit.Framework;

namespace commercetools.Tests
{
    /// <summary>
    /// Tests ShippingRate Tiers
    /// </summary>
    /// <see cref="ShippingRate.Tiers"/>
    /// <see cref="CartValueTier"/>
    /// <see cref="CartClassificationTier"/>
    /// <see cref="CartScoreTier"/>
    [TestFixture]
    public class TierTests
    {
        /// <summary>
        /// Tests the deserialization of a ShippingRate with CartClassificationTiers.
        /// </summary>
        /// <see cref="ShippingRate.Tiers"/>
        /// <see cref="CartClassificationTier"/>
        [Test]
        public void ShouldCreateValidCartClassificationTier()
        {
            var data =
                "{\r\n          \"price\": {\r\n            \"currencyCode\": \"EUR\",\r\n            \"centAmount\": 1000\r\n          },\r\n          \"tiers\": [{\r\n            \"type\" : \"CartClassification\",\r\n            \"value\": \"Medium\",\r\n            \"price\": {\r\n              \"currencyCode\": \"EUR\",\r\n              \"centAmount\": 2500\r\n            }\r\n          },\r\n          {\r\n            \"type\" : \"CartClassification\",\r\n            \"value\": \"Heavy\",\r\n            \"price\": {\r\n              \"currencyCode\": \"EUR\",\r\n              \"centAmount\": 5000\r\n            }\r\n          }\r\n          ]\r\n        }";

            ShippingRate shippingRate = JsonConvert.DeserializeObject<ShippingRate>(data, Common.Helper.DefaultJsonSerializerSettings);

            Assert.IsInstanceOf<CartClassificationTier>(shippingRate.Tiers[0]);
            Assert.IsInstanceOf<CartClassificationTier>(shippingRate.Tiers[1]);
        }

        /// <summary>
        /// Tests the deserialization of a ShippingRate with CartValueTiers.
        /// </summary>
        /// <see cref="ShippingRate.Tiers"/>
        /// <see cref="CartValueTier"/>
        [Test]
        public void ShouldCreateValidCartValueTier()
        {
            var data =
                "{\r\n          \"price\": {\r\n            \"currencyCode\": \"EUR\",\r\n            \"centAmount\": 400\r\n          },\r\n          \"tiers\": [{\r\n            \"type\" : \"CartValue\",\r\n            \"minimumCentAmount\": 5000,\r\n            \"price\": {\r\n              \"currencyCode\": \"EUR\",\r\n              \"centAmount\": 300\r\n            }\r\n          },\r\n          {\r\n            \"type\" : \"CartValue\",\r\n            \"minimumCentAmount\": 7500,\r\n            \"price\": {\r\n              \"currencyCode\": \"EUR\",\r\n              \"centAmount\": 200\r\n            }\r\n          },\r\n          {\r\n            \"type\" : \"CartValue\",\r\n            \"minimumCentAmount\": 1000,\r\n            \"price\": {\r\n              \"currencyCode\": \"EUR\",\r\n              \"centAmount\": 0\r\n            }\r\n          }\r\n          ]\r\n        }";

            ShippingRate shippingRate = JsonConvert.DeserializeObject<ShippingRate>(data, Common.Helper.DefaultJsonSerializerSettings);

            Assert.IsInstanceOf<CartValueTier>(shippingRate.Tiers[0]);
            Assert.IsInstanceOf<CartValueTier>(shippingRate.Tiers[1]);
        }

        /// <summary>
        /// Tests the deserialization of a ShippingRate with CartScoreTiers.
        /// </summary>
        /// <see cref="ShippingRate.Tiers"/>
        /// <see cref="CartScoreTier"/>
        [Test]
        public void ShouldCreateValidCartScoreTier()
        {
            var data =
                "{\r\n          \"price\": {\r\n            \"currencyCode\": \"USD\",\r\n            \"centAmount\": 500\r\n          },\r\n          \"tiers\": [{\r\n            \"type\" : \"CartScore\",\r\n            \"score\": 5,\r\n            \"price\": {\r\n              \"currencyCode\": \"USD\",\r\n              \"centAmount\": 750\r\n            }\r\n          },\r\n          {\r\n            \"type\" : \"CartScore\",\r\n            \"score\": 10,\r\n            \"price\": {\r\n              \"currencyCode\": \"USD\",\r\n              \"centAmount\": 1000\r\n            }\r\n          },\r\n          {\r\n            \"type\" : \"CartScore\",\r\n            \"score\": 15,\r\n            \"priceFunction\": {\r\n              \"currencyCode\": \"USD\",\r\n              \"function\": \"(50 * x) + 750\"\r\n            }\r\n          }\r\n          ]\r\n        }";

            ShippingRate shippingRate = JsonConvert.DeserializeObject<ShippingRate>(data, Common.Helper.DefaultJsonSerializerSettings);

            Assert.IsInstanceOf<CartScoreTier>(shippingRate.Tiers[0]);
            Assert.IsInstanceOf<CartScoreTier>(shippingRate.Tiers[1]);
        }
    }
}