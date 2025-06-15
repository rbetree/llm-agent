namespace llm_agent.UI.Controls.ChatForm
{
	partial class Chatbox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chatbox));
            topPanel = new System.Windows.Forms.Panel();
            streamCheckBox = new System.Windows.Forms.CheckBox();
            modelComboBox = new System.Windows.Forms.ComboBox();
            bottomPanel = new System.Windows.Forms.Panel();
            chatTextbox = new System.Windows.Forms.TextBox();
            attachButton = new System.Windows.Forms.Button();
            removeButton = new System.Windows.Forms.Button();
            sendButton = new System.Windows.Forms.Button();
            itemsPanel = new System.Windows.Forms.Panel();
            topPanel.SuspendLayout();
            bottomPanel.SuspendLayout();
            SuspendLayout();
            // 
            // topPanel
            // 
            topPanel.BackColor = System.Drawing.Color.FromArgb(76, 76, 128);
            topPanel.Controls.Add(streamCheckBox);
            topPanel.Controls.Add(modelComboBox);
            topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            topPanel.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            topPanel.Location = new System.Drawing.Point(0, 0);
            topPanel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            topPanel.Name = "topPanel";
            topPanel.Padding = new System.Windows.Forms.Padding(23);
            topPanel.Size = new System.Drawing.Size(936, 52);
            topPanel.TabIndex = 0;
            topPanel.Paint += topPanel_Paint;
            // 
            // streamCheckBox
            // 
            streamCheckBox.AutoSize = true;
            streamCheckBox.Checked = true;
            streamCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            streamCheckBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            streamCheckBox.Location = new System.Drawing.Point(541, 14);
            streamCheckBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            streamCheckBox.Name = "streamCheckBox";
            streamCheckBox.Size = new System.Drawing.Size(95, 24);
            streamCheckBox.TabIndex = 1;
            streamCheckBox.Text = "流式响应";
            streamCheckBox.UseVisualStyleBackColor = true;
            // 
            // modelComboBox
            // 
            modelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            modelComboBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            modelComboBox.FormattingEnabled = true;
            modelComboBox.Location = new System.Drawing.Point(9, 12);
            modelComboBox.Margin = new System.Windows.Forms.Padding(0);
            modelComboBox.Name = "modelComboBox";
            modelComboBox.Size = new System.Drawing.Size(504, 28);
            modelComboBox.TabIndex = 2;
            modelComboBox.SelectedIndexChanged += modelComboBox_SelectedIndexChanged_1;
            // 
            // bottomPanel
            // 
            bottomPanel.BackColor = System.Drawing.Color.FromArgb(76, 76, 128);
            bottomPanel.Controls.Add(chatTextbox);
            bottomPanel.Controls.Add(attachButton);
            bottomPanel.Controls.Add(removeButton);
            bottomPanel.Controls.Add(sendButton);
            bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            bottomPanel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            bottomPanel.Location = new System.Drawing.Point(0, 823);
            bottomPanel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Padding = new System.Windows.Forms.Padding(23, 16, 23, 16);
            bottomPanel.Size = new System.Drawing.Size(936, 104);
            bottomPanel.TabIndex = 1;
            // 
            // chatTextbox
            // 
            chatTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            chatTextbox.Location = new System.Drawing.Point(23, 16);
            chatTextbox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            chatTextbox.Multiline = true;
            chatTextbox.Name = "chatTextbox";
            chatTextbox.Size = new System.Drawing.Size(696, 72);
            chatTextbox.TabIndex = 7;
            // 
            // attachButton
            // 
            attachButton.BackColor = System.Drawing.Color.GhostWhite;
            attachButton.BackgroundImage = (System.Drawing.Image)resources.GetObject("attachButton.BackgroundImage");
            attachButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            attachButton.Dock = System.Windows.Forms.DockStyle.Right;
            attachButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            attachButton.ForeColor = System.Drawing.SystemColors.ControlText;
            attachButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            attachButton.Location = new System.Drawing.Point(719, 16);
            attachButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            attachButton.Name = "attachButton";
            attachButton.Size = new System.Drawing.Size(53, 72);
            attachButton.TabIndex = 6;
            attachButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            attachButton.UseVisualStyleBackColor = false;
            attachButton.Visible = false;
            // 
            // removeButton
            // 
            removeButton.BackColor = System.Drawing.Color.Red;
            removeButton.Dock = System.Windows.Forms.DockStyle.Right;
            removeButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            removeButton.Font = new System.Drawing.Font("Segoe UI Symbol", 9.75F, System.Drawing.FontStyle.Bold);
            removeButton.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            removeButton.Location = new System.Drawing.Point(772, 16);
            removeButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            removeButton.Name = "removeButton";
            removeButton.Size = new System.Drawing.Size(28, 72);
            removeButton.TabIndex = 5;
            removeButton.Text = "X";
            removeButton.UseVisualStyleBackColor = false;
            removeButton.Visible = false;
            // 
            // sendButton
            // 
            sendButton.BackColor = System.Drawing.Color.FromArgb(76, 76, 128);
            sendButton.Dock = System.Windows.Forms.DockStyle.Right;
            sendButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            sendButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            sendButton.Location = new System.Drawing.Point(800, 16);
            sendButton.Margin = new System.Windows.Forms.Padding(0);
            sendButton.Name = "sendButton";
            sendButton.Size = new System.Drawing.Size(113, 72);
            sendButton.TabIndex = 1;
            sendButton.Text = "Send";
            sendButton.UseVisualStyleBackColor = false;
            // 
            // itemsPanel
            // 
            itemsPanel.AutoScroll = true;
            itemsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            itemsPanel.Location = new System.Drawing.Point(0, 52);
            itemsPanel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            itemsPanel.Name = "itemsPanel";
            itemsPanel.Size = new System.Drawing.Size(936, 771);
            itemsPanel.TabIndex = 2;
            // 
            // Chatbox
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            Controls.Add(itemsPanel);
            Controls.Add(bottomPanel);
            Controls.Add(topPanel);
            Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            Name = "Chatbox";
            Size = new System.Drawing.Size(936, 927);
            topPanel.ResumeLayout(false);
            topPanel.PerformLayout();
            bottomPanel.ResumeLayout(false);
            bottomPanel.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel topPanel;

		private System.Windows.Forms.Panel bottomPanel;
		private System.Windows.Forms.Button sendButton;
		private System.Windows.Forms.Button attachButton;
		private System.Windows.Forms.Button removeButton;
		private System.Windows.Forms.TextBox chatTextbox;
		private System.Windows.Forms.Panel itemsPanel;
		private System.Windows.Forms.CheckBox streamCheckBox;
		private System.Windows.Forms.ComboBox modelComboBox;
	}
}
