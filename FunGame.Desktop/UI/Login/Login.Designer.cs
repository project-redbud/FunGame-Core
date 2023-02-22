namespace Milimoe.FunGame.Desktop.UI
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.ExitButton = new Milimoe.FunGame.Desktop.Library.Component.ExitButton(this.components);
            this.MinButton = new Milimoe.FunGame.Desktop.Library.Component.MinButton(this.components);
            this.Username = new System.Windows.Forms.Label();
            this.Password = new System.Windows.Forms.Label();
            this.UsernameText = new System.Windows.Forms.TextBox();
            this.PasswordText = new System.Windows.Forms.TextBox();
            this.RegButton = new System.Windows.Forms.Button();
            this.GoToLogin = new System.Windows.Forms.Button();
            this.ForgetPassword = new System.Windows.Forms.Button();
            this.FastLogin = new System.Windows.Forms.Button();
            this.TransparentRect = new Milimoe.FunGame.Desktop.Library.Component.TransparentRect();
            this.TransparentRect.SuspendLayout();
            this.SuspendLayout();
            // 
            // Title
            // 
            this.Title.Font = new System.Drawing.Font("LanaPixel", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Title.Location = new System.Drawing.Point(7, 6);
            this.Title.Size = new System.Drawing.Size(387, 47);
            this.Title.TabIndex = 8;
            this.Title.Text = "Welcome to FunGame!";
            this.Title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ExitButton
            // 
            this.ExitButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ExitButton.BackColor = System.Drawing.Color.White;
            this.ExitButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ExitButton.BackgroundImage")));
            this.ExitButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.ExitButton.FlatAppearance.BorderSize = 0;
            this.ExitButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.ExitButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ExitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExitButton.Font = new System.Drawing.Font("LanaPixel", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ExitButton.ForeColor = System.Drawing.Color.Red;
            this.ExitButton.Location = new System.Drawing.Point(451, 4);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.RelativeForm = this;
            this.ExitButton.Size = new System.Drawing.Size(47, 47);
            this.ExitButton.TabIndex = 7;
            this.ExitButton.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.ExitButton.UseVisualStyleBackColor = false;
            // 
            // MinButton
            // 
            this.MinButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.MinButton.BackColor = System.Drawing.Color.White;
            this.MinButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("MinButton.BackgroundImage")));
            this.MinButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.MinButton.FlatAppearance.BorderSize = 0;
            this.MinButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.MinButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.MinButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MinButton.Font = new System.Drawing.Font("LanaPixel", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.MinButton.ForeColor = System.Drawing.Color.Black;
            this.MinButton.Location = new System.Drawing.Point(398, 4);
            this.MinButton.Name = "MinButton";
            this.MinButton.RelativeForm = this;
            this.MinButton.Size = new System.Drawing.Size(47, 47);
            this.MinButton.TabIndex = 6;
            this.MinButton.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.MinButton.UseVisualStyleBackColor = false;
            // 
            // Username
            // 
            this.Username.Font = new System.Drawing.Font("LanaPixel", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Username.Location = new System.Drawing.Point(56, 111);
            this.Username.Name = "Username";
            this.Username.Size = new System.Drawing.Size(75, 33);
            this.Username.TabIndex = 9;
            this.Username.Text = "账号";
            this.Username.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Password
            // 
            this.Password.Font = new System.Drawing.Font("LanaPixel", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Password.Location = new System.Drawing.Point(56, 144);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(75, 33);
            this.Password.TabIndex = 10;
            this.Password.Text = "密码";
            this.Password.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UsernameText
            // 
            this.UsernameText.Font = new System.Drawing.Font("LanaPixel", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.UsernameText.Location = new System.Drawing.Point(143, 114);
            this.UsernameText.Name = "UsernameText";
            this.UsernameText.Size = new System.Drawing.Size(216, 29);
            this.UsernameText.TabIndex = 0;
            // 
            // PasswordText
            // 
            this.PasswordText.Font = new System.Drawing.Font("LanaPixel", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.PasswordText.Location = new System.Drawing.Point(143, 148);
            this.PasswordText.Name = "PasswordText";
            this.PasswordText.PasswordChar = '*';
            this.PasswordText.Size = new System.Drawing.Size(216, 29);
            this.PasswordText.TabIndex = 1;
            // 
            // RegButton
            // 
            this.RegButton.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.RegButton.Location = new System.Drawing.Point(365, 113);
            this.RegButton.Name = "RegButton";
            this.RegButton.Size = new System.Drawing.Size(81, 33);
            this.RegButton.TabIndex = 4;
            this.RegButton.Text = "立即注册";
            this.RegButton.UseVisualStyleBackColor = true;
            this.RegButton.Click += new System.EventHandler(this.RegButton_Click);
            // 
            // GoToLogin
            // 
            this.GoToLogin.Font = new System.Drawing.Font("LanaPixel", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GoToLogin.Location = new System.Drawing.Point(277, 216);
            this.GoToLogin.Name = "GoToLogin";
            this.GoToLogin.Size = new System.Drawing.Size(128, 42);
            this.GoToLogin.TabIndex = 2;
            this.GoToLogin.Text = "账号登录";
            this.GoToLogin.UseVisualStyleBackColor = true;
            this.GoToLogin.Click += new System.EventHandler(this.GoToLogin_Click);
            // 
            // ForgetPassword
            // 
            this.ForgetPassword.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForgetPassword.Location = new System.Drawing.Point(365, 147);
            this.ForgetPassword.Name = "ForgetPassword";
            this.ForgetPassword.Size = new System.Drawing.Size(81, 32);
            this.ForgetPassword.TabIndex = 5;
            this.ForgetPassword.Text = "找回密码";
            this.ForgetPassword.UseVisualStyleBackColor = true;
            this.ForgetPassword.Click += new System.EventHandler(this.ForgetPassword_Click);
            // 
            // FastLogin
            // 
            this.FastLogin.Font = new System.Drawing.Font("LanaPixel", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FastLogin.Location = new System.Drawing.Point(114, 216);
            this.FastLogin.Name = "FastLogin";
            this.FastLogin.Size = new System.Drawing.Size(130, 42);
            this.FastLogin.TabIndex = 3;
            this.FastLogin.Text = "快捷登录";
            this.FastLogin.UseVisualStyleBackColor = true;
            this.FastLogin.Click += new System.EventHandler(this.FastLogin_Click);
            // 
            // TransparentRect
            // 
            this.TransparentRect.BackColor = System.Drawing.Color.WhiteSmoke;
            this.TransparentRect.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.TransparentRect.Controls.Add(this.Title);
            this.TransparentRect.Controls.Add(this.MinButton);
            this.TransparentRect.Controls.Add(this.ExitButton);
            this.TransparentRect.Controls.Add(this.FastLogin);
            this.TransparentRect.Controls.Add(this.UsernameText);
            this.TransparentRect.Controls.Add(this.ForgetPassword);
            this.TransparentRect.Controls.Add(this.Username);
            this.TransparentRect.Controls.Add(this.GoToLogin);
            this.TransparentRect.Controls.Add(this.Password);
            this.TransparentRect.Controls.Add(this.RegButton);
            this.TransparentRect.Controls.Add(this.PasswordText);
            this.TransparentRect.Location = new System.Drawing.Point(0, 0);
            this.TransparentRect.Name = "TransparentRect";
            this.TransparentRect.Opacity = 125;
            this.TransparentRect.Radius = 20;
            this.TransparentRect.ShapeBorderStyle = Milimoe.FunGame.Desktop.Library.Component.TransparentRect.ShapeBorderStyles.ShapeBSNone;
            this.TransparentRect.Size = new System.Drawing.Size(503, 289);
            this.TransparentRect.TabIndex = 11;
            this.TransparentRect.TabStop = false;
            this.TransparentRect.Controls.SetChildIndex(this.PasswordText, 0);
            this.TransparentRect.Controls.SetChildIndex(this.RegButton, 0);
            this.TransparentRect.Controls.SetChildIndex(this.Password, 0);
            this.TransparentRect.Controls.SetChildIndex(this.GoToLogin, 0);
            this.TransparentRect.Controls.SetChildIndex(this.Username, 0);
            this.TransparentRect.Controls.SetChildIndex(this.ForgetPassword, 0);
            this.TransparentRect.Controls.SetChildIndex(this.UsernameText, 0);
            this.TransparentRect.Controls.SetChildIndex(this.FastLogin, 0);
            this.TransparentRect.Controls.SetChildIndex(this.ExitButton, 0);
            this.TransparentRect.Controls.SetChildIndex(this.MinButton, 0);
            this.TransparentRect.Controls.SetChildIndex(this.Title, 0);
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(503, 289);
            this.Controls.Add(this.TransparentRect);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Login";
            this.Opacity = 0.9D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.TransparentRect.ResumeLayout(false);
            this.TransparentRect.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Library.Component.ExitButton ExitButton;
        private Library.Component.MinButton MinButton;
        private Label Username;
        private Label Password;
        private TextBox UsernameText;
        private TextBox PasswordText;
        private Button RegButton;
        private Button GoToLogin;
        private Button ForgetPassword;
        private Button FastLogin;
        private Library.Component.TransparentRect TransparentRect;
    }
}