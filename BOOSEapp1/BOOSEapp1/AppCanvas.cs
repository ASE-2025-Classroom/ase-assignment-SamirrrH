using BOOSE;
using System;
using System.Drawing;

namespace BOOSEapp1
{
    /// <summary>
    /// A simple drawing canvas that implements BOOSE's ICanvas.
    /// It draws onto a Bitmap using a Graphics object.
    /// </summary>
    public class AppCanvas : ICanvas
    {
        /// <summary>
        /// The image we draw on.
        /// </summary>
        private Bitmap canvasBitmap;

        /// <summary>
        /// The tool used to draw on the bitmap.
        /// </summary>
        private Graphics g;

        /// <summary>
        /// Current drawing position (cursor).
        /// </summary>
        private int xPos, yPos;

        /// <summary>
        /// Pen for outlines/lines and brush for filled shapes.
        /// </summary>
        private Pen pen;
        private Brush brush;

        /// <summary>
        /// Creates a new canvas with the given size.
        /// </summary>
        /// <param name="xsize">Canvas width.</param>
        /// <param name="ysize">Canvas height.</param>
        public AppCanvas(int xsize, int ysize)
        {
            canvasBitmap = new Bitmap(xsize, ysize);
            g = Graphics.FromImage(canvasBitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // ✅ FIX: thinner pen to match lecturer output
            pen = new Pen(Color.Red, 3f);
            brush = new SolidBrush(pen.Color);

            xPos = 0;
            yPos = 0;

            Clear();
        }

        /// <summary>
        /// Gets or sets the current X position.
        /// </summary>
        public int Xpos
        {
            get => xPos;
            set => xPos = value;
        }

        /// <summary>
        /// Gets or sets the current Y position.
        /// </summary>
        public int Ypos
        {
            get => yPos;
            set => yPos = value;
        }

        /// <summary>
        /// Gets or sets the pen/brush colour.
        /// </summary>
        public object PenColour
        {
            get => pen.Color;
            set
            {
                if (value is Color c)
                {
                    pen.Color = c;
                    brush = new SolidBrush(c);
                }
            }
        }

        /// <summary>
        /// Draws a circle at the current position.
        /// </summary>
        /// <param name="radius">Circle radius.</param>
        /// <param name="filled">True to fill, false for outline.</param>
        public void Circle(int radius, bool filled)
        {
            int d = radius * 2;
            int x = Xpos - radius;
            int y = Ypos - radius;

            if (filled)
                g.FillEllipse(brush, x, y, d, d);
            else
                g.DrawEllipse(pen, x, y, d, d);
        }

        /// <summary>
        /// Clears the whole canvas to a background colour.
        /// </summary>
        public void Clear()
        {
            g.Clear(Color.Gray);
        }

        /// <summary>
        /// Draws a line from the current position to (x, y),
        /// then moves the current position to (x, y).
        /// </summary>
        /// <param name="x">Target X.</param>
        /// <param name="y">Target Y.</param>
        public void DrawTo(int x, int y)
        {
            g.DrawLine(pen, Xpos, Ypos, x, y);
            Xpos = x;
            Ypos = y;
        }

        /// <summary>
        /// Returns the canvas bitmap so it can be displayed.
        /// </summary>
        /// <returns>The bitmap object.</returns>
        public object getBitmap()
        {
            return canvasBitmap;
        }

        /// <summary>
        /// Moves the current position without drawing.
        /// </summary>
        /// <param name="x">New X.</param>
        /// <param name="y">New Y.</param>
        public void MoveTo(int x, int y)
        {
            Xpos = x;
            Ypos = y;
        }

        /// <summary>
        /// Draws a rectangle at the current position.
        /// </summary>
        /// <param name="width">Rectangle width.</param>
        /// <param name="height">Rectangle height.</param>
        /// <param name="filled">True to fill, false for outline.</param>
        public void Rect(int width, int height, bool filled)
        {
            Rectangle rect = new Rectangle(Xpos, Ypos, width, height);

            if (filled)
                g.FillRectangle(brush, rect);
            else
                g.DrawRectangle(pen, rect);
        }

        /// <summary>
        /// Resets the current position back to (0, 0).
        /// </summary>
        public void Reset()
        {
            Xpos = 0;
            Ypos = 0;
        }

        /// <summary>
        /// Resizes the canvas and clears it.
        /// </summary>
        /// <param name="width">New width.</param>
        /// <param name="height">New height.</param>
        public void Set(int width, int height)
        {
            canvasBitmap = new Bitmap(width, height);
            g = Graphics.FromImage(canvasBitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Clear();
            Reset();
        }

        /// <summary>
        /// Sets the drawing colour using RGB values.
        /// </summary>
        /// <param name="red">Red (0-255).</param>
        /// <param name="green">Green (0-255).</param>
        /// <param name="blue">Blue (0-255).</param>
        public void SetColour(int red, int green, int blue)
        {
            Color c = Color.FromArgb(red, green, blue);
            pen.Color = c;
            brush = new SolidBrush(c);
        }

        /// <summary>
        /// Draws a triangle using the current position as the left base point.
        /// </summary>
        /// <param name="width">Triangle width.</param>
        /// <param name="height">Triangle height.</param>
        public void Tri(int width, int height)
        {
            Point p1 = new Point(Xpos, Ypos);
            Point p2 = new Point(Xpos + width, Ypos);
            Point p3 = new Point(Xpos + width / 2, Ypos - height);

            Point[] points = { p1, p2, p3 };
            g.DrawPolygon(pen, points);
        }

        /// <summary>
        /// Draws text at the current position.
        /// </summary>
        /// <param name="text">Text to draw.</param>
        public void WriteText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text cannot be empty.");

            using var textBrush = new SolidBrush(pen.Color);
            using var font = new Font("Calibri", 12f);
            g.DrawString(text, font, textBrush, Xpos, Ypos);
        }
    }
}
