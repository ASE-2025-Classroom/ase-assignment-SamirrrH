using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSE;
using BOOSEapp1;

namespace BOOSEapp1.tests
{
    /// <summary>
    /// Tests for the AppArray class.
    /// </summary>
    [TestClass]
    public class AppArrayTests
    {
        /// <summary>
        /// Checks that an int array can store and read values.
        /// </summary>
        [TestMethod]
        public void Array_Int_AllocatesAndStoresValue()
        {
            var program = TestProgramFactory.CreateProgram();

            var arr = new AppArray();
            arr.Set(program, "int myArr 5");
            arr.Compile();
            arr.Execute();

            Assert.IsTrue(program.VariableExists("myArr"));

            var stored = (AppArray)program.GetVariable("myArr");
            Assert.AreEqual("int", stored.type);

            stored.SetIntArray(123, 0, 0);
            Assert.AreEqual(123, stored.GetIntArray(0, 0));
        }

        /// <summary>
        /// Checks that a real array can store and read values.
        /// </summary>
        [TestMethod]
        public void Array_Real_AllocatesAndStoresValue()
        {
            var program = TestProgramFactory.CreateProgram();

            var arr = new AppArray();
            arr.Set(program, "real rArr 3,4");
            arr.Compile();
            arr.Execute();

            var stored = (AppArray)program.GetVariable("rArr");
            Assert.AreEqual("real", stored.type);

            stored.SetRealArray(2.5, 2, 3);
            Assert.AreEqual(2.5, stored.GetRealArray(2, 3), 1e-9);
        }

        /// <summary>
        /// Checks that using bad indexes throws an error.
        /// </summary>
        [TestMethod]
        public void Array_OutOfBounds_ThrowsCanvasException()
        {
            var program = TestProgramFactory.CreateProgram();

            var arr = new AppArray();
            arr.Set(program, "int myArr 2,2");
            arr.Compile();
            arr.Execute();

            var stored = (AppArray)program.GetVariable("myArr");

            Assert.ThrowsException<CanvasException>(() => stored.SetIntArray(1, 5, 0));
            Assert.ThrowsException<CanvasException>(() => stored.GetIntArray(0, 9));
        }
    }
}
