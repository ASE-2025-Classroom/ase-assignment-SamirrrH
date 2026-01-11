using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using BOOSE;
using BOOSEapp1;

namespace BOOSEapp1.tests
{
    /// <summary>
    /// Tests for the AppWhile class.
    /// </summary>
    [TestClass]
    public class AppWhileTests
    {
        /// <summary>
        /// Checks that AppWhile implements ICommand.
        /// </summary>
        [TestMethod]
        public void AppWhile_ImplementsICommand()
        {
            Assert.IsTrue(typeof(ICommand).IsAssignableFrom(typeof(AppWhile)));
        }

        /// <summary>
        /// Checks that Compile and Execute methods exist.
        /// </summary>
        [TestMethod]
        public void AppWhile_HasPublicCompileAndExecute()
        {
            var t = typeof(AppWhile);

            Assert.IsNotNull(t.GetMethod("Compile", BindingFlags.Public | BindingFlags.Instance));
            Assert.IsNotNull(t.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance));
        }

        /// <summary>
        /// Checks that AppWhile has a parameterless constructor.
        /// </summary>
        [TestMethod]
        public void AppWhile_HasParameterlessConstructor_ForFactoryUse()
        {
            Assert.IsNotNull(typeof(AppWhile).GetConstructor(Type.EmptyTypes));
        }
    }
}
