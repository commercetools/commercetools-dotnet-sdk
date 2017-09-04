using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using commercetools.Core.CartDiscounts;
using commercetools.Core.CartDiscounts.UpdateActions;
using commercetools.Core.Common;
using commercetools.Core.Project;
using Xunit;

namespace commercetools.Core.Tests
{
    /// <summary>
    /// Test the API methods in the CartDiscountManager class, along with some of the cart update actions.
    /// </summary>
    public class CartDiscountManagerTest : IDisposable
    {
        private Client _client;
        private CartDiscount _testCartDiscount;
        private Project.Project _project;

        /// <summary>
        /// Test setup
        /// </summary>
        public CartDiscountManagerTest()
        {
            _client = new Client(Helper.GetConfiguration());

            Task<Response<Project.Project>> projectTask = _client.Project().GetProjectAsync();
            projectTask.Wait();
            Assert.True(projectTask.Result.Success);
            _project = projectTask.Result.Result;

            Assert.True(_project.Languages.Count > 0);
            Assert.True(_project.Currencies.Count > 0);

            Task<CartDiscount> cartDiscountTask =
                Helper.CreateTestCartDiscount(this._project, this._client);
            cartDiscountTask.Wait();

            Assert.NotNull(cartDiscountTask.Result);

            _testCartDiscount = cartDiscountTask.Result;
            Assert.NotNull(_testCartDiscount);
            Assert.NotNull(_testCartDiscount.Id); 
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        public void Dispose()
        {
            Task task = _client.CartDiscounts().DeleteCartDiscountAsync(_testCartDiscount);
            task.Wait();         
        }

        /// <summary>
        /// Tests the CartDiscountManager.GetCartDiscountByIdAsync method.
        /// </summary>
        /// <see cref="CartDiscountManager.GetCartDiscountByIdAsync"/>
        [Fact]
        public async Task ShouldGetCartDiscountByIdAsync()
        {
            Response<CartDiscount> response = await _client.CartDiscounts().GetCartDiscountByIdAsync(_testCartDiscount.Id);
            Assert.True(response.Success);

            CartDiscount cartDiscount = response.Result;
            Assert.NotNull(cartDiscount.Id);
            Assert.Equal(cartDiscount.Id, _testCartDiscount.Id);
        }

        /// <summary>
        /// Tests the CartDiscountManager.CreateCartDiscountAsync method.
        /// </summary>
        /// <see cref="CartDiscountManager.CreateCartDiscountAsync"/>
        [Fact]
        public async Task ShouldCreateCartDiscountAsync()
        {
            // Arrange
            CartDiscountDraft cartDiscountDraft = await Helper.GetTestCartDiscountDraft(this._project, this._client);

            // Act
            Response<CartDiscount> cartDiscountResponse = await _client.CartDiscounts().CreateCartDiscountAsync(cartDiscountDraft);

            // Assert
            var cartDiscount = cartDiscountResponse.Result;
            Assert.NotNull(cartDiscount);
            Assert.NotNull(cartDiscount.Id);
            Assert.Equal(cartDiscountDraft.SortOrder, cartDiscount.SortOrder);
            Assert.Equal(cartDiscountDraft.CartPredicate, cartDiscount.CartPredicate);
            Assert.Equal(cartDiscountDraft.Name.Values, cartDiscount.Name.Values);
            Assert.Equal(cartDiscountDraft.Description.Values, cartDiscount.Description.Values);
            Assert.True((cartDiscount.ValidFrom - cartDiscountDraft.ValidFrom).Value.TotalSeconds <= 1);
            Assert.True((cartDiscount.ValidUntil - cartDiscountDraft.ValidUntil).Value.TotalSeconds <= 1);
            Assert.Equal(cartDiscountDraft.IsActive, cartDiscount.IsActive);
            Assert.Equal(cartDiscountDraft.RequiresDiscountCode, cartDiscount.RequiresDiscountCode);
            Assert.Equal(cartDiscount.Target.Type, cartDiscountDraft.Target.Type);
            Assert.Equal(cartDiscount.Target.Predicate, cartDiscountDraft.Target.Predicate);
            Assert.Equal(cartDiscount.Value.Type, cartDiscountDraft.Value.Type);

            // Cleanup
            await _client.CartDiscounts().DeleteCartDiscountAsync(cartDiscount);
        }

        [Fact]
        public async Task ShouldChangeCartPredicateCartDiscountAsync()
        {
            // Arrange
            var changePredicateAction = new ChangeCartPredicateAction("lineItemCount(1=1) > 4");
            var cartDiscount = await Helper.CreateTestCartDiscount(this._project, this._client);

            // Act
            var updatedCartDiscountResponse = await this._client.CartDiscounts()
                .UpdateCartDiscountAsync(cartDiscount, changePredicateAction);


            // Assert
            var updatedCartDiscount = updatedCartDiscountResponse.Result;
            Assert.NotNull(updatedCartDiscount);
            Assert.NotNull(updatedCartDiscount.Id);
            Assert.Equal(updatedCartDiscount.CartPredicate, changePredicateAction.CartPredicate);

            // Cleanup
            await _client.CartDiscounts().DeleteCartDiscountAsync(updatedCartDiscount);
        }

        [Fact]
        public async Task ShouldChangeIsActiveChangeRequiresDiscountCodeCartDiscountAsync()
        {
            // Arrange
            var cartDiscount = await Helper.CreateTestCartDiscount(this._project, this._client);
            var changeActiveAction = new ChangeIsActiveAction(!cartDiscount.IsActive);
            var changeRequiresDiscountCodeAction = new ChangeRequiresDiscountCodeAction(!cartDiscount.RequiresDiscountCode);


            // Act
            var updatedCartDiscountResponse = await this._client.CartDiscounts()
                .UpdateCartDiscountAsync(cartDiscount, new List<UpdateAction> { changeActiveAction, changeRequiresDiscountCodeAction});


            // Assert
            var updatedCartDiscount = updatedCartDiscountResponse.Result;
            Assert.NotNull(updatedCartDiscount);
            Assert.NotNull(updatedCartDiscount.Id);
            Assert.Equal(updatedCartDiscount.IsActive, changeActiveAction.IsActive);
            Assert.Equal(updatedCartDiscount.RequiresDiscountCode, changeRequiresDiscountCodeAction.RequiresDiscountCode);

            // Cleanup
            await _client.CartDiscounts().DeleteCartDiscountAsync(updatedCartDiscount);
        }

        [Fact]
        public async Task ShouldChangeNameSetDescriptionCartDiscountAsync()
        {
            // Arrange
            var cartDiscount = await Helper.CreateTestCartDiscount(this._project, this._client);
            LocalizedString name = new LocalizedString();
            LocalizedString description = new LocalizedString();

            foreach (string language in this._project.Languages)
            {
                string randomPostfix = Helper.GetRandomString(10);
                name.SetValue(language, string.Concat("change-cart-discount-name", language, " ", randomPostfix));
                description.SetValue(language, string.Concat("change-cart-discount-description", language, "-", randomPostfix));
            }
            var changeNameAction = new ChangeNameAction(name);
            var setDescriptionAction = new SetDescriptionAction(description);

            // Act
            var updatedCartDiscountResponse = await this._client.CartDiscounts()
                .UpdateCartDiscountAsync(cartDiscount, new List<UpdateAction> {
                    changeNameAction, setDescriptionAction });


            // Assert
            var updatedCartDiscount = updatedCartDiscountResponse.Result;
            Assert.NotNull(updatedCartDiscount);
            Assert.NotNull(updatedCartDiscount.Id);
            Assert.Equal(updatedCartDiscount.Name.Values, name.Values);
            Assert.Equal(updatedCartDiscount.Description.Values, description.Values);

            // Cleanup
            await _client.CartDiscounts().DeleteCartDiscountAsync(updatedCartDiscount);
        }

        [Fact]
        public async Task ShouldSetValidFromSetValidUntilCartDiscountAsync()
        {
            // Arrange 
            var cartDiscount = await Helper.CreateTestCartDiscount(this._project, this._client);
            var setValidFrom = new SetValidFromAction(DateTime.UtcNow.AddDays(3));
            var setValidUntil = new SetValidUntilAction(setValidFrom.ValidFrom.Value.AddDays(10));

            // Act
            var updatedCartDiscountResponse = await this._client.CartDiscounts()
                .UpdateCartDiscountAsync(cartDiscount, new List<UpdateAction> {
                    setValidFrom, setValidUntil });


            // Assert
            var updatedCartDiscount = updatedCartDiscountResponse.Result;
            Assert.NotNull(updatedCartDiscount);
            Assert.NotNull(updatedCartDiscount.Id);
            Assert.True((setValidFrom.ValidFrom - updatedCartDiscount.ValidFrom).Value.TotalSeconds <= 1);
            Assert.True((setValidUntil.ValidUntil - updatedCartDiscount.ValidUntil).Value.TotalSeconds <= 1);
            // Cleanup
            await _client.CartDiscounts().DeleteCartDiscountAsync(updatedCartDiscount);
        }

        [Fact]
        public async Task ShouldChangeTargetCartDiscountAsync()
        {
            // Arrange 
            var cartDiscount = await Helper.CreateTestCartDiscount(this._project, this._client);
            var changetarget = new ChangeTargetAction(new CartDiscountTarget(CartDiscountTargetType.CustomLineItems, "money.centAmount > 1000"));

            // Act
            var updatedCartDiscountResponse = await this._client.CartDiscounts()
                .UpdateCartDiscountAsync(cartDiscount, changetarget);


            // Assert
            var updatedCartDiscount = updatedCartDiscountResponse.Result;
            Assert.NotNull(updatedCartDiscount);
            Assert.NotNull(updatedCartDiscount.Id);
            Assert.Equal(updatedCartDiscount.Target.Type, changetarget.Target.Type);
            Assert.Equal(updatedCartDiscount.Target.Predicate, changetarget.Target.Predicate);
            // Cleanup
            await _client.CartDiscounts().DeleteCartDiscountAsync(updatedCartDiscount);
        }

        [Fact]
        public async Task ShouldChangeValueCartDiscountAsync()
        {
            // Arrange 
            var moneyList = new List<Money>();
            foreach (var currency in this._project.Currencies)
            {
                moneyList.Add(new Money { CentAmount = Helper.GetRandomNumber(100, 1000), CurrencyCode = currency}); 
            }
            var cartDiscount = await Helper.CreateTestCartDiscount(this._project, this._client);
            var changeValue = new ChangeValueAction(new AbsoluteCartDiscountValue(moneyList));

            // Act
            var updatedCartDiscountResponse = await this._client.CartDiscounts()
                .UpdateCartDiscountAsync(cartDiscount, changeValue);


            // Assert
            var updatedCartDiscount = updatedCartDiscountResponse.Result;
            Assert.NotNull(updatedCartDiscount);
            Assert.NotNull(updatedCartDiscount.Id);
            Assert.Equal(updatedCartDiscount.Value.Type, changeValue.Value.Type);
            

            // Cleanup
            await _client.CartDiscounts().DeleteCartDiscountAsync(updatedCartDiscount);
        }

        [Fact]
        public async Task ShouldChangeSortOrderCartDiscountAsync()
        {
            // Arrange            
            var cartDiscount = await Helper.CreateTestCartDiscount(this._project, this._client);
            var cartDiscountDraft = await Helper.GetTestCartDiscountDraft(this._project, this._client);
            var changeSortOrder = new ChangeSortOrderAction(cartDiscountDraft.SortOrder);

            // Act
            var updatedCartDiscountResponse = await this._client.CartDiscounts()
                .UpdateCartDiscountAsync(cartDiscount, changeSortOrder);


            // Assert
            var updatedCartDiscount = updatedCartDiscountResponse.Result;
            Assert.NotNull(updatedCartDiscount);
            Assert.NotNull(updatedCartDiscount.Id);
            Assert.Equal(updatedCartDiscount.SortOrder, changeSortOrder.SortOrder);

            // Cleanup
            await _client.CartDiscounts().DeleteCartDiscountAsync(updatedCartDiscount);
        }

        [Fact]
        public async Task ShouldDeleteCartDiscountAsync()
        {
            // Arrange            
            var cartDiscount = await Helper.CreateTestCartDiscount(this._project, this._client);

            // Act
            var updatedCartDiscountResponse = await this._client.CartDiscounts()
                .DeleteCartDiscountAsync(cartDiscount);


            // Assert
            var updatedCartDiscount = updatedCartDiscountResponse.Result;
            Assert.NotNull(updatedCartDiscount);
            Assert.NotNull(updatedCartDiscount.Id);
            var getCartTask = await this._client.CartDiscounts().GetCartDiscountByIdAsync(updatedCartDiscount.Id);

            Assert.Equal(404, getCartTask.StatusCode);
            Assert.Equal(false, getCartTask.Success);
            // Cleanup
        }
    }
}
