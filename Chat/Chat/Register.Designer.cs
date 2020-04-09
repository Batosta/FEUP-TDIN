namespace ChatClient
{
    partial class Register
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
            this.password_label = new System.Windows.Forms.Label();
            this.username_label = new System.Windows.Forms.Label();
            this.password_box = new System.Windows.Forms.TextBox();
            this.username_box = new System.Windows.Forms.TextBox();
            this.register_button = new System.Windows.Forms.Button();
            this.name_box = new System.Windows.Forms.TextBox();
            this.name_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // password_label
            // 
            this.password_label.AutoSize = true;
            this.password_label.Location = new System.Drawing.Point(69, 149);
            this.password_label.Name = "password_label";
            this.password_label.Size = new System.Drawing.Size(53, 13);
            this.password_label.TabIndex = 4;
            this.password_label.Text = "Password";
            // 
            // username_label
            // 
            this.username_label.AutoSize = true;
            this.username_label.Location = new System.Drawing.Point(69, 77);
            this.username_label.Name = "username_label";
            this.username_label.Size = new System.Drawing.Size(55, 13);
            this.username_label.TabIndex = 2;
            this.username_label.Text = "Username";
            this.username_label.Click += new System.EventHandler(this.username_label_Click);
            // 
            // password_box
            // 
            this.password_box.Location = new System.Drawing.Point(130, 146);
            this.password_box.Name = "password_box";
            this.password_box.PasswordChar = '*';
            this.password_box.Size = new System.Drawing.Size(187, 20);
            this.password_box.TabIndex = 2;
            // 
            // username_box
            // 
            this.username_box.Location = new System.Drawing.Point(130, 74);
            this.username_box.Name = "username_box";
            this.username_box.Size = new System.Drawing.Size(187, 20);
            this.username_box.TabIndex = 0;
            // 
            // register_button
            // 
            this.register_button.Location = new System.Drawing.Point(163, 233);
            this.register_button.Name = "register_button";
            this.register_button.Size = new System.Drawing.Size(75, 23);
            this.register_button.TabIndex = 3;
            this.register_button.Text = "Register";
            this.register_button.UseVisualStyleBackColor = true;
            this.register_button.Click += new System.EventHandler(this.register_button_Click);
            // 
            // name_box
            // 
            this.name_box.Location = new System.Drawing.Point(130, 110);
            this.name_box.Name = "name_box";
            this.name_box.Size = new System.Drawing.Size(187, 20);
            this.name_box.TabIndex = 1;
            this.name_box.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // name_label
            // 
            this.name_label.AutoSize = true;
            this.name_label.Location = new System.Drawing.Point(77, 113);
            this.name_label.Name = "name_label";
            this.name_label.Size = new System.Drawing.Size(35, 13);
            this.name_label.TabIndex = 3;
            this.name_label.Text = "Name";
            this.name_label.Click += new System.EventHandler(this.label2_Click);
            // 
            // Register
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 311);
            this.Controls.Add(this.name_label);
            this.Controls.Add(this.name_box);
            this.Controls.Add(this.password_label);
            this.Controls.Add(this.username_label);
            this.Controls.Add(this.password_box);
            this.Controls.Add(this.username_box);
            this.Controls.Add(this.register_button);
            this.Name = "Register";
            this.Text = "Register";
            this.Load += new System.EventHandler(this.Register_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label password_label;
        private System.Windows.Forms.Label username_label;
        private System.Windows.Forms.TextBox password_box;
        private System.Windows.Forms.TextBox username_box;
        private System.Windows.Forms.Button register_button;
        private System.Windows.Forms.TextBox name_box;
        private System.Windows.Forms.Label name_label;
    }
}