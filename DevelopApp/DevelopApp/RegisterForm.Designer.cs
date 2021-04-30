namespace DevelopApp
{
    partial class RegisterForm
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
            this.RegisterButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RepeatPasswordText = new System.Windows.Forms.TextBox();
            this.PasswordText = new System.Windows.Forms.TextBox();
            this.LoginText = new System.Windows.Forms.TextBox();
            this.serverIPText = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // RegisterButton
            // 
            this.RegisterButton.Location = new System.Drawing.Point(169, 234);
            this.RegisterButton.Name = "RegisterButton";
            this.RegisterButton.Size = new System.Drawing.Size(75, 23);
            this.RegisterButton.TabIndex = 2;
            this.RegisterButton.Text = "Register";
            this.RegisterButton.UseVisualStyleBackColor = true;
            this.RegisterButton.Click += new System.EventHandler(this.RegisterButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.serverIPText);
            this.groupBox1.Controls.Add(this.RepeatPasswordText);
            this.groupBox1.Controls.Add(this.PasswordText);
            this.groupBox1.Controls.Add(this.LoginText);
            this.groupBox1.Location = new System.Drawing.Point(163, 31);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 174);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Register";
            // 
            // RepeatPasswordText
            // 
            this.RepeatPasswordText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.RepeatPasswordText.ForeColor = System.Drawing.Color.Gray;
            this.RepeatPasswordText.Location = new System.Drawing.Point(6, 96);
            this.RepeatPasswordText.Name = "RepeatPasswordText";
            this.RepeatPasswordText.Size = new System.Drawing.Size(187, 24);
            this.RepeatPasswordText.TabIndex = 2;
            this.RepeatPasswordText.Text = "Password";
            this.RepeatPasswordText.UseSystemPasswordChar = true;
            // 
            // PasswordText
            // 
            this.PasswordText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.PasswordText.ForeColor = System.Drawing.Color.Gray;
            this.PasswordText.Location = new System.Drawing.Point(7, 57);
            this.PasswordText.Name = "PasswordText";
            this.PasswordText.Size = new System.Drawing.Size(187, 24);
            this.PasswordText.TabIndex = 1;
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
            this.LoginText.TabIndex = 0;
            this.LoginText.Text = "Login";
            // 
            // serverIPText
            // 
            this.serverIPText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.serverIPText.ForeColor = System.Drawing.Color.Gray;
            this.serverIPText.Location = new System.Drawing.Point(6, 136);
            this.serverIPText.Name = "serverIPText";
            this.serverIPText.Size = new System.Drawing.Size(187, 24);
            this.serverIPText.TabIndex = 3;
            this.serverIPText.Text = "Server IP";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(281, 234);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Login";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // RegisterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 311);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.RegisterButton);
            this.Name = "RegisterForm";
            this.Text = "Register";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button RegisterButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox RepeatPasswordText;
        private System.Windows.Forms.TextBox PasswordText;
        private System.Windows.Forms.TextBox LoginText;
        private System.Windows.Forms.TextBox serverIPText;
        private System.Windows.Forms.Button button1;
    }
}