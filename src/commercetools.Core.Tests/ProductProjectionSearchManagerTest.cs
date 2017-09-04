using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using commercetools.Core.Common;
using commercetools.Core.ProductProjections;
using commercetools.Core.ProductProjectionSearch;
using commercetools.Core.Products;
using commercetools.Core.Products.UpdateActions;
using commercetools.Core.ProductTypes;
using commercetools.Core.Project;
using commercetools.Core.TaxCategories;
using Xunit;

namespace commercetools.Core.Tests
{
    /// <summary>
    /// Test the API methods in the ProductProjectionSearchManager class.
    /// </summary>
    public class ProductProjectionSearchManagerTest : IDisposable
    {
        private Client _client;
        private Project.Project _project;
        private ProductType _testProductType;
        private TaxCategory _testTaxCategory;
        private List<Product> _testProducts;

        /// <summary>
        /// Test setup
        /// </summary>
        public ProductProjectionSearchManagerTest()
        {
            _client = new Client(Helper.GetConfiguration());

            Task<Response<Project.Project>> projectTask = _client.Project().GetProjectAsync();
            projectTask.Wait();
            Assert.True(projectTask.Result.Success);
            _project = projectTask.Result.Result;

            Assert.True(_project.Languages.Count > 0);
            Assert.True(_project.Currencies.Count > 0);

            ProductTypeDraft productTypeDraft = Helper.GetTestProductTypeDraft();
            Task<Response<ProductType>> productTypeTask = _client.ProductTypes().CreateProductTypeAsync(productTypeDraft);
            productTypeTask.Wait();
            Assert.True(productTypeTask.Result.Success);

            _testProductType = productTypeTask.Result.Result;
            Assert.NotNull(_testProductType.Id);

            TaxCategoryDraft taxCategoryDraft = Helper.GetTestTaxCategoryDraft(_project);
            Task<Response<TaxCategory>> taxCategoryTask = _client.TaxCategories().CreateTaxCategoryAsync(taxCategoryDraft);
            taxCategoryTask.Wait();
            Assert.True(taxCategoryTask.Result.Success);

            _testTaxCategory = taxCategoryTask.Result.Result;
            Assert.NotNull(_testTaxCategory.Id);

            _testProducts = new List<Product>();

            for (int i = 0; i < 5; i++)
            {
                ProductDraft productDraft = Helper.GetTestProductDraft(_project, _testProductType.Id, _testTaxCategory.Id);
                productDraft.Publish = true;

                LocalizedString name = new LocalizedString();

                foreach (string language in _project.Languages)
                {
                    name.SetValue(language, string.Concat("Test Product ", i, " ", language, " ", Helper.GetRandomString(10)));
                }

                productDraft.Name = name;

                Task<Response<Product>> productTask = _client.Products().CreateProductAsync(productDraft);
                productTask.Wait();
                Assert.True(productTask.Result.Success);

                Product product = productTask.Result.Result;
                Assert.NotNull(product.Id);

                _testProducts.Add(product);
            }
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        public void Dispose()
        {
            Task task;

            foreach (Product product in _testProducts)
            {
                if (product.MasterData.Published.HasValue && product.MasterData.Published == true)
                {
                    Task<Response<Product>> productTask = _client.Products().UpdateProductAsync(product, new UnpublishAction());
                    productTask.Wait();

                    if (productTask.Result.Success)
                    {
                        Product updatedProduct = productTask.Result.Result;
                        task = _client.Products().DeleteProductAsync(updatedProduct);
                        task.Wait();
                    }
                }
                else
                {
                    task = _client.Products().DeleteProductAsync(product);
                    task.Wait();
                }
            }

            task = _client.ProductTypes().DeleteProductTypeAsync(_testProductType);
            task.Wait();

            task = _client.TaxCategories().DeleteTaxCategoryAsync(_testTaxCategory);
            task.Wait();
        }

        /// <summary>
        /// Tests the ProductProjectionSearchManager.SearchProductProjectionsAsync method, retrieving results.
        /// </summary>
        /// <see cref="ProductProjectionSearchManager.SearchProductProjectionsAsync"/>
        [Fact]
        public async Task ShouldGetSearchResultsAsync()
        {
            Response<ProductProjectionQueryResult> response 
                = await _client.ProductProjectionSearch().SearchProductProjectionsAsync("Test Product 1");
            Assert.True(response.Success);

            ProductProjectionQueryResult productProjectionQueryResult = response.Result;
            Assert.NotNull(productProjectionQueryResult.Results);
            Assert.True(productProjectionQueryResult.Results.Count >= 1);
        }

        /// <summary>
        /// Tests the ProductProjectionSearchManager.SearchProductProjectionsAsync method, retrieving facets with results.
        /// </summary>
        /// <see cref="ProductProjectionSearchManager.SearchProductProjectionsAsync"/>
        [Fact]
        public async Task ShouldGetSearchResultsAndFacetsAsync()
        {
            string[] facet = new string[] {
                "variants.price.centAmount:range(* to 50),(50 to 100),(100 to *)"
            };

            Response<ProductProjectionQueryResult> response 
                = await _client.ProductProjectionSearch().SearchProductProjectionsAsync("Test Product 1", facet: facet);
            Assert.True(response.Success);

            ProductProjectionQueryResult productProjectionQueryResult = response.Result;
            Assert.NotNull(productProjectionQueryResult.Results);
            Assert.True(productProjectionQueryResult.Results.Count >= 1);
            Assert.True(productProjectionQueryResult.Facets.Count >= 1);
        }
    }
}
