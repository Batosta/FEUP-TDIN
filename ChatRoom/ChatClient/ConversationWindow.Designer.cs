namespace ChatClient
{
    partial class ConversationWindow
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
            this.message_viewer = new System.Windows.Forms.ListView();
            this.msg_text_box = new System.Windows.Forms.TextBox();
            this.send_message_button = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // message_viewer
            // 
            this.message_viewer.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.message_viewer.HideSelection = false;
            this.message_viewer.HoverSelection = true;
            this.message_viewer.Location = new System.Drawing.Point(32, 34);
            this.message_viewer.Name = "message_viewer";
            this.message_viewer.Size = new System.Drawing.Size(358, 281);
            this.message_viewer.TabIndex = 0;
            this.message_viewer.TileSize = new System.Drawing.Size(1, 30);
            this.message_viewer.UseCompatibleStateImageBehavior = false;
            this.message_viewer.View = System.Windows.Forms.View.SmallIcon;
            // 
            // msg_text_box
            // 
            this.msg_text_box.Location = new System.Drawing.Point(32, 332);
            this.msg_text_box.Name = "msg_text_box";
            this.msg_text_box.Size = new System.Drawing.Size(225, 20);
            this.msg_text_box.TabIndex = 1;
            // 
            // send_message_button
            // 
            this.send_message_button.Location = new System.Drawing.Point(265, 331);
            this.send_message_button.Name = "send_message_button";
            this.send_message_button.Size = new System.Drawing.Size(60, 23);
            this.send_message_button.TabIndex = 2;
            this.send_message_button.Text = "Send Message";
            this.send_message_button.UseVisualStyleBackColor = true;
            this.send_message_button.Click += new System.EventHandler(this.Send_message_button_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(331, 331);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(60, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Send file";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(63, 362);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(290, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Write /pm <username> <message> to send private message";
            // 
            // ConversationWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 391);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.send_message_button);
            this.Controls.Add(this.msg_text_box);
            this.Controls.Add(this.message_viewer);
            this.MaximumSize = new System.Drawing.Size(444, 450);
            this.MinimumSize = new System.Drawing.Size(444, 430);
            this.Name = "ConversationWindow";
            this.Text = "ConversationWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConversationWindow_FormClosing);
            this.Load += new System.EventHandler(this.ConversationWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView message_viewer;
        private System.Windows.Forms.TextBox msg_text_box;
        private System.Windows.Forms.Button send_message_button;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
    }
}