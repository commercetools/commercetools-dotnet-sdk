using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using commercetools.Core.Carts;
using commercetools.Core.Common;
using commercetools.Core.Common.UpdateActions;
using commercetools.Core.Customers;
using commercetools.Core.Customers.UpdateActions;
using commercetools.Core.Messages;
using commercetools.Core.Project;
using Xunit;

namespace commercetools.Core.Tests
{
    /// <summary>
    /// Test the API methods in the CustomerManager class.
    /// </summary>
    public class CustomerManagerTest : IDisposable
    {
        private Client _client;
        private Project.Project _project;
        private Cart _testCart;
        private Customer _testCustomer;

        /// <summary>
        /// Test setup
        /// </summary>
        public CustomerManagerTest()
        {
            _client = new Client(Helper.GetConfiguration());

            Task<Response<Project.Project>> projectTask = _client.Project().GetProjectAsync();
            projectTask.Wait();
            Assert.True(projectTask.Result.Success);
            _project = projectTask.Result.Result;

            CustomerDraft customerDraft = Helper.GetTestCustomerDraft();
            Task<Response<CustomerCreatedMessage>> customerTask = _client.Customers().CreateCustomerAsync(customerDraft);
            customerTask.Wait();
            Assert.True(customerTask.Result.Success);

            CustomerCreatedMessage customerCreatedMessage = customerTask.Result.Result;
            Assert.NotNull(customerCreatedMessage.Customer);
            Assert.NotNull(customerCreatedMessage.Customer.Id);

            _testCustomer = customerCreatedMessage.Customer;

            CartDraft cartDraft = Helper.GetTestCartDraft(_project, _testCustomer.Id);
            Task<Response<Cart>> cartTask = _client.Carts().CreateCartAsync(cartDraft);
            cartTask.Wait();
            Assert.True(cartTask.Result.Success);

            _testCart = cartTask.Result.Result;
            Assert.NotNull(_testCart.Id);
            Assert.Equal(_testCart.CustomerId, _testCustomer.Id);
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        public void Dispose()
        {
            Task<Response<Cart>> cartTask = _client.Carts().DeleteCartAsync(_testCart);
            cartTask.Wait();

            Task<Response<Customer>> customerTask = _client.Customers().DeleteCustomerAsync(_testCustomer);
            customerTask.Wait();
        }

        /// <summary>
        /// Tests the CustomerManager.GetCustomerByIdAsync method.
        /// </summary>
        /// <see cref="CustomerManager.GetCustomerByIdAsync"/>
        [Fact]
        public async Task ShouldGetCustomerByIdAsync()
        {
            Response<Customer> response = await _client.Customers().GetCustomerByIdAsync(_testCustomer.Id);
            Assert.True(response.Success);

            Customer customer = response.Result;
            Assert.NotNull(customer.Id);
            Assert.Equal(customer.Id, _testCustomer.Id);
        }

        [Fact]
        public async Task ShouldGetCustomerByKeyAsync()
        {
            Response<Customer> response = await _client.Customers().GetCustomerByKeyAsync(_testCustomer.Key);
            Assert.True(response.Success);

            Customer customer = response.Result;
            Assert.NotNull(customer.Id);
            Assert.Equal(customer.Id, _testCustomer.Id);
        }

        /// <summary>
        /// Tests the CustomerManager.QueryCustomersAsync method.
        /// </summary>
        /// <see cref="CustomerManager.QueryCustomersAsync"/>
        [Fact]
        public async Task ShouldQueryCustomersAsync()
        {
            Response<CustomerQueryResult> response = await _client.Customers().QueryCustomersAsync();
            Assert.True(response.Success);

            CustomerQueryResult customerQueryResult = response.Result;
            Assert.NotNull(customerQueryResult.Results);
            Assert.True(customerQueryResult.Results.Count >= 1);
        }

        /// <summary>
        /// Tests the CustomerManager.CreateCartAsync and CustomerManager.DeleteCartAsync methods.
        /// </summary>
        /// <see cref="CustomerManager.CreateCustomerAsync"/>
        /// <seealso cref="CustomerManager.DeleteCustomerAsync(Customer)"/>
        [Fact]
        public async Task ShouldCreateAndDeleteCustomerAsync()
        {
            CustomerDraft customerDraft = Helper.GetTestCustomerDraft();
            Response<CustomerCreatedMessage> customerCreatedResponse = await _client.Customers().CreateCustomerAsync(customerDraft);
            Assert.True(customerCreatedResponse.Success);

            CustomerCreatedMessage customerCreatedMessage = customerCreatedResponse.Result;
            Assert.NotNull(customerCreatedMessage.Customer);
            Assert.NotNull(customerCreatedMessage.Customer.Id);

            Customer customer = customerCreatedMessage.Customer;

            string deletedCustomerId = customer.Id;

            Response<Customer> customerResponse = await _client.Customers().DeleteCustomerAsync(customer);
            Assert.True(customerResponse.Success);

            customerResponse = await _client.Customers().GetCustomerByIdAsync(deletedCustomerId);
            Assert.False(customerResponse.Success);
        }

        /// <summary>
        /// Tests the CustomerManager.UpdateCustomerAsync method.
        /// </summary>
        /// <see cref="CustomerManager.UpdateCustomerAsync(Customer, System.Collections.Generic.List{UpdateAction})"/>
        [Fact]
        public async Task ShouldUpdateCustomerAsync()
        {
            string newEmail = string.Concat(Helper.GetRandomString(10), "@example.com");
            string newExternalId = Helper.GetRandomNumber(10000, 99999).ToString();

            SetExternalIdAction setExternalIdAction = new SetExternalIdAction();
            setExternalIdAction.ExternalId = newExternalId;

            GenericAction changeEmailAction = new GenericAction("changeEmail");
            changeEmailAction.SetProperty("email", newEmail);

            List<UpdateAction> actions = new List<UpdateAction>();
            actions.Add(setExternalIdAction);
            actions.Add(changeEmailAction);
            
            Response<Customer> response = await _client.Customers().UpdateCustomerAsync(_testCustomer, actions);
            Assert.True(response.Success);

            _testCustomer = response.Result;
            Assert.NotNull(_testCustomer.Id);
            Assert.Equal(_testCustomer.Email, newEmail);
            Assert.Equal(_testCustomer.ExternalId, newExternalId);
        }

        /// <summary>
        /// Tests the CustomerManager.AuthenticateCustomerAsync method.
        /// </summary>
        /// <see cref="CustomerManager.AuthenticateCustomerAsync"/>
        [Fact]
        public async Task ShouldAuthenticateCustomer()
        {
            string email = string.Concat("authtest", Helper.GetRandomString(20), "@example.com");
            string password = "AuthTest123!";

            CustomerDraft customerDraft = Helper.GetTestCustomerDraft();
            customerDraft.Email = email;
            customerDraft.Password = password;

            Response<CustomerCreatedMessage> customerCreatedResponse = await _client.Customers().CreateCustomerAsync(customerDraft);
            Assert.True(customerCreatedResponse.Success);

            CustomerCreatedMessage customerCreatedMessage = customerCreatedResponse.Result;
            Assert.NotNull(customerCreatedMessage.Customer);
            Assert.Equal(customerCreatedMessage.Customer.Email, email);

            Customer customer = customerCreatedMessage.Customer;

            Response<CustomerSignInResult> customerSignInResponse = await _client.Customers().AuthenticateCustomerAsync(email, password);
            Assert.True(customerSignInResponse.Success);

            CustomerSignInResult customerSignInResult = customerSignInResponse.Result;
            Assert.NotNull(customerSignInResult.Customer);
            Assert.Equal(customerSignInResult.Customer.Email, email);

            await _client.Customers().DeleteCustomerAsync(customer);
        }
    }
}