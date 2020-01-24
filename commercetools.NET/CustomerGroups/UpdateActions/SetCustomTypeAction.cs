using commercetools.Common;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace commercetools.CustomerGroups.UpdateActions
{
    /// <summary>
    /// This action sets or removes the custom type for an existing customerGroup.
    /// </summary>
    public class SetCustomTypeAction : UpdateAction
    {
        #region Properties

        /// <summary>
        /// Type
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public ResourceIdentifier Type { get; set; }

        /// <summary>
        /// Fields
        /// </summary>
        [JsonProperty(PropertyName = "fields")]
        public JObject Fields { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public SetCustomTypeAction()
        {
            this.Action = "setCustomType";
        }

        #endregion
    }
}
