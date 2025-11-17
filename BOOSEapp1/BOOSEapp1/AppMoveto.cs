using BOOSE;
using System;

namespace BOOSEapp1
{
    /// <summary>
    /// Moves the canvas cursor to a new X and Y position.
    /// </summary>
    public class AppMoveTo : CommandTwoParameters
    {
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
        public AppMoveTo(Canvas c, int x, int y) : base(c)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Runs the moveto command and updates the cursor position.
        /// </summary>
        public override void Execute()
        {
            // Let the base class read the parameters.
            base.Execute();

            x = Paramsint[0];
            y = Paramsint[1];

            // Do a simple bounds check.
            if (x < 0 || y < 0)
            {
                throw new CanvasException("MoveTo coordinates must be positive");
            }

            // Move the cursor on the canvas without drawing.
            Canvas.MoveTo(x, y);
        }
    }
}