﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using commercetools.Common;
using commercetools.Common.UpdateActions;
using commercetools.Products;
using commercetools.Products.UpdateActions;
using commercetools.ProductTypes;
using commercetools.Project;
using commercetools.TaxCategories;

using NUnit.Framework;

namespace commercetools.Tests
{
    /// <summary>
    /// Test the API methods in the ProductManager class.
    /// </summary>
    [TestFixture]
    public class ProductManagerTest
    {
        private Client _client;
        private Project.Project _project;
        private List<Product> _testProducts;
        private ProductType _testProductType;
        private TaxCategory _testTaxCategory;

        private string _productIdWithHighPrecisionPrice;
        private string _productIdWithCentPrecisionPrice;

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

            for (int i = 0; i < 5; i++) // Default Money
            {
                ProductDraft productDraft = Helper.GetTestProductDraft(_project, _testProductType.Id, _testTaxCategory.Id);

                Task<Response<Product>> productTask = _client.Products().CreateProductAsync(productDraft);
                productTask.Wait();
                Assert.IsTrue(productTask.Result.Success);

                Product product = productTask.Result.Result;
                Assert.NotNull(product.Id);

                _testProducts.Add(product);
            }
            //Add Product with Money Price => CentPrecision Money Type
            _productIdWithCentPrecisionPrice = AddProductWithSpecificMoneyType(MoneyTestTypes.CentPrecision);
            //Add Product with Money Price => HighPrecision Money Type
            _productIdWithHighPrecisionPrice = AddProductWithSpecificMoneyType(MoneyTestTypes.HighPrecision);
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
        /// Tests the ProductManager.GetProductByIdAsync method.
        /// </summary>
        /// <see cref="ProductManager.GetProductByIdAsync"/>
        [Test]
        public async Task ShouldGetProductByIdAsync()
        {
            Response<Product> response = await _client.Products().GetProductByIdAsync(_testProducts[0].Id);
            Assert.IsTrue(response.Success);

            Product product = response.Result;
            Assert.NotNull(product.Id);
            Assert.AreEqual(product.Id, _testProducts[0].Id);
        }

        /// <summary>
        /// Tests the ProductManager.GetProductByKeyAsync method.
        /// </summary>
        /// <see cref="ProductManager.GetProductByKeyAsync"/>
        [Test]
        public async Task ShouldGetProductByKeyAsync()
        {
            Response<Product> response = await _client.Products().GetProductByKeyAsync(_testProducts[1].Key);
            Assert.IsTrue(response.Success);

            Product product = response.Result;
            Assert.NotNull(product.Id);
            Assert.AreEqual(product.Key, _testProducts[1].Key);
        }

        /// <summary>
        /// Tests the ProductManager.QueryProductsAsync method.
        /// </summary>
        /// <see cref="ProductManager.QueryProductsAsync"/>
        [Test]
        public async Task ShouldQueryProductsAsync()
        {
            Response<ProductQueryResult> response = await _client.Products().QueryProductsAsync();
            Assert.IsTrue(response.Success);

            ProductQueryResult productQueryResult = response.Result;
            Assert.NotNull(productQueryResult.Results);
            Assert.GreaterOrEqual(productQueryResult.Results.Count, 1);

            int limit = 2;
            response = await _client.Products().QueryProductsAsync(limit: limit);
            Assert.IsTrue(response.Success);

            productQueryResult = response.Result;
            Assert.NotNull(productQueryResult.Results);
            Assert.LessOrEqual(productQueryResult.Results.Count, limit);
        }

        /// <summary>
        /// Tests the ProductManager.CreateProductAsync and ProductManager.DeleteProductAsync methods.
        /// </summary>
        /// <see cref="ProductManager.CreateProductAsync"/>
        /// <seealso cref="ProductManager.DeleteProductAsync(commercetools.Products.Product)"/>
        [Test]
        public async Task ShouldCreateAndDeleteProductAsync()
        {
            ProductDraft productDraft = Helper.GetTestProductDraft(_project, _testProductType.Id, _testTaxCategory.Id);
            Image image = new Image("http://via.placeholder.com/350x150", new ImageDimensions(350, 150)) { Label = "TestLabel" };
            productDraft.MasterVariant.Images = new List<Image>() { image };

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
            Assert.IsTrue(response.Success);

            Product product = response.Result;
            Assert.NotNull(product.Id);

            Assert.NotNull(product.MasterData);
            Assert.NotNull(product.MasterData.Current);
            Assert.NotNull(product.MasterData.Current.MasterVariant);
            Assert.NotNull(product.MasterData.Current.MasterVariant.Images);
            Assert.IsTrue(product.MasterData.Current.MasterVariant.Images.Count == 1);
            Image imageReturned = product.MasterData.Current.MasterVariant.Images[0];
            Assert.AreEqual(image.Label, imageReturned.Label);
            Assert.AreEqual(image.Url, imageReturned.Url);
            Assert.AreEqual(image.Dimensions.Width, imageReturned.Dimensions.Width);
            Assert.AreEqual(image.Dimensions.Height, imageReturned.Dimensions.Height);

            string deletedProductId = product.Id;

            response = await _client.Products().DeleteProductAsync(product.Id, product.Version);
            Assert.IsTrue(response.Success);

            response = await _client.Products().GetProductByIdAsync(deletedProductId);
            Assert.IsFalse(response.Success);
        }

