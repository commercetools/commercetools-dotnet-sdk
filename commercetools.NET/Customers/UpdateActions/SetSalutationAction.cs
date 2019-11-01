using commercetools.Common;
using Newtonsoft.Json;

namespace commercetools.Customers.UpdateActions
{
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
