using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CLIESC_EUREKABANK_SOAP_GR08
{
    public class RoundedPanel : Panel
    {
        public int CornerRadius { get; set; } = 20;
        public Color BorderColor { get; set; } = Color.Transparent;
        public float BorderWidth { get; set; } = 0;
        public bool ApplyShadow { get; set; } = true;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            var path = GetRoundedRectangle(rect, CornerRadius);

            using (var brush = new SolidBrush(BackColor))
                e.Graphics.FillPath(brush, path);

            if (BorderWidth > 0 && BorderColor != Color.Transparent)
                using (var pen = new Pen(BorderColor, BorderWidth))
                    e.Graphics.DrawPath(pen, path);
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}