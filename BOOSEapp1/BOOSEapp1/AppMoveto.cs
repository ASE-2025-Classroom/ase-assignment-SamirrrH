using BOOSE;
using System;

namespace BOOSEapp1
{
    /// <summary>
    /// Moves the canvas cursor to a new X and Y position.
    /// </summary>
    public class AppMoveTo : CommandTwoParameters
    {
        /// <summary>
        /// The target X and Y position.
        /// </summary>
        private int x;
        private int y;

        /// <summary>
        /// Default constructor for factory use.
        /// </summary>
        public AppMoveTo() : base()
        {
        }

        /// <summary>
        /// Creates the command with a canvas and target position.
        /// </summary>
        /// <param name="c">The canvas to move on.</param>
        /// <param name="x">Target X position.</param>
        /// <param name="y">Target Y position.</param>
        public AppMoveTo(Canvas c, int x, int y) : base(c)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Reads the two parameters and moves the canvas cursor.
        /// </summary>
        public override void Execute()
        {
            // Let the base class read the parameters.
            base.Execute();

            /// <summary>
            /// Gets the X and Y values passed to the command.
            /// </summary>
            x = Paramsint[0];
            y = Paramsint[1];

            /// <summary>
            /// Checks that the coordinates are not negative.
            /// </summary>
            // Do a simple bounds check.
            if (x < 0 || y < 0)
            {
                throw new CanvasException("MoveTo coordinates must be positive");
            }

            /// <summary>
            /// Moves the cursor without drawing anything.
            /// </summary>
            // Move the cursor on the canvas without drawing.
            Canvas.MoveTo(x, y);
        }
    }
}
