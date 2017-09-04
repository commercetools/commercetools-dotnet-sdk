using commercetools.Core.Common;
using Newtonsoft.Json;

namespace commercetools.Core.CartDiscounts.UpdateActions
{
    public class ChangeTargetAction: UpdateAction
    {
        [JsonProperty(PropertyName = "target")]
        public CartDiscountTarget Target { get; }

        public ChangeTargetAction(CartDiscountTarget target)
        {
            this.Action = "changeTarget";
            this.Target = target;
        }
    }
}
