using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using commercetools.Core.Common;
using commercetools.Core.Common.UpdateActions;
using commercetools.Core.Customers;
using commercetools.Core.Messages;
using commercetools.Core.Payments;
using commercetools.Core.Payments.UpdateActions;
using commercetools.Core.Project;
using Newtonsoft.Json.Linq;
using Xunit;

namespace commercetools.Core.Tests
{
    /// <summary>
    /// Test the API methods in the PaymentManager class.
    /// </summary>
    public class PaymentManagerTest : IDisposable
    {
        private Client _client;
        private Project.Project _project;
        private List<Customer> _testCustomers;
        private List<Payment> _testPayments;

        /// <summary>
        /// Test setup
        /// </summary>
        public PaymentManagerTest()
        {
            _client = new Client(Helper.GetConfiguration());

            Task<Response<Project.Project>> projectTask = _client.Project().GetProjectAsync();
            projectTask.Wait();
            Assert.True(projectTask.Result.Success);
            _project = projectTask.Result.Result;

            _testCustomers = new List<Customer>();
            _testPayments = new List<Payment>();

            for (int i = 0; i < 5; i++)
            {
                CustomerDraft customerDraft = Helper.GetTestCustomerDraft();
                Task<Response<CustomerCreatedMessage>> customerCreatedTask = _client.Customers().CreateCustomerAsync(customerDraft);
                customerCreatedTask.Wait();
                Assert.True(customerCreatedTask.Result.Success);

                CustomerCreatedMessage customerCreatedMessage = customerCreatedTask.Result.Result;
                Assert.NotNull(customerCreatedMessage.Customer);
                Assert.NotNull(customerCreatedMessage.Customer.Id);

                _testCustomers.Add(customerCreatedMessage.Customer);

                PaymentDraft paymentDraft = Helper.GetTestPaymentDraft(_project, customerCreatedMessage.Customer.Id);
                Task<Response<Payment>> paymentTask = _client.Payments().CreatePaymentAsync(paymentDraft);
                paymentTask.Wait();
                Assert.True(paymentTask.Result.Success);

                Payment payment = paymentTask.Result.Result;
                Assert.NotNull(payment.Id);

                _testPayments.Add(payment);
            }
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        public void Dispose()
        {
            foreach (Customer customer in _testCustomers)
            {
                Task task = _client.Customers().DeleteCustomerAsync(customer);
                task.Wait();
            }

            foreach (Payment payment in _testPayments)
            {
                Task task = _client.Payments().DeletePaymentAsync(payment.Id, payment.Version);
                task.Wait();
            }
        }

        /// <summary>
        /// Tests the PaymentManager.GetPaymentByIdAsync method.
        /// </summary>
        /// <see cref="PaymentManager.GetPaymentByIdAsync"/>
        [Fact]
        public async Task ShouldGetPaymentByIdAsync()
        {
            Response<Payment> response = await _client.Payments().GetPaymentByIdAsync(_testPayments[0].Id);
            Assert.True(response.Success);

            Payment payment = response.Result;
            Assert.NotNull(payment.Id);
            Assert.Equal(payment.Id, _testPayments[0].Id);
        }

        /// <summary>
        /// Tests the PaymentManager.QueryPaymentsAsync method.
        /// </summary>
        /// <see cref="PaymentManager.QueryPaymentsAsync"/>
        [Fact]
        public async Task ShouldQueryPaymentsAsync()
        {
            Response<PaymentQueryResult> response = await _client.Payments().QueryPaymentsAsync();
            Assert.True(response.Success);

            PaymentQueryResult paymentQueryResult = response.Result;
            Assert.NotNull(paymentQueryResult.Results);

            int limit = 2;
            response = await _client.Payments().QueryPaymentsAsync(limit: limit);
            Assert.True(response.Success);

            paymentQueryResult = response.Result;
            Assert.NotNull(paymentQueryResult.Results);
            Assert.True(paymentQueryResult.Results.Count <= limit);
        }

        /// <summary>
        /// Tests the PaymentManager.CreateCartAsync and PaymentManager.DeleteCartAsync methods.
        /// </summary>
        /// <see cref="PaymentManager.CreatePaymentAsync"/>
        /// <seealso cref="PaymentManager.DeletePaymentAsync(Payment)"/>
        [Fact]
        public async Task ShouldCreateAndDeletePaymentAsync()
        {
            PaymentDraft paymentDraft = Helper.GetTestPaymentDraft(_project, _testCustomers[1].Id);
            Response<Payment> response = await _client.Payments().CreatePaymentAsync(paymentDraft);
            Assert.True(response.Success);

            Payment payment = response.Result;
            Assert.NotNull(payment.Id);

            string deletedPaymentId = payment.Id;

            Response<JObject> deleteResponse = await _client.Payments().DeletePaymentAsync(payment);
            Assert.True(deleteResponse.Success);

            response = await _client.Payments().GetPaymentByIdAsync(deletedPaymentId);
            Assert.False(response.Success);
        }

        /// <summary>
        /// Tests the PaymentManager.UpdatePaymentAsync method.
        /// </summary>
        /// <see cref="PaymentManager.UpdatePaymentAsync(Payment, System.Collections.Generic.List{UpdateAction})"/>
        [Fact]
        public async Task ShouldUpdatePaymentAsync()
        {
            Money newAmountPlanned = Helper.GetTestMoney(_project);
            string newExternalId = Helper.GetRandomNumber(10000, 99999).ToString();

            ChangeAmountPlannedAction changeAmountPlannedAction = new ChangeAmountPlannedAction(newAmountPlanned);

            GenericAction setExternalIdAction = new GenericAction("setExternalId");
            setExternalIdAction.SetProperty("externalId", newExternalId);

            List<UpdateAction> actions = new List<UpdateAction>();
            actions.Add(changeAmountPlannedAction);
            actions.Add(setExternalIdAction);

            Response<Payment> response = await _client.Payments().UpdatePaymentAsync(_testPayments[2], actions);
            Assert.True(response.Success);

            _testPayments[2] = response.Result;
            Assert.NotNull(_testPayments[2].Id);
            Assert.Equal(_testPayments[2].ExternalId, newExternalId);
            Assert.Equal(_testPayments[2].AmountPlanned.CentAmount, newAmountPlanned.CentAmount);
        }
    }
}
