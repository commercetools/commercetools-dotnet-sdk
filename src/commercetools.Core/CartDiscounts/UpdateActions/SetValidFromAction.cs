using System;
using commercetools.Core.Common;
using Newtonsoft.Json;

namespace commercetools.Core.CartDiscounts.UpdateActions
{
    public class SetValidFromAction: UpdateAction
    {
        [JsonProperty(PropertyName = "validFrom")]
        public DateTime? ValidFrom { get; }
        public SetValidFromAction(DateTime validFrom)
        {
            this.Action = "setValidFrom";
            this.ValidFrom = validFrom;
        }
    }
}
