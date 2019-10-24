using commercetools.Common;

using Newtonsoft.Json;

namespace commercetools.Customers.UpdateActions
{
    /// <summary>
    /// SetKeyAction
    /// </summary>
    /// <see href="https://docs.commercetools.com/http-api-projects-customers#set-key"/>
    public class SetKeyAction : UpdateAction
    {
        #region Properties

        /// <summary>
        /// Key
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
