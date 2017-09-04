using commercetools.Core.Common;
using Newtonsoft.Json;

namespace commercetools.Core.Payments.UpdateActions
{
    /// <summary>
    /// Sets the method used, e.g. a conventional string representing Credit Card or Cash Advance.
    /// </summary>
    /// <see href="https://dev.commercetools.com/http-api-projects-payments.html#set-methodinfomethod"/>
    public class SetMethodInfoMethodAction : UpdateAction
    {
        #region Properties

        /// <summary>
        /// Method
        /// </summary>
        /// <remarks>
        /// If not provided, the method is unset.
        /// </remarks>
        [JsonProperty(PropertyName = "method")]
        public string Method { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public SetMethodInfoMethodAction()
        {
            this.Action = "setMethodInfoMethod";
        }

        #endregion
    }
}
