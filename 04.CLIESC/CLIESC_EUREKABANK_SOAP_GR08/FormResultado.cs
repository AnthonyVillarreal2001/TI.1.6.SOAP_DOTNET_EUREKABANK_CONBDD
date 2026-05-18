using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CLIESC_EUREKABANK_SOAP_GR08
{
    public partial class FormResultado : Form
    {
        public FormResultado(bool exito, decimal saldo, string cuenta, string operacion)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(450, 400);
            this.BackColor = Color.White;
            this.ShowInTaskbar = false;

            this.Paint += (s, e) => {
                var path = new GraphicsPath();
                path.AddArc(0, 0, 30, 30, 180, 90);
                path.AddArc(this.Width - 30, 0, 30, 30, 270, 90);
                path.AddArc(this.Width - 30, this.Height - 30, 30, 30, 0, 90);
                path.AddArc(0, this.Height - 30, 30, 30, 90, 90);
                this.Region = new Region(path);
            };

            Panel main = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25) };

            PictureBox icon = new PictureBox
            {
                Size = new Size(80, 80),
                Location = new Point((this.Width - 80) / 2, 30),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            if (exito)
                icon.Image = DibujarIconoExito();
            else
                icon.Image = DibujarIconoError();

            Label titulo = new Label
            {
                Text = exito ? "¡Operación Exitosa!" : "Error en la Operación",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = exito ? Color.FromArgb(46, 204, 113) : Color.FromArgb(231, 76, 60),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 50
            };
            Label mensaje = new Label
            {
                Text = exito ? $"{operacion} realizado correctamente." : "Ocurrió un error, verifique los datos.",
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 40
            };
            Label saldoLabel = new Label { Dock = DockStyle.Top, Height = 50, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 11) };
            if (exito && saldo > 0)
                saldoLabel.Text = $"Nuevo saldo de cuenta {cuenta}: ${saldo:N2}";

            Button btnOk = new Button
            {
                Text = "Volver al inicio",
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(180, 40),
                Location = new Point((this.Width - 180) / 2, 290),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += (s, e) => this.Close();

            main.Controls.Add(btnOk);
            main.Controls.Add(saldoLabel);
            main.Controls.Add(mensaje);
            main.Controls.Add(titulo);
            main.Controls.Add(icon);
            this.Controls.Add(main);

            if (exito) AnimacionConfeti();
        }

        private Image DibujarIconoExito()
        {
            Bitmap bmp = new Bitmap(80, 80);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.White);
                g.FillEllipse(new SolidBrush(Color.FromArgb(46, 204, 113)), 0, 0, 80, 80);
                using (Pen pen = new Pen(Color.White, 6))
                {
                    g.DrawLine(pen, 20, 40, 35, 55);
                    g.DrawLine(pen, 35, 55, 60, 25);
                }
            }
            return bmp;
        }

        private Image DibujarIconoError()
        {
            Bitmap bmp = new Bitmap(80, 80);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.White);
                g.FillEllipse(new SolidBrush(Color.FromArgb(231, 76, 60)), 0, 0, 80, 80);
                using (Pen pen = new Pen(Color.White, 6))
                {
                    g.DrawLine(pen, 25, 25, 55, 55);
                    g.DrawLine(pen, 55, 25, 25, 55);
                }
            }
            return bmp;
        }

        private void AnimacionConfeti()
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 30;
            int cont = 0;
            timer.Tick += (s, e) => {
                if (cont++ > 50) timer.Stop();
                Random r = new Random();
                using (Graphics g = this.CreateGraphics())
                {
                    for (int i = 0; i < 5; i++)
                    {
                        var color = Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
                        using (Brush b = new SolidBrush(color))
                        {
                            g.FillEllipse(b, r.Next(this.Width), r.Next(this.Height), 4, 4);
                        }
                    }
                }
            };
            timer.Start();
        }

        private void InitializeComponent() { }
    }
}