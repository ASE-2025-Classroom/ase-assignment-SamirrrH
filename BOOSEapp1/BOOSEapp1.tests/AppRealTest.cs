using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSE;
using BOOSEapp1;

namespace BOOSEapp1.tests
{
    /// <summary>
    /// Tests for the AppReal class.
    /// </summary>
    [TestClass]
    public class AppRealTests
    {
        /// <summary>
        /// Checks that a real without a value starts at zero.
        /// </summary>
        [TestMethod]
        public void Real_DeclarationWithoutInit_DefaultsToZero()
        {
            var program = TestProgramFactory.CreateProgram();

            var cmd = new AppReal();
            cmd.Set(program, "x");
            cmd.Compile();
            cmd.Execute();

            Assert.IsTrue(program.VariableExists("x"));

            var v = program.GetVariable("x");
            Assert.IsInstanceOfType(v, typeof(AppReal));
            Assert.AreEqual(0.0, ((AppReal)v).Value, 1e-9);
        }

        /// <summary>
        /// Checks that a real with a value is set correctly.
        /// </summary>
        [TestMethod]
        public void Real_DeclarationWithInit_SetsValue()
        {
            var program = TestProgramFactory.CreateProgram();

            var cmd = new AppReal();
            cmd.Set(program, "pi = 3.5");
            cmd.Compile();
            cmd.Execute();

            var v = (AppReal)program.GetVariable("pi");
            Assert.AreEqual(3.5, v.Value, 1e-9);
        }

        /// <summary>
        /// Checks that a real can use another variable in an expression.
        /// </summary>
        [TestMethod]
        public void Real_ExpressionInit_UsesExistingVariable()
        {
            var program = TestProgramFactory.CreateProgram();

            var a = new AppReal();
            a.Set(program, "a = 2.5");
            a.Compile();
            a.Execute();

            var b = new AppReal();
            b.Set(program, "b = a * 2");
            b.Compile();
            b.Execute();

            var bv = (AppReal)program.GetVariable("b");
            Assert.AreEqual(5.0, bv.Value, 1e-9);
        }
    }
}
