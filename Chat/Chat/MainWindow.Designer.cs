namespace ChatClient
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
            this.label1 = new System.Windows.Forms.Label();
            this.onlineSessions = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(311, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Active Users";
            // 
            // onlineSessions
            // 
            this.onlineSessions.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.onlineSessions.HideSelection = false;
            this.onlineSessions.Location = new System.Drawing.Point(314, 37);
            this.onlineSessions.Name = "onlineSessions";
            this.onlineSessions.Size = new System.Drawing.Size(121, 192);
            this.onlineSessions.TabIndex = 2;
            this.onlineSessions.UseCompatibleStateImageBehavior = false;
            // 
            // MainWindow
            // 
            this.ClientSize = new System.Drawing.Size(470, 390);
            this.Controls.Add(this.onlineSessions);
            this.Controls.Add(this.label1);
            this.Name = "MainWindow";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView onlineSessions;
    }
}
