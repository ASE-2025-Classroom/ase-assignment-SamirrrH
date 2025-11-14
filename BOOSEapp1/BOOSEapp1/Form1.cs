using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BOOSEapp1
{
    public partial class Form1 : Form
    {
        private AppCanvas canvas;
        private TextBox txtProgram;
        private Button btnRun;

        public Form1()
        {
            InitializeComponent();

            
            txtProgram = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 10),
                Left = 10,
                Top = 10,
                Width = 320,
                Height = ClientSize.Height - 60,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };
            Controls.Add(txtProgram);

            
            btnRun = new Button
            {
                Text = "Run",
                Left = 10,
                Top = ClientSize.Height - 40,
                Width = 80,
                Height = 30,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            btnRun.Click += (s, e) =>
            {
                RunScript(txtProgram.Text);
                Invalidate();   
            };
            Controls.Add(btnRun);

            
            this.Paint += Form1_Paint;

            
            canvas = new AppCanvas(1000, 600);
            canvas.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var bmp = (Bitmap)canvas.getBitmap();
            if (bmp == null) return;

            int left = txtProgram.Right + 20;
            int top = 10;

            e.Graphics.DrawImage(bmp, left, top);
        }

        private void RunScript(string src)
        {
            canvas.Clear();
            canvas.Reset();

            var lines = src.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (line.Length == 0) continue;
                if (line.StartsWith("*") || line.StartsWith("#")) continue;

                int space = line.IndexOf(' ');
                string cmd = (space < 0 ? line : line[..space]).ToLowerInvariant();
                string args = (space < 0 ? "" : line[(space + 1)..]).Trim();

                switch (cmd)
                {
                    case "moveto":
                        {
                            var (x, y) = Two(args);
                            canvas.MoveTo(x, y);
                            break;
                        }
                    case "drawto":
                        {
                            var (x, y) = Two(args);
                            canvas.DrawTo(x, y);
                            break;
                        }
                    case "pen":
                    case "setcolour":
                        {
                            var (r, g, b) = Three(args);
                            canvas.SetColour(r, g, b);
                            break;
                        }
                    case "circle":
                        {
                            int r = One(args);
                            canvas.Circle(r, false);
                            break;
                        }
                    case "rect":
                        {
                            var (w, h) = Two(args);
                            canvas.Rect(w, h, false);
                            break;
                        }
                    case "tri":
                        {
                            var (w, h) = Two(args);
                            canvas.Tri(w, h);
                            break;
                        }
                    case "write":
                        {
                            canvas.WriteText(args.Trim().Trim('"'));
                            break;
                        }
                    case "clear":
                        canvas.Clear();
                        break;

                    case "reset":
                        canvas.Reset();
                        break;
                }
            }
        }

        private static int One(string s) => int.Parse(s.Trim());

        private static (int, int) Two(string s)
        {
            var p = s.Split(',', StringSplitOptions.RemoveEmptyEntries)
                     .Select(t => t.Trim())
                     .ToArray();
            return (int.Parse(p[0]), int.Parse(p[1]));
        }

        private static (int, int, int) Three(string s)
        {
            var p = s.Split(',', StringSplitOptions.RemoveEmptyEntries)
                     .Select(t => t.Trim())
                     .ToArray();
            return (int.Parse(p[0]), int.Parse(p[1]), int.Parse(p[2]));
        }
    }
}
