using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSE;
using BOOSEapp1;

namespace BOOSEapp1.tests
{
    /// <summary>
    /// Tests for the AppInteg (int variable) class.
    /// </summary>
    [TestClass]
    public class AppIntegTests
    {
        /// <summary>
        /// Checks that an int without a value starts at zero.
        /// </summary>
        [TestMethod]
        public void Int_DeclarationWithoutInit_DefaultsToZero()
        {
            var program = TestProgramFactory.CreateProgram();

            var cmd = new AppInteg();
            cmd.Set(program, "x");
            cmd.Compile();
            cmd.Execute();

            Assert.IsTrue(program.VariableExists("x"));
            Assert.AreEqual("0", program.GetVarValue("x"));
        }

        /// <summary>
        /// Checks that an int with a value is set correctly.
        /// </summary>
        [TestMethod]
        public void Int_DeclarationWithInit_SetsValue()
        {
            var program = TestProgramFactory.CreateProgram();

            var cmd = new AppInteg();
            cmd.Set(program, "radius = 50");
            cmd.Compile();
            cmd.Execute();

            Assert.IsTrue(program.VariableExists("radius"));
            Assert.AreEqual("50", program.GetVarValue("radius"));
        }

        /// <summary>
        /// Checks that an int can use another variable in an expression.
        /// </summary>
        [TestMethod]
        public void Int_ExpressionInit_UsesExistingVariable()
        {
            var program = TestProgramFactory.CreateProgram();

            var a = new AppInteg();
            a.Set(program, "a = 10");
            a.Compile();
            a.Execute();

            var b = new AppInteg();
            b.Set(program, "b = a + 5");
            b.Compile();
            b.Execute();

            Assert.AreEqual("15", program.GetVarValue("b"));
        }
    }
}
