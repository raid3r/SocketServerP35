namespace SocketChatGuiClient
{
    partial class LoginForm
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
            tbxLogin = new TextBox();
            tbxPassword = new TextBox();
            btnLogin = new Button();
            SuspendLayout();
            // 
            // tbxLogin
            // 
            tbxLogin.Location = new Point(23, 52);
            tbxLogin.Name = "tbxLogin";
            tbxLogin.Size = new Size(229, 23);
            tbxLogin.TabIndex = 0;
            // 
            // tbxPassword
            // 
            tbxPassword.Location = new Point(24, 92);
            tbxPassword.Name = "tbxPassword";
            tbxPassword.Size = new Size(228, 23);
            tbxPassword.TabIndex = 1;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(26, 145);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(75, 23);
            btnLogin.TabIndex = 2;
            btnLogin.Text = "Увійти";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(264, 357);
            Controls.Add(btnLogin);
            Controls.Add(tbxPassword);
            Controls.Add(tbxLogin);
            Name = "LoginForm";
            Text = "LoginForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tbxLogin;
        private TextBox tbxPassword;
        private Button btnLogin;
    }
}