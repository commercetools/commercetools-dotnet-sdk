using System;

using commercetools.Core.Common;

using Newtonsoft.Json;

namespace commercetools.Core.Project.UpdateActions
{
    public class ChangeNameAction : UpdateAction
    {

        public ChangeNameAction(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            this.Name = name;
            this.Action = "changeName";
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
    }
}
