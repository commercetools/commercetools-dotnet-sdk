using commercetools.Core.Common;

namespace commercetools.Core.Types
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Creates an instance of the TypeManager.
        /// </summary>
        /// <returns>TypeManager</returns>
        public static TypeManager Types(this Client client)
        {
            return new TypeManager(client);
        }
    }
}
