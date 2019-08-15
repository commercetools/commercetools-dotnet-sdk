using System.Threading.Tasks;
using commercetools.Common;
using commercetools.CustomObjects;
using commercetools.Project;
using NUnit.Framework;

namespace commercetools.Tests
{
    /// <summary>
    /// Test the API methods in the CustomObjectManager class.
    /// </summary>
    [TestFixture]
    public class CustomObjectManagerTest
    {
        private Client _client;
        private CustomObject<string> _testCustomObject;

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

            CustomObjectDraft<string> customObjectDraft = Helper.GetTestCustomObjectDraft();
            Task<Response<CustomObject<string>>> customObjectTask = _client.CustomObjects().CreateOrUpdateCustomObjectAsync(customObjectDraft);
            customObjectTask.Wait();
            Assert.IsTrue(customObjectTask.Result.Success);

            CustomObject<string> customObject = customObjectTask.Result.Result;
            Assert.NotNull(customObject.Id);

            _testCustomObject = customObject;
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        [OneTimeTearDown]
        public void Dispose()
        {
            Task<Response<CustomObject<string>>> customObjectTask = _client.CustomObjects().DeleteCustomObjectAsync(_testCustomObject);
            customObjectTask.Wait();
        }

        /// <summary>
        /// Tests the CustomObjectManager.GetCustomObjectByIdAsync method.
        /// </summary>
        /// <see cref="CustomObjectManager.GetCustomObjectByIdAsync"/>
        [Test]
        public async Task ShouldGetCustomObjectByIdAsync()
        {
            Response<CustomObject<string>> response = await _client.CustomObjects().GetCustomObjectByIdAsync<string>(_testCustomObject.Id);
            Assert.IsTrue(response.Success);

            CustomObject<string> customObject = response.Result;
            Assert.NotNull(customObject.Id);
            Assert.AreEqual(customObject.Id, _testCustomObject.Id);
        }

        /// <summary>
        /// Tests the CustomObjectManager.QueryCustomObjectsAsync method.
        /// </summary>
        /// <see cref="CustomObjectManager.QueryCustomObjectsAsync"/>
        [Test]
        public async Task ShouldQueryCustomObjectsAsync()
        {
            Response<CustomObjectQueryResult<string>> response = await _client.CustomObjects().QueryCustomObjectsAsync<string>();
            Assert.IsTrue(response.Success);

            CustomObjectQueryResult<string> customObjectQueryResult = response.Result;
            Assert.NotNull(customObjectQueryResult.Results);
            Assert.GreaterOrEqual(customObjectQueryResult.Results.Count, 1);
        }

        /// <summary>
        /// Tests the CustomObjectManager.CreatOrUpdateCustomObjectAsync
        /// </summary>
        /// <see cref="CustomObjectManager.CreateOrUpdateCustomObjectAsync"/>
        /// <seealso cref="CustomObjectManager.DeleteCustomObjectAsync(commercetools.CustomObjects.CustomObject)"/>
        [Test]
        public async Task ShouldCreateAndDeleteCustomObjectAsync()
        {
            CustomObjectDraft<string> customObjectDraft = Helper.GetTestCustomObjectDraft();
            Response<CustomObject<string>> customObjectCreatedResponse = await _client.CustomObjects().CreateOrUpdateCustomObjectAsync(customObjectDraft);
            Assert.IsTrue(customObjectCreatedResponse.Success);

            CustomObject<string> customObject = customObjectCreatedResponse.Result;
            Assert.NotNull(customObject.Id);
            
            string deletedCustomObjectId = customObject.Id;
            Response<CustomObject<string>> customObjectResponse = await _client.CustomObjects().DeleteCustomObjectAsync(customObject);
            Assert.IsTrue(customObjectResponse.Success);

            customObjectResponse = await _client.CustomObjects().GetCustomObjectByIdAsync<string>(deletedCustomObjectId);
            Assert.IsFalse(customObjectResponse.Success);
        }

        /// <summary>
        /// Tests the CustomObjectManager.UpdateCustomObjectAsync method.
        /// </summary>
        /// <see cref="CustomObjectManager.UpdateCustomObjectAsync(commercetools.CustomObjects.CustomObject, System.Collections.Generic.List{commercetools.Common.UpdateAction})"/>
        [Test]
        public async Task ShouldUpdateCustomObjectAsync()
        {
            CustomObjectDraft<string> customObjectDraft = Helper.GetTestCustomObjectDraft();
            customObjectDraft.Container = _testCustomObject.Container;
            customObjectDraft.Key = _testCustomObject.Key;
            customObjectDraft.Version = _testCustomObject.Version + 1;
            customObjectDraft.Value = "newValue";
            
            Response<CustomObject<string>> response = await _client.CustomObjects().CreateOrUpdateCustomObjectAsync(customObjectDraft);
            Assert.IsTrue(response.Success);

            CustomObject<string> updatedObject = response.Result;
            Assert.NotNull(updatedObject.Id);
            Assert.AreEqual(_testCustomObject.Id, updatedObject.Id);
            Assert.AreEqual("newValue", updatedObject.Value);
        }
    }
}
