using System.Collections.Generic;
using System.Threading.Tasks;

using commercetools.Common;
using commercetools.Products;
using commercetools.ProductProjections;
using commercetools.ProductTypes;
using commercetools.Project;
using commercetools.TaxCategories;
using Newtonsoft.Json;
using NUnit.Framework;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace commercetools.Tests
{
    /// <summary>
    /// Test the API methods in the ProductProjectionManager class.
    /// </summary>
    [TestFixture]
    public class ProductProjectionManagerTest
    {
        private Client _client;
        private Project.Project _project;
        private List<Product> _testProducts;
        private ProductType _testProductType;
        private TaxCategory _testTaxCategory;

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

            ProductTypeDraft productTypeDraft = Helper.GetTestProductTypeDraft();
            Task<Response<ProductType>> productTypeTask = _client.ProductTypes().CreateProductTypeAsync(productTypeDraft);
            productTypeTask.Wait();
            Assert.IsTrue(productTypeTask.Result.Success);

            _testProductType = productTypeTask.Result.Result;
            Assert.NotNull(_testProductType.Id);

            TaxCategoryDraft taxCategoryDraft = Helper.GetTestTaxCategoryDraft(_project);
            Task<Response<TaxCategory>> taxCategoryTask = _client.TaxCategories().CreateTaxCategoryAsync(taxCategoryDraft);
            taxCategoryTask.Wait();
            Assert.IsTrue(taxCategoryTask.Result.Success);

            _testTaxCategory = taxCategoryTask.Result.Result;
            Assert.NotNull(_testTaxCategory.Id);

            _testProducts = new List<Product>();

            for (int i = 0; i < 5; i++)
            {
                ProductDraft productDraft = Helper.GetTestProductDraft(_project, _testProductType.Id, _testTaxCategory.Id);
                Task<Response<Product>> productTask = _client.Products().CreateProductAsync(productDraft);
                productTask.Wait();
                var errors = productTask.Result.Errors;
                Assert.IsTrue(productTask.Result.Success);

                Product product = productTask.Result.Result;
                Assert.NotNull(product.Id);

                _testProducts.Add(product);
            }
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        [OneTimeTearDown]
        public void Dispose()
        {
            Task task;

            foreach (Product product in _testProducts)
            {
                task = _client.Products().DeleteProductAsync(product);
                task.Wait();
            }

            task = _client.ProductTypes().DeleteProductTypeAsync(_testProductType);
            task.Wait();

            task = _client.TaxCategories().DeleteTaxCategoryAsync(_testTaxCategory);
            task.Wait();
        }

        /// <summary>
        /// Tests the ProductProjectionManager.GetProductProjectionByIdAsync method.
        /// </summary>
        /// <see cref="ProductProjectionManager.GetProductProjectionByIdAsync"/>
        [Test]
        public async Task ShouldGetProductProjectionByIdAsync()
        {
            Response<ProductProjection> response = await _client.ProductProjections().GetProductProjectionByIdAsync(_testProducts[0].Id, true);
            Assert.IsTrue(response.Success);

            ProductProjection productProjection = response.Result;
            Assert.NotNull(productProjection.Id);
            Assert.AreEqual(productProjection.Id, _testProducts[0].Id);
        }

        /// <summary>
        /// Tests the ProductProjectionManager.GetProductProjectionByKeyAsync method.
        /// </summary>
        /// <see cref="ProductProjectionManager.GetProductProjectionByKeyAsync"/>
        [Test]
        public async Task ShouldGetProductProjectionByKeyAsync()
        {
            Response<ProductProjection> response = await _client.ProductProjections().GetProductProjectionByKeyAsync(_testProducts[1].Key, true);
            Assert.IsTrue(response.Success);

            ProductProjection productProjection = response.Result;
            Assert.NotNull(productProjection.Id);
            Assert.AreEqual(productProjection.Id, _testProducts[1].Id);
        }

        /// <summary>
        /// Tests the ProductProjectionManager.QueryProductProjectionsAsync method.
        /// </summary>
        /// <see cref="ProductProjectionManager.QueryProductProjectionsAsync"/>
        [Test]
        public async Task ShouldQueryProductProjectionsAsync()
        {
            Response<ProductProjectionQueryResult> response = await _client.ProductProjections().QueryProductProjectionsAsync();
            Assert.IsTrue(response.Success);

            ProductProjectionQueryResult productProjectionQueryResult = response.Result;
            Assert.NotNull(productProjectionQueryResult.Results);

            int limit = 2;
            response = await _client.ProductProjections().QueryProductProjectionsAsync(limit: limit);
            Assert.IsTrue(response.Success);

            productProjectionQueryResult = response.Result;
            Assert.NotNull(productProjectionQueryResult.Results);
            Assert.LessOrEqual(productProjectionQueryResult.Results.Count, limit);
        }

        [Test]
        public async Task TestProductVariantAvailabilityDeserialize()
        {
            dynamic data = JsonConvert.DeserializeObject("{ \"isOnStock\": true, \"restockableInDays\": 3, \"availableQuantity\": 3, \"channels\": { \"foo\": { \"isOnStock\": true, \"restockableInDays\": 3, \"availableQuantity\": 4 } } }");
            var availability = new ProductVariantAvailability(data);

            Assert.IsInstanceOf<ProductVariantAvailability>(availability.Channels["foo"]);
            Assert.AreEqual(4, availability.Channels["foo"].AvailableQuantity);
        }

        [Test]
        public async Task TestSystemTextJson()
        {
            dynamic data = JsonConvert.DeserializeObject("{ \"id\": \"fe9e6c47-0f6f-4c97-bcd0-4bd6de0d1120\", \"version\": 3, \"lastMessageSequenceNumber\": 1, \"createdAt\": \"2019-10-15T12:00:03.197Z\", \"lastModifiedAt\": \"2019-10-15T12:00:03.717Z\", \"lastModifiedBy\": {   \"clientId\": \"h-QvaF3NpsjPBWeXa6TUOnq0\",   \"isPlatformClient\": false }, \"createdBy\": {   \"clientId\": \"h-QvaF3NpsjPBWeXa6TUOnq0\",   \"isPlatformClient\": false }, \"productType\": {   \"typeId\": \"product-type\",   \"id\": \"2245e83c-7b0b-467b-b634-57563e695f90\" }, \"catalogs\": [], \"masterData\": {   \"current\": {\"name\": {  \"en\": \"GZFGKIENKT\"},\"description\": {  \"en\": \"V3DBJ5TGL1FO0YW7H9FG\"},\"categories\": [  {\"typeId\": \"category\",\"id\": \"9e4a0d7a-35f3-4613-a9a8-64fe8df4dfc0\"  }],\"categoryOrderHints\": {},\"slug\": {  \"en\": \"PU17FSMHXD\"},\"masterVariant\": {  \"id\": 1,  \"sku\": \"X6E7VQE5FC\",  \"key\": \"X6E7VQE5FC\",  \"prices\": [],  \"images\": [],  \"attributes\": [],  \"assets\": []},\"variants\": [],\"searchKeywords\": {  \"en\": [{ \"text\": \"jeans\"}  ]}   },   \"staged\": {\"name\": {  \"en\": \"GZFGKIENKT\"},\"description\": {  \"en\": \"V3DBJ5TGL1FO0YW7H9FG\"},\"categories\": [  {\"typeId\": \"category\",\"id\": \"9e4a0d7a-35f3-4613-a9a8-64fe8df4dfc0\"  }],\"categoryOrderHints\": {},\"slug\": {  \"en\": \"PU17FSMHXD\"},\"masterVariant\": {  \"id\": 1,  \"sku\": \"X6E7VQE5FC\",  \"key\": \"X6E7VQE5FC\",  \"prices\": [],  \"images\": [],  \"attributes\": [],  \"assets\": []},\"variants\": [],\"searchKeywords\": {  \"en\": [{ \"text\": \"jeans\"}  ]}   },   \"published\": false,   \"hasStagedChanges\": false }, \"key\": \"X6E7VQE5FC\", \"catalogData\": {}, \"reviewRatingStatistics\": {   \"averageRating\": 2,   \"highestRating\": 3,   \"lowestRating\": 1,   \"count\": 2,   \"ratingsDistribution\": {\"1\": 1,\"3\": 1   } }, \"taxCategory\": {   \"typeId\": \"tax-category\",   \"id\": \"4bcd7728-0990-4a0b-b662-db5d97f8d5d1\" }, \"state\": {   \"typeId\": \"state\",   \"id\": \"2e1af2e0-4d5f-42fa-8c7e-2c56bc328c89\" }, \"lastVariantId\": 1}");
            var product = new Product(data);

            Assert.IsInstanceOf<Product>(product);

            var productJson = JsonSerializer.Serialize(product);
            Assert.IsNotEmpty(productJson);
        }

    }
}
