using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using commercetools.Core.CartDiscounts;
using commercetools.Core.Common;
using commercetools.Core.DiscountCodes;
using commercetools.Core.DiscountCodes.UpdateActions;
using commercetools.Core.Project;
using Xunit;
using ChangeIsActiveAction = commercetools.Core.DiscountCodes.UpdateActions.ChangeIsActiveAction;
using SetDescriptionAction = commercetools.Core.DiscountCodes.UpdateActions.SetDescriptionAction;

namespace commercetools.Core.Tests
{
    public class DiscountCodeManagerTests : IDisposable
    {
        private Client _client;
        private DiscountCode _testDiscountCode;

        private Project.Project _project;

        /// <summary>
        /// Test setup
        /// </summary>
        public DiscountCodeManagerTests()
        {
            _client = new Client(Helper.GetConfiguration());

            Task<Response<Project.Project>> projectTask = _client.Project().GetProjectAsync();
            projectTask.Wait();
            Assert.True(projectTask.Result.Success);
            _project = projectTask.Result.Result;

            Assert.True(_project.Languages.Count > 0);
            Assert.True(_project.Currencies.Count > 0);

            Task<DiscountCode> discountCodeTask =
                Helper.CreateTestDiscountCode(this._project, this._client);
            discountCodeTask.Wait();

            Assert.NotNull(discountCodeTask.Result);

            _testDiscountCode = discountCodeTask.Result;
            Assert.NotNull(_testDiscountCode);
            Assert.NotNull(_testDiscountCode.Id);
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        public void Dispose()
        {
            Task<DiscountCode> deleteDiscountCodeTask =
                Helper.DeleteDiscountCode(this._client, _testDiscountCode);
            deleteDiscountCodeTask.Wait();
        }

        /// <summary>
        /// Tests the DiscountCodeManager.GetDiscountCodeByIdAsync method.
        /// </summary>
        /// <see cref="DiscountCodeManager.GetDiscountCodeByIdAsync"/>
        [Fact]
        public async Task ShouldGetDiscountCodeByIdAsync()
        {
            Response<DiscountCode> response = await _client.DiscountCodes().GetDiscountCodeByIdAsync(_testDiscountCode.Id);
            Assert.True(response.Success);

            DiscountCode discountCode = response.Result;
            Assert.NotNull(discountCode.Id);
            Assert.Equal(_testDiscountCode.Id, discountCode.Id);
        }

        /// <summary>
        /// Tests the DiscountCodeManager.QueryDiscountCodesAsync method.
        /// </summary>
        /// <see cref="DiscountCodeManager.QueryDiscountCodesAsync"/>
        [Fact]
        public async Task ShouldQueryDiscountCodeByCodeAsync()
        {
            Response<DiscountCodeQueryResult> response = await _client.DiscountCodes().QueryDiscountCodesAsync("code=\""+_testDiscountCode.Code +"\"");
            Assert.True(response.Success);

            DiscountCodeQueryResult discountCode = response.Result;
            Assert.NotNull(discountCode);
            Assert.Equal(1, discountCode.Count);
        }

        /// <summary>
        /// Tests the DiscountCodeManager.CreateDiscountCodeAsync method.
        /// </summary>
        /// <see cref="DiscountCodeManager.CreateDiscountCodeAsync"/>
        [Fact]
        public async Task ShouldCreateDiscountCodeAsync()
        {
            // Arrange
            var discountCodeDraft = await Helper.GetDiscountCodeDraft(this._project, this._client);
            var cartDiscountReferences = discountCodeDraft.CartDiscounts.Select(d => new Reference
            {
                Id = d.Id,
                ReferenceType = ReferenceType.CartDiscount
            });
            // Act
            Response<DiscountCode> discountCodeResponse = await _client.DiscountCodes().CreateDiscountCodeAsync(discountCodeDraft);

            // Assert
            var discountCode = discountCodeResponse.Result;
            Assert.NotNull(discountCode);
            Assert.NotNull(discountCode.Id);
            Assert.Equal(discountCodeDraft.CartPredicate, discountCode.CartPredicate);
            Assert.Equal(discountCodeDraft.Name.Values, discountCode.Name.Values);
            Assert.Equal(discountCodeDraft.Description.Values, discountCode.Description.Values);
            Assert.Equal(discountCodeDraft.IsActive, discountCode.IsActive);
            Assert.Equal(discountCodeDraft.MaxApplications, discountCode.MaxApplications);
            Assert.Equal(discountCodeDraft.MaxApplicationsPerCustomer, discountCode.MaxApplicationsPerCustomer);
            for (var i = 0; i < discountCode.CartDiscounts.Count; i++)
            {
                var cartDiscount = discountCode.CartDiscounts[i];
                var cartDiscountReference = cartDiscountReferences.ElementAt(i);
;               Assert.Equal(cartDiscount.Id, cartDiscountReference.Id);
                Assert.Equal(cartDiscount.ReferenceType, cartDiscountReference.ReferenceType);
            }
            // Cleanup
            await Helper.DeleteDiscountCode(this._client, discountCode);
        }

        /// <summary>
        /// Tests the DiscountCodeManager.DeleteDiscountCodeAsync method.
        /// </summary>
        /// <see cref="DiscountCodeManager.DeleteDiscountCodeAsync(DiscountCode)"/>
        [Fact]
        public async Task ShouldDeleteDiscountCodeAsync()
        {
            // Arrange            
            var discountCode = await Helper.CreateTestDiscountCode(this._project, this._client);

            // Act
            Response<DiscountCode> discountCodeDeleteResponse = await _client.DiscountCodes().DeleteDiscountCodeAsync(discountCode);


            // Assert
            var deletedtDiscountCode = discountCodeDeleteResponse.Result;
            Assert.NotNull(deletedtDiscountCode);
            Assert.NotNull(deletedtDiscountCode.Id);
            var getDiscountCodeTask = await this._client.DiscountCodes().GetDiscountCodeByIdAsync(deletedtDiscountCode.Id);

            Assert.Equal(404, getDiscountCodeTask.StatusCode);
            Assert.Equal(false, getDiscountCodeTask.Success);
        }

        /// <summary>
        /// Tests the DiscountCodeManager.UpdateDiscountCodeAsync method.
        /// </summary>
        /// <see cref="DiscountCodeManager.UpdateDiscountCodeAsync(DiscountCode,UpdateAction)"/>
        [Fact]
        public async Task ShouldSetNameSetDescriptionDiscountCodeAsync()
        {
            // Arrange
            var discountCode = await Helper.CreateTestDiscountCode(this._project, this._client);
            LocalizedString name = new LocalizedString();
            LocalizedString description = new LocalizedString();

            foreach (string language in this._project.Languages)
            {
                string randomPostfix = Helper.GetRandomString(10);
                name.SetValue(language, string.Concat("change-discount-code-name", language, " ", randomPostfix));
                description.SetValue(language, string.Concat("change-discount-code-description", language, "-", randomPostfix));
            }
            var setNameAction = new SetNameAction { Name = name };
            var setDescriptionAction = new SetDescriptionAction { Description = description };

            // Act
            var updatedDiscountCodeResponse = await this._client.DiscountCodes()
                .UpdateDiscountCodeAsync(discountCode, new List<UpdateAction> {
                    setNameAction, setDescriptionAction });


            // Assert
            var updatedDiscountCode = updatedDiscountCodeResponse.Result;
            Assert.NotNull(updatedDiscountCode);
            Assert.NotNull(updatedDiscountCode.Id);
            Assert.Equal(updatedDiscountCode.Name.Values, name.Values);
            Assert.Equal(updatedDiscountCode.Description.Values, description.Values);

            // Cleanup
            await Helper.DeleteDiscountCode(this._client, updatedDiscountCode);
        }

        /// <summary>
        /// Tests the DiscountCodeManager.UpdateDiscountCodeAsync method.
        /// </summary>
        /// <see cref="DiscountCodeManager.UpdateDiscountCodeAsync(DiscountCode,UpdateAction)"/>
        [Fact]
        public async Task ShouldRemoveNameandDescriptionDiscountCodeAsync()
        {
            // Arrange
            var discountCode = await Helper.CreateTestDiscountCode(this._project, this._client);
            var setNameAction = new SetNameAction();
            var setDescriptionAction = new SetDescriptionAction();

            // Act
            var updatedDiscountCodeResponse = await this._client.DiscountCodes()
                .UpdateDiscountCodeAsync(discountCode, new List<UpdateAction> {
                    setNameAction, setDescriptionAction });


            // Assert
            var updatedDiscountCode = updatedDiscountCodeResponse.Result;
            Assert.NotNull(updatedDiscountCode);
            Assert.NotNull(updatedDiscountCode.Id);
            Assert.Equal(0, updatedDiscountCode.Name.Values.Count);
            Assert.Equal(0, updatedDiscountCode.Description.Values.Count);

            // Cleanup
            await Helper.DeleteDiscountCode(this._client, updatedDiscountCode);
        }

        /// <summary>
        /// Tests the DiscountCodeManager.UpdateDiscountCodeAsync method.
        /// </summary>
        /// <see cref="DiscountCodeManager.UpdateDiscountCodeAsync(DiscountCode,UpdateAction)"/>
        [Fact]
        public async Task ShouldSetMaxApplicationsSetMaxApplicationsPerCustomerDiscountCodeAsync()
        {
            // Arrange
            var discountCode = await Helper.CreateTestDiscountCode(this._project, this._client);
            var setMaxApplications = new SetMaxApplicationsAction { MaxApplications = Helper.GetRandomNumber(3000, 5000) };
            var setMaxApplicationsPerCustomer = new SetMaxApplicationsPerCustomerAction { MaxApplicationsPerCustomer = Helper.GetRandomNumber(3000, 5000) };

            // Act
            var updatedDiscountCodeResponse = await this._client.DiscountCodes()
                .UpdateDiscountCodeAsync(discountCode, new List<UpdateAction> {
                    setMaxApplications, setMaxApplicationsPerCustomer });


            // Assert
            var updatedDiscountCode = updatedDiscountCodeResponse.Result;
            Assert.NotNull(updatedDiscountCode);
            Assert.NotNull(updatedDiscountCode.Id);
            Assert.Equal(updatedDiscountCode.MaxApplications, setMaxApplications.MaxApplications);
            Assert.Equal(updatedDiscountCode.MaxApplicationsPerCustomer, setMaxApplicationsPerCustomer.MaxApplicationsPerCustomer);

            // Cleanup
            await Helper.DeleteDiscountCode(this._client, updatedDiscountCode);
        }

        /// <summary>
        /// Tests the DiscountCodeManager.UpdateDiscountCodeAsync method.
        /// </summary>
        /// <see cref="DiscountCodeManager.UpdateDiscountCodeAsync(DiscountCode,UpdateAction)"/>
        [Fact]
        public async Task ShouldRemoveMaxApplicationsAndMAxApplicationsPerCustomerDiscountCodeAsync()
        {
            // Arrange
            var discountCode = await Helper.CreateTestDiscountCode(this._project, this._client);
            var setMaxApplications = new SetMaxApplicationsAction();
            var setMaxApplicationsPerCustomer = new SetMaxApplicationsPerCustomerAction();

            // Act
            var updatedDiscountCodeResponse = await this._client.DiscountCodes()
                .UpdateDiscountCodeAsync(discountCode, new List<UpdateAction> {
                    setMaxApplications, setMaxApplicationsPerCustomer });


            // Assert
            var updatedDiscountCode = updatedDiscountCodeResponse.Result;
            Assert.NotNull(updatedDiscountCode);
            Assert.NotNull(updatedDiscountCode.Id);
            Assert.Null(updatedDiscountCode.MaxApplications);
            Assert.Null(updatedDiscountCode.MaxApplicationsPerCustomer);

            // Cleanup
            await Helper.DeleteDiscountCode(this._client, updatedDiscountCode);
        }

        /// <summary>
        /// Tests the DiscountCodeManager.UpdateDiscountCodeAsync method.
        /// </summary>
        /// <see cref="DiscountCodeManager.UpdateDiscountCodeAsync(DiscountCode,UpdateAction)"/>
        [Fact]
        public async Task ShouldChangeCartPredicateDiscountCodeAsync()
        {
            // Arrange
            var discountCode = await Helper.CreateTestDiscountCode(this._project, this._client);
            var setCartPredicate = new SetCartPredicateAction {CartPredicate = "totalPrice.centAmount > 100000" };

            // Act
            var updatedDiscountCodeResponse = await this._client.DiscountCodes()
                .UpdateDiscountCodeAsync(discountCode, setCartPredicate);


            // Assert
            var updatedDiscountCode = updatedDiscountCodeResponse.Result;
            Assert.NotNull(updatedDiscountCode);
            Assert.NotNull(updatedDiscountCode.Id);
            Assert.Equal(setCartPredicate.CartPredicate, updatedDiscountCode.CartPredicate);

            // Cleanup
            await Helper.DeleteDiscountCode(this._client, updatedDiscountCode);
        }

        /// <summary>
        /// Tests the DiscountCodeManager.UpdateDiscountCodeAsync method.
        /// </summary>
        /// <see cref="DiscountCodeManager.UpdateDiscountCodeAsync(DiscountCode,UpdateAction)"/>
        [Fact]
        public async Task ShouldRemoveCartPredicateDiscountCodeAsync()
        {
            // Arrange
            var discountCode = await Helper.CreateTestDiscountCode(this._project, this._client);
            var setCartPredicate = new SetCartPredicateAction();

            // Act
            var updatedDiscountCodeResponse = await this._client.DiscountCodes()
                .UpdateDiscountCodeAsync(discountCode, setCartPredicate);


            // Assert
            var updatedDiscountCode = updatedDiscountCodeResponse.Result;
            Assert.NotNull(updatedDiscountCode);
            Assert.NotNull(updatedDiscountCode.Id);
            Assert.Null(updatedDiscountCode.CartPredicate);

            // Cleanup
            await Helper.DeleteDiscountCode(this._client, updatedDiscountCode);
        }

        /// <summary>
        /// Tests the DiscountCodeManager.UpdateDiscountCodeAsync method.
        /// </summary>
        /// <see cref="DiscountCodeManager.UpdateDiscountCodeAsync(DiscountCode,UpdateAction)"/>
        [Fact]
        public async Task ShouldChangeCartDiscountsDiscountCodeAsync()
        {
            // Arrange
            var discountCode = await Helper.CreateTestDiscountCode(this._project, this._client);
            var oldCartDiscountId = discountCode.CartDiscounts.First().Id;
            var cartDiscount = await Helper.CreateTestCartDiscount(this._project, this._client);
            var reference = new List<Reference>()
            {
                new Reference {ReferenceType = ReferenceType.CartDiscount, Id = cartDiscount.Id}
            };
            var changeCartDiscounts = new ChangeCartDiscountsAction(reference);

            // Act
            var updatedDiscountCodeResponse = await this._client.DiscountCodes()
                .UpdateDiscountCodeAsync(discountCode, changeCartDiscounts);


            // Assert
            var updatedDiscountCode = updatedDiscountCodeResponse.Result;
            Assert.NotNull(updatedDiscountCode);
            Assert.NotNull(updatedDiscountCode.Id);
            for (var i = 0; i < discountCode.CartDiscounts.Count; i++)
            {
                var crtDiscount = updatedDiscountCode.CartDiscounts[i];
                var cartDiscountReference = changeCartDiscounts.CartDiscounts[i];
                ; Assert.Equal(crtDiscount.Id, cartDiscountReference.Id);
                Assert.Equal(crtDiscount.ReferenceType, cartDiscountReference.ReferenceType);
            }

            // Cleanup
            await Helper.DeleteDiscountCode(this._client, updatedDiscountCode);
            await this._client.CartDiscounts().DeleteCartDiscountAsync(oldCartDiscountId, 1);
        }

        /// <summary>
        /// Tests the DiscountCodeManager.UpdateDiscountCodeAsync method.
        /// </summary>
        /// <see cref="DiscountCodeManager.UpdateDiscountCodeAsync(DiscountCode,UpdateAction)"/>
        [Fact]
        public async Task ShouldChangeIsActiveDiscountCodeAsync()
        {
            // Arrange
            var discountCode = await Helper.CreateTestDiscountCode(this._project, this._client);
            var changeIsActive = new ChangeIsActiveAction(!discountCode.IsActive);

            // Act
            var updatedDiscountCodeResponse = await this._client.DiscountCodes()
                .UpdateDiscountCodeAsync(discountCode, changeIsActive);


            // Assert
            var updatedDiscountCode = updatedDiscountCodeResponse.Result;
            Assert.NotNull(updatedDiscountCode);
            Assert.NotNull(updatedDiscountCode.Id);
            Assert.Equal(changeIsActive.IsActive, updatedDiscountCode.IsActive);

            // Cleanup
            await Helper.DeleteDiscountCode(this._client, updatedDiscountCode);
        }
    }
}
