using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using BOOSE;
using BOOSEapp1;

namespace BOOSEapp1.tests
{
    /// <summary>
    /// Tests for the AppFor class.
    /// </summary>
    [TestClass]
    public class AppForTests
    {
        /// <summary>
        /// Checks that AppFor implements ICommand.
        /// </summary>
        [TestMethod]
        public void AppFor_ImplementsICommand()
        {
            Assert.IsTrue(typeof(ICommand).IsAssignableFrom(typeof(AppFor)));
        }

        /// <summary>
        /// Checks that Compile and Execute methods exist.
        /// </summary>
        [TestMethod]
        public void AppFor_HasPublicCompileAndExecute()
        {
            var t = typeof(AppFor);

            Assert.IsNotNull(t.GetMethod("Compile", BindingFlags.Public | BindingFlags.Instance));
            Assert.IsNotNull(t.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance));
        }

        /// <summary>
        /// Checks that AppFor has a parameterless constructor.
        /// </summary>
        [TestMethod]
        public void AppFor_HasParameterlessConstructor_ForFactoryUse()
        {
            Assert.IsNotNull(typeof(AppFor).GetConstructor(Type.EmptyTypes));
        }

        /// <summary>
        /// Checks that AppFor has all needed public properties.
        /// </summary>
        [TestMethod]
        public void AppFor_ExposesExpectedProperties()
        {
            var t = typeof(AppFor);

            Assert.IsNotNull(t.GetProperty("From", BindingFlags.Public | BindingFlags.Instance));
            Assert.IsNotNull(t.GetProperty("To", BindingFlags.Public | BindingFlags.Instance));
            Assert.IsNotNull(t.GetProperty("Step", BindingFlags.Public | BindingFlags.Instance));
            Assert.IsNotNull(t.GetProperty("LoopVariableName", BindingFlags.Public | BindingFlags.Instance));
        }
    }
}
