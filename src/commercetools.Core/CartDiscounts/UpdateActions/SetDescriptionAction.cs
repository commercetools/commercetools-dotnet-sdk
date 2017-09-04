using commercetools.Core.Common;
using Newtonsoft.Json;

namespace commercetools.Core.CartDiscounts.UpdateActions
{
    public class SetDescriptionAction: UpdateAction
    {
        [JsonProperty(PropertyName = "description")]
        public LocalizedString Description { get; }

        public SetDescriptionAction(LocalizedString description)
        {
            this.Action = "setDescription";
            this.Description = description;
        }
    }
}
