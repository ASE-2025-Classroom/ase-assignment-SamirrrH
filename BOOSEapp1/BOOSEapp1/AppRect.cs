using BOOSE;
using System;

namespace BOOSEapp1
{
    /// <summary>
    /// Draws a rectangle on the canvas using the given width and height.
    /// </summary>
    public class AppRect : CommandTwoParameters
    {
        /// <summary>
        /// The width and height of the rectangle.
        /// </summary>
        private int width;
        private int height;

        /// <summary>
        /// Default constructor used by the factory.
        /// </summary>
        public AppRect() : base()
        {
        }

        /// <summary>
        /// Creates the command with a canvas and size values.
        /// </summary>
        /// <param name="c">The canvas to draw on.</param>
        /// <param name="width">Rectangle width.</param>
        /// <param name="height">Rectangle height.</param>
        public AppRect(Canvas c, int width, int height) : base(c)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Reads the parameters, checks them, and draws the rectangle.
        /// </summary>
        public override void Execute()
        {
            // Let the base class read the parameters.
            base.Execute();

            /// <summary>
            /// Gets the width and height values from the command.
            /// </summary>
            width = Paramsint[0];
            height = Paramsint[1];

            /// <summary>
            /// Makes sure both values are valid.
            /// </summary>
            // Quick range check for both values.
            if (width < 1 || height < 1)
            {
                throw new CanvasException("Rectangle width and height must be greater than 0.");
            }

            /// <summary>
            /// Draws the rectangle outline on the canvas.
            /// </summary>
            // Draw the rectangle outline.
            Canvas.Rect(width, height, false);
        }
    }
}
