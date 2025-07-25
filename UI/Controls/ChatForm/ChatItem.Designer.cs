﻿namespace llm_agent.UI.Controls.ChatForm
{
	partial class ChatItem
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            authorPanel = new System.Windows.Forms.Panel();
            authorLabel = new System.Windows.Forms.Label();
            bodyPanel = new System.Windows.Forms.Panel();
            bodyTextBox = new System.Windows.Forms.RichTextBox();
            authorPanel.SuspendLayout();
            bodyPanel.SuspendLayout();
            SuspendLayout();
            // 
            // authorPanel
            // 
            authorPanel.Controls.Add(authorLabel);
            authorPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            authorPanel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            authorPanel.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            authorPanel.Location = new System.Drawing.Point(8, 73);
            authorPanel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            authorPanel.Name = "authorPanel";
            authorPanel.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            authorPanel.Size = new System.Drawing.Size(893, 40);
            authorPanel.TabIndex = 8;
            // 
            // authorLabel
            // 
            authorLabel.AutoSize = true;
            authorLabel.Dock = System.Windows.Forms.DockStyle.Left;
            authorLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            authorLabel.Location = new System.Drawing.Point(0, 8);
            authorLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            authorLabel.Name = "authorLabel";
            authorLabel.Size = new System.Drawing.Size(142, 19);
            authorLabel.TabIndex = 0;
            authorLabel.Text = "System - 10/22/2020";
            // 
            // bodyPanel
            // 
            bodyPanel.BackColor = System.Drawing.Color.FromArgb(76, 76, 128);
            bodyPanel.Controls.Add(bodyTextBox);
            bodyPanel.Dock = System.Windows.Forms.DockStyle.Left;
            bodyPanel.Location = new System.Drawing.Point(8, 4);
            bodyPanel.Margin = new System.Windows.Forms.Padding(0);
            bodyPanel.Name = "bodyPanel";
            bodyPanel.Padding = new System.Windows.Forms.Padding(2);
            bodyPanel.Size = new System.Drawing.Size(545, 69);
            bodyPanel.TabIndex = 9;
            // 
            // bodyTextBox
            // 
            bodyTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            bodyTextBox.BackColor = System.Drawing.Color.FromArgb(76, 76, 128);
            bodyTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            bodyTextBox.Font = new System.Drawing.Font("Segoe UI Emoji", 9F);
            bodyTextBox.ForeColor = System.Drawing.Color.White;
            bodyTextBox.Location = new System.Drawing.Point(4, 4);
            bodyTextBox.Margin = new System.Windows.Forms.Padding(0);
            bodyTextBox.Name = "bodyTextBox";
            bodyTextBox.ReadOnly = true;
            bodyTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            bodyTextBox.Size = new System.Drawing.Size(530, 55);
            bodyTextBox.TabIndex = 4;
            bodyTextBox.Text = "Hello there. This is a test for the longer word usage.";
            // 
            // ChatItem
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlLightLight;
            Controls.Add(bodyPanel);
            Controls.Add(authorPanel);
            Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            Name = "ChatItem";
            Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            Size = new System.Drawing.Size(909, 117);
            Load += ChatItem_Load;
            authorPanel.ResumeLayout(false);
            authorPanel.PerformLayout();
            bodyPanel.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel authorPanel;
		private System.Windows.Forms.Label authorLabel;
		private System.Windows.Forms.Panel bodyPanel;
		private System.Windows.Forms.RichTextBox bodyTextBox;
	}
}
