using System;
using System.Drawing;
using System.Windows.Forms;
using ServicioEB2;

namespace CLIESC_EUREKABANK_SOAP_GR08
{
    public partial class FormMenu : Form
    {
        private CoreBancarioSoapClient soap;
        private string usuarioActual;
        private FlowLayoutPanel flowCards;
        private Panel cardsContainer;  // Contenedor con AutoScroll para centrar
        private OperacionCard cardMovimientos, cardDeposito, cardRetiro, cardTransferencia;
        private Panel headerPanel, footerPanel;
        private Label footerLabel;
        private Button logoutBtn;
        private PictureBox avatarBox;

        public FormMenu(string usuario)
        {
            this.usuarioActual = usuario;
            this.Text = "EurekaBank";
            this.WindowState = FormWindowState.Maximized;
            this.BackgroundImage = Properties.Resources.sullivan;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // Overlay oscuro
            Panel overlay = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(200, 0, 0, 0) };
            this.Controls.Add(overlay);

            // ========= HEADER ========= (sin cambios)
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(10, 61, 66),
                Padding = new Padding(20, 0, 20, 0)
            };

            PictureBox logo = new PictureBox
            {
                Image = Properties.Resources.logo_ldu,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(60, 60),
                Location = new Point(20, 10)
            };
            Label title = new Label
            {
                Text = "EurekaBank",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(90, 20),
                AutoSize = true
            };
            avatarBox = new PictureBox
            {
                Image = Properties.Resources.monster,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(45, 45),
                Name = "avatarBox"
            };
            Label userBadge = new Label
            {
                Text = usuarioActual,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Name = "userBadge"
            };
            logoutBtn = new Button
            {
                Text = "Cerrar Sesión",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                Size = new Size(110, 35),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 }
            };
            logoutBtn.Click += (s, e) => { this.Close(); Application.Exit(); };

            headerPanel.Controls.Add(logo);
            headerPanel.Controls.Add(title);
            headerPanel.Controls.Add(avatarBox);
            headerPanel.Controls.Add(userBadge);
            headerPanel.Controls.Add(logoutBtn);

            // ========= CONTENEDOR CON SCROLL PARA CENTRAR CARDS =========
            cardsContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                AutoScroll = true      // Permite scroll si las cards no caben
            };

            // FlowLayoutPanel que se autoajusta (sin tamaño forzado)
            flowCards = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                BackColor = Color.Transparent,
                Margin = new Padding(10)
            };
            cardsContainer.Controls.Add(flowCards);

            // ========= FOOTER ========= (sin cambios)
            footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 45,
                BackColor = Color.FromArgb(10, 61, 66)
            };
            footerLabel = new Label
            {
                Text = "Desarrollado por Ariel R. y Anthony V. | EurekaBank © 2025",
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9)
            };
            footerPanel.Controls.Add(footerLabel);

            overlay.Controls.Add(headerPanel);
            overlay.Controls.Add(cardsContainer);
            overlay.Controls.Add(footerPanel);

            // Cliente SOAP
            soap = new CoreBancarioSoapClient(
                CoreBancarioSoapClient.EndpointConfiguration.BasicHttpBinding_CoreBancarioSoap,
                "https://localhost:7299/CoreBancario.svc");

            // Crear tarjetas
            CargarTarjetas();

            // Centrar las cards cada vez que se redimensione el contenedor o cambie el contenido
            cardsContainer.Resize += (s, e) => CentrarCards();
            flowCards.ControlAdded += (s, e) => CentrarCards();
            flowCards.ControlRemoved += (s, e) => CentrarCards();
            CentrarCards(); // centrar al inicio

            this.Resize += (s, e) => ReubicarHeader();
            ReubicarHeader();
        }

        private void ReubicarHeader()
        {
            if (avatarBox != null)
            {
                avatarBox.Location = new Point(this.Width - 200, 17);
                var userBadge = headerPanel.Controls["userBadge"] as Label;
                if (userBadge != null) userBadge.Location = new Point(this.Width - 145, 30);
                logoutBtn.Location = new Point(this.Width - 110, 22);
            }
        }

        private void CentrarCards()
        {
            // No centramos si no hay cards
            if (flowCards.Controls.Count == 0) return;

            // Calculamos el ancho total preferido del flowLayout (según su contenido)
            int flowWidth = flowCards.PreferredSize.Width;
            int flowHeight = flowCards.PreferredSize.Height;

            // Ancho disponible en el contenedor (restando el scroll bar vertical si está visible)
            int containerWidth = cardsContainer.ClientSize.Width - (cardsContainer.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0);
            int containerHeight = cardsContainer.ClientSize.Height;

            // Centrar horizontalmente
            int x = (containerWidth - flowWidth) / 2;
            if (x < 0) x = 0;  // si no cabe, pegar a la izquierda (el scroll ayudará)

            // Centrar verticalmente
            int y = (containerHeight - flowHeight) / 2;
            if (y < 0) y = 0;

            flowCards.Location = new Point(x, y);
        }

        private void CargarTarjetas()
        {
            Image iconMov = CrearIconoTexto("📊");
            Image iconDep = CrearIconoTexto("💰");
            Image iconRet = CrearIconoTexto("💸");
            Image iconTra = CrearIconoTexto("🔄");

            cardMovimientos = new OperacionCard("Consulta de Movimientos", iconMov);
            cardDeposito = new OperacionCard("Depósito", iconDep);
            cardRetiro = new OperacionCard("Retiro", iconRet);
            cardTransferencia = new OperacionCard("Transferencia", iconTra);

            AsignarContenidoMovimientos(cardMovimientos);
            AsignarContenidoDeposito(cardDeposito);
            AsignarContenidoRetiro(cardRetiro);
            AsignarContenidoTransferencia(cardTransferencia);

            cardMovimientos.CardClicked += CardClickedHandler;
            cardDeposito.CardClicked += CardClickedHandler;
            cardRetiro.CardClicked += CardClickedHandler;
            cardTransferencia.CardClicked += CardClickedHandler;

            flowCards.Controls.Add(cardMovimientos);
            flowCards.Controls.Add(cardDeposito);
            flowCards.Controls.Add(cardRetiro);
            flowCards.Controls.Add(cardTransferencia);
        }

        private Image CrearIconoTexto(string emoji)
        {
            Bitmap bmp = new Bitmap(48, 48);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                using (Font f = new Font("Segoe UI Emoji", 32))
                {
                    g.DrawString(emoji, f, Brushes.White, new PointF(4, 4));
                }
            }
            return bmp;
        }

        private void CardClickedHandler(object sender, OperacionCard clickedCard)
        {
            foreach (OperacionCard card in flowCards.Controls)
            {
                if (card == clickedCard)
                {
                    if (card.Content.Height == 0)
                        card.Expand();
                    else
                        card.Collapse();
                }
                else
                {
                    card.Collapse();
                }
            }
            // Re-centrar después de expandir/colapsar (cambia el tamaño del flowLayout)
            CentrarCards();
        }

        // ========= MÉTODOS DE CONTENIDO (igual que antes, pero sin cambios) =========
        private void AsignarContenidoMovimientos(OperacionCard card)
        {
            Panel p = card.Content;
            p.Controls.Clear();
            Label lblCuenta = new Label { Text = "Número de cuenta:", Location = new Point(10, 10), AutoSize = true, Font = new Font("Segoe UI", 9) };
            TextBox txtCuenta = new TextBox { Location = new Point(10, 35), Width = 200, BorderStyle = BorderStyle.FixedSingle };
            Button btnVer = new Button
            {
                Text = "Ver movimientos",
                Location = new Point(10, 70),
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 180,
                Height = 30
            };
            btnVer.FlatAppearance.BorderSize = 0;
            btnVer.Click += async (s, e) => {
                string cuenta = txtCuenta.Text.Trim().PadLeft(8, '0');
                try
                {
                    var movs = await soap.TraerMovimientosAsync(cuenta);
                    FormMovimientos frm = new FormMovimientos(cuenta, movs);
                    frm.ShowDialog();
                }
                catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
            };
            p.Controls.Add(lblCuenta);
            p.Controls.Add(txtCuenta);
            p.Controls.Add(btnVer);
        }

        private void AsignarContenidoDeposito(OperacionCard card)
        {
            Panel p = card.Content;
            p.Controls.Clear();
            Label lblCuenta = new Label { Text = "Cuenta destino:", Location = new Point(10, 10), AutoSize = true, Font = new Font("Segoe UI", 9) };
            TextBox txtCuenta = new TextBox { Location = new Point(10, 35), Width = 200, BorderStyle = BorderStyle.FixedSingle };
            Label lblMonto = new Label { Text = "Importe (USD):", Location = new Point(10, 70), AutoSize = true, Font = new Font("Segoe UI", 9) };
            NumericUpDown nudMonto = new NumericUpDown { Location = new Point(10, 95), Width = 200, DecimalPlaces = 2, ThousandsSeparator = true };
            Button btnDep = new Button
            {
                Text = "Depositar",
                Location = new Point(10, 130),
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 180,
                Height = 30
            };
            btnDep.FlatAppearance.BorderSize = 0;
            btnDep.Click += async (s, e) => {
                string cuenta = txtCuenta.Text.Trim().PadLeft(8, '0');
                decimal monto = nudMonto.Value;
                var resp = await soap.RegDepositoAsync(cuenta, monto, null);
                MostrarResultado(resp.Estado == 1, resp.Saldo, cuenta, "Depósito");
                if (resp.Estado == 1) { txtCuenta.Clear(); nudMonto.Value = 0; }
            };
            p.Controls.Add(lblCuenta); p.Controls.Add(txtCuenta);
            p.Controls.Add(lblMonto); p.Controls.Add(nudMonto);
            p.Controls.Add(btnDep);
        }

        private void AsignarContenidoRetiro(OperacionCard card)
        {
            Panel p = card.Content;
            p.Controls.Clear();
            Label lblCuenta = new Label { Text = "Cuenta origen:", Location = new Point(10, 10), AutoSize = true, Font = new Font("Segoe UI", 9) };
            TextBox txtCuenta = new TextBox { Location = new Point(10, 35), Width = 200, BorderStyle = BorderStyle.FixedSingle };
            Label lblMonto = new Label { Text = "Importe (USD):", Location = new Point(10, 70), AutoSize = true, Font = new Font("Segoe UI", 9) };
            NumericUpDown nudMonto = new NumericUpDown { Location = new Point(10, 95), Width = 200, DecimalPlaces = 2 };
            Button btnRet = new Button
            {
                Text = "Retirar",
                Location = new Point(10, 130),
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 180,
                Height = 30
            };
            btnRet.FlatAppearance.BorderSize = 0;
            btnRet.Click += async (s, e) => {
                string cuenta = txtCuenta.Text.Trim().PadLeft(8, '0');
                decimal monto = nudMonto.Value;
                var resp = await soap.RegRetiroAsync(cuenta, monto, null);
                MostrarResultado(resp.Estado == 1, resp.Saldo, cuenta, "Retiro");
                if (resp.Estado == 1) { txtCuenta.Clear(); nudMonto.Value = 0; }
            };
            p.Controls.Add(lblCuenta); p.Controls.Add(txtCuenta);
            p.Controls.Add(lblMonto); p.Controls.Add(nudMonto);
            p.Controls.Add(btnRet);
        }

        private void AsignarContenidoTransferencia(OperacionCard card)
        {
            Panel p = card.Content;
            p.Controls.Clear();
            Label lblOrigen = new Label { Text = "Cuenta origen:", Location = new Point(10, 10), AutoSize = true, Font = new Font("Segoe UI", 9) };
            TextBox txtOrigen = new TextBox { Location = new Point(10, 35), Width = 200, BorderStyle = BorderStyle.FixedSingle };
            Label lblDestino = new Label { Text = "Cuenta destino:", Location = new Point(10, 70), AutoSize = true, Font = new Font("Segoe UI", 9) };
            TextBox txtDestino = new TextBox { Location = new Point(10, 95), Width = 200, BorderStyle = BorderStyle.FixedSingle };
            Label lblMonto = new Label { Text = "Importe (USD):", Location = new Point(10, 130), AutoSize = true, Font = new Font("Segoe UI", 9) };
            NumericUpDown nudMonto = new NumericUpDown { Location = new Point(10, 155), Width = 200, DecimalPlaces = 2 };
            Button btnTra = new Button
            {
                Text = "Transferir",
                Location = new Point(10, 190),
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 180,
                Height = 30
            };
            btnTra.FlatAppearance.BorderSize = 0;
            btnTra.Click += async (s, e) => {
                string origen = txtOrigen.Text.Trim().PadLeft(8, '0');
                string destino = txtDestino.Text.Trim().PadLeft(8, '0');
                decimal monto = nudMonto.Value;
                var resp = await soap.RegTransferenciaAsync(origen, destino, monto, null);
                MostrarResultado(resp.Estado == 1, resp.Saldo, origen, "Transferencia");
                if (resp.Estado == 1) { txtOrigen.Clear(); txtDestino.Clear(); nudMonto.Value = 0; }
            };
            p.Controls.Add(lblOrigen); p.Controls.Add(txtOrigen);
            p.Controls.Add(lblDestino); p.Controls.Add(txtDestino);
            p.Controls.Add(lblMonto); p.Controls.Add(nudMonto);
            p.Controls.Add(btnTra);
        }

        private void MostrarResultado(bool exito, decimal saldo, string cuenta, string operacion)
        {
            FormResultado frm = new FormResultado(exito, saldo, cuenta, operacion);
            frm.ShowDialog();
        }

        private void InitializeComponent() { }
    }
}