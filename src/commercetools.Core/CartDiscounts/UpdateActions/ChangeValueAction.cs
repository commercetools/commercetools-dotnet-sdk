using commercetools.Core.Common;
using Newtonsoft.Json;

namespace commercetools.Core.CartDiscounts.UpdateActions
{
    public class ChangeValueAction : UpdateAction
    {
        [JsonProperty(PropertyName = "value")]
        public CartDiscountValue Value { get; }

        public ChangeValueAction(CartDiscountValue value)
        {
            this.Action = "changeValue";
            this.Value = value;
        }
    }
}
