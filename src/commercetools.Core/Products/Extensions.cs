using commercetools.Core.Common;

namespace commercetools.Core.Products
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Creates an instance of the ProductManager.
        /// </summary>
        /// <returns>ProductManager</returns>
        public static ProductManager Products(this Client client)
        {
            return new ProductManager(client);
        }
    }
}
