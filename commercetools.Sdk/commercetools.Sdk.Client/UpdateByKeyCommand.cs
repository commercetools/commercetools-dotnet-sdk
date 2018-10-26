﻿using commercetools.Sdk.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace commercetools.Sdk.Client
{
    public class UpdateByKeyCommand<T> : UpdateCommand<T>
    {
        public UpdateByKeyCommand(string key, int version, List<UpdateAction> updateActions)
        {
            this.ParameterKey = "key";
            this.ParameterValue = key;
            this.Version = version;
            this.UpdateActions = updateActions;
        }
    }
}