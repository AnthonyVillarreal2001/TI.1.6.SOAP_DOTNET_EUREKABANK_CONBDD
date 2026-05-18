using System;
using System.Drawing;
using System.Windows.Forms;
using ServicioEB2;

namespace CLIESC_EUREKABANK_SOAP_GR08
{
    public partial class FormMovimientos : Form
    {
        public FormMovimientos(string cuenta, Movimiento[] movimientos)
        {
            this.Text = $"Movimientos - {cuenta}";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackgroundImage = Properties.Resources.sullivan;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            Panel overlay = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(200, 0, 0, 0) };
            this.Controls.Add(overlay);

            RoundedPanel container = new RoundedPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.White,
                CornerRadius = 25,
                ApplyShadow = true,
                Margin = new Padding(20)
            };
            Label title = new Label
            {
                Text = $"Movimientos de cuenta {cuenta}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(10, 61, 66),
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };

            DataGridView dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(248, 248, 248) }
            };
            dgv.Columns.Add("Numero", "Nro");
            dgv.Columns.Add("Fecha", "Fecha");
            dgv.Columns.Add("Tipo", "Tipo");
            dgv.Columns.Add("Accion", "Acción");
            dgv.Columns.Add("Importe", "Importe (USD)");

            foreach (var m in movimientos)
            {
                string tipo = m.TipoCodigo switch
                {
                    "003" => "Depósito",
                    "004" => "Retiro",
                    "008" => "Transferencia",
                    "009" => "Transferencia",
                    _ => "Otro"
                };
                string accion = (m.TipoCodigo == "003" || m.TipoCodigo == "008") ? "INGRESO" : "SALIDA";
                int row = dgv.Rows.Add(m.Numero, m.Fecha.ToString("yyyy-MM-dd HH:mm"), tipo, accion, m.Importe.ToString("N2"));
                if (accion == "INGRESO")
                    dgv.Rows[row].DefaultCellStyle.ForeColor = Color.FromArgb(46, 204, 113);
                else
                    dgv.Rows[row].DefaultCellStyle.ForeColor = Color.FromArgb(231, 76, 60);
            }

            container.Controls.Add(dgv);
            container.Controls.Add(title);
            overlay.Controls.Add(container);
        }

        private void InitializeComponent() { }
    }
}