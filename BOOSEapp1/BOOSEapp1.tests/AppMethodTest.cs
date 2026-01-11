using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using BOOSE;
using BOOSEapp1;

namespace BOOSEapp1.tests
{
    /// <summary>
    /// Tests for the AppMethod class.
    /// </summary>
    [TestClass]
    public class AppMethodTests
    {
        /// <summary>
        /// Checks that AppMethod implements ICommand.
        /// </summary>
        [TestMethod]
        public void AppMethod_ImplementsICommand()
        {
            Assert.IsTrue(typeof(ICommand).IsAssignableFrom(typeof(AppMethod)));
        }

        /// <summary>
        /// Checks that Compile and Execute methods exist.
        /// </summary>
        [TestMethod]
        public void AppMethod_HasPublicCompileAndExecute()
        {
            var t = typeof(AppMethod);

            Assert.IsNotNull(t.GetMethod("Compile", BindingFlags.Public | BindingFlags.Instance));
            Assert.IsNotNull(t.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance));
        }

        /// <summary>
        /// Checks that AppMethod has a parameterless constructor.
        /// </summary>
        [TestMethod]
        public void AppMethod_HasParameterlessConstructor_ForFactoryUse()
        {
            Assert.IsNotNull(typeof(AppMethod).GetConstructor(Type.EmptyTypes));
        }

        /// <summary>
        /// Checks that AppMethod exposes all needed public properties.
        /// </summary>
        [TestMethod]
        public void AppMethod_ExposesExpectedProperties()
        {
            var t = typeof(AppMethod);

            Assert.IsNotNull(t.GetProperty("MethodName", BindingFlags.Public | BindingFlags.Instance));
            Assert.IsNotNull(t.GetProperty("Type", BindingFlags.Public | BindingFlags.Instance));
            Assert.IsNotNull(t.GetProperty("ReturnLine", BindingFlags.Public | BindingFlags.Instance));
            Assert.IsNotNull(t.GetProperty("LocalVariables", BindingFlags.Public | BindingFlags.Instance));
            Assert.IsNotNull(t.GetProperty("ParameterTypes", BindingFlags.Public | BindingFlags.Instance));
            Assert.IsNotNull(t.GetProperty("ParameterNames", BindingFlags.Public | BindingFlags.Instance));
        }
    }
}
