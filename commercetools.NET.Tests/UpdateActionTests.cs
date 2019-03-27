using commercetools.Products.UpdateActions;
using NUnit.Framework;

namespace commercetools.Tests
{
    /// <summary>
    /// Tests the Update Actions
    /// </summary>
    [TestFixture]
    public class UpdateActionTests
    {
        /// <summary>
        /// Tests whether the default value of the property Staged by SetAttributeAction is set to true.
        /// <see cref="SetAttributeAction.Staged"/>
        /// </summary>
        [Test]
        public void SetAttributeActionStagedDefaultsToTrue()
        {
            var actionName = "Testaction";
            var variantId = 1;

            var setAttributeAction = new SetAttributeAction(actionName, variantId);
            Assert.True(setAttributeAction.Staged);
        }
    }
}
