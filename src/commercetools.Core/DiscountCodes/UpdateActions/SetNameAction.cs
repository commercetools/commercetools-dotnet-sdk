using commercetools.Core.Common;
using Newtonsoft.Json;

namespace commercetools.Core.DiscountCodes.UpdateActions
{
    public class SetNameAction : UpdateAction
    {
        [JsonProperty(PropertyName = "name")]
        public LocalizedString Name { get; set; }

        public SetNameAction()
        {
            this.Action = "setName";
        }
    }
}
