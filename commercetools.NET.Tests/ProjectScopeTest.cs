using System.Collections.Generic;
using System.Threading.Tasks;
using commercetools.Common;
using NUnit.Framework;

namespace commercetools.Tests
{
    public class ProjectScopeTest
    {
        [Test]
        public async Task ProjectScopeDefault()
        {
            var c = new Configuration();
            Assert.IsNull(c.ProjectKey);
            Assert.IsNull(c.ScopeString);
            Assert.AreEqual(ProjectScope.ManageProject, c.Scope);
        }

        [Test]
        public async Task ProjectScopeProjectKey()
        {
            var c = new Configuration();
            c.ProjectKey = "foo";
            c.Scope = ProjectScope.ManageProject;

            Assert.AreEqual("foo", c.ProjectKey);
            Assert.AreEqual("manage_project:foo", c.ScopeString);
            Assert.AreEqual(ProjectScope.ManageProject, c.Scope);
        }

        [Test]
        public async Task ProjectKeyOnly()
        {
            var c = new Configuration()
            {
                ProjectKey =  "foo"
            };

            Assert.AreEqual("foo", c.ProjectKey);
            Assert.AreEqual("manage_project:foo", c.ScopeString);
            Assert.AreEqual(ProjectScope.ManageProject, c.Scope);
        }

        [Test]
        public async Task ProjectScopeInit()
        {
            var c = new Configuration()
            {
                ProjectKey =  "foo",
                Scope = ProjectScope.ManageProducts
            };

            Assert.AreEqual("foo", c.ProjectKey);
            Assert.AreEqual("manage_products:foo", c.ScopeString);
            Assert.AreEqual(ProjectScope.ManageProducts, c.Scope);
        }

        [Test]
        public async Task ProjectScopeInitOrder()
        {
            var c = new Configuration()
            {
                Scope = ProjectScope.ManageProducts,
                ProjectKey =  "foo"
            };

            Assert.AreEqual("foo", c.ProjectKey);
            Assert.AreEqual("manage_products:foo", c.ScopeString);
            Assert.AreEqual(ProjectScope.ManageProducts, c.Scope);
        }

        [Test]
        public async Task ProjectScopeConstructor()
        {
            var c = new Configuration(
                "https://auth.europe-west1.gcp.commercetools.com/oauth/token",
                "https://api.europe-west1.gcp.commercetools.com",
                "foo",
                "[your client ID]",
                "[your client secret]",
                ProjectScope.ManageProducts);

            Assert.AreEqual("foo", c.ProjectKey);
            Assert.AreEqual("manage_products:foo", c.ScopeString);
            Assert.AreEqual(ProjectScope.ManageProducts, c.Scope);
        }

        [Test]
        public async Task ProjectScopeStringSet()
        {
            var c = new Configuration()
            {
                ScopeString = "manage_customers:foo manage_products:foo",
                ProjectKey =  "foo"
            };

            Assert.AreEqual("foo", c.ProjectKey);
            Assert.AreEqual("manage_customers:foo manage_products:foo", c.ScopeString);
            Assert.AreEqual(ProjectScope.ManageProject, c.Scope);
        }


        [Test]
        public async Task ProjectScopeStringScope()
        {
            var c = new Configuration()
            {
                ScopeString = "manage_customers:foo manage_products:foo",
                Scope =  ProjectScope.ManageOrders,
                ProjectKey = "foo"
            };

            Assert.AreEqual("foo", c.ProjectKey);
            Assert.AreEqual("manage_customers:foo manage_products:foo", c.ScopeString);
            Assert.AreEqual(ProjectScope.ManageOrders, c.Scope);
        }

        [Test]
        public async Task ProjectScopeStringSingleConstructor()
        {
            var c = new Configuration(
                "https://auth.europe-west1.gcp.commercetools.com/oauth/token",
                "https://api.europe-west1.gcp.commercetools.com",
                "foo",
                "[your client ID]",
                "[your client secret]",
                "manage_customers:foo");


            Assert.AreEqual("foo", c.ProjectKey);
            Assert.AreEqual("manage_customers:foo", c.ScopeString);
            Assert.AreEqual(ProjectScope.ManageProject, c.Scope);
        }

        [Test]
        public async Task ProjectScopeStringConstructor()
        {
            var c = new Configuration(
                "https://auth.europe-west1.gcp.commercetools.com/oauth/token",
                "https://api.europe-west1.gcp.commercetools.com",
                "foo",
                "[your client ID]",
                "[your client secret]",
                "manage_customers:foo manage_products:foo");


            Assert.AreEqual("foo", c.ProjectKey);
            Assert.AreEqual("manage_customers:foo manage_products:foo", c.ScopeString);
            Assert.AreEqual(ProjectScope.ManageProject, c.Scope);
        }

        [Test]
        public async Task ProjectScopeListConstructor()
        {
            var c = new Configuration(
                "https://auth.europe-west1.gcp.commercetools.com/oauth/token",
                "https://api.europe-west1.gcp.commercetools.com",
                "foo",
                "[your client ID]",
                "[your client secret]",
                new HashSet<ProjectScope>() { ProjectScope.ManageCustomers, ProjectScope.ManageProducts });


            Assert.AreEqual("foo", c.ProjectKey);
            Assert.AreEqual("manage_customers:foo manage_products:foo", c.ScopeString);
            Assert.AreEqual(ProjectScope.ManageCustomers, c.Scope);
        }
    }
}
