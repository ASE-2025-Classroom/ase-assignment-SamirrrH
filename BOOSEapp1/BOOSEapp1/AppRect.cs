using BOOSE;
using System;

namespace BOOSEapp1
{
    /// <summary>
    /// Draws a rectangle on the canvas using the given width and height.
    /// </summary>
    public class AppRect : CommandTwoParameters
    {
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
        public AppRect(Canvas c, int width, int height) : base(c)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Runs the rectangle command.
        /// </summary>
        public override void Execute()
        {
            // Let the base class read the parameters.
            base.Execute();

            width = Paramsint[0];
            height = Paramsint[1];

            // Quick range check for both values.
            if (width < 1 || height < 1)
            {
                throw new CanvasException("Rectangle width and height must be greater than 0.");
            }

            // Draw the rectangle outline.
            Canvas.Rect(width, height, false);
        }
    }
}