using System;
using commercetools.ShippingMethods.Tiers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace commercetools.Common.Converters
{
    /// <summary>
    /// Custom converter for the Tier class.
    /// </summary>
    public class TierJsonConverter : JsonConverter
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings
            = new JsonSerializerSettings() { ContractResolver = new TierContractResolver() };

        /// <summary>
        /// CanConvert
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns>True if the type is Tier, false otherwise</returns>
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Tier));
        }

        /// <summary>
        /// ReadJson
        /// </summary>
        /// <param name="reader">JsonReader</param>
        /// <param name="objectType">Object type</param>
        /// <param name="existingValue">Existing value</param>
        /// <param name="serializer">Serializer</param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            var tierType = jsonObject["type"].Value<string>();

            switch (tierType)
            {
                case "CartValue":
                    return JsonConvert.DeserializeObject<CartValueTier>(jsonObject.ToString(), JsonSerializerSettings);
                case "CartClassification":
                    return JsonConvert.DeserializeObject<CartClassificationTier>(jsonObject.ToString(), JsonSerializerSettings);
                case "CartScore":
                    return JsonConvert.DeserializeObject<CartScoreTier>(jsonObject.ToString(), JsonSerializerSettings);
                default:
                    throw new ArgumentException($"Cannot deserialize tier. TierType: {tierType}.");
            }
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}