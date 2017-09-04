using commercetools.Core.Common;
using Newtonsoft.Json;

namespace commercetools.Core.Customers.UpdateActions
{
    /// <summary>
    /// SetVatIdAction
    /// </summary>
    /// <see href="http://dev.commercetools.com/http-api-projects-customers.html#set-vat-id"/>
    public class SetVatIdAction : UpdateAction
    {
        #region Properties

        /// <summary>
        /// VatId
        /// </summary>
        [JsonProperty(PropertyName = "vatId")]
        public string VatId { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public SetVatIdAction()
        {
            this.Action = "setVatId";
        }

        #endregion
    }
}
