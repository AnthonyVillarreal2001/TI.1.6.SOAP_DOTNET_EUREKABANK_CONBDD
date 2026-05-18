using System;
using System.Drawing;
using System.Windows.Forms;
using ServicioEB2;

namespace CLIESC_EUREKABANK_SOAP_GR08
{
    public partial class FormLogin : Form
    {
        private CoreBancarioSoapClient soap;
        public FormLogin()
        {
            InitializeComponent();
            this.Text = "EurekaBank - Acceso";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackgroundImage = Properties.Resources.sullivan;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // Overlay oscuro
            Panel overlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(180, 0, 0, 0)
            };
            this.Controls.Add(overlay);

            // Panel central redondeado
            RoundedPanel loginBox = new RoundedPanel
            {
                Size = new Size(380, 320),
                BackColor = Color.White,
                CornerRadius = 30,
                ApplyShadow = true,
                Location = new Point((this.Width - 380) / 2, (this.Height - 320) / 2)
            };

            // Logo
            PictureBox logo = new PictureBox
            {
                Image = Properties.Resources.logo_ldu,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(80, 80),
                Location = new Point(150, 20)
            };

            // Campos
            Label lblUser = new Label { Text = "Usuario", Location = new Point(40, 120), Font = new Font("Segoe UI", 10), ForeColor = Color.FromArgb(10, 61, 66) };
            TextBox txtUser = new TextBox { Location = new Point(40, 145), Width = 300, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };
            Label lblPass = new Label { Text = "Contraseña", Location = new Point(40, 185), Font = new Font("Segoe UI", 10), ForeColor = Color.FromArgb(10, 61, 66) };
            TextBox txtPass = new TextBox { Location = new Point(40, 210), Width = 300, UseSystemPasswordChar = true, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };
            Button btnLogin = new Button
            {
                Text = "INGRESAR",
                Location = new Point(40, 255),
                Width = 300,
                Height = 40,
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnLogin.FlatAppearance.BorderSize = 0;

            soap = new CoreBancarioSoapClient(
                CoreBancarioSoapClient.EndpointConfiguration.BasicHttpBinding_CoreBancarioSoap,
                "https://localhost:7299/CoreBancario.svc"); // Cambia por tu URL

            btnLogin.Click += async (s, e) => {
                string user = txtUser.Text;
                string pass = txtPass.Text;
                var result = await soap.ValidarIngresoAsync(user, pass);
                if (result == "Exitoso")
                {
                    FormMenu menu = new FormMenu(user);
                    menu.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            loginBox.Controls.Add(logo);
            loginBox.Controls.Add(lblUser);
            loginBox.Controls.Add(txtUser);
            loginBox.Controls.Add(lblPass);
            loginBox.Controls.Add(txtPass);
            loginBox.Controls.Add(btnLogin);
            overlay.Controls.Add(loginBox);
        }

        private void InitializeComponent() { }
    }
}