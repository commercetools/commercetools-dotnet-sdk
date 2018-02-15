using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using commercetools.Core.Common;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace commercetools.Core.CartDiscounts
{
    public class CartDiscountExpandedReference
    {
        #region Properties

        [JsonProperty(PropertyName = "typeId")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ReferenceType? ReferenceType { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }


        [JsonProperty(PropertyName = "obj")]
        public CartDiscount CartDiscount { get; set; }
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public CartDiscountExpandedReference()
        {
        }

        /// <summary>
        /// Initializes this instance with JSON data from an API response.
        /// </summary>
        /// <param name="data">JSON object</param>
        public CartDiscountExpandedReference(dynamic data)
        {
            if (data == null)
            {
                return;
            }

            ReferenceType? referenceType;

            this.ReferenceType = Helper.TryGetEnumByEnumMemberAttribute<ReferenceType?>((string)data.typeId, out referenceType) ? referenceType : null;
            this.Id = data.id;
            this.CartDiscount = new CartDiscount(data.obj);
        }

        #endregion
    }
}
