using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using commercetools.Core.Common;
using commercetools.Core.Common.UpdateActions;
using commercetools.Core.Customers;
using commercetools.Core.Messages;
using commercetools.Core.Project;
using commercetools.Core.Types;
using Newtonsoft.Json.Linq;
using Xunit;
using Type = commercetools.Core.Types.Type;

namespace commercetools.Core.Tests
{
    /// <summary>
    /// Test the API methods in the TypeManager class.
    /// </summary>
    public class TypeManagerTest : IDisposable
    {
        private Client _client;
        private Project.Project _project;
        private List<Type> _testTypes;

        /// <summary>
        /// Test setup
        /// </summary>
        public TypeManagerTest()
        {
            _client = new Client(Helper.GetConfiguration());

            Task<Response<Project.Project>> projectTask = _client.Project().GetProjectAsync();
            projectTask.Wait();
            Assert.True(projectTask.Result.Success);
            _project = projectTask.Result.Result;

            Assert.True(_project.Languages.Count > 0);

            CustomerDraft customerDraft = Helper.GetTestCustomerDraft();
            Task<Response<CustomerCreatedMessage>> customerTask = _client.Customers().CreateCustomerAsync(customerDraft);
            customerTask.Wait();
            Assert.True(customerTask.Result.Success);

            _testTypes = new List<Type>();

            for (int i = 0; i < 5; i++)
            {
                TypeDraft typeDraft = Helper.GetTypeDraft(_project);
                Task<Response<Type>> typeTask = _client.Types().CreateTypeAsync(typeDraft);
                typeTask.Wait();
                Assert.True(typeTask.Result.Success);

                Type type = typeTask.Result.Result;
                Assert.NotNull(type.Id);

                _testTypes.Add(type);

            }
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        public void Dispose()
        {
            Task task;

            foreach (Type type in _testTypes)
            {
                task = _client.Types().DeleteTypeAsync(type);
                task.Wait();
            }
        }

        /// <summary>
        /// Tests the TypeManager.GetTypeByIdAsync method.
        /// </summary>
        /// <see cref="TypeManager.GetTypeByIdAsync"/>
        [Fact]
        public async Task ShouldGetTypeByIdAsync()
        {
            Response<Type> response = await _client.Types().GetTypeByIdAsync(_testTypes[0].Id);
            Assert.True(response.Success);

            Type type = response.Result;
            Assert.NotNull(type.Id);
            Assert.Equal(type.Id, _testTypes[0].Id);
        }

        /// <summary>
        /// Tests the TypeManager.QueryTypesAsync method.
        /// </summary>
        /// <see cref="TypeManager.QueryTypesAsync"/>
        [Fact]
        public async Task ShouldQueryShippingMethodsAsync()
        {
            Response<TypeQueryResult> response = await _client.Types().QueryTypesAsync();
            Assert.True(response.Success);

            TypeQueryResult typeQueryResult = response.Result;
            Assert.NotNull(typeQueryResult.Results);
            Assert.True(typeQueryResult.Results.Count >= 1);

            int limit = 2;
            response = await _client.Types().QueryTypesAsync(limit: limit);
            Assert.True(response.Success);

            typeQueryResult = response.Result;
            Assert.NotNull(typeQueryResult.Results);
            Assert.True(typeQueryResult.Results.Count <= limit);
        }

        /// <summary>
        /// Tests the TypeManager.CreateTypeAsync and TypeManager.DeleteTypeAsync methods.
        /// </summary>
        /// <see cref="TypeManager.CreateTypeAsync"/>
        /// <seealso cref="TypeManager.DeleteTypeAsync(commercetools.Types.Type)"/>
        [Fact]
        public async Task ShouldCreateAndDeleteTypeAsync()
        {
            TypeDraft typeDraft = Helper.GetTypeDraft(_project);
            Response<Type> typeResponse = await _client.Types().CreateTypeAsync(typeDraft);
            Assert.True(typeResponse.Success);

            Type type = typeResponse.Result;
            Assert.NotNull(type.Id);

            string deletedTypeId = type.Id;

            Response<JObject> deleteResponse = await _client.Types().DeleteTypeAsync(type);
            Assert.True(deleteResponse.Success);

            typeResponse = await _client.Types().GetTypeByIdAsync(deletedTypeId);
            Assert.False(typeResponse.Success);
        }

        /// <summary>
        /// Tests the TypeManager.UpdateTypeAsync method.
        /// </summary>
        /// <see cref="TypeManager.UpdateTypeAsync(commercetools.Types.Type, System.Collections.Generic.List{UpdateAction})"/>
        [Fact]
        public async Task ShouldUpdateTypeAsync()
        {
            string randomPostfix = Helper.GetRandomString(10);
            LocalizedString newName = new LocalizedString();
            LocalizedString newDescription = new LocalizedString();

            foreach (string language in _project.Languages)
            {
                newName[language] = string.Concat("Test Type ", language, " ", randomPostfix);
                newDescription[language] = string.Concat("Test Description ", language, " ", randomPostfix);
            }

            GenericAction changeNameAction = new GenericAction("changeName");
            changeNameAction.SetProperty("name", newName);

            GenericAction setDescriptionAction = new GenericAction("setDescription");
            setDescriptionAction.SetProperty("description", newDescription);

            List<UpdateAction> actions = new List<UpdateAction>();
            actions.Add(changeNameAction);
            actions.Add(setDescriptionAction);

            Response<Type> response = await _client.Types().UpdateTypeAsync(_testTypes[0], actions);
            Assert.True(response.Success);

            _testTypes[0] = response.Result;
            Assert.NotNull(_testTypes[0].Id);

            foreach (string language in _project.Languages)
            {
                Assert.Equal(_testTypes[0].Name[language], newName[language]);
                Assert.Equal(_testTypes[0].Description[language], newDescription[language]);
            }
        }
    }
}
