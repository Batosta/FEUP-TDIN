namespace Chat
{
    partial class Form1
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
            this.register_button = new System.Windows.Forms.Button();
            this.nickname_box = new System.Windows.Forms.TextBox();
            this.password_box = new System.Windows.Forms.TextBox();
            this.nickname_label = new System.Windows.Forms.Label();
            this.password_label = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // register_button
            // 
            this.register_button.Location = new System.Drawing.Point(152, 232);
            this.register_button.Name = "register_button";
            this.register_button.Size = new System.Drawing.Size(75, 23);
            this.register_button.TabIndex = 0;
            this.register_button.Text = "Register";
            this.register_button.UseVisualStyleBackColor = true;
            this.register_button.Click += new System.EventHandler(this.button1_Click);
            // 
            // nickname_box
            // 
            this.nickname_box.Location = new System.Drawing.Point(121, 70);
            this.nickname_box.Name = "nickname_box";
            this.nickname_box.Size = new System.Drawing.Size(187, 20);
            this.nickname_box.TabIndex = 1;
            // 
            // password_box
            // 
            this.password_box.Location = new System.Drawing.Point(121, 142);
            this.password_box.Name = "password_box";
            this.password_box.Size = new System.Drawing.Size(187, 20);
            this.password_box.TabIndex = 2;
            // 
            // nickname_label
            // 
            this.nickname_label.AutoSize = true;
            this.nickname_label.Location = new System.Drawing.Point(60, 73);
            this.nickname_label.Name = "nickname_label";
            this.nickname_label.Size = new System.Drawing.Size(55, 13);
            this.nickname_label.TabIndex = 3;
            this.nickname_label.Text = "Nickname";
            this.nickname_label.Click += new System.EventHandler(this.label1_Click);
            // 
            // password_label
            // 
            this.password_label.AutoSize = true;
            this.password_label.Location = new System.Drawing.Point(60, 145);
            this.password_label.Name = "password_label";
            this.password_label.Size = new System.Drawing.Size(53, 13);
            this.password_label.TabIndex = 4;
            this.password_label.Text = "Password";
            this.password_label.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(132, 205);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Don\'t have an account?";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 305);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.password_label);
            this.Controls.Add(this.nickname_label);
            this.Controls.Add(this.password_box);
            this.Controls.Add(this.nickname_box);
            this.Controls.Add(this.register_button);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button register_button;
        private System.Windows.Forms.TextBox nickname_box;
        private System.Windows.Forms.TextBox password_box;
        private System.Windows.Forms.Label nickname_label;
        private System.Windows.Forms.Label password_label;
        private System.Windows.Forms.Label label1;
    }
}

