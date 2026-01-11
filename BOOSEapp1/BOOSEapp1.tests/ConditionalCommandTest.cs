using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEapp1;

namespace BOOSEapp1.tests
{
    /// <summary>
    /// Tests that check if the main conditional command classes exist.
    /// </summary>
    [TestClass]
    public class ConditionalCommandsTests
    {
        /// <summary>
        /// Checks that If, While, For and Method classes are available.
        /// </summary>
        [TestMethod]
        public void ConditionalCommands_TypesExist()
        {
            Assert.IsNotNull(typeof(AppIf));
            Assert.IsNotNull(typeof(AppWhile));
            Assert.IsNotNull(typeof(AppFor));
            Assert.IsNotNull(typeof(AppMethod));
        }
    }
}
