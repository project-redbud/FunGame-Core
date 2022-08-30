using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunGame.Desktop.Models.Component
{
    public partial class ExitButton : Button
    {
        public ExitButton()
        {
            InitializeComponent();
            Anchor = System.Windows.Forms.AnchorStyles.None;
            BackColor = System.Drawing.Color.White;
            BackgroundImage = global::FunGame.Desktop.Properties.Resources.exit;
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
        }

        public ExitButton(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            Anchor = System.Windows.Forms.AnchorStyles.None;
            BackColor = System.Drawing.Color.White;
            BackgroundImage = global::FunGame.Desktop.Properties.Resources.exit;
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
        }
    }
}
