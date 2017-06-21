﻿using commercetools.Common;
using Newtonsoft.Json;

namespace commercetools.CartDiscounts.UpdateActions
{
    public class ChangeIsActive: UpdateAction
    {
        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; }

        public ChangeIsActive(bool isActive)
        {
            this.Action = "changeIsActive";
            this.IsActive = isActive;
        }
    }
}