        /// <summary>
        /// Tests the ProductManager.UpdateProductAsync method.
        /// </summary>
        /// <see cref="ProductManager.UpdateProductAsync(commercetools.Products.Product, System.Collections.Generic.List{commercetools.Common.UpdateAction}, string, string, Guid, Guid)"/>
        [Test]
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
            Assert.IsTrue(response.Success);

            _testProducts[2] = response.Result;
            Assert.NotNull(_testProducts[2].Id);
            Assert.AreEqual(_testProducts[2].Key, newKey);

            foreach (string language in _project.Languages)
            {
                Assert.AreEqual(_testProducts[2].MasterData.Staged.Slug.GetValue(language), newSlug.GetValue(language));
            }
        }

        /// <summary>
        /// Tests the ProductManager.UpdateProductByKeyAsync method.
        /// </summary>
        /// <see cref="ProductManager.UpdateProductByKeyAsync"/>
        [Test]
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
            Assert.IsTrue(response.Success);

            _testProducts[1] = response.Result;
            Assert.NotNull(_testProducts[1].Id);

            foreach (string language in _project.Languages)
            {
                Assert.AreEqual(_testProducts[1].MasterData.Staged.Name[language], newName[language]);
                Assert.AreEqual(_testProducts[1].MasterData.Staged.Slug[language], newSlug[language]);
            }
        }


        [Test]
        public async Task ShouldGetProductWithHighPrecisionPriceAsync()
        {
            Response<Product> response = await _client.Products().GetProductByIdAsync(_productIdWithHighPrecisionPrice);
            Assert.IsTrue(response.Success);

            Product product = response.Result;
            Assert.NotNull(product.Id);
            Assert.AreEqual(product.Id, _productIdWithHighPrecisionPrice);

            Assert.NotNull(product.MasterData.Staged.MasterVariant);
            Assert.IsNotEmpty(product.MasterData.Staged.MasterVariant.Prices);
            Assert.IsNotNull(product.MasterData.Staged.MasterVariant.Prices[0].Value);

            Assert.IsInstanceOf(typeof(HighPrecisionMoney), product.MasterData.Staged.MasterVariant.Prices[0].Value);

            var highPrecisionMoney = product.MasterData.Staged.MasterVariant.Prices[0].Value as HighPrecisionMoney;

            Assert.NotNull(highPrecisionMoney);
            Assert.AreEqual(3, highPrecisionMoney.FractionDigits);
            Assert.AreEqual(1000, highPrecisionMoney.PreciseAmount);

        }

        [Test]
        public async Task ShouldGetProductWithCentPrecisionPriceAsync()
        {
            Response<Product> response = await _client.Products().GetProductByIdAsync(_productIdWithCentPrecisionPrice);
            Assert.IsTrue(response.Success);

            Product product = response.Result;
            Assert.NotNull(product.Id);
            Assert.AreEqual(product.Id, _productIdWithCentPrecisionPrice);

            Assert.NotNull(product.MasterData.Staged.MasterVariant);
            Assert.IsNotEmpty(product.MasterData.Staged.MasterVariant.Prices);
            Assert.IsNotNull(product.MasterData.Staged.MasterVariant.Prices[0].Value);

            Assert.IsInstanceOf(typeof(CentPrecisionMoney), product.MasterData.Staged.MasterVariant.Prices[0].Value);

            var centPrecisionMoney = product.MasterData.Staged.MasterVariant.Prices[0].Value as CentPrecisionMoney;

            Assert.NotNull(centPrecisionMoney);
            Assert.AreEqual(2, centPrecisionMoney.FractionDigits);
        }

        private string AddProductWithSpecificMoneyType(MoneyTestTypes moneyTestTypes)
        {
            ProductDraft productDraft = Helper.GetTestProductDraft(_project, _testProductType.Id, _testTaxCategory.Id, moneyTestTypes);

            Task<Response<Product>> productTask = _client.Products().CreateProductAsync(productDraft);
            productTask.Wait();
            Assert.IsTrue(productTask.Result.Success);

            Product product = productTask.Result.Result;
            Assert.NotNull(product.Id);

            _testProducts.Add(product);
            return product.Id;
        }
    }
}
