using BOOSE;
using System;

namespace BOOSEapp1
{
    /// <summary>
    /// Draws a circle on the canvas using the given radius.
    /// </summary>
    public class AppCircle : CommandOneParameter
    {
        /// <summary>
        /// The radius of the circle to draw.
        /// </summary>
        private int radius;

        /// <summary>
        /// Default constructor used by the command factory.
        /// </summary>
        public AppCircle() : base()
        {
        }

        /// <summary>
        /// Creates a circle command with a canvas and radius.
        /// </summary>
        /// <param name="c">The canvas to draw on.</param>
        /// <param name="radius">The radius of the circle.</param>
        public AppCircle(Canvas c, int radius) : base(c)
        {
            this.radius = radius;
        }

        /// <summary>
        /// Runs the command, checks the value, and draws the circle.
        /// </summary>
        public override void Execute()
        {
            // Parse the parameter from BOOSE.
            base.Execute();

            /// <summary>
            /// Gets the radius value passed to the command.
            /// </summary>
            radius = Paramsint[0];

            /// <summary>
            /// Checks that the radius is a valid positive number.
            /// </summary>
            if (radius < 1)
            {
                throw new CanvasException("Circle radius must be greater than 0.");
            }

            /// <summary>
            /// Draws the circle outline on the canvas.
            /// </summary>
            Canvas.Circle(radius, false);
        }
    }
}
