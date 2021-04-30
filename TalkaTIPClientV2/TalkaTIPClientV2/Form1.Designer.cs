namespace TalkaTIPClientV2
{
    partial class game
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(game));
            this.button1 = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.Play = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.Player1 = new System.Windows.Forms.Label();
            this.Player2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.Xsc = new System.Windows.Forms.Label();
            this.Csc = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.youwith = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.Cplayer = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.DarkRed;
            this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button1.Font = new System.Drawing.Font("Constantia", 10F, System.Drawing.FontStyle.Bold);
            this.button1.ForeColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.button1.Location = new System.Drawing.Point(0, 486);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(601, 29);
            this.button1.TabIndex = 0;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.BackColor = System.Drawing.Color.LightBlue;
            this.checkedListBox1.ForeColor = System.Drawing.Color.Black;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(299, 45);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(300, 196);
            this.checkedListBox1.TabIndex = 3;
            //this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
            this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
            // 
            // Play
            // 
            this.Play.AutoSize = true;
            this.Play.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Play.Enabled = false;
            this.Play.Font = new System.Drawing.Font("Constantia", 10F, System.Drawing.FontStyle.Bold);
            this.Play.Location = new System.Drawing.Point(0, 459);
            this.Play.Margin = new System.Windows.Forms.Padding(10);
            this.Play.Name = "Play";
            this.Play.Size = new System.Drawing.Size(601, 27);
            this.Play.TabIndex = 9;
            this.Play.Text = "Play";
            this.Play.UseVisualStyleBackColor = true;
            this.Play.Click += new System.EventHandler(this.Play_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Constantia", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label4.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.label4.Location = new System.Drawing.Point(305, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(142, 29);
            this.label4.TabIndex = 10;
            this.label4.Text = "Connected:";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // Player1
            // 
            this.Player1.BackColor = System.Drawing.Color.Transparent;
            this.Player1.Font = new System.Drawing.Font("Constantia", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Player1.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.Player1.Location = new System.Drawing.Point(149, 258);
            this.Player1.Name = "Player1";
            this.Player1.Size = new System.Drawing.Size(204, 41);
            this.Player1.TabIndex = 12;
            this.Player1.Text = "N/A";
            this.Player1.Click += new System.EventHandler(this.Player1_Click);
            // 
            // Player2
            // 
            this.Player2.AutoSize = true;
            this.Player2.BackColor = System.Drawing.Color.Transparent;
            this.Player2.Font = new System.Drawing.Font("Constantia", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Player2.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.Player2.Location = new System.Drawing.Point(150, 313);
            this.Player2.Name = "Player2";
            this.Player2.Size = new System.Drawing.Size(57, 29);
            this.Player2.TabIndex = 14;
            this.Player2.Text = "N/A";
            this.Player2.Click += new System.EventHandler(this.Player2_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Comic Sans MS", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label6.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.label6.Location = new System.Drawing.Point(428, 361);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(134, 35);
            this.label6.TabIndex = 15;
            this.label6.Text = "X-Score: ";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Comic Sans MS", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label8.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.label8.Location = new System.Drawing.Point(391, 390);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(183, 35);
            this.label8.TabIndex = 16;
            this.label8.Text = "Circle-Score: ";
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // Xsc
            // 
            this.Xsc.AutoSize = true;
            this.Xsc.BackColor = System.Drawing.Color.Transparent;
            this.Xsc.Font = new System.Drawing.Font("Constantia", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Xsc.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.Xsc.Location = new System.Drawing.Point(555, 364);
            this.Xsc.Name = "Xsc";
            this.Xsc.Size = new System.Drawing.Size(27, 29);
            this.Xsc.TabIndex = 17;
            this.Xsc.Text = "0";
            // 
            // Csc
            // 
            this.Csc.AutoSize = true;
            this.Csc.BackColor = System.Drawing.Color.Transparent;
            this.Csc.Font = new System.Drawing.Font("Constantia", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Csc.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.Csc.Location = new System.Drawing.Point(556, 394);
            this.Csc.Name = "Csc";
            this.Csc.Size = new System.Drawing.Size(27, 29);
            this.Csc.TabIndex = 18;
            this.Csc.Text = "0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Font = new System.Drawing.Font("Constantia", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label11.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.label11.Location = new System.Drawing.Point(30, 361);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(157, 29);
            this.label11.TabIndex = 19;
            this.label11.Text = "You are with:";
            this.label11.Click += new System.EventHandler(this.label11_Click);
            // 
            // youwith
            // 
            this.youwith.AutoSize = true;
            this.youwith.BackColor = System.Drawing.Color.Transparent;
            this.youwith.Font = new System.Drawing.Font("Constantia", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.youwith.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.youwith.Location = new System.Drawing.Point(183, 360);
            this.youwith.Name = "youwith";
            this.youwith.Size = new System.Drawing.Size(57, 29);
            this.youwith.TabIndex = 20;
            this.youwith.Text = "N/A";
            this.youwith.Click += new System.EventHandler(this.youwith_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.Font = new System.Drawing.Font("Constantia", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label12.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.label12.Location = new System.Drawing.Point(4, 395);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(183, 29);
            this.label12.TabIndex = 21;
            this.label12.Text = "Current Player:";
            this.label12.Click += new System.EventHandler(this.label12_Click);
            // 
            // Cplayer
            // 
            this.Cplayer.AutoSize = true;
            this.Cplayer.BackColor = System.Drawing.Color.Transparent;
            this.Cplayer.Font = new System.Drawing.Font("Constantia", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Cplayer.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.Cplayer.Location = new System.Drawing.Point(183, 395);
            this.Cplayer.Name = "Cplayer";
            this.Cplayer.Size = new System.Drawing.Size(57, 29);
            this.Cplayer.TabIndex = 22;
            this.Cplayer.Text = "N/A";
            this.Cplayer.Click += new System.EventHandler(this.Cplayer_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.Image = global::TalkaTIPClientV2.Properties.Resources.player2;
            this.pictureBox2.Location = new System.Drawing.Point(-1, 303);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(144, 50);
            this.pictureBox2.TabIndex = 24;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.pictureBox1.Image = global::TalkaTIPClientV2.Properties.Resources.player1;
            this.pictureBox1.Location = new System.Drawing.Point(-1, 247);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(144, 52);
            this.pictureBox1.TabIndex = 23;
            this.pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panel1.BackgroundImage = global::TalkaTIPClientV2.Properties.Resources.panelback;
            this.panel1.Location = new System.Drawing.Point(0, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(300, 240);
            this.panel1.TabIndex = 2;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseClick);
            // 
            // game
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Snow;
            this.BackgroundImage = global::TalkaTIPClientV2.Properties.Resources.bground;
            this.ClientSize = new System.Drawing.Size(601, 515);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Cplayer);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.youwith);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.Csc);
            this.Controls.Add(this.Xsc);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.Player2);
            this.Controls.Add(this.Player1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Play);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button1);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Font = new System.Drawing.Font("Constantia", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "game";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TicTacToe";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.game_FormClosing);
            this.Load += new System.EventHandler(this.game_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Button Play;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label Player1;
        private System.Windows.Forms.Label Player2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label Xsc;
        private System.Windows.Forms.Label Csc;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label youwith;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label Cplayer;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}

