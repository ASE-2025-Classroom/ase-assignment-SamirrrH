using BOOSE;
using BOOSEapp1;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace BOOSEapp1
{
    public partial class Form1 : Form
    {
        private AppCanvas canvas;
        private CommandFactory factory;
        private StoredProgram program;
        private IParser parser;

        private TextBox txtProgram;
        private Button btnRun;
        private Button btnReset;  // RESET BUTTON

        public Form1()
        {
            InitializeComponent();

            this.Width = 1100;
            this.Height = 800;

            txtProgram = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Calibri", 10),
                Left = 10,
                Top = 10,
                Width = 300,
                Height = 650
            };
            Controls.Add(txtProgram);

            btnRun = new Button
            {
                Text = "Run",
                Font = new Font("Calibri", 10),
                Left = 10,
                Top = 670,
                Width = 80,
                Height = 30
            };
            btnRun.Click += BtnRun_Click;
            Controls.Add(btnRun);

            // --- RESET BUTTON ---
            btnReset = new Button
            {
                Text = "Reset Canvas",     // Button label
                Font = new Font("Calibri", 10),
                Left = 100,
                Top = 670,
                Width = 120,
                Height = 30
            };
            btnReset.Click += BtnReset_Click;
            Controls.Add(btnReset);
            // ----------------------

            canvas = new AppCanvas(1000, 600);
            canvas.Clear();

            factory = new AppCommandFactory();
            program = new StoredProgram(canvas);
            parser = new Parser(factory, program);

            this.Paint += Form1_Paint;
            this.Load += Form1_Load;

            Debug.WriteLine(AboutBOOSE.about());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        public AppCanvas Canvas => canvas;

        private void BtnRun_Click(object sender, EventArgs e)
        {
            try
            {
                canvas.Clear();
                parser.ParseProgram(txtProgram.Text);
                program.Run();
                Invalidate();
            }
            catch (BOOSE.CanvasException ex)
            {
                MessageBox.Show(ex.Message, "BOOSE Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- RESET BUTTON LOGIC ---
        private void BtnReset_Click(object sender, EventArgs e)
        {
            canvas.Clear();   // Same action as a clear button
            Invalidate();     // Refresh screen to show blank canvas
        }
        // --------------------------

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Bitmap bmp = (Bitmap)canvas.getBitmap();
            if (bmp != null)
            {
                e.Graphics.DrawImage(bmp, 330, 10);
            }
        }

        public void RunScript(string source)
        {
            canvas.Clear();
            parser.ParseProgram(source);
            program.Run();
        }
    }
}