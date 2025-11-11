using BOOSE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOOSEcode3
{
    internal class AppCanvas : ICanvas
    {
        Bitmap CanvasBitmap;
        Graphics g;
        private int xPos, yPos; //pen position when drawing
        Pen Pen;

        public AppCanvas(int xsize, int ysize)
        {
            CanvasBitmap = new Bitmap(xsize, ysize);
            g = Graphics.FromImage(CanvasBitmap);
            xPos = 100;
            yPos = 100;
            Pen = new Pen(Color.Black);
        }
        public int Xpos { get => xPos; set => xPos = value; }
        public int Ypos { get => yPos; set => yPos = value; }
        public object PenColour { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Circle(int radius, bool filled)
        {
            g.DrawEllipse(Pen, Xpos, Ypos, radius * 2, radius *2);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void DrawTo(int x, int y)
        {
            throw new NotImplementedException();
        }

        public object getBitmap()
        {
            throw new NotImplementedException();
        }

        public void MoveTo(int x, int y)
        {
            Xpos = x;
            Ypos = y;
        }

        public void Rect(int width, int height, bool filled)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Set(int width, int height)
        {
            throw new NotImplementedException();
        }

        public void SetColour(int red, int green, int blue)
        {
            throw new NotImplementedException();
        }

        public void Tri(int width, int height)
        {
            throw new NotImplementedException();
        }

        public void WriteText(string text)
        {
            throw new NotImplementedException();
        }
    }
}
