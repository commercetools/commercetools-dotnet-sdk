using System;
using commercetools.ShippingMethods.Tiers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace commercetools.Common.Converters
{
    /// <summary>
    /// Resolves the tier's default contract 
    /// </summary>
    public class TierContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Makes sure the object is of type Tier and checks whether the class is an abstract
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(Tier).IsAssignableFrom(objectType) && !objectType.IsAbstract)
            {
                return null; 
            }

            return base.ResolveContractConverter(objectType);
        }
    }
}
