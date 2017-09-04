using System;
using System.Threading.Tasks;
using commercetools.Core.Common;
using commercetools.Core.Project;
using Xunit;

namespace commercetools.Core.Tests
{
    /// <summary>
    /// Test the API methods in the ProjectManager class.
    /// </summary>
    public class ProjectManagerTest : IDisposable
    {
        private Client _client;

        /// <summary>
        /// Test setup
        /// </summary>
        public ProjectManagerTest()
        {
            _client = new Client(Helper.GetConfiguration());
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Tests the ProjectManager.GetProjectAsync method.
        /// </summary>
        /// <see cref="ProjectManager.GetProjectAsync"/>
        [Fact]
        public async Task ShouldGetProjectAsync()
        {
            Response<Project.Project> response = await _client.Project().GetProjectAsync();
            Assert.True(response.Success);

            Project.Project project = response.Result;
            Assert.NotNull(project.Key);
            Assert.NotNull(project.Name);
            Assert.NotNull(project.CreatedAt);
        }
    }
}
