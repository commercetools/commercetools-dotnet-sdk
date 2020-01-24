using System.Collections.Generic;
using System.Threading.Tasks;
using commercetools.Carts;
using commercetools.Common;
using commercetools.CustomerGroups;
using commercetools.CustomerGroups.UpdateActions;
using commercetools.Customers;
using commercetools.Messages;
using commercetools.Project;
using NUnit.Framework;

namespace commercetools.Tests
{
    /// <summary>
    /// Test the API methods in the CustomerGroupManager class.
    /// </summary>
    [TestFixture]
    public class CustomerGroupManagerTest
    {
        private Client _client;
        private Project.Project _project;
        private List<CustomerGroup> _testCustomerGroups;

        /// <summary>
        /// Test setup
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            _client = Helper.GetClient();
            
            Task<Response<Project.Project>> projectTask = _client.Project().GetProjectAsync();
            projectTask.Wait();
            Assert.IsTrue(projectTask.Result.Success);
            _project = projectTask.Result.Result;

            _testCustomerGroups = new List<CustomerGroup>();
            
            var customerGroupDraft = Helper.GetTestCustomerGroupDraft();
            var customerGroupTask = _client.CustomerGroups().CreateCustomerGroupAsync(customerGroupDraft);
            customerGroupTask.Wait();
            Assert.IsTrue(customerGroupTask.Result.Success);

            var customerGroup = customerGroupTask.Result.Result;
            Assert.NotNull(customerGroupTask.Id);

            _testCustomerGroups.Add(customerGroup);
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        [OneTimeTearDown]
        public void Dispose()
        {
            foreach (var customerGroup in _testCustomerGroups)
            {
                var customerGroupTask = _client.CustomerGroups().DeleteCustomerGroupAsync(customerGroup);
                customerGroupTask.Wait();
            }
        }
        
        /// <summary>
        /// Tests the CustomerGroupManager.GetCustomerGroupByIdAsync method.
        /// </summary>
        [Test]
        public async Task ShouldGetCustomerGroupByIdAsync()
        {
            var response = await _client.CustomerGroups().GetCustomerGroupByIdAsync(_testCustomerGroups[0].Id);
            Assert.IsTrue(response.Success);

            var customerGroup = response.Result;
            Assert.NotNull(customerGroup.Id);
            Assert.AreEqual(customerGroup.Id, _testCustomerGroups[0].Id);
        }
        
        [Test]
        public async Task ShouldQueryCustomerGroupsAsync()
        {
            var response = await _client.CustomerGroups().QueryCustomerGroupsAsync();
            Assert.IsTrue(response.Success);

            var customerGroupQueryResult = response.Result;
            Assert.NotNull(customerGroupQueryResult.Results);
            Assert.GreaterOrEqual(customerGroupQueryResult.Results.Count, 1);

            int limit = 2;
            response = await _client.CustomerGroups().QueryCustomerGroupsAsync(limit: limit);
            Assert.IsTrue(response.Success);

            customerGroupQueryResult = response.Result;
            Assert.NotNull(customerGroupQueryResult.Results);
            Assert.LessOrEqual(customerGroupQueryResult.Results.Count, limit);
        }
        
        [Test]
        public async Task ShouldCreateAndDeleteCustomerGroupAsync()
        {
            var customerGroupDraft = Helper.GetTestCustomerGroupDraft();
            var response = await _client.CustomerGroups().CreateCustomerGroupAsync(customerGroupDraft);
            Assert.IsTrue(response.Success);

            var customerGroup = response.Result;
            Assert.NotNull(customerGroup.Id);

            string deletedCustomerGroupId = customerGroup.Id;

            response = await _client.CustomerGroups().DeleteCustomerGroupAsync(customerGroup);
            Assert.IsTrue(response.Success);

            response = await _client.CustomerGroups().GetCustomerGroupByIdAsync(deletedCustomerGroupId);
            Assert.IsFalse(response.Success);
        }
        
        [Test]
        public async Task ShouldUpdateCustomerGroupAsync()
        {
            string newName = string.Concat(_testCustomerGroups[0].Name, " Updated");
            string newKey = Helper.GetRandomString(10);

            var changeNameAction = new ChangeNameAction(newName);
            var setKeyAction = new SetKeyAction(newKey);

            List<UpdateAction> actions = new List<UpdateAction>();
            actions.Add(changeNameAction);
            actions.Add(setKeyAction);

            var response = await _client.CustomerGroups().UpdateCustomerGroupAsync(_testCustomerGroups[0], actions);
            Assert.IsTrue(response.Success);

            _testCustomerGroups[0] = response.Result;
            Assert.NotNull(_testCustomerGroups[0].Id);
            Assert.AreEqual(_testCustomerGroups[0].Name, newName);
            Assert.AreEqual(_testCustomerGroups[0].Key, newKey);
        }
    }
}