using commercetools.Core.Common;
using Newtonsoft.Json;

namespace commercetools.Core.CartDiscounts.UpdateActions
{
    public class ChangeRequiresDiscountCodeAction: UpdateAction
    {
        [JsonProperty(PropertyName = "requiresDiscountCode")]
        public bool RequiresDiscountCode { get; }

        public ChangeRequiresDiscountCodeAction(bool requiresDiscountCode)
        {
            this.Action = "changeRequiresDiscountCode";
            this.RequiresDiscountCode = requiresDiscountCode;
        }
    }
}
