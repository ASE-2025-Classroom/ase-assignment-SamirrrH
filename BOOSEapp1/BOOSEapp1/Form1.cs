using BOOSE;
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
        private Parser parser;

        private TextBox txtProgram;
        private Button btnRun;
        private Button btnReset;

        public Form1()
        {
            InitializeComponent();

            this.Width = 1100;
            this.Height = 800;

            Debug.WriteLine(AboutBOOSE.about());

            // ---------------- TEXT BOX ----------------
            txtProgram = new TextBox
            {
                Multiline = true,

                // Keep method headers on one "real" line (no automatic wrapping)
                WordWrap = false,
                ScrollBars = ScrollBars.Both,

                Font = new Font("Calibri", 10),
                Left = 10,
                Top = 10,
                Width = 300,
                Height = 650
            };
            Controls.Add(txtProgram);

            // ---------------- RUN BUTTON ----------------
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

            // ---------------- RESET BUTTON ----------------
            btnReset = new Button
            {
                Text = "Reset Canvas",
                Font = new Font("Calibri", 10),
                Left = 100,
                Top = 670,
                Width = 120,
                Height = 30
            };
            btnReset.Click += BtnReset_Click;
            Controls.Add(btnReset);

            // ---------------- CANVAS ----------------
            canvas = new AppCanvas(1000, 600);
            canvas.Clear();

            AppWrite.SetCanvas(canvas);

            // Create initial factory (fine), but we will recreate it on every Run too
            factory = new AppCommandFactory();

            // Paint event
            this.Paint += Form1_Paint;
        }

        // ---------------- RUN ----------------
        private void BtnRun_Click(object sender, EventArgs e)
        {
            try
            {
                canvas.Clear();

                // ✅ IMPORTANT: recreate factory/program/parser EACH RUN
                factory = new AppCommandFactory();
                program = new StoredProgram(canvas);
                parser = new AppParser(factory, program);

                // ✅ reset write canvas each run
                AppWrite.SetCanvas(canvas);

                parser.ParseProgram(txtProgram.Text);
                program.Run();

                Invalidate();
            }
            catch (CanvasException ex)
            {
                MessageBox.Show(ex.Message, "BOOSE Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------------- RESET ----------------
        private void BtnReset_Click(object sender, EventArgs e)
        {
            canvas.Clear();
            Invalidate();
        }

        // ---------------- DRAW CANVAS ----------------
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Bitmap bmp = (Bitmap)canvas.getBitmap();
            if (bmp != null)
            {
                e.Graphics.DrawImage(bmp, 330, 10);
            }
        }
    }
}
