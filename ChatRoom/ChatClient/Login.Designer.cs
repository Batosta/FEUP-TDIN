namespace ChatClient
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
            this.login_button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.password_label = new System.Windows.Forms.Label();
            this.username_label = new System.Windows.Forms.Label();
            this.password_box = new System.Windows.Forms.TextBox();
            this.username_box = new System.Windows.Forms.TextBox();
            this.register_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // login_button
            // 
            this.login_button.Location = new System.Drawing.Point(161, 157);
            this.login_button.Name = "login_button";
            this.login_button.Size = new System.Drawing.Size(75, 23);
            this.login_button.TabIndex = 11;
            this.login_button.Text = "Login";
            this.login_button.UseVisualStyleBackColor = true;
            this.login_button.Click += new System.EventHandler(this.login_button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(139, 237);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Don\'t have an account?";
            // 
            // password_label
            // 
            this.password_label.AutoSize = true;
            this.password_label.Location = new System.Drawing.Point(57, 124);
            this.password_label.Name = "password_label";
            this.password_label.Size = new System.Drawing.Size(53, 13);
            this.password_label.TabIndex = 10;
            this.password_label.Text = "Password";
            // 
            // username_label
            // 
            this.username_label.AutoSize = true;
            this.username_label.Location = new System.Drawing.Point(55, 72);
            this.username_label.Name = "username_label";
            this.username_label.Size = new System.Drawing.Size(55, 13);
            this.username_label.TabIndex = 9;
            this.username_label.Text = "Username";
            // 
            // password_box
            // 
            this.password_box.Location = new System.Drawing.Point(116, 121);
            this.password_box.Name = "password_box";
            this.password_box.PasswordChar = '*';
            this.password_box.Size = new System.Drawing.Size(187, 20);
            this.password_box.TabIndex = 8;
            // 
            // username_box
            // 
            this.username_box.Location = new System.Drawing.Point(116, 69);
            this.username_box.Name = "username_box";
            this.username_box.Size = new System.Drawing.Size(187, 20);
            this.username_box.TabIndex = 7;
            // 
            // register_button
            // 
            this.register_button.Location = new System.Drawing.Point(161, 264);
            this.register_button.Name = "register_button";
            this.register_button.Size = new System.Drawing.Size(75, 23);
            this.register_button.TabIndex = 13;
            this.register_button.Text = "Register";
            this.register_button.UseVisualStyleBackColor = true;
            this.register_button.Click += new System.EventHandler(this.register_button_Click);
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(383, 345);
            this.Controls.Add(this.login_button);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.password_label);
            this.Controls.Add(this.username_label);
            this.Controls.Add(this.password_box);
            this.Controls.Add(this.username_box);
            this.Controls.Add(this.register_button);
            this.Name = "Login";
            this.Text = "Login";
            this.Load += new System.EventHandler(this.Login_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Login_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button login_button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label password_label;
        private System.Windows.Forms.Label username_label;
        private System.Windows.Forms.TextBox password_box;
        private System.Windows.Forms.TextBox username_box;
        private System.Windows.Forms.Button register_button;
    }
}

