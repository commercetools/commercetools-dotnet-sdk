using System;
using Newtonsoft.Json;

namespace commercetools.CustomObjects
{
    /// <summary>
    /// Custom objects are a way to store arbitrary JSON-formatted data on the commercetools platform.
    /// <see href="https://docs.commercetools.com/http-api-projects-custom-objects.html#customobject"/>
    /// </summary>
    public class CustomObject
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; private set; }
        
        [JsonProperty(PropertyName = "version")]
        public int Version { get; private set; }
        
        [JsonProperty(PropertyName = "container")]
        public string Container { get; private set; }
        
        [JsonProperty(PropertyName = "key")]
        public string Key { get; private set; }
        
        [JsonProperty(PropertyName = "createdAt")]
        public DateTime CreatedAt { get; private set; }
        
        [JsonProperty(PropertyName = "lastModifiedAt")]
        public DateTime LastModifiedAt { get; private set; }
        
        [JsonProperty(PropertyName = "value")]
        public object Value { get; private set; }

        public CustomObject(dynamic data)
        {
            this.Id = data.id;
            this.Version = data.version;
            this.Container = data.container;
            this.Key = data.key;
            this.CreatedAt = data.createdAt;
            this.LastModifiedAt = data.lastModifiedAt;
            this.Value = data.value;
        }
    }
}