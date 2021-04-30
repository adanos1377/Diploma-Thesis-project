namespace DevelopApp
{
    partial class LogonForm
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
            this.LogInButton = new System.Windows.Forms.Button();
            this.RegisterButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.PasswordText = new System.Windows.Forms.TextBox();
            this.LoginText = new System.Windows.Forms.TextBox();
            this.serverIPText = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LogInButton
            // 
            this.LogInButton.Location = new System.Drawing.Point(155, 233);
            this.LogInButton.Name = "LogInButton";
            this.LogInButton.Size = new System.Drawing.Size(75, 23);
            this.LogInButton.TabIndex = 2;
            this.LogInButton.Text = "Log in";
            this.LogInButton.UseVisualStyleBackColor = true;
            this.LogInButton.Click += new System.EventHandler(this.LogInButton_Click);
            // 
            // RegisterButton
            // 
            this.RegisterButton.Location = new System.Drawing.Point(280, 233);
            this.RegisterButton.Name = "RegisterButton";
            this.RegisterButton.Size = new System.Drawing.Size(75, 23);
            this.RegisterButton.TabIndex = 3;
            this.RegisterButton.Text = "Register";
            this.RegisterButton.UseVisualStyleBackColor = true;
            this.RegisterButton.Click += new System.EventHandler(this.RegisterButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.serverIPText);
            this.groupBox1.Controls.Add(this.PasswordText);
            this.groupBox1.Controls.Add(this.LoginText);
            this.groupBox1.Location = new System.Drawing.Point(155, 64);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 133);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log in";
            // 
            // PasswordText
            // 
            this.PasswordText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.PasswordText.ForeColor = System.Drawing.Color.Gray;
            this.PasswordText.Location = new System.Drawing.Point(7, 61);
            this.PasswordText.Name = "PasswordText";
            this.PasswordText.Size = new System.Drawing.Size(187, 24);
            this.PasswordText.TabIndex = 3;
            this.PasswordText.Text = "Password";
            this.PasswordText.UseSystemPasswordChar = true;
            // 
            // LoginText
            // 
            this.LoginText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.LoginText.ForeColor = System.Drawing.Color.Gray;
            this.LoginText.Location = new System.Drawing.Point(7, 20);
            this.LoginText.Name = "LoginText";
            this.LoginText.Size = new System.Drawing.Size(187, 24);
            this.LoginText.TabIndex = 2;
            this.LoginText.Text = "Login";
            // 
            // serverIPText
            // 
            this.serverIPText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.serverIPText.ForeColor = System.Drawing.Color.Gray;
            this.serverIPText.Location = new System.Drawing.Point(6, 98);
            this.serverIPText.Name = "serverIPText";
            this.serverIPText.Size = new System.Drawing.Size(187, 24);
            this.serverIPText.TabIndex = 4;
            this.serverIPText.Text = "Server IP";
            // 
            // LogonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 324);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.RegisterButton);
            this.Controls.Add(this.LogInButton);
            this.Name = "LogonForm";
            this.Text = "LogonForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button LogInButton;
        private System.Windows.Forms.Button RegisterButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox PasswordText;
        private System.Windows.Forms.TextBox LoginText;
        private System.Windows.Forms.TextBox serverIPText;
    }
}