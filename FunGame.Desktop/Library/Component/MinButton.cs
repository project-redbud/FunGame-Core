using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Desktop.Library.Component
{
    public partial class MinButton : Button
    {
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
        }
    }
}
