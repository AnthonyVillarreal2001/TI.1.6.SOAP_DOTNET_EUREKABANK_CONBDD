using System;
using System.Drawing;
using System.Windows.Forms;

namespace CLIESC_EUREKABANK_SOAP_GR08
{
    public partial class OperacionCard : UserControl
    {
        private bool expanded = false;
        private RoundedPanel headerPanel;
        private Panel contentPanel;
        private Label titleLabel;
        private PictureBox iconBox;
        public event EventHandler<OperacionCard> CardClicked;
        public Panel Content => contentPanel;

        public OperacionCard(string titulo, Image icono)
        {
            this.Size = new Size(280, 70);
            this.BackColor = Color.Transparent;
            this.Margin = new Padding(10);

            // Header redondeado
            headerPanel = new RoundedPanel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(10, 61, 66),
                CornerRadius = 15,
                ApplyShadow = true
            };
            iconBox = new PictureBox
            {
                Image = icono,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(40, 40),
                Location = new Point(20, 15)
            };
            titleLabel = new Label
            {
                Text = titulo,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(75, 25),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(iconBox);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Click += (s, e) => CardClicked?.Invoke(this, this);
            iconBox.Click += (s, e) => CardClicked?.Invoke(this, this);
            titleLabel.Click += (s, e) => CardClicked?.Invoke(this, this);

            // Contenido colapsable
            contentPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 0,
                Visible = false,
                BackColor = Color.FromArgb(250, 250, 250),
                Padding = new Padding(12)
            };

            this.Controls.Add(contentPanel);
            this.Controls.Add(headerPanel);
        }

        public void Expand()
        {
            if (expanded) return;
            expanded = true;
            contentPanel.Visible = true;
            contentPanel.Height = 180; // Ajusta según tu contenido
            this.Height = 70 + 180;
        }

        public void Collapse()
        {
            if (!expanded) return;
            expanded = false;
            contentPanel.Visible = false;
            contentPanel.Height = 0;
            this.Height = 70;
        }
    }
}