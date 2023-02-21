using System.ComponentModel;

namespace Milimoe.FunGame.Desktop.Library.Component
{
    public partial class MinButton : Button
    {
        public GeneralForm? RelativeForm { get; set; }

        public MinButton()
        {
            InitializeComponent();
            Anchor = System.Windows.Forms.AnchorStyles.None;
            BackColor = System.Drawing.Color.White;
            BackgroundImage = global::Milimoe.FunGame.Desktop.Properties.Resources.min;
            FlatAppearance.BorderColor = System.Drawing.Color.White;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Font = new System.Drawing.Font("LanaPixel", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ForeColor = System.Drawing.Color.Black;
            Location = new System.Drawing.Point(750, 3);
            Size = new System.Drawing.Size(47, 47);
            TextAlign = System.Drawing.ContentAlignment.TopLeft;
            UseVisualStyleBackColor = false;

            Click += MinForm_Click;
        }

        public MinButton(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            Anchor = System.Windows.Forms.AnchorStyles.None;
            BackColor = System.Drawing.Color.White;
            BackgroundImage = global::Milimoe.FunGame.Desktop.Properties.Resources.min;
            FlatAppearance.BorderColor = System.Drawing.Color.White;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Font = new System.Drawing.Font("LanaPixel", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ForeColor = System.Drawing.Color.Red;
            Location = new System.Drawing.Point(750, 3);
            Size = new System.Drawing.Size(47, 47);
            TextAlign = System.Drawing.ContentAlignment.TopLeft;
            UseVisualStyleBackColor = false;

            Click += MinForm_Click;
        }

        /// <summary>
        /// 自带的最小化窗口
        /// 绑定RelativeForm才能生效
        /// </summary>
        /// <param name="sender">object?</param>
        /// <param name="e">EventArgs</param>
        private void MinForm_Click(object? sender, EventArgs e)
        {
            if (RelativeForm != null)
            {
                RelativeForm.WindowState = FormWindowState.Minimized;
            }
        }
    }
}
