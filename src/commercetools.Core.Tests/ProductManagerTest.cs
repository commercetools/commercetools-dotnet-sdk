using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using commercetools.Core.Common;
using commercetools.Core.Common.UpdateActions;
using commercetools.Core.Products;
using commercetools.Core.Products.UpdateActions;
using commercetools.Core.ProductTypes;
using commercetools.Core.Project;
using commercetools.Core.TaxCategories;
using Xunit;

namespace commercetools.Core.Tests
{
    /// <summary>
    /// Test the API methods in the ProductManager class.
    /// </summary>
    public class ProductManagerTest : IDisposable
    {
        private Client _client;
        private Project.Project _project;
        private List<Product> _testProducts;
        private ProductType _testProductType;
        private TaxCategory _testTaxCategory;

        /// <summary>
        /// Test setup
        /// </summary>
        public ProductManagerTest()
        {
            _client = new Client(Helper.GetConfiguration());

            Task<Response<Project.Project>> projectTask = _client.Project().GetProjectAsync();
            projectTask.Wait();
            Assert.True(projectTask.Result.Success);
            _project = projectTask.Result.Result;

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
                task = _client.Products().DeleteProductAsync(product);
                task.Wait();
            }

            task = _client.ProductTypes().DeleteProductTypeAsync(_testProductType);
            task.Wait();

            task = _client.TaxCategories().DeleteTaxCategoryAsync(_testTaxCategory);
            task.Wait();
        }

        /// <summary>
        /// Tests the ProductManager.GetProductByIdAsync method.
        /// </summary>
        /// <see cref="ProductManager.GetProductByIdAsync"/>
        [Fact]
        public async Task ShouldGetProductByIdAsync()
        {
            Response<Product> response = await _client.Products().GetProductByIdAsync(_testProducts[0].Id);
            Assert.True(response.Success);

            Product product = response.Result; 
            Assert.NotNull(product.Id);
            Assert.Equal(product.Id, _testProducts[0].Id);
        }

        /// <summary>
        /// Tests the ProductManager.GetProductByKeyAsync method.
        /// </summary>
        /// <see cref="ProductManager.GetProductByKeyAsync"/>
        [Fact]
        public async Task ShouldGetProductByKeyAsync()
        {
            Response<Product> response = await _client.Products().GetProductByKeyAsync(_testProducts[1].Key);
            Assert.True(response.Success);

            Product product = response.Result; 
            Assert.NotNull(product.Id);
            Assert.Equal(product.Key, _testProducts[1].Key);
        }

        /// <summary>
        /// Tests the ProductManager.QueryProductsAsync method.
        /// </summary>
        /// <see cref="ProductManager.QueryProductsAsync"/>
        [Fact]
        public async Task ShouldQueryProductsAsync()
        {
            Response<ProductQueryResult> response = await _client.Products().QueryProductsAsync();
            Assert.True(response.Success);

            ProductQueryResult productQueryResult = response.Result;
            Assert.NotNull(productQueryResult.Results);
            Assert.True(productQueryResult.Results.Count >= 1);

            int limit = 2;
            response = await _client.Products().QueryProductsAsync(limit: limit);
            Assert.True(response.Success);

            productQueryResult = response.Result;
            Assert.NotNull(productQueryResult.Results);
            Assert.True(productQueryResult.Results.Count <= limit);
        }

