using Newtonsoft.Json;

namespace commercetools.CustomObjects
{
    /// <summary>
    /// API representation for creating a new Customer.
    /// </summary>
    /// <see href="https://docs.commercetools.com/http-api-projects-custom-objects.html#customobjectdraft"/>
    public class CustomObjectDraft<T>
    {
        /// <summary>
        /// Container serves as a namespace for related custom objects
        /// </summary>
        [JsonProperty(PropertyName = "container")]
        public string Container { get; set; }
        
        /// <summary>
        /// A unique key within the custom object container
        /// </summary>
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }
        
        /// <summary>
        /// Value of the custom object. Can be any supported JSON value.
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public T Value { get; set; }
        
        /// <summary>
        /// The optional version of the custom object. 
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public int? Version { get; set; }

        public CustomObjectDraft(string container, string key, T value)
        {
            this.Container = container;
            this.Key = key;
            this.Value = value;
        }
    }
}