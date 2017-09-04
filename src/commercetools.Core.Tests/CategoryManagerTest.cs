using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using commercetools.Core.Categories;
using commercetools.Core.Categories.UpdateActions;
using commercetools.Core.Common;
using commercetools.Core.Common.UpdateActions;
using commercetools.Core.Project;
using Xunit;

namespace commercetools.Core.Tests
{
    /// <summary>
    /// Test the API methods in the CategoryManager class.
    /// </summary>
    public class CategoryManagerTest : IDisposable
    {
        private Client _client;
        private Project.Project _project;
        private List<Category> _testCategories;
        
        /// <summary>
        /// Test setup
        /// </summary>
        public CategoryManagerTest()
        {
            _client = new Client(Helper.GetConfiguration());

            Task<Response<Project.Project>> projectTask = _client.Project().GetProjectAsync();
            projectTask.Wait();
            Assert.True(projectTask.Result.Success);
            _project = projectTask.Result.Result;

            _testCategories = new List<Category>();

            for (int i = 0; i < 5; i++)
            {
                CategoryDraft categoryDraft = Helper.GetTestCategoryDraft(_project);
                Task<Response<Category>> categoryTask = _client.Categories().CreateCategoryAsync(categoryDraft);
                categoryTask.Wait();
                Assert.True(categoryTask.Result.Success);

                Category category = categoryTask.Result.Result;
                Assert.NotNull(category.Id);

                _testCategories.Add(category);
            }
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        public void Dispose()
        {
            foreach (Category category in _testCategories)
            {
                Task<Response<Category>> categoryTask = _client.Categories().DeleteCategoryAsync(category);
                categoryTask.Wait();
            }
        }

        /// <summary>
        /// Tests the CategoryManager.GetCategoryByIdAsync method.
        /// </summary>
        /// <see cref="CategoryManager.GetCategoryByIdAsync"/>
        [Fact]
        public async Task ShouldGetCategoryByIdAsync()
        {
            Response<Category> response = await _client.Categories().GetCategoryByIdAsync(_testCategories[0].Id);
            Assert.True(response.Success);

            Category category = response.Result;
            Assert.NotNull(category.Id);
            Assert.Equal(category.Id, _testCategories[0].Id);
        }

        /// <summary>
        /// Tests the CategoryManager.QueryCategoriesAsync method.
        /// </summary>
        /// <see cref="CategoryManager.QueryCategoriesAsync"/>
        [Fact]
        public async Task ShouldQueryCategoriesAsync()
        {
            Response<CategoryQueryResult> response = await _client.Categories().QueryCategoriesAsync();
            Assert.True(response.Success);

            CategoryQueryResult categoryQueryResult = response.Result;
            Assert.NotNull(categoryQueryResult.Results);
            Assert.True(categoryQueryResult.Results.Count >= 1);

            int limit = 2;
            response = await _client.Categories().QueryCategoriesAsync(limit: limit);
            Assert.True(response.Success);

            categoryQueryResult = response.Result;
            Assert.NotNull(categoryQueryResult.Results);
            Assert.True(categoryQueryResult.Results.Count <= limit);
        }

        /// <summary>
        /// Tests the CategoryManager.CreateCategoryAsync and CategoryManager.DeleteCategoryAsync methods.
        /// </summary>
        /// <see cref="CategoryManager.CreateCategoryAsync"/>
        /// <seealso cref="CategoryManager.DeleteCategoryAsync(Category)"/>
        [Fact]
        public async Task ShouldCreateAndDeleteCategoryAsync()
        {
            CategoryDraft categoryDraft = Helper.GetTestCategoryDraft(_project);
            Response<Category> response = await _client.Categories().CreateCategoryAsync(categoryDraft);
            Assert.True(response.Success);

            Category category = response.Result;
            Assert.NotNull(category.Id);

            string deletedCategoryId = category.Id;

            response = await _client.Categories().DeleteCategoryAsync(category);
            Assert.True(response.Success);

            response = await _client.Categories().GetCategoryByIdAsync(deletedCategoryId);
            Assert.False(response.Success);
        }

        /// <summary>
        /// Tests the CategoryManager.UpdateCategoryAsync method.
        /// </summary>
        /// <see cref="CategoryManager.UpdateCategoryAsync(Category, System.Collections.Generic.List{UpdateAction})"/>
        [Fact]
        public async Task ShouldUpdateCategoryAsync()
        {
            LocalizedString newName = new LocalizedString();
            LocalizedString newSlug = new LocalizedString();

            foreach (string language in _project.Languages)
            {
                newName.SetValue(language, string.Concat("New Name ", language));
                newSlug.SetValue(language, string.Concat("slug-updated-", language));
            }

            ChangeNameAction changeNameAction = new ChangeNameAction(newName);

            GenericAction changeSlugAction = new GenericAction("changeSlug");
            changeSlugAction.SetProperty("slug", newSlug);

            List<UpdateAction> actions = new List<UpdateAction>();
            actions.Add(changeNameAction);
            actions.Add(changeSlugAction);

            Response<Category> response = await _client.Categories().UpdateCategoryAsync(_testCategories[2], actions);
            Assert.True(response.Success);

            _testCategories[2] = response.Result;
            Assert.NotNull(_testCategories[2].Id);

            foreach (string language in _project.Languages)
            {
                Assert.Equal(_testCategories[2].Name[language], newName[language]);
                Assert.Equal(_testCategories[2].Slug[language], newSlug[language]);
            }
        }
    }
}