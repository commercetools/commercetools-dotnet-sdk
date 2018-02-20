using commercetools.Core.Common;
using Newtonsoft.Json;

namespace commercetools.Core.Customers.UpdateActions
{
    /// <summary>
    /// Set key
    /// </summary>
    /// <see href="https://docs.commercetools.com/http-api-projects-customers.html#set-key"/>
    public class SetKeyAction : UpdateAction
    {
        #region Properties

        /// <summary>
        /// Customer key
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