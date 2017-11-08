using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

using Xunit;

namespace commercetools.Core.Tests.ProjectTests
{
    public class ProjectTests
    {
        [Xunit.Fact]
        public void ShoudInstantiateProjectWithCorrectVersion()
        { 

            dynamic data = new ExpandoObject();
            data.version = 1;
            data.key = "key";
            data.name = "name";
            data.countries = null;
            data.currencies = null;
            data.languages = null;
            data.createdAt = null;
            data.trialUntil = null;
            data.messages = null;
            
            Core.Project.Project project = new Core.Project.Project(data);
            
            Assert.Equal(project.Version, 1);
        }
    }
}
