using BOOSE;
using System.Diagnostics;

namespace BOOSEcode3
{
    public partial class Form1 : Form
    {
        AppCanvas canvas;
        CommandFactory Factory;
        StoredProgram Program;
        IParser Parser;
        public Form1()
        {
            InitializeComponent();
            Debug.WriteLine(AboutBOOSE.about());
            canvas = new AppCanvas(640, 480);
            canvas.Circle(100, true);
            canvas.Circle(50, true);
            Factory = new CommandFactory();
            Program = new StoredProgram(canvas);
            Parser = new Parser(Factory, Program);


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
           Graphics graphics = e.Graphics;
           Bitmap b = (Bitmap)canvas.getBitmap();
          
           Program.Run();

           graphics.DrawImage(b, 0, 0);
        }
    }
}
