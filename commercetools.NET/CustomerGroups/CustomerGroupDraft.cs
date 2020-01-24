using commercetools.CustomFields;
using Newtonsoft.Json;

namespace commercetools.CustomerGroups
{
    public class CustomerGroupDraft
    {
        #region Properties

        [JsonProperty(PropertyName = "groupName")]
        public string GroupName { get; set; }
        
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "custom")]
        public CustomFieldsDraft Custom { get; set; }
        #endregion

        #region Constructors

        public CustomerGroupDraft(string groupName)
        {
            this.GroupName = groupName;
        }
        public CustomerGroupDraft(string groupName, string key) : this(groupName)
        {
            this.Key = key;
        }
        #endregion
    }
}