using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using commercetools.Core.Common;
using commercetools.Core.Common.UpdateActions;
using commercetools.Core.Project;
using commercetools.Core.Zones;
using Newtonsoft.Json.Linq;
using Xunit;

namespace commercetools.Core.Tests
{
    /// <summary>
    /// Test the API methods in the ZoneManager class.
    /// </summary>
    public class ZoneManagerTest : IDisposable
    {
        private Client _client;
        private Project.Project _project;
        private List<Zone> _testZones;

        /// <summary>
        /// Test setup
        /// </summary>
        public ZoneManagerTest()
        {
            _client = new Client(Helper.GetConfiguration());

            Task<Response<Project.Project>> projectTask = _client.Project().GetProjectAsync();
            projectTask.Wait();
            Assert.True(projectTask.Result.Success);
            _project = projectTask.Result.Result;

            _testZones = new List<Zone>();

            for (int i = 0; i < 5; i++)
            {
                ZoneDraft zoneDraft = Helper.GetTestZoneDraft();
                Task<Response<Zone>> zoneTask = _client.Zones().CreateZoneAsync(zoneDraft);
                zoneTask.Wait();
                Assert.True(zoneTask.Result.Success);

                Zone zone = zoneTask.Result.Result;
                Assert.NotNull(zone.Id);

                _testZones.Add(zone);
            }
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        public void Dispose()
        {
            foreach (Zone zone in _testZones)
            {
                Task task = _client.Zones().DeleteZoneAsync(zone);
                task.Wait();
            }
        }

        /// <summary>
        /// Tests the ZoneManager.GetZoneByIdAsync method.
        /// </summary>
        /// <see cref="ZoneManager.GetZoneByIdAsync"/>
        [Fact]
        public async Task ShouldGetZoneByIdAsync()
        {
            Response<Zone> response = await _client.Zones().GetZoneByIdAsync(_testZones[0].Id);
            Assert.True(response.Success);

            Zone zone = response.Result;
            Assert.NotNull(zone);
            Assert.Equal(zone.Id, _testZones[0].Id);
        }

        /// <summary>
        /// Tests the ZoneManager.QueryZonesAsync method.
        /// </summary>
        /// <see cref="ZoneManager.QueryZonesAsync"/>
        [Fact]
        public async Task ShouldQueryZonesAsync()
        {
            Response<ZoneQueryResult> response = await _client.Zones().QueryZonesAsync();
            Assert.True(response.Success);

            ZoneQueryResult zoneQueryResult = response.Result;
            Assert.NotNull(zoneQueryResult.Results);
            Assert.True(zoneQueryResult.Results.Count >= 1);

            int limit = 2;
            response = await _client.Zones().QueryZonesAsync(limit: limit);
            Assert.True(response.Success);

            zoneQueryResult = response.Result;
            Assert.NotNull(zoneQueryResult.Results);
            Assert.True(zoneQueryResult.Results.Count <= limit);
        }

        /// <summary>
        /// Tests the ZoneManager.CreateZoneAsync and ZoneManager.DeleteZoneAsync methods.
        /// </summary>
        /// <see cref="ZoneManager.CreateZoneAsync"/>
        /// <seealso cref="ZoneManager.DeleteZoneAsync(Zone)"/>
        [Fact]
        public async Task ShouldCreateAndDeleteZoneAsync()
        {
            ZoneDraft zoneDraft = Helper.GetTestZoneDraft();
            Response<Zone> response = await _client.Zones().CreateZoneAsync(zoneDraft);
            Assert.True(response.Success);

            Zone zone = response.Result;
            Assert.NotNull(zone.Id);

            string deletedZoneId = zone.Id;

            Response<JObject> deleteResponse = await _client.Zones().DeleteZoneAsync(zone);
            Assert.True(deleteResponse.Success);

            response = await _client.Zones().GetZoneByIdAsync(deletedZoneId);
            Assert.False(response.Success);
        }

        /// <summary>
        /// Tests the ZoneManager.UpdateZoneAsync method.
        /// </summary>
        /// <see cref="ZoneManager.UpdateZoneAsync(Zone, System.Collections.Generic.List{UpdateAction})"/>
        [Fact]
        public async Task ShouldUpdateZoneAsync()
        {
            string newName = string.Concat("Test Zone ", Helper.GetRandomString(10));
            string newDescription = string.Concat("Test Description ", Helper.GetRandomString(10));

            GenericAction changeNameAction = new GenericAction("changeName");
            changeNameAction.SetProperty("name", newName);

            GenericAction setDescriptionAction = new GenericAction("setDescription");
            setDescriptionAction.SetProperty("description", newDescription);

            List<UpdateAction> actions = new List<UpdateAction>();
            actions.Add(changeNameAction);
            actions.Add(setDescriptionAction);

            Response<Zone> response = await _client.Zones().UpdateZoneAsync(_testZones[1], actions);
            Assert.True(response.Success);

            _testZones[1] = response.Result;
            Assert.NotNull(_testZones[1].Id);

            foreach (string language in _project.Languages)
            {
                Assert.Equal(_testZones[1].Name, newName);
                Assert.Equal(_testZones[1].Description, newDescription);
            }
        }
    }
}
