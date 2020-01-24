using commercetools.Common;

using Newtonsoft.Json;

namespace commercetools.CustomerGroups.UpdateActions
{
    /// <summary>
    /// SetKeyAction
    /// </summary>
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
        public SetKeyAction(string key)
        {
            this.Action = "setKey";
            this.Key = key;
        }

        #endregion
    }
}
