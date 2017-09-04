using commercetools.Core.Common;

namespace commercetools.Core.Orders
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Creates an instance of the OrderManager.
        /// </summary>
        /// <returns>OrderManager</returns>
        public static OrderManager Orders(this Client client)
        {
            return new OrderManager(client);
        }
    }
}
