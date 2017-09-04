using commercetools.Core.Common;
using Newtonsoft.Json;

namespace commercetools.Core.DiscountCodes.UpdateActions
{
    public class SetMaxApplicationsAction : UpdateAction
    {
        [JsonProperty(PropertyName = "maxApplications")]
        public int? MaxApplications { get; set; }

        public SetMaxApplicationsAction()
        {
            this.Action = "setMaxApplications";
        }
    }
}
