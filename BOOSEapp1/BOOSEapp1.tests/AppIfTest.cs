using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using BOOSE;
using BOOSEapp1;

namespace BOOSEapp1.tests
{
    /// <summary>
    /// Tests for the AppIf class.
    /// </summary>
    [TestClass]
    public class AppIfTests
    {
        /// <summary>
        /// Checks that AppIf implements ICommand.
        /// </summary>
        [TestMethod]
        public void AppIf_ImplementsICommand()
        {
            Assert.IsTrue(typeof(ICommand).IsAssignableFrom(typeof(AppIf)));
        }

        /// <summary>
        /// Checks that Compile and Execute methods exist.
        /// </summary>
        [TestMethod]
        public void AppIf_HasPublicCompileAndExecute()
        {
            var t = typeof(AppIf);

            Assert.IsNotNull(t.GetMethod("Compile", BindingFlags.Public | BindingFlags.Instance));
            Assert.IsNotNull(t.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance));
        }

        /// <summary>
        /// Checks that AppIf has a parameterless constructor.
        /// </summary>
        [TestMethod]
        public void AppIf_HasParameterlessConstructor_ForFactoryUse()
        {
            Assert.IsNotNull(typeof(AppIf).GetConstructor(Type.EmptyTypes));
        }
    }
}
