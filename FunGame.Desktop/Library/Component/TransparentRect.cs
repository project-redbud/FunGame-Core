using System.Drawing.Drawing2D;

namespace Milimoe.FunGame.Desktop.Library.Component
{
    partial class TransparentRect : GroupBox
    {
        public enum ShapeBorderStyles
        {
            ShapeBSNone,
            ShapeBSFixedSingle,
        };
        private ShapeBorderStyles _borderStyle = ShapeBorderStyles.ShapeBSNone;
        private Color _backColor = Color.Black;
        private Color _borderColor = Color.White;
        private int _radius = 20;
        private int _opacity = 125;
        protected int pointX = 0;
        protected int pointY = 0;
        protected Rectangle iRect = new Rectangle();
        public TransparentRect()
        {
            base.BackColor = Color.Transparent;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.Opaque, false);
            UpdateStyles();
        }
        public new Color BackColor
        {
            get { return _backColor; }
            set { _backColor = value; Invalidate(); }
        }
        public ShapeBorderStyles ShapeBorderStyle
        {
            get { return _borderStyle; }
            set { _borderStyle = value; this.Invalidate(); }
        }
        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; Invalidate(); }
        }
        public int Opacity
        {
            get { return _opacity; }
            set { _opacity = value; this.Invalidate(); }
        }
        public int Radius
        {
            get { return _radius; }
            set { _radius = value; this.Invalidate(); }
        }
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; this.Invalidate(); }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            SmoothingMode sm = e.Graphics.SmoothingMode;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (_borderStyle == ShapeBorderStyles.ShapeBSFixedSingle) DrawBorder(e.Graphics);
            DrawLabelBackground(e.Graphics);
            e.Graphics.SmoothingMode = sm;
        }
        private void DrawBorder(Graphics g)
        {
            Rectangle rect = this.ClientRectangle;
            rect.Width--;
            rect.Height--;
            using (GraphicsPath bp = GetPath(rect, _radius))
            {
                using (Pen p = new Pen(_borderColor))
                {
                    g.DrawPath(p, bp);
                }
            }
        }
        private void DrawLabelBackground(Graphics g)
        {
            Rectangle rect = this.ClientRectangle;
            iRect = rect;
            rect.X++;
            rect.Y++;
            rect.Width -= 2;
            rect.Height -= 2;
            using (GraphicsPath bb = GetPath(rect, _radius))
            {
                using (Brush br = new SolidBrush(Color.FromArgb(_opacity, _backColor)))
                {
                    g.FillPath(br, bb);
                }
            }
        }
        protected GraphicsPath GetPath(Rectangle rc, int r)
        {
            int x = rc.X, y = rc.Y, w = rc.Width, h = rc.Height;
            r = r << 1;
            GraphicsPath path = new GraphicsPath();
            if (r > 0)
            {
                if (r > h) { r = h; };
                if (r > w) { r = w; };
                path.AddArc(x, y, r, r, 180, 90);
                path.AddArc(x + w - r, y, r, r, 270, 90);
                path.AddArc(x + w - r, y + h - r, r, r, 0, 90);
                path.AddArc(x, y + h - r, r, r, 90, 90);
                path.CloseFigure();
            }
            else
            {
                path.AddRectangle(rc);
            }
            return path;
        }
    }
}