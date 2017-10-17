﻿using System;
using commercetools.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace commercetools.CartDiscounts
{
    public class CartDiscountDraft
    {
        #region properties

        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public LocalizedString Name { get; private set; }

        /// <summary>
        /// Description
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public LocalizedString Description { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public CartDiscountValue Value { get; private set; }

        /// <summary>
        /// Cart Discount Predicate
        /// </summary>
        [JsonProperty(PropertyName = "cartPredicate")]
        public string CartPredicate { get; private set; }

        /// <summary>
        /// Cart Discount Target
        /// </summary>
        [JsonProperty(PropertyName = "target")]
        public CartDiscountTarget Target { get; set; }

        /// <summary>
        /// Sort Order
        /// </summary>
        [JsonProperty(PropertyName = "sortOrder")]
        public string SortOrder { get; private set; }

        /// <summary>
        /// Only active discount can be applied to the cart. Defaults to true.
        /// </summary>
        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Valid from
        /// </summary>
        [JsonProperty(PropertyName = "validFrom")]
        public DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Valid until
        /// </summary>
        [JsonProperty(PropertyName = "validUntil")]
        public DateTime? ValidUntil { get; set; }

        /// <summary>
        /// States whether the discount can only be used in a connection with a DiscountCode.
        /// </summary>
        [JsonProperty(PropertyName = "requiresDiscountCode")]
        public bool RequiresDiscountCode { get; private set; }

        /// <summary>
        /// Specifies whether the application of this discount causes the following discounts to be ignored. Defaults to Stacking.
        /// </summary>
        [JsonProperty(PropertyName = "stackingMode")]
        [JsonConverter(typeof(StringEnumConverter))]
        public StackingMode? StackingMode { get; private set; }

        #endregion

        public CartDiscountDraft(
            LocalizedString name, 
            CartDiscountValue cartDiscountValue,
            string cartPredicate,
            string sortOrder,
            bool requiresDiscountCode)
        {
            if (name == null)
                throw new ArgumentException(nameof(name));

            if (string.IsNullOrWhiteSpace(sortOrder))
                throw new ArgumentException(nameof(sortOrder));

            if (string.IsNullOrWhiteSpace(cartPredicate))
                throw new ArgumentException(nameof(cartPredicate));

            if (cartDiscountValue == null)
                throw new ArgumentException(nameof(cartDiscountValue));


            Name = name;
            CartPredicate = cartPredicate;
            Value = cartDiscountValue;
            SortOrder = sortOrder;
            RequiresDiscountCode = requiresDiscountCode;
        }
    }
}
