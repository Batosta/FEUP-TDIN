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
            this.SuspendLayout();
            // 
            // message_viewer
            // 
            this.message_viewer.HideSelection = false;
            this.message_viewer.Location = new System.Drawing.Point(50, 38);
            this.message_viewer.Name = "message_viewer";
            this.message_viewer.Size = new System.Drawing.Size(272, 196);
            this.message_viewer.TabIndex = 0;
            this.message_viewer.UseCompatibleStateImageBehavior = false;
            // 
            // msg_text_box
            // 
            this.msg_text_box.Location = new System.Drawing.Point(50, 240);
            this.msg_text_box.Name = "msg_text_box";
            this.msg_text_box.Size = new System.Drawing.Size(272, 20);
            this.msg_text_box.TabIndex = 1;
            // 
            // send_message_button
            // 
            this.send_message_button.Location = new System.Drawing.Point(50, 266);
            this.send_message_button.Name = "send_message_button";
            this.send_message_button.Size = new System.Drawing.Size(272, 23);
            this.send_message_button.TabIndex = 2;
            this.send_message_button.Text = "Send Message";
            this.send_message_button.UseVisualStyleBackColor = true;
            // 
            // ConversationWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 307);
            this.Controls.Add(this.send_message_button);
            this.Controls.Add(this.msg_text_box);
            this.Controls.Add(this.message_viewer);
            this.Name = "ConversationWindow";
            this.Text = "ConversationWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView message_viewer;
        private System.Windows.Forms.TextBox msg_text_box;
        private System.Windows.Forms.Button send_message_button;
    }
}