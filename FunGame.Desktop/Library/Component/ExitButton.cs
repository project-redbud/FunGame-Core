using System.ComponentModel;

namespace Milimoe.FunGame.Desktop.Library.Component
{
    public partial class ExitButton : Button
    {
        public GeneralForm? RelativeForm { get; set; }

        public ExitButton()
        {
            InitializeComponent();
            Anchor = System.Windows.Forms.AnchorStyles.None;
            BackColor = System.Drawing.Color.White;
            BackgroundImage = global::Milimoe.FunGame.Desktop.Properties.Resources.exit;
            FlatAppearance.BorderColor = System.Drawing.Color.White;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Font = new System.Drawing.Font("LanaPixel", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ForeColor = System.Drawing.Color.Red;
            Location = new System.Drawing.Point(750, 3);
            Size = new System.Drawing.Size(47, 47);
            TextAlign = System.Drawing.ContentAlignment.TopLeft;
            UseVisualStyleBackColor = false;

            Click += ExitButton_Click;
        }

        public ExitButton(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            Anchor = System.Windows.Forms.AnchorStyles.None;
            BackColor = System.Drawing.Color.White;
            BackgroundImage = global::Milimoe.FunGame.Desktop.Properties.Resources.exit;
            FlatAppearance.BorderColor = System.Drawing.Color.White;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Font = new System.Drawing.Font("LanaPixel", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ForeColor = System.Drawing.Color.Red;
            Location = new System.Drawing.Point(750, 3);
            Size = new System.Drawing.Size(47, 47);
            TextAlign = System.Drawing.ContentAlignment.TopLeft;
            UseVisualStyleBackColor = false;

            Click += ExitButton_Click;
        }

        /// <summary>
        /// 自带的关闭按钮，可以重写
        /// 绑定RelativeForm才能生效
        /// </summary>
        /// <param name="sender">object?</param>
        /// <param name="e">EventArgs</param>
        protected virtual void ExitButton_Click(object? sender, EventArgs e)
        {
            RelativeForm?.Dispose();
        }
    }
}
