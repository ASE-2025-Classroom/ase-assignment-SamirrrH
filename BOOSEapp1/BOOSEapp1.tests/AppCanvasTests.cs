using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BOOSEapp1.Tests
{
    [TestClass]
    public class AppCanvasTests
    {
        [TestMethod]
        public void MoveTo_SetsPenPosition()
        {
            // Arrange
            var canvas = new BOOSEapp1.AppCanvas(500, 500);

            // Act
            canvas.MoveTo(150, 200);

            // Assert
            Assert.AreEqual(150, canvas.Xpos);
            Assert.AreEqual(200, canvas.Ypos);
        }

        [TestMethod]
        public void DrawTo_UpdatesPenPosition()
        {
            // Arrange
            var canvas = new BOOSEapp1.AppCanvas(500, 500);
            canvas.MoveTo(10, 10);

            // Act
            canvas.DrawTo(100, 50);

            // Assert
            Assert.AreEqual(100, canvas.Xpos);
            Assert.AreEqual(50, canvas.Ypos);
        }

        [TestMethod]
        public void MultilineProgram_EndsAtExpectedPosition()
        {
            // Arrange
            var canvas = new BOOSEapp1.AppCanvas(500, 500);

            // Simulate:
            // moveto 10,10
            // drawto 60,10
            // circle 20
            canvas.MoveTo(10, 10);
            canvas.DrawTo(60, 10);
            canvas.Circle(20, false);

            // Assert – final pen position is from last DrawTo
            Assert.AreEqual(60, canvas.Xpos);
            Assert.AreEqual(10, canvas.Ypos);
        }
    }
}
