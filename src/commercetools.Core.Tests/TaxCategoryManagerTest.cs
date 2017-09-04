using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using commercetools.Core.Common;
using commercetools.Core.Common.UpdateActions;
using commercetools.Core.Project;
using commercetools.Core.TaxCategories;
using Newtonsoft.Json.Linq;
using Xunit;

namespace commercetools.Core.Tests
{
    /// <summary>
    /// Test the API methods in the TaxCategoryManager class.
    /// </summary>
    public class TaxCategoryManagerTest : IDisposable
    {
        private Client _client;
        private Project.Project _project;
        private List<TaxCategory> _testTaxCategories;

        /// <summary>
        /// Test setup
        /// </summary>
        public TaxCategoryManagerTest()
        {
            _client = new Client(Helper.GetConfiguration());

            Task<Response<Project.Project>> projectTask = _client.Project().GetProjectAsync();
            projectTask.Wait();
            Assert.True(projectTask.Result.Success);
            _project = projectTask.Result.Result;

            Assert.NotNull(_project.Countries);
            Assert.True(_project.Countries.Count >= 1);

            _testTaxCategories = new List<TaxCategory>();

            for (int i = 0; i < 5; i++)
            {
                TaxCategoryDraft taxCategoryDraft = Helper.GetTestTaxCategoryDraft(_project);
                Task<Response<TaxCategory>> taxCategoryTask = _client.TaxCategories().CreateTaxCategoryAsync(taxCategoryDraft);
                taxCategoryTask.Wait();
                Assert.True(taxCategoryTask.Result.Success);

                TaxCategory taxCategory = taxCategoryTask.Result.Result;
                Assert.NotNull(taxCategory.Id);

                _testTaxCategories.Add(taxCategory);
            }
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        public void Dispose()
        {
            foreach (TaxCategory taxCategory in _testTaxCategories)
            {
                Task task = _client.TaxCategories().DeleteTaxCategoryAsync(taxCategory);
                task.Wait();
            }
        }

        /// <summary>
        /// Tests the TaxCategoryManager.GetTaxCategoryByIdAsync method.
        /// </summary>
        /// <see cref="TaxCategoryManager.GetTaxCategoryByIdAsync"/>
        [Fact]
        public async Task ShouldGetTaxCategoryByIdAsync()
        {
            Response<TaxCategory> response = await _client.TaxCategories().GetTaxCategoryByIdAsync(_testTaxCategories[0].Id);
            Assert.True(response.Success);

            TaxCategory taxCategory = response.Result;
            Assert.NotNull(taxCategory.Id);
            Assert.Equal(taxCategory.Id, _testTaxCategories[0].Id);
        }

        /// <summary>
        /// Tests the TaxCategoryManager.QueryTaxCategoriesAsync method.
        /// </summary>
        /// <see cref="TaxCategoryManager.QueryTaxCategoriesAsync"/>
        [Fact]
        public async Task ShouldQueryTaxCategoriesAsync()
        {
            Response<TaxCategoryQueryResult> response = await _client.TaxCategories().QueryTaxCategoriesAsync();
            Assert.True(response.Success);

            TaxCategoryQueryResult taxCategoryQueryResult = response.Result;
            Assert.NotNull(taxCategoryQueryResult.Results);
            Assert.True(taxCategoryQueryResult.Results.Count >= 1);

            int limit = 2;
            response = await _client.TaxCategories().QueryTaxCategoriesAsync(limit: limit);

            taxCategoryQueryResult = response.Result;
            Assert.NotNull(taxCategoryQueryResult.Results);
            Assert.True(taxCategoryQueryResult.Results.Count <= limit);
        }

        /// <summary>
        /// Tests the TaxCategoryManager.CreateTaxCategoryAsync and TaxCategoryManager.DeleteTaxCategoryAsync methods.
        /// </summary>
        /// <see cref="TaxCategoryManager.CreateTaxCategoryAsync"/>
        /// <seealso cref="TaxCategoryManager.DeleteTaxCategoryAsync(TaxCategory)"/>
        [Fact]
        public async Task ShouldCreateAndDeleteTaxCategoryAsync()
        {
            TaxCategoryDraft taxCategoryDraft = Helper.GetTestTaxCategoryDraft(_project);
            Response<TaxCategory> response = await _client.TaxCategories().CreateTaxCategoryAsync(taxCategoryDraft);
            Assert.True(response.Success);

            TaxCategory taxCategory = response.Result;
            Assert.NotNull(taxCategory.Id);
            Assert.Equal(taxCategory.Name, taxCategoryDraft.Name);
            Assert.Equal(taxCategory.Description, taxCategoryDraft.Description);
            Assert.Equal(taxCategory.Rates.Count, taxCategoryDraft.Rates.Count);

            string deletedTaxCategoryId = taxCategory.Id;

            Response<JObject> deleteResponse = await _client.TaxCategories().DeleteTaxCategoryAsync(taxCategory.Id, taxCategory.Version);
            Assert.True(deleteResponse.Success);

            response = await _client.TaxCategories().GetTaxCategoryByIdAsync(deletedTaxCategoryId);
            Assert.False(response.Success);
        }

        /// <summary>
        /// Tests the TaxCategoryManager.UpdateTaxCategoryAsync method.
        /// </summary>
        /// <see cref="TaxCategoryManager.UpdateTaxCategoryAsync(TaxCategory, System.Collections.Generic.List{UpdateAction})"/>
        [Fact]
        public async Task ShouldUpdateTaxCategoryAsync()
        {
            string newName = string.Concat(_testTaxCategories[1].Name, " Updated");
            string newDescription = string.Concat(_testTaxCategories[1].Description, " Updated");

            GenericAction changeNameAction = new GenericAction("changeName");
            changeNameAction.SetProperty("name", newName);

            GenericAction setDescriptionAction = new GenericAction("setDescription");
            setDescriptionAction.SetProperty("description", newDescription);

            List<UpdateAction> actions = new List<UpdateAction>();
            actions.Add(changeNameAction);
            actions.Add(setDescriptionAction);

            Response<TaxCategory> response = await _client.TaxCategories().UpdateTaxCategoryAsync(_testTaxCategories[1], actions);
            Assert.True(response.Success);

            _testTaxCategories[1] = response.Result;
            Assert.NotNull(_testTaxCategories[1].Id);
            Assert.Equal(_testTaxCategories[1].Name, newName);
            Assert.Equal(_testTaxCategories[1].Description, newDescription);
        }
    }
}
