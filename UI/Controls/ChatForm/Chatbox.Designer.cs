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
            statusLabel = new System.Windows.Forms.Label();
            phoneLabel = new System.Windows.Forms.Label();
            clientnameLabel = new System.Windows.Forms.Label();
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
            topPanel.BackColor = System.Drawing.Color.FromArgb(100, 101, 165);
            topPanel.Controls.Add(streamCheckBox);
            topPanel.Controls.Add(modelComboBox);
            topPanel.Controls.Add(statusLabel);
            topPanel.Controls.Add(phoneLabel);
            topPanel.Controls.Add(clientnameLabel);
            topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            topPanel.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            topPanel.Location = new System.Drawing.Point(0, 0);
            topPanel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            topPanel.Name = "topPanel";
            topPanel.Padding = new System.Windows.Forms.Padding(23);
            topPanel.Size = new System.Drawing.Size(616, 75);
            topPanel.TabIndex = 0;
            topPanel.Paint += topPanel_Paint;
            // 
            // streamCheckBox
            // 
            streamCheckBox.AutoSize = true;
            streamCheckBox.Checked = true;
            streamCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            streamCheckBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            streamCheckBox.Location = new System.Drawing.Point(261, 6);
            streamCheckBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            streamCheckBox.Name = "streamCheckBox";
            streamCheckBox.Size = new System.Drawing.Size(95, 24);
            streamCheckBox.TabIndex = 4;
            streamCheckBox.Text = "流式响应";
            streamCheckBox.UseVisualStyleBackColor = true;
            // 
            // modelComboBox
            // 
            modelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            modelComboBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            modelComboBox.FormattingEnabled = true;
            modelComboBox.Location = new System.Drawing.Point(184, 36);
            modelComboBox.Margin = new System.Windows.Forms.Padding(0);
            modelComboBox.Name = "modelComboBox";
            modelComboBox.Size = new System.Drawing.Size(409, 28);
            modelComboBox.TabIndex = 3;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            statusLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            statusLabel.Location = new System.Drawing.Point(177, 7);
            statusLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new System.Drawing.Size(85, 20);
            statusLabel.TabIndex = 2;
            statusLabel.Text = "LastViewed";
            statusLabel.Click += statusLabel_Click;
            // 
            // phoneLabel
            // 
            phoneLabel.AutoSize = true;
            phoneLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            phoneLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            phoneLabel.Location = new System.Drawing.Point(28, 0);
            phoneLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            phoneLabel.Name = "phoneLabel";
            phoneLabel.Size = new System.Drawing.Size(148, 28);
            phoneLabel.TabIndex = 1;
            phoneLabel.Text = "(408) 262-9190";
            // 
            // clientnameLabel
            // 
            clientnameLabel.AutoSize = true;
            clientnameLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold);
            clientnameLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            clientnameLabel.Location = new System.Drawing.Point(23, 30);
            clientnameLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            clientnameLabel.Name = "clientnameLabel";
            clientnameLabel.Size = new System.Drawing.Size(149, 32);
            clientnameLabel.TabIndex = 0;
            clientnameLabel.Text = "Client Name";
            clientnameLabel.Click += clientnameLabel_Click;
            // 
            // bottomPanel
            // 
            bottomPanel.BackColor = System.Drawing.Color.FromArgb(100, 101, 165);
            bottomPanel.Controls.Add(chatTextbox);
            bottomPanel.Controls.Add(attachButton);
            bottomPanel.Controls.Add(removeButton);
            bottomPanel.Controls.Add(sendButton);
            bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            bottomPanel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            bottomPanel.Location = new System.Drawing.Point(0, 844);
            bottomPanel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Padding = new System.Windows.Forms.Padding(23, 16, 23, 16);
            bottomPanel.Size = new System.Drawing.Size(616, 83);
            bottomPanel.TabIndex = 1;
            // 
            // chatTextbox
            // 
            chatTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            chatTextbox.Location = new System.Drawing.Point(23, 16);
            chatTextbox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            chatTextbox.Multiline = true;
            chatTextbox.Name = "chatTextbox";
            chatTextbox.Size = new System.Drawing.Size(376, 51);
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
            attachButton.Location = new System.Drawing.Point(399, 16);
            attachButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            attachButton.Name = "attachButton";
            attachButton.Size = new System.Drawing.Size(53, 51);
            attachButton.TabIndex = 6;
            attachButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            attachButton.UseVisualStyleBackColor = false;
            // 
            // removeButton
            // 
            removeButton.BackColor = System.Drawing.Color.Red;
            removeButton.Dock = System.Windows.Forms.DockStyle.Right;
            removeButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            removeButton.Font = new System.Drawing.Font("Segoe UI Symbol", 9.75F, System.Drawing.FontStyle.Bold);
            removeButton.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            removeButton.Location = new System.Drawing.Point(452, 16);
            removeButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            removeButton.Name = "removeButton";
            removeButton.Size = new System.Drawing.Size(28, 51);
            removeButton.TabIndex = 5;
            removeButton.Text = "X";
            removeButton.UseVisualStyleBackColor = false;
            removeButton.Visible = false;
            // 
            // sendButton
            // 
            sendButton.BackColor = System.Drawing.Color.LightSlateGray;
            sendButton.Dock = System.Windows.Forms.DockStyle.Right;
            sendButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            sendButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            sendButton.Location = new System.Drawing.Point(480, 16);
            sendButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            sendButton.Name = "sendButton";
            sendButton.Size = new System.Drawing.Size(113, 51);
            sendButton.TabIndex = 1;
            sendButton.Text = "Send";
            sendButton.UseVisualStyleBackColor = false;
            // 
            // itemsPanel
            // 
            itemsPanel.AutoScroll = true;
            itemsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            itemsPanel.Location = new System.Drawing.Point(0, 75);
            itemsPanel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            itemsPanel.Name = "itemsPanel";
            itemsPanel.Size = new System.Drawing.Size(616, 769);
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
            Size = new System.Drawing.Size(616, 927);
            topPanel.ResumeLayout(false);
            topPanel.PerformLayout();
            bottomPanel.ResumeLayout(false);
            bottomPanel.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel topPanel;
		private System.Windows.Forms.Label statusLabel;
		private System.Windows.Forms.Label phoneLabel;
		private System.Windows.Forms.Label clientnameLabel;
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
