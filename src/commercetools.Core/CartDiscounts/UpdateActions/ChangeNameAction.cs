using commercetools.Core.Common;
using Newtonsoft.Json;

namespace commercetools.Core.CartDiscounts.UpdateActions
{
    public class ChangeNameAction: UpdateAction
    {
        [JsonProperty(PropertyName = "name")]
        public LocalizedString Name { get; }

        public ChangeNameAction(LocalizedString name)
        {
            this.Action = "changeName";
            this.Name = name;
        }
    }
}
