using commercetools.Core.Common;
using Newtonsoft.Json;

namespace commercetools.Core.CartDiscounts.UpdateActions
{
    public class ChangeIsActiveAction: UpdateAction
    {
        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; }

        public ChangeIsActiveAction(bool isActive)
        {
            this.Action = "changeIsActive";
            this.IsActive = isActive;
        }
    }
}
