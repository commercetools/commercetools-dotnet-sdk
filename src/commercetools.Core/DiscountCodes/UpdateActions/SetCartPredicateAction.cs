using commercetools.Core.Common;
using Newtonsoft.Json;

namespace commercetools.Core.DiscountCodes.UpdateActions
{
    public class SetCartPredicateAction : UpdateAction
    {
        [JsonProperty(PropertyName = "cartPredicate")]
        public string CartPredicate { get; set; }

        public SetCartPredicateAction()
        {
            this.Action = "setCartPredicate";
        }
    }
}
