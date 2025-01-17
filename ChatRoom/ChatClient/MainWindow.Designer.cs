﻿namespace ChatClient
{
    partial class MainWindow
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
            this.activeSessionsList = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.start_conversation = new System.Windows.Forms.Button();
            this.logout_button = new System.Windows.Forms.Button();
            this.help_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // activeSessionsList
            // 
            this.activeSessionsList.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.activeSessionsList.CheckBoxes = true;
            this.activeSessionsList.HideSelection = false;
            this.activeSessionsList.Location = new System.Drawing.Point(57, 48);
            this.activeSessionsList.Name = "activeSessionsList";
            this.activeSessionsList.Size = new System.Drawing.Size(130, 184);
            this.activeSessionsList.TabIndex = 4;
            this.activeSessionsList.UseCompatibleStateImageBehavior = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.label1.Location = new System.Drawing.Point(56, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 26);
            this.label1.TabIndex = 3;
            this.label1.Text = "Active Users";
            // 
            // start_conversation
            // 
            this.start_conversation.Location = new System.Drawing.Point(12, 240);
            this.start_conversation.Name = "start_conversation";
            this.start_conversation.Size = new System.Drawing.Size(210, 40);
            this.start_conversation.TabIndex = 5;
            this.start_conversation.Text = "Start Conversation";
            this.start_conversation.UseVisualStyleBackColor = true;
            this.start_conversation.Click += new System.EventHandler(this.start_conversation_Click);
            // 
            // logout_button
            // 
            this.logout_button.Location = new System.Drawing.Point(12, 330);
            this.logout_button.Name = "logout_button";
            this.logout_button.Size = new System.Drawing.Size(210, 25);
            this.logout_button.TabIndex = 6;
            this.logout_button.Text = "Logout";
            this.logout_button.UseVisualStyleBackColor = true;
            this.logout_button.Click += new System.EventHandler(this.logout_button_Click);
            // 
            // help_button
            // 
            this.help_button.Location = new System.Drawing.Point(12, 293);
            this.help_button.Name = "help_button";
            this.help_button.Size = new System.Drawing.Size(210, 25);
            this.help_button.TabIndex = 7;
            this.help_button.Text = "Help";
            this.help_button.UseVisualStyleBackColor = true;
            this.help_button.Click += new System.EventHandler(this.help_button_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(234, 361);
            this.Controls.Add(this.help_button);
            this.Controls.Add(this.logout_button);
            this.Controls.Add(this.start_conversation);
            this.Controls.Add(this.activeSessionsList);
            this.Controls.Add(this.label1);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView activeSessionsList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button start_conversation;
        private System.Windows.Forms.Button logout_button;
        private System.Windows.Forms.Button help_button;
    }
}