using commercetools.Common;

namespace commercetools.CustomObjects
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Creates an instance of the CustomObjectManager.
        /// </summary>
        /// <returns>CustomObjectManager</returns>
        public static CustomObjectManager CustomObjects(this IClient client)
        {
            return new CustomObjectManager(client);
        }
    }
}
