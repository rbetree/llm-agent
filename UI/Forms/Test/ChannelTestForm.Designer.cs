namespace llm_agent.UI.Forms
{
    partial class ChannelTestForm
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
            this.components = new System.ComponentModel.Container();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblChannelInfo = new System.Windows.Forms.Label();
            this.txtChannelInfo = new System.Windows.Forms.TextBox();
            this.lblModel = new System.Windows.Forms.Label();
            this.cboModel = new System.Windows.Forms.ComboBox();
            this.lblTestMessage = new System.Windows.Forms.Label();
            this.txtTestMessage = new System.Windows.Forms.TextBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lblLog = new System.Windows.Forms.Label();
            this.channelListBox = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Location = new System.Drawing.Point(12, 279);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(560, 23);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "就绪";
            // 
            // lblChannelInfo
            // 
            this.lblChannelInfo.AutoSize = true;
            this.lblChannelInfo.Location = new System.Drawing.Point(12, 9);
            this.lblChannelInfo.Name = "lblChannelInfo";
            this.lblChannelInfo.Size = new System.Drawing.Size(77, 17);
            this.lblChannelInfo.TabIndex = 1;
            this.lblChannelInfo.Text = "渠道信息：";
            // 
            // txtChannelInfo
            // 
            this.txtChannelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChannelInfo.Location = new System.Drawing.Point(12, 29);
            this.txtChannelInfo.Multiline = true;
            this.txtChannelInfo.Name = "txtChannelInfo";
            this.txtChannelInfo.ReadOnly = true;
            this.txtChannelInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtChannelInfo.Size = new System.Drawing.Size(560, 100);
            this.txtChannelInfo.TabIndex = 1;
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.Location = new System.Drawing.Point(12, 251);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(77, 17);
            this.lblModel.TabIndex = 2;
            this.lblModel.Text = "选择模型：";
            // 
            // cboModel
            // 
            this.cboModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboModel.FormattingEnabled = true;
            this.cboModel.Location = new System.Drawing.Point(95, 248);
            this.cboModel.Name = "cboModel";
            this.cboModel.Size = new System.Drawing.Size(384, 24);
            this.cboModel.TabIndex = 3;
            // 
            // lblTestMessage
            // 
            this.lblTestMessage.AutoSize = true;
            this.lblTestMessage.Location = new System.Drawing.Point(12, 190);
            this.lblTestMessage.Name = "lblTestMessage";
            this.lblTestMessage.Size = new System.Drawing.Size(67, 17);
            this.lblTestMessage.TabIndex = 5;
            this.lblTestMessage.Text = "测试消息:";
            // 
            // txtTestMessage
            // 
            this.txtTestMessage.Location = new System.Drawing.Point(15, 210);
            this.txtTestMessage.Multiline = true;
            this.txtTestMessage.Name = "txtTestMessage";
            this.txtTestMessage.Size = new System.Drawing.Size(555, 70);
            this.txtTestMessage.TabIndex = 6;
            this.txtTestMessage.Text = "这是一条测试消息，请回复以验证API连接是否正常。";
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.Location = new System.Drawing.Point(485, 248);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(87, 28);
            this.btnTest.TabIndex = 4;
            this.btnTest.Text = "开始测试";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.Location = new System.Drawing.Point(12, 305);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(560, 244);
            this.txtLog.TabIndex = 6;
            this.txtLog.WordWrap = false;
            // 
            // lblLog
            // 
            this.lblLog.AutoSize = true;
            this.lblLog.Location = new System.Drawing.Point(12, 290);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(67, 17);
            this.lblLog.TabIndex = 9;
            this.lblLog.Text = "测试日志:";
            // 
            // channelListBox
            // 
            this.channelListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.channelListBox.CheckOnClick = true;
            this.channelListBox.FormattingEnabled = true;
            this.channelListBox.Location = new System.Drawing.Point(12, 135);
            this.channelListBox.Name = "channelListBox";
            this.channelListBox.Size = new System.Drawing.Size(560, 106);
            this.channelListBox.TabIndex = 2;
            // 
            // ChannelTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 561);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.cboModel);
            this.Controls.Add(this.lblModel);
            this.Controls.Add(this.channelListBox);
            this.Controls.Add(this.txtChannelInfo);
            this.Controls.Add(this.lblChannelInfo);
            this.MinimumSize = new System.Drawing.Size(500, 600);
            this.Name = "ChannelTestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "渠道测试";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblChannelInfo;
        private System.Windows.Forms.TextBox txtChannelInfo;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.ComboBox cboModel;
        private System.Windows.Forms.Label lblTestMessage;
        private System.Windows.Forms.TextBox txtTestMessage;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label lblLog;
        private System.Windows.Forms.CheckedListBox channelListBox;
    }
} 