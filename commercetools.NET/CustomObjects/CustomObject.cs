using System;
using System.Reflection;
using commercetools.Common;
using Newtonsoft.Json;

namespace commercetools.CustomObjects
{
    /// <summary>
    /// Custom objects are a way to store arbitrary JSON-formatted data on the commercetools platform.
    /// <see href="https://docs.commercetools.com/http-api-projects-custom-objects.html#customobject"/>
    /// </summary>
    public class CustomObject<T>
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
        public T Value { get; private set; }

        public CustomObject(dynamic data)
        {
            this.Id = data.id;
            this.Version = data.version;
            this.Container = data.container;
            this.Key = data.key;
            this.CreatedAt = data.createdAt;
            this.LastModifiedAt = data.lastModifiedAt;
            
            ConstructorInfo constructor = Helper.GetConstructorWithDataParameter(typeof(T));
            if (constructor != null)
            {
                Helper.ObjectActivator<T> activator = Helper.GetActivator<T>(constructor);
                this.Value = activator(data);
            }
        }
    }
}