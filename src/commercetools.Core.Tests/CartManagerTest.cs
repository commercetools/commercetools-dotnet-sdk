using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using commercetools.Core.Carts;
using commercetools.Core.Carts.UpdateActions;
using commercetools.Core.Common;
using commercetools.Core.Common.UpdateActions;
using commercetools.Core.Customers;
using commercetools.Core.Messages;
using commercetools.Core.Payments;
using commercetools.Core.Products;
using commercetools.Core.ProductTypes;
using commercetools.Core.Project;
using commercetools.Core.ShippingMethods;
using commercetools.Core.TaxCategories;
using commercetools.Core.Types;
using commercetools.Core.Zones;
using commercetools.Core.Zones.UpdateActions;
using Newtonsoft.Json.Linq;
using Xunit;
using Type = commercetools.Core.Types.Type;

namespace commercetools.Core.Tests
{
    /// <summary>
    /// Test the API methods in the CartManager class, along with some of the cart update actions.
    /// </summary>
    public class CartManagerTest : IDisposable
    {
        private Client _client;
        private Project.Project _project;
        private List<Cart> _testCarts;
        private List<Customer> _testCustomers;
        private Payment _testPayment;
        private Product _testProduct;
        private ProductType _testProductType;
        private ShippingMethod _testShippingMethod;
        private TaxCategory _testTaxCategory;
        private Zone _testZone;
        private Type _testType;
        private bool _createdTestZone;

        /// <summary>
        /// Test setup
        /// </summary>
        public  CartManagerTest()
        {
            _client = new Client(Helper.GetConfiguration());

            Task<Response<Project.Project>> projectTask = _client.Project().GetProjectAsync();
            projectTask.Wait();
            Assert.True(projectTask.Result.Success);
            _project = projectTask.Result.Result;

            Assert.True(_project.Languages.Count > 0);
            Assert.True(_project.Currencies.Count > 0);

            _testCustomers = new List<Customer>();
            _testCarts = new List<Cart>();

            for (int i = 0; i < 5; i++)
            {
                CustomerDraft customerDraft = Helper.GetTestCustomerDraft();
                Task<Response<CustomerCreatedMessage>> customerTask = _client.Customers().CreateCustomerAsync(customerDraft);
                customerTask.Wait();
                Assert.True(customerTask.Result.Success);

                CustomerCreatedMessage customerCreatedMessage = customerTask.Result.Result;
                Assert.NotNull(customerCreatedMessage.Customer);
                Assert.NotNull(customerCreatedMessage.Customer.Id);

                _testCustomers.Add(customerCreatedMessage.Customer);

                CartDraft cartDraft = Helper.GetTestCartDraft(_project, customerCreatedMessage.Customer.Id);
                Task<Response<Cart>> cartTask = _client.Carts().CreateCartAsync(cartDraft);
                cartTask.Wait();
                Assert.True(cartTask.Result.Success);
                Cart cart = cartTask.Result.Result;
                Assert.NotNull(cart.Id);

                _testCarts.Add(cart);
            }

            ProductTypeDraft productTypeDraft = Helper.GetTestProductTypeDraft();
            Task<Response<ProductType>> testProductTypeTask = _client.ProductTypes().CreateProductTypeAsync(productTypeDraft);
            testProductTypeTask.Wait();
            Assert.True(testProductTypeTask.Result.Success);
            _testProductType = testProductTypeTask.Result.Result;
            Assert.NotNull(_testProductType.Id);

            TaxCategoryDraft taxCategoryDraft = Helper.GetTestTaxCategoryDraft(_project);
            Task<Response<TaxCategory>> taxCategoryTask = _client.TaxCategories().CreateTaxCategoryAsync(taxCategoryDraft);
            taxCategoryTask.Wait();
            Assert.True(taxCategoryTask.Result.Success);
            _testTaxCategory = taxCategoryTask.Result.Result;
            Assert.NotNull(_testTaxCategory.Id);

            Task<Response<ZoneQueryResult>> zoneQueryResultTask =_client.Zones().QueryZonesAsync();
            zoneQueryResultTask.Wait();
            Assert.True(zoneQueryResultTask.Result.Success);

            if (zoneQueryResultTask.Result.Result.Results.Count > 0)
            {
                _testZone = zoneQueryResultTask.Result.Result.Results[0];
                _createdTestZone = false;
            }
            else
            {
                ZoneDraft zoneDraft = Helper.GetTestZoneDraft();
                Task<Response<Zone>> zoneTask = _client.Zones().CreateZoneAsync(zoneDraft);
                zoneTask.Wait();
                Assert.True(zoneTask.Result.Success);
                _testZone = zoneTask.Result.Result;
                _createdTestZone = true;
            }

            Assert.NotNull(_testZone.Id);

            foreach (string country in _project.Countries)
            {
                Location location =
                    _testZone.Locations
                        .Where(l => l.Country.Equals(country, StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault();

                if (location == null)
                {
                    location = new Location();
                    location.Country = country;

                    AddLocationAction addLocationAction = new AddLocationAction(location);
                    Task<Response<Zone>> updateZoneTask = _client.Zones().UpdateZoneAsync(_testZone, addLocationAction);
                    updateZoneTask.Wait();
                    Assert.True(updateZoneTask.Result.Success);
                    _testZone = updateZoneTask.Result.Result;
                }
            }

            Assert.NotNull(_testZone.Locations.Count > 0);

            ShippingMethodDraft shippingMethodDraft = Helper.GetTestShippingMethodDraft(_project, _testTaxCategory, _testZone);
            Task<Response<ShippingMethod>> shippingMethodTask = _client.ShippingMethods().CreateShippingMethodAsync(shippingMethodDraft);
            shippingMethodTask.Wait();
            Assert.True(shippingMethodTask.Result.Success);
            _testShippingMethod = shippingMethodTask.Result.Result;

            Assert.NotNull(_testShippingMethod.Id);

            ProductDraft productDraft = Helper.GetTestProductDraft(_project, _testProductType.Id, _testTaxCategory.Id);
            Task<Response<Product>> testProductTask = _client.Products().CreateProductAsync(productDraft);
            testProductTask.Wait();
            Assert.True(testProductTask.Result.Success);
            _testProduct = testProductTask.Result.Result;

            Assert.NotNull(_testProduct.Id);

            PaymentDraft paymentDraft = Helper.GetTestPaymentDraft(_project, _testCustomers[0].Id);
            Task<Response<Payment>> paymentTask = _client.Payments().CreatePaymentAsync(paymentDraft);
            paymentTask.Wait();
            Assert.True(paymentTask.Result.Success);
            _testPayment = paymentTask.Result.Result;

            Assert.NotNull(_testPayment.Id);

            TypeDraft typeDraft = Helper.GetTypeDraft(_project);
            Task<Response<Type>> typeTask = _client.Types().CreateTypeAsync(typeDraft);
            typeTask.Wait();
            Assert.True(typeTask.Result.Success);
            _testType = typeTask.Result.Result;
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        public void Dispose()
        {
            Task task;

            foreach (Cart cart in _testCarts)
            {
                task = _client.Carts().DeleteCartAsync(cart);
                task.Wait();
            }

            task = _client.Payments().DeletePaymentAsync(_testPayment);
            task.Wait();

            foreach (Customer customer in _testCustomers)
            {
                task = _client.Customers().DeleteCustomerAsync(customer);
                task.Wait();
            }

            task = _client.Products().DeleteProductAsync(_testProduct);
            task.Wait();

            task = _client.ProductTypes().DeleteProductTypeAsync(_testProductType);
            task.Wait();

            task = _client.ShippingMethods().DeleteShippingMethodAsync(_testShippingMethod);
            task.Wait();

            task = _client.TaxCategories().DeleteTaxCategoryAsync(_testTaxCategory);
            task.Wait();

            if (_createdTestZone)
            {
                task = _client.Zones().DeleteZoneAsync(_testZone);
                task.Wait();
            }

            task = _client.Types().DeleteTypeAsync(_testType);
            task.Wait();
        }

        /// <summary>
        /// Tests the CartManager.GetCartByIdAsync method.
        /// </summary>
        /// <see cref="CartManager.GetCartByIdAsync"/>
        [Fact]
        public async Task ShouldGetCartByIdAsync()
        {
            Response<Cart> response = await _client.Carts().GetCartByIdAsync(_testCarts[0].Id);
            Assert.True(response.Success);

            Cart cart = response.Result;
            Assert.NotNull(cart.Id);
            Assert.Equal(cart.Id, _testCarts[0].Id);
        }

        /// <summary>
        /// Tests the CartManager.GetCartByCustomerIdAsync method.
        /// </summary>
        /// <see cref="CartManager.GetCartByCustomerIdAsync"/>
        [Fact]
        public async Task ShouldGetCartByCustomerIdAsync()
        {
            Response<Cart> response = await _client.Carts().GetCartByCustomerIdAsync(_testCustomers[0].Id);
            Assert.True(response.Success);

            Cart cart = response.Result;
            Assert.NotNull(cart.Id);
            Assert.Equal(cart.CustomerId, _testCustomers[0].Id);
        }

        /// <summary>
        /// Tests the CartManager.QueryCartsAsync method.
        /// </summary>
        /// <see cref="CartManager.QueryCartsAsync"/>
        [Fact]
        public async Task ShouldQueryCartsAsync()
        {
            Response<CartQueryResult> response = await _client.Carts().QueryCartsAsync();
            Assert.True(response.Success);

            CartQueryResult cartQueryResult = response.Result;
            Assert.NotNull(cartQueryResult.Results);

            int limit = 2;
            response = await _client.Carts().QueryCartsAsync(limit: limit);
            Assert.True(response.Success);

            cartQueryResult = response.Result;
            Assert.NotNull(cartQueryResult.Results);
            Assert.True(cartQueryResult.Results.Count <= limit);
        }

        /// <summary>
        /// Tests the CartManager.CreateCartAsync and CartManager.DeleteCartAsync methods.
        /// </summary>
        /// <see cref="CartManager.CreateCartAsync"/>
        /// <seealso cref="CartManager.DeleteCartAsync(Cart)"/>
        [Fact]
        public async Task ShouldCreateAndDeleteCartAsync()
        {
            CartDraft cartDraft = Helper.GetTestCartDraft(_project);

            Response<Cart> response = await _client.Carts().CreateCartAsync(cartDraft);
            Assert.True(response.Success);

            Cart cart = response.Result;
            Assert.NotNull(cart.Id);
            Assert.Equal(cart.Country, cartDraft.Country);
            Assert.Equal(cart.InventoryMode, cartDraft.InventoryMode);
            Assert.Equal(cart.ShippingAddress, cartDraft.ShippingAddress);
            Assert.Equal(cart.BillingAddress, cartDraft.BillingAddress);
            Assert.Equal(cartDraft.DeleteDaysAfterLastModification, cart.DeleteDaysAfterLastModification);

            string deletedCartId = cart.Id;

            response = await _client.Carts().DeleteCartAsync(cart);
            Assert.True(response.Success);

            cart = response.Result;

            response = await _client.Carts().GetCartByIdAsync(deletedCartId);
            Assert.False(response.Success);
        }

        /// <summary>
        /// Tests the CartManager.CreateCartAsync and CartManager.DeleteCartAsync methods for a cart with custom line items.
        /// </summary>
        /// <see cref="CartManager.CreateCartAsync"/>
        /// <seealso cref="CartManager.DeleteCartAsync(Cart)"/>
        [Fact]
        public async Task ShouldCreateAndDeleteCartWithCustomLineItemsAsync()
        {
            CartDraft cartDraft = Helper.GetTestCartDraftWithCustomLineItems(_project);

            Response<Cart> response = await _client.Carts().CreateCartAsync(cartDraft);
            Assert.True(response.Success);

            Cart cart = response.Result;
            Assert.NotNull(cart.Id);
            Assert.Equal(cart.Country, cartDraft.Country);
            Assert.Equal(cart.InventoryMode, cartDraft.InventoryMode);
            Assert.Equal(cart.ShippingAddress, cartDraft.ShippingAddress);
            Assert.Equal(cart.BillingAddress, cartDraft.BillingAddress);

            string deletedCartId = cart.Id;

            response = await _client.Carts().DeleteCartAsync(cart);
            Assert.True(response.Success);

            cart = response.Result;

            response = await _client.Carts().GetCartByIdAsync(deletedCartId);
            Assert.False(response.Success);
        }

        /// <summary>
        /// Tests the AddLineItemAction, ChangeLineItemQuantityAction and RemoveLineItemAction update actions.
        /// </summary>
        /// <see cref="CartManager.UpdateCartAsync(Cart, UpdateAction)"/>
        [Fact]
        public async Task ShouldAddChangeAndRemoveLineItemAsync()
        {
            int quantity = 2;
            int newQuantity = 3;

            AddLineItemAction addLineItemAction =
                new AddLineItemAction(_testProduct.Id, _testProduct.MasterData.Current.MasterVariant.Id);
            addLineItemAction.Quantity = quantity;
            Response<Cart> response = await _client.Carts().UpdateCartAsync(_testCarts[0], addLineItemAction);
            Assert.True(response.Success);

            _testCarts[0] = response.Result;
            Assert.NotNull(_testCarts[0].Id);
            Assert.NotNull(_testCarts[0].LineItems);
            Assert.Equal(_testCarts[0].LineItems.Count, 1);
            Assert.Equal(_testCarts[0].LineItems[0].ProductId, _testProduct.Id);
            Assert.Equal(_testCarts[0].LineItems[0].Variant.Id, _testProduct.MasterData.Current.MasterVariant.Id);
            Assert.Equal(_testCarts[0].LineItems[0].Quantity, quantity);

            ChangeLineItemQuantityAction changeLineItemQuantityAction =
                new ChangeLineItemQuantityAction(_testCarts[0].LineItems[0].Id, newQuantity);
            response = await _client.Carts().UpdateCartAsync(_testCarts[0], changeLineItemQuantityAction);
            Assert.True(response.Success);

            _testCarts[0] = response.Result;
            Assert.NotNull(_testCarts[0].Id);
            Assert.NotNull(_testCarts[0].LineItems);
            Assert.Equal(_testCarts[0].LineItems.Count, 1);
            Assert.Equal(_testCarts[0].LineItems[0].Quantity, newQuantity);

            RemoveLineItemAction removeLineItemAction = new RemoveLineItemAction(_testCarts[0].LineItems[0].Id);
            response = await _client.Carts().UpdateCartAsync(_testCarts[0], removeLineItemAction);
            Assert.True(response.Success);

            _testCarts[0] = response.Result;
            Assert.NotNull(_testCarts[0].Id);
            Assert.NotNull(_testCarts[0].LineItems);
            Assert.Equal(_testCarts[0].LineItems.Count, 0);
        }

        /// <summary>
        /// Tests the SetShippingAddressAction update action.
        /// </summary>
        /// <see cref="CartManager.UpdateCartAsync(Cart, UpdateAction)"/>
        [Fact]
        public async Task ShouldSetShippingAddressAsync()
        {
            Address newShippingAddress = Helper.GetTestAddress(_project);

            newShippingAddress.FirstName = "New";
            newShippingAddress.LastName = "Shipping";
            newShippingAddress.StreetName = "First Ave.";
            newShippingAddress.StreetNumber = "321";
            newShippingAddress.Country = "US";
            newShippingAddress.PostalCode = Helper.GetRandomNumber(10000, 90000).ToString();

            SetShippingAddressAction setShippingAddressAction = new SetShippingAddressAction
            {
                Address = newShippingAddress
            };
            Response<Cart> response = await _client.Carts().UpdateCartAsync(_testCarts[1], setShippingAddressAction);
            Assert.True(response.Success);

            _testCarts[1] = response.Result;
            Assert.NotNull(_testCarts[1].Id);
            Assert.NotNull(_testCarts[1].ShippingAddress);
            Assert.Equal(_testCarts[1].ShippingAddress.FirstName, newShippingAddress.FirstName);
            Assert.Equal(_testCarts[1].ShippingAddress.LastName, newShippingAddress.LastName);
            Assert.Equal(_testCarts[1].ShippingAddress.StreetName, newShippingAddress.StreetName);
            Assert.Equal(_testCarts[1].ShippingAddress.StreetNumber, newShippingAddress.StreetNumber);
            Assert.Equal(_testCarts[1].ShippingAddress.Country, newShippingAddress.Country);
            Assert.Equal(_testCarts[1].ShippingAddress.PostalCode, newShippingAddress.PostalCode);
        }

        /// <summary>
        /// Tests the SetBillingAddressAction update action.
        /// </summary>
        /// <see cref="CartManager.UpdateCartAsync(Cart, UpdateAction)"/>
        [Fact]
        public async Task ShouldSetBillingAddressAsync()
        {
            Address newBillingAddress = Helper.GetTestAddress(_project);

            newBillingAddress.FirstName = "New";
            newBillingAddress.LastName = "Billing";
            newBillingAddress.StreetName = "Second Ave.";
            newBillingAddress.StreetNumber = "125";
            newBillingAddress.Country = "US";
            newBillingAddress.PostalCode = Helper.GetRandomNumber(10000, 90000).ToString();

            SetBillingAddressAction setBillingAddressAction = new SetBillingAddressAction { Address = newBillingAddress };
            Response<Cart> response = await _client.Carts().UpdateCartAsync(_testCarts[2], setBillingAddressAction);
            Assert.True(response.Success);

            _testCarts[2] = response.Result;
            Assert.NotNull(_testCarts[2].Id);
            Assert.NotNull(_testCarts[2].BillingAddress);
            Assert.Equal(_testCarts[2].BillingAddress.FirstName, newBillingAddress.FirstName);
            Assert.Equal(_testCarts[2].BillingAddress.LastName, newBillingAddress.LastName);
            Assert.Equal(_testCarts[2].BillingAddress.StreetName, newBillingAddress.StreetName);
            Assert.Equal(_testCarts[2].BillingAddress.StreetNumber, newBillingAddress.StreetNumber);
            Assert.Equal(_testCarts[2].BillingAddress.Country, newBillingAddress.Country);
            Assert.Equal(_testCarts[2].BillingAddress.PostalCode, newBillingAddress.PostalCode);
        }

        /// <summary>
        /// Tests the SetShippingMethodAction update action.
        /// </summary>
        /// <see cref="CartManager.UpdateCartAsync(Cart, UpdateAction)"/>
        [Fact]
        public async Task ShouldSetShippingMethodAsync()
        {
            Reference shippingMethod = new Reference();
            shippingMethod.Id = _testShippingMethod.Id;
            shippingMethod.ReferenceType = Common.ReferenceType.ShippingMethod;

            SetShippingMethodAction setShippingMethodAction = new SetShippingMethodAction { ShippingMethod = shippingMethod };
            Response<Cart> response = await _client.Carts().UpdateCartAsync(_testCarts[3], setShippingMethodAction);
            Assert.True(response.Success);

            _testCarts[3] = response.Result;
            Assert.NotNull(_testCarts[3].Id);
            Assert.NotNull(_testCarts[3].ShippingInfo);
            Assert.NotNull(_testCarts[3].ShippingInfo.ShippingMethod);
            Assert.Equal(_testCarts[3].ShippingInfo.ShippingMethod.Id, _testShippingMethod.Id);
        }

        /// <summary>
        /// Tests the SetCustomerIdAction update action.
        /// </summary>
        /// <see cref="CartManager.UpdateCartAsync(Cart, UpdateAction)"/>
        [Fact]
        public async Task ShouldSetCustomerIdAsync()
        {
            CustomerDraft customerDraft = Helper.GetTestCustomerDraft();
            Response<CustomerCreatedMessage> customerResponse = await _client.Customers().CreateCustomerAsync(customerDraft);
            Assert.True(customerResponse.Success);

            CustomerCreatedMessage customerCreatedMessage = customerResponse.Result;
            Assert.NotNull(customerCreatedMessage.Customer);

            Customer customer = customerCreatedMessage.Customer;

            SetCustomerIdAction setCustomerIdAction = new SetCustomerIdAction { CustomerId = customer.Id };
            Response<Cart> cartResponse = await _client.Carts().UpdateCartAsync(_testCarts[0], setCustomerIdAction);
            Assert.True(cartResponse.Success);

            _testCarts[0] = cartResponse.Result;
            Assert.NotNull(_testCarts[0].Id);
            Assert.Equal(_testCarts[0].CustomerId, customer.Id);

            setCustomerIdAction = new SetCustomerIdAction();
            cartResponse = await _client.Carts().UpdateCartAsync(_testCarts[0], setCustomerIdAction);
            Assert.True(cartResponse.Success);

            _testCarts[0] = cartResponse.Result;
            Assert.NotNull(_testCarts[0].Id);
            Assert.NotEqual(_testCarts[0].CustomerId, customer.Id);

            await _client.Customers().DeleteCustomerAsync(customer);
        }

        /// <summary>
        /// Tests the RecalculateAction update action.
        /// </summary>
        /// <see cref="CartManager.UpdateCartAsync(Cart, UpdateAction)"/>
        [Fact]
        public async Task ShouldRecalculateAsync()
        {
            RecalculateAction recalculateAction = new RecalculateAction();
            Response<Cart> response = await _client.Carts().UpdateCartAsync(_testCarts[0], recalculateAction);
            Assert.True(response.Success);

            _testCarts[0] = response.Result;
            Assert.NotNull(_testCarts[0].Id);
        }

        /// <summary>
        /// Tests the SetCustomTypeAction update action.
        /// </summary>
        /// <see cref="CartManager.UpdateCartAsync(Cart, UpdateAction)"/>
        [Fact]
        public async Task ShouldSetCustomTypeAsync()
        {
            ResourceIdentifier typeResourceIdentifier = new ResourceIdentifier 
            { 
                Id = _testType.Id, 
                TypeId = Common.ReferenceType.Type 
            };

            string fieldName = _testType.FieldDefinitions[0].Name;

            JObject fields = new JObject();
            fields.Add(fieldName, "Here is the value of my field.");

            SetCustomTypeAction setCustomTypeAction = new SetCustomTypeAction 
            { 
                Type = typeResourceIdentifier, 
                Fields = fields 
            };

            Response<Cart> cartResponse = await _client.Carts().UpdateCartAsync(_testCarts[1], setCustomTypeAction);
            Assert.True(cartResponse.Success);
            _testCarts[1] = cartResponse.Result;

            Assert.NotNull(_testCarts[1].Custom.Fields);
            Assert.Equal(fields[fieldName], _testCarts[1].Custom.Fields[fieldName]);
        }

        /// <summary>
        /// Tests the AddPaymentAction and RemovePaymentAction update actions.
        /// </summary>
        /// <returns>Task</returns>
        /// <see cref="CartManager.UpdateCartAsync(Cart, UpdateAction)"/>
        [Fact]
        public async Task ShouldAddAndRemovePaymentAsync()
        {
            Reference paymentReference = new Reference();
            paymentReference.Id = _testPayment.Id;
            paymentReference.ReferenceType = Common.ReferenceType.Payment;

            AddPaymentAction addPaymentAction = new AddPaymentAction(paymentReference);
            Response<Cart> response = await _client.Carts().UpdateCartAsync(_testCarts[4], addPaymentAction);
            Assert.True(response.Success);

            _testCarts[4] = response.Result;
            Assert.NotNull(_testCarts[4].Id);
            Assert.Equal(_testCarts[4].PaymentInfo.Payments.Count, 1);
            Assert.Equal(_testCarts[4].PaymentInfo.Payments[0].Id, _testPayment.Id);

            RemovePaymentAction removePaymentAction = new RemovePaymentAction(paymentReference);
            response = await _client.Carts().UpdateCartAsync(_testCarts[4], removePaymentAction);
            Assert.True(response.Success);

            _testCarts[4] = response.Result;
            Assert.NotNull(_testCarts[4].Id);
            Assert.Null(_testCarts[4].PaymentInfo.Payments);
        }

        /// <summary>
        /// Tests the CartManager.UpdateCartAsync method..
        /// </summary>
        /// <see cref="CartManager.UpdateCartAsync(Cart, System.Collections.Generic.List{UpdateAction})"/>
        [Fact]
        public async Task ShouldUpdateCartAsync()
        {
            CustomerDraft customerDraft = Helper.GetTestCustomerDraft();
            Response<CustomerCreatedMessage> messageResponse = await _client.Customers().CreateCustomerAsync(customerDraft);
            Assert.True(messageResponse.Success);

            CustomerCreatedMessage customerCreatedMessage = messageResponse.Result;
            Assert.NotNull(customerCreatedMessage.Customer);

            Customer customer = customerCreatedMessage.Customer;
            Assert.NotNull(customer.Id);

            SetCustomerIdAction setCustomerIdAction = new SetCustomerIdAction();
            setCustomerIdAction.CustomerId = customer.Id;

            GenericAction recalculateAction = new GenericAction("recalculate");

            List<UpdateAction> actions = new List<UpdateAction>();
            actions.Add(setCustomerIdAction);
            actions.Add(recalculateAction);

            Response<Cart> cartResponse = await _client.Carts().UpdateCartAsync(_testCarts[0], actions);
            Assert.True(cartResponse.Success);

            _testCarts[0] = cartResponse.Result;
            Assert.NotNull(_testCarts[0].Id);
            Assert.Equal(_testCarts[0].CustomerId, customer.Id);

            await _client.Customers().DeleteCustomerAsync(customer.Id, customer.Version);
        }
    }
}
