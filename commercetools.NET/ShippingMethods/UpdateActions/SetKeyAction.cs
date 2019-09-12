using commercetools.Common;

using Newtonsoft.Json;

namespace commercetools.ShippingMethods.UpdateActions
{
    /// <summary>
    /// Set Key
    /// </summary>
    /// <see href="https://docs.commercetools.com/http-api-projects-shippingMethods#set-key"/>
    public class SetKeyAction : UpdateAction
    {
        #region Properties

        /// <summary>
        /// Description
        /// </summary>
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public SetKeyAction()
        {
            this.Action = "setKey";
        }

        #endregion
    }
}
