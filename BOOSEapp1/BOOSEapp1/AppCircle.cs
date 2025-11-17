using BOOSE;
using System;

namespace BOOSEapp1
{
    /// <summary>
    /// Draws a circle on the canvas using the given radius.
    /// </summary>
    public class AppCircle : CommandOneParameter
    {
        private int radius;

        /// <summary>
        /// Default constructor used by the factory.
        /// </summary>
        public AppCircle() : base()
        {
        }

        /// <summary>
        /// Creates the command with a specific canvas and radius.
        /// </summary>
        public AppCircle(Canvas c, int radius) : base(c)
        {
            this.radius = radius;
        }

        /// <summary>
        /// Runs the circle command and draws the shape.
        /// </summary>
        public override void Execute()
        {
            // Parse the parameter from BOOSE.
            base.Execute();

            radius = Paramsint[0];

            // Make sure the radius is a valid number.
            if (radius < 1)
            {
                throw new CanvasException("Circle radius must be greater than 0.");
            }

            // Draws the circle outline.
            Canvas.Circle(radius, false);
        }
    }
}