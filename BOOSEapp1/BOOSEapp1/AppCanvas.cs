using BOOSE;
using System;
using System.Drawing;

namespace BOOSEapp1
{
    internal class AppCanvas : ICanvas
    {
        private Bitmap canvasBitmap;
        private Graphics g;
        private int xPos, yPos; // pen position
        private Pen pen;
        private Brush brush;

        public AppCanvas(int xsize, int ysize)
        {
            canvasBitmap = new Bitmap(xsize, ysize);
            g = Graphics.FromImage(canvasBitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            pen = new Pen(Color.Black);
            brush = new SolidBrush(Color.Black);

            xPos = 100;
            yPos = 100;

            Clear();
        }

        public int Xpos
        {
            get => xPos;
            set => xPos = value;
        }

        public int Ypos
        {
            get => yPos;
            set => yPos = value;
        }

        
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

        public void Circle(int radius, bool filled)
        {
            
            Rectangle rect = new Rectangle(Xpos, Ypos, radius * 2, radius * 2);

            if (filled)
                g.FillEllipse(brush, rect);
            else
                g.DrawEllipse(pen, rect);
        }

        public void Clear()
        {
            g.Clear(Color.White);
        }

        public void DrawTo(int x, int y)
        {
            g.DrawLine(pen, Xpos, Ypos, x, y);
            Xpos = x;
            Ypos = y;
        }

        public object getBitmap()
        {
            return canvasBitmap;
        }

        public void MoveTo(int x, int y)
        {
            Xpos = x;
            Ypos = y;
        }

        public void Rect(int width, int height, bool filled)
        {
            Rectangle rect = new Rectangle(Xpos, Ypos, width, height);

            if (filled)
                g.FillRectangle(brush, rect);
            else
                g.DrawRectangle(pen, rect);
        }

        public void Reset()
        {
            Xpos = 100;
            Ypos = 100;
        }

        public void Set(int width, int height)
        {
            
            canvasBitmap = new Bitmap(width, height);
            g = Graphics.FromImage(canvasBitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Clear();
            Reset();
        }

        public void SetColour(int red, int green, int blue)
        {
            Color c = Color.FromArgb(red, green, blue);
            pen.Color = c;
            brush = new SolidBrush(c);
        }

        public void Tri(int width, int height)
        {
            
            Point p1 = new Point(Xpos, Ypos);
            Point p2 = new Point(Xpos + width, Ypos);
            Point p3 = new Point(Xpos + width / 2, Ypos - height);

            Point[] points = { p1, p2, p3 };
            g.DrawPolygon(pen, points);
        }

        public void WriteText(string text)
        {
            using (Font font = SystemFonts.DefaultFont)
            {
                g.DrawString(text, font, brush, Xpos, Ypos);
            }
        }
    }
}
