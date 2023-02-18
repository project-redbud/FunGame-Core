namespace Milimoe.FunGame.Desktop.Library.Component
{
    partial class ShowMessage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowMessage));
            this.MsgText = new System.Windows.Forms.Label();
            this.LeftButton = new System.Windows.Forms.Button();
            this.Exit = new FunGame.Desktop.Library.Component.ExitButton(this.components);
            this.RightButton = new System.Windows.Forms.Button();
            this.MidButton = new System.Windows.Forms.Button();
            this.TransparentRect = new FunGame.Desktop.Library.Component.TransparentRect();
            this.InputButton = new System.Windows.Forms.Button();
            this.InputText = new System.Windows.Forms.TextBox();
            this.TransparentRect.SuspendLayout();
            this.SuspendLayout();
            // 
            // MsgText
            // 
            this.MsgText.BackColor = System.Drawing.Color.Transparent;
            this.MsgText.Font = new System.Drawing.Font("LanaPixel", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.MsgText.Location = new System.Drawing.Point(2, 51);
            this.MsgText.Name = "MsgText";
            this.MsgText.Size = new System.Drawing.Size(232, 73);
            this.MsgText.TabIndex = 100;
            this.MsgText.Text = "Message";
            this.MsgText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LeftButton
            // 
            this.LeftButton.Font = new System.Drawing.Font("LanaPixel", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.LeftButton.Location = new System.Drawing.Point(13, 127);
            this.LeftButton.Name = "LeftButton";
            this.LeftButton.Size = new System.Drawing.Size(98, 37);
            this.LeftButton.TabIndex = 1;
            this.LeftButton.Text = "Left";
            this.LeftButton.UseVisualStyleBackColor = true;
            this.LeftButton.Click += new System.EventHandler(this.LeftButton_Click);
            // 
            // Exit
            // 
            this.Exit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Exit.BackColor = System.Drawing.Color.White;
            this.Exit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Exit.BackgroundImage")));
            this.Exit.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.Exit.FlatAppearance.BorderSize = 0;
            this.Exit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.Exit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Exit.Font = new System.Drawing.Font("LanaPixel", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Exit.ForeColor = System.Drawing.Color.Red;
            this.Exit.Location = new System.Drawing.Point(187, 1);
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(47, 47);
            this.Exit.TabIndex = 3;
            this.Exit.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.Exit.UseVisualStyleBackColor = false;
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // RightButton
            // 
            this.RightButton.Font = new System.Drawing.Font("LanaPixel", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.RightButton.Location = new System.Drawing.Point(125, 127);
            this.RightButton.Name = "RightButton";
            this.RightButton.Size = new System.Drawing.Size(98, 37);
            this.RightButton.TabIndex = 2;
            this.RightButton.Text = "Right";
            this.RightButton.UseVisualStyleBackColor = true;
            this.RightButton.Click += new System.EventHandler(this.RightButton_Click);
            // 
            // MidButton
            // 
            this.MidButton.Font = new System.Drawing.Font("LanaPixel", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.MidButton.Location = new System.Drawing.Point(65, 127);
            this.MidButton.Name = "MidButton";
            this.MidButton.Size = new System.Drawing.Size(98, 37);
            this.MidButton.TabIndex = 1;
            this.MidButton.Text = "Middle";
            this.MidButton.UseVisualStyleBackColor = true;
            this.MidButton.Click += new System.EventHandler(this.MidButton_Click);
            // 
            // Title
            // 
            this.Title.BackColor = System.Drawing.Color.Transparent;
            this.Title.Font = new System.Drawing.Font("LanaPixel", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Title.Location = new System.Drawing.Point(2, 1);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(179, 47);
            this.Title.TabIndex = 97;
            this.Title.Text = "Message";
            this.Title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TransparentRect
            // 
            this.TransparentRect.BackColor = System.Drawing.Color.WhiteSmoke;
            this.TransparentRect.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.TransparentRect.Controls.Add(this.InputButton);
            this.TransparentRect.Controls.Add(this.InputText);
            this.TransparentRect.Controls.Add(this.Title);
            this.TransparentRect.Controls.Add(this.MidButton);
            this.TransparentRect.Controls.Add(this.RightButton);
            this.TransparentRect.Controls.Add(this.Exit);
            this.TransparentRect.Controls.Add(this.LeftButton);
            this.TransparentRect.Controls.Add(this.MsgText);
            this.TransparentRect.Location = new System.Drawing.Point(0, 0);
            this.TransparentRect.Name = "TransparentRect";
            this.TransparentRect.Opacity = 125;
            this.TransparentRect.Radius = 20;
            this.TransparentRect.ShapeBorderStyle = FunGame.Desktop.Library.Component.TransparentRect.ShapeBorderStyles.ShapeBSNone;
            this.TransparentRect.Size = new System.Drawing.Size(235, 170);
            this.TransparentRect.TabIndex = 103;
            this.TransparentRect.TabStop = false;
            // 
            // InputButton
            // 
            this.InputButton.Font = new System.Drawing.Font("LanaPixel", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.InputButton.Location = new System.Drawing.Point(168, 130);
            this.InputButton.Name = "InputButton";
            this.InputButton.Size = new System.Drawing.Size(66, 34);
            this.InputButton.TabIndex = 2;
            this.InputButton.Text = "OK";
            this.InputButton.UseVisualStyleBackColor = true;
            this.InputButton.Visible = false;
            this.InputButton.Click += new System.EventHandler(this.InputButton_Click);
            // 
            // InputText
            // 
            this.InputText.Font = new System.Drawing.Font("LanaPixel", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.InputText.Location = new System.Drawing.Point(2, 130);
            this.InputText.MaxLength = 21;
            this.InputText.Name = "InputText";
            this.InputText.Size = new System.Drawing.Size(163, 34);
            this.InputText.TabIndex = 1;
            this.InputText.Visible = false;
            this.InputText.WordWrap = false;
            this.InputText.KeyUp += new KeyEventHandler(this.InputText_KeyUp);
            // 
            // ShowMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(235, 170);
            this.Controls.Add(this.TransparentRect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ShowMessage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Show";
            this.TransparentRect.ResumeLayout(false);
            this.TransparentRect.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Label MsgText;
        private Button LeftButton;
        private ExitButton Exit;
        private Button RightButton;
        private Button MidButton;
        private TransparentRect TransparentRect;
        private Button InputButton;
        private TextBox InputText;
    }
}