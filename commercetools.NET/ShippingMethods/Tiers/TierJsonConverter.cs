using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace commercetools.ShippingMethods.Tiers
{
    public class TierJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Tier));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            var tierType = jsonObject["type"].Value<string>();

            switch (tierType)
            {
                case "CartValue":
                    return jsonObject.ToObject<CartValueTier>(serializer);
                case "CartClassification":
                    return jsonObject.ToObject<CartClassificationTier>(serializer);
                case "CartScore":
                    return jsonObject.ToObject<CartScoreTier>(serializer);
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