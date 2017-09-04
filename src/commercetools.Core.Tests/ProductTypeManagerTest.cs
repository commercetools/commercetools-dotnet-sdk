using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using commercetools.Core.Common;
using commercetools.Core.Common.UpdateActions;
using commercetools.Core.ProductTypes;
using Newtonsoft.Json.Linq;
using Xunit;

namespace commercetools.Core.Tests
{
    /// <summary>
    /// Test the API methods in the ProductTypeManager class.
    /// </summary>
    public class ProductTypeManagerTest : IDisposable
    {
        private Client _client;
        private List<ProductType> _testProductTypes;

        /// <summary>
        /// Test setup
        /// </summary>
        public ProductTypeManagerTest()
        {
            _client = new Client(Helper.GetConfiguration());
            _testProductTypes = new List<ProductType>();

            for (int i = 0; i < 5; i++)
            {
                ProductTypeDraft productTypeDraft = Helper.GetTestProductTypeDraft();
                Task<Response<ProductType>> task = _client.ProductTypes().CreateProductTypeAsync(productTypeDraft);
                task.Wait();
                Assert.True(task.Result.Success);

                ProductType productType = task.Result.Result;
                Assert.NotNull(productType.Id);

                _testProductTypes.Add(productType);
            }
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        public void Dispose()
        {
            foreach (ProductType productType in _testProductTypes)
            {
                Task task = _client.ProductTypes().DeleteProductTypeAsync(productType);
                task.Wait();
            }
        }

        /// <summary>
        /// Tests the ProductTypeManager.GetProductTypeByIdAsync method.
        /// </summary>
        /// <see cref="ProductTypeManager.GetProductTypeByIdAsync"/>
        [Fact]
        public async Task ShouldGetProductTypeByIdAsync()
        {
            Response<ProductType> response = await _client.ProductTypes().GetProductTypeByIdAsync(_testProductTypes[0].Id);
            Assert.True(response.Success);

            ProductType productType = response.Result;
            Assert.NotNull(productType.Id);
            Assert.Equal(productType.Id, _testProductTypes[0].Id);
        }

        /// <summary>
        /// Tests the ProductTypeManager.GetProductTypeByKeyAsync method.
        /// </summary>
        /// <see cref="ProductTypeManager.GetProductTypeByKeyAsync"/>
        [Fact]
        public async Task ShouldGetProductTypeByKeyAsync()
        {
            Response<ProductType> response = await _client.ProductTypes().GetProductTypeByKeyAsync(_testProductTypes[0].Key);
            Assert.True(response.Success);

            ProductType productType = response.Result;
            Assert.NotNull(productType.Id);
            Assert.Equal(productType.Id, _testProductTypes[0].Id);
        }

        /// <summary>
        /// Tests the ProductTypeManager.QueryProductTypesAsync method.
        /// </summary>
        /// <see cref="ProductTypeManager.QueryProductTypesAsync"/>
        [Fact]
        public async Task ShouldQueryProductTypesAsync()
        {
            Response<ProductTypeQueryResult> response = await _client.ProductTypes().QueryProductTypesAsync();
            Assert.True(response.Success);

            ProductTypeQueryResult productTypeQueryResult = response.Result;
            Assert.NotNull(productTypeQueryResult.Results);
            Assert.True(productTypeQueryResult.Results.Count >= 1);

            int limit = 2;
            response = await _client.ProductTypes().QueryProductTypesAsync(limit: limit);
            Assert.True(response.Success);

            productTypeQueryResult = response.Result;
            Assert.NotNull(productTypeQueryResult.Results);
            Assert.True(productTypeQueryResult.Results.Count <= limit);
        }

        /// <summary>
        /// Tests the ProductTypeManager.CreateProductTypeAsync and ProductTypeManager.DeleteProductTypeAsync methods.
        /// </summary>
        /// <see cref="ProductTypeManager.CreateProductTypeAsync"/>
        /// <seealso cref="ProductTypeManager.DeleteProductTypeAsync"/>
        [Fact]
        public async Task ShouldCreateAndDeleteProductTypeAsync()
        {
            ProductTypeDraft productTypeDraft = Helper.GetTestProductTypeDraft();
            Response<ProductType> response = await _client.ProductTypes().CreateProductTypeAsync(productTypeDraft);
            Assert.True(response.Success);

            ProductType productType = response.Result;
            Assert.NotNull(productType.Id);

            string deletedProductTypeId = productType.Id;

            Response<JObject> deleteResponse = await _client.ProductTypes().DeleteProductTypeAsync(productType);
            Assert.True(deleteResponse.Success);

            response = await _client.ProductTypes().GetProductTypeByIdAsync(deletedProductTypeId);
            Assert.False(response.Success);
        }

        /// <summary>
        /// Tests the ProductTypeManager.UpdateProductTypeAsync method.
        /// </summary>
        /// <see cref="ProductTypeManager.UpdateProductTypeAsync(ProductType, System.Collections.Generic.List{UpdateAction})"/>
        [Fact]
        public async Task ShouldUpdateProductTypeAsync()
        {
            string newKey = Helper.GetRandomString(15);
            string newName = string.Concat("Test Product Type", Helper.GetRandomString(10));

            GenericAction setKeyAction = new GenericAction("setKey");
            setKeyAction.SetProperty("key", newKey);

            GenericAction changeNameAction = new GenericAction("changeName");
            changeNameAction.SetProperty("name", newName);

            List<UpdateAction> actions = new List<UpdateAction>();
            actions.Add(setKeyAction);
            actions.Add(changeNameAction);

            Response<ProductType> response = await _client.ProductTypes().UpdateProductTypeAsync(_testProductTypes[0], actions);
            Assert.True(response.Success);

            _testProductTypes[0] = response.Result;
            Assert.NotNull(_testProductTypes[0].Id);
            Assert.Equal(_testProductTypes[0].Key, newKey);
            Assert.Equal(_testProductTypes[0].Name, newName);
        }
    }
}
