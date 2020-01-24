using commercetools.Common;

namespace commercetools.CustomerGroups
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Creates an instance of the CustomerManager.
        /// </summary>
        /// <returns>CustomerGroupsManager</returns>
        public static CustomerGroupManager CustomerGroups(this IClient client)
        {
            return new CustomerGroupManager(client);
        }
    }
}
