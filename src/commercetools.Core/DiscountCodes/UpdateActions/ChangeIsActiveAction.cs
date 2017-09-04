using commercetools.Core.Common;
using Newtonsoft.Json;

namespace commercetools.Core.DiscountCodes.UpdateActions
{
    public class ChangeIsActiveAction : UpdateAction
    {
        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; private set; }

        public ChangeIsActiveAction(bool isActive)
        {
            this.Action = "changeIsActive";
            this.IsActive = isActive;
        }
    }
}