        /// <summary>
        /// Tests the ProductManager.CreateProductAsync and ProductManager.DeleteProductAsync methods.
        /// </summary>
        /// <see cref="ProductManager.CreateProductAsync"/>
        /// <seealso cref="ProductManager.DeleteProductAsync(Product)"/>
        [Fact]
        public async Task ShouldCreateAndDeleteProductAsync()
        {
            ProductDraft productDraft = Helper.GetTestProductDraft(_project, _testProductType.Id, _testTaxCategory.Id);

            LocalizedString name = new LocalizedString();
            LocalizedString slug = new LocalizedString();

            foreach (string language in _project.Languages)
            {
                name.SetValue(language, string.Concat("Test Product", language, " ", Helper.GetRandomString(10)));
                slug.SetValue(language, string.Concat("test-product-", language, "-", Helper.GetRandomString(10)));
            }

            productDraft.Name = name;
            productDraft.Slug = slug;

            Response<Product> response = await _client.Products().CreateProductAsync(productDraft);
            Assert.True(response.Success);

            Product product = response.Result;
            Assert.NotNull(product.Id);

            string deletedProductId = product.Id;

            response = await _client.Products().DeleteProductAsync(product.Id, product.Version);
            Assert.True(response.Success);

            response = await _client.Products().GetProductByIdAsync(deletedProductId);
            Assert.False(response.Success);
        }

        /// <summary>
        /// Tests the ProductManager.UpdateProductAsync method.
        /// </summary>
        /// <see cref="ProductManager.UpdateProductAsync(Product, System.Collections.Generic.List{UpdateAction}, string, string, Guid, Guid)"/>
        [Fact]
        public async Task ShouldUpdateProductAsync()
        {
            string newKey = Helper.GetRandomString(15);
            LocalizedString newSlug = new LocalizedString();

            foreach (string language in _project.Languages)
            {
                newSlug.SetValue(language, string.Concat("updated-product-", language, "-", Helper.GetRandomString(10)));
            }

            SetKeyAction setKeyAction = new SetKeyAction();
            setKeyAction.Key = newKey;

            GenericAction changeSlugAction = new GenericAction("changeSlug");
            changeSlugAction.SetProperty("slug", newSlug);
            changeSlugAction.SetProperty("staged", true);

            List<UpdateAction> actions = new List<UpdateAction>();
            actions.Add(setKeyAction);
            actions.Add(changeSlugAction);

            Response<Product> response = await _client.Products().UpdateProductAsync(_testProducts[2], actions);
            Assert.True(response.Success);

            _testProducts[2] = response.Result;
            Assert.NotNull(_testProducts[2].Id);
            Assert.Equal(_testProducts[2].Key, newKey);

            foreach (string language in _project.Languages)
            {
                Assert.Equal(_testProducts[2].MasterData.Staged.Slug.GetValue(language), newSlug.GetValue(language));
            }
        }

        /// <summary>
        /// Tests the ProductManager.UpdateProductByKeyAsync method.
        /// </summary>
        /// <see cref="ProductManager.UpdateProductByKeyAsync"/>
        [Fact]
        public async Task ShouldUpdateProductByKeyAsync()
        {
            LocalizedString newName = new LocalizedString();
            LocalizedString newSlug = new LocalizedString();

            foreach (string language in _project.Languages)
            {
                newName.SetValue(language, string.Concat("Updated Product ", language, " ", Helper.GetRandomString(10)));
                newSlug.SetValue(language, string.Concat("updated-product-", language, "-", Helper.GetRandomString(10)));
            }

            ChangeNameAction changeNameAction = new ChangeNameAction(newName);

            GenericAction changeSlugAction = new GenericAction("changeSlug");
            changeSlugAction.SetProperty("slug", newSlug);
            changeSlugAction.SetProperty("staged", true);

            List<UpdateAction> actions = new List<UpdateAction>();
            actions.Add(changeNameAction);
            actions.Add(changeSlugAction);

            Response<Product> response = await _client.Products().UpdateProductByKeyAsync(_testProducts[1].Key, _testProducts[1].Version, actions);
            Assert.True(response.Success);

            _testProducts[1] = response.Result;
            Assert.NotNull(_testProducts[1].Id);

            foreach (string language in _project.Languages)
            {
                Assert.Equal(_testProducts[1].MasterData.Staged.Name[language], newName[language]);
                Assert.Equal(_testProducts[1].MasterData.Staged.Slug[language], newSlug[language]);
            }
        }
    }
}
