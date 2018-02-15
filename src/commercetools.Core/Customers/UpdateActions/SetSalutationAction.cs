using commercetools.Core.Common;
using Newtonsoft.Json;

namespace commercetools.Core.Customers.UpdateActions
{
    /// <summary>
    /// SetTitleAction
    /// </summary>
    /// <see href="http://dev.commercetools.com/http-api-projects-customers.html#set-salutation"/>
    public class SetSalutationAction : UpdateAction
    {
        #region Properties

        /// <summary>
        /// Title
        /// </summary>
        [JsonProperty(PropertyName = "salutation")]
        public string Salutation { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public SetSalutationAction()
        {
            this.Action = "setSalutation";
        }

        #endregion
    }
}
