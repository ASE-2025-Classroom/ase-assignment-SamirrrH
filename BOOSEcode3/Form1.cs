using BOOSE;
using System.Diagnostics;

namespace BOOSEcode3
{
    public partial class Form1 : Form
    {
        AppCanvas canvas;
        public Form1()
        {
            InitializeComponent();
            Debug.WriteLine(AboutBOOSE.about());
            canvas = new AppCanvas(640, 480);
            canvas.Circle(100, true)


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
           Graphics graphics = e.Graphics;
           Bitmap b = (Bitmap)canvas.getBitmap(); 
        }
    }
}
