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
        private CustomObject _testCustomObject;

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

            CustomObjectDraft customObjectDraft = Helper.GetTestCustomObjectDraft();
            Task<Response<CustomObject>> customObjectTask = _client.CustomObjects().CreateOrUpdateCustomObjectAsync(customObjectDraft);
            customObjectTask.Wait();
            Assert.IsTrue(customObjectTask.Result.Success);

            CustomObject customObject = customObjectTask.Result.Result;
            Assert.NotNull(customObject.Id);

            _testCustomObject = customObject;
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        [OneTimeTearDown]
        public void Dispose()
        {
            Task<Response<CustomObject>> customObjectTask = _client.CustomObjects().DeleteCustomObjectAsync(_testCustomObject);
            customObjectTask.Wait();
        }

        /// <summary>
        /// Tests the CustomObjectManager.GetCustomObjectByIdAsync method.
        /// </summary>
        /// <see cref="CustomObjectManager.GetCustomObjectByIdAsync"/>
        [Test]
        public async Task ShouldGetCustomObjectByIdAsync()
        {
            Response<CustomObject> response = await _client.CustomObjects().GetCustomObjectByIdAsync(_testCustomObject.Id);
            Assert.IsTrue(response.Success);

            CustomObject customObject = response.Result;
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
            Response<CustomObjectQueryResult> response = await _client.CustomObjects().QueryCustomObjectsAsync();
            Assert.IsTrue(response.Success);

            CustomObjectQueryResult customObjectQueryResult = response.Result;
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
            CustomObjectDraft customObjectDraft = Helper.GetTestCustomObjectDraft();
            Response<CustomObject> customObjectCreatedResponse = await _client.CustomObjects().CreateOrUpdateCustomObjectAsync(customObjectDraft);
            Assert.IsTrue(customObjectCreatedResponse.Success);

            CustomObject customObject = customObjectCreatedResponse.Result;
            Assert.NotNull(customObject.Id);
            
            string deletedCustomObjectId = customObject.Id;
            Response<CustomObject> customObjectResponse = await _client.CustomObjects().DeleteCustomObjectAsync(customObject);
            Assert.IsTrue(customObjectResponse.Success);

            customObjectResponse = await _client.CustomObjects().GetCustomObjectByIdAsync(deletedCustomObjectId);
            Assert.IsFalse(customObjectResponse.Success);
        }

        /// <summary>
        /// Tests the CustomObjectManager.UpdateCustomObjectAsync method.
        /// </summary>
        /// <see cref="CustomObjectManager.UpdateCustomObjectAsync(commercetools.CustomObjects.CustomObject, System.Collections.Generic.List{commercetools.Common.UpdateAction})"/>
        [Test]
        public async Task ShouldUpdateCustomObjectAsync()
        {
            CustomObjectDraft customObjectDraft = Helper.GetTestCustomObjectDraft();
            customObjectDraft.Container = _testCustomObject.Container;
            customObjectDraft.Key = _testCustomObject.Key;
            customObjectDraft.Version = _testCustomObject.Version + 1;
            
            Response<CustomObject> response = await _client.CustomObjects().CreateOrUpdateCustomObjectAsync(customObjectDraft);
            Assert.IsTrue(response.Success);

            CustomObject updatedObject = response.Result;
            Assert.NotNull(updatedObject.Id);
            Assert.AreEqual(_testCustomObject.Id, updatedObject.Id);
        }
    }
}
