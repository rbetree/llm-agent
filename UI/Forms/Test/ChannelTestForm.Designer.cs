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
            this.lblChannelInfo = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.txtLog = new llm_agent.UI.Controls.SimpleMessageDisplay.SimpleMessageDisplay();
            this.lblLog = new System.Windows.Forms.Label();
            this.channelListBox = new System.Windows.Forms.CheckedListBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblChannelInfo
            // 
            this.lblChannelInfo.AutoSize = true;
            this.lblChannelInfo.Location = new System.Drawing.Point(12, 20);
            this.lblChannelInfo.Name = "lblChannelInfo";
            this.lblChannelInfo.Size = new System.Drawing.Size(167, 17);
            this.lblChannelInfo.TabIndex = 1;
            this.lblChannelInfo.Text = "请选择要进行测试的渠道:";
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.Location = new System.Drawing.Point(460, 180);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(112, 32);
            this.btnTest.TabIndex = 7;
            this.btnTest.Text = "开始批量测试";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(12, 250);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(560, 240);
            this.txtLog.TabIndex = 8;
            // 
            // lblLog
            // 
            this.lblLog.AutoSize = true;
            this.lblLog.Location = new System.Drawing.Point(12, 230);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(67, 17);
            this.lblLog.TabIndex = 9;
            this.lblLog.Text = "测试结果:";
            // 
            // channelListBox
            // 
            this.channelListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.channelListBox.CheckOnClick = true;
            this.channelListBox.FormattingEnabled = true;
            this.channelListBox.Location = new System.Drawing.Point(12, 45);
            this.channelListBox.Name = "channelListBox";
            this.channelListBox.Size = new System.Drawing.Size(560, 123);
            this.channelListBox.TabIndex = 2;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Location = new System.Drawing.Point(12, 180);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(442, 23);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "就绪";
            // 
            // ChannelTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 500);
            this.Controls.Add(this.lblLog);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.channelListBox);
            this.Controls.Add(this.lblChannelInfo);
            this.Controls.Add(this.lblStatus);
            this.MinimumSize = new System.Drawing.Size(500, 500);
            this.Name = "ChannelTestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "渠道批量联通测试";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblChannelInfo;
        private System.Windows.Forms.Button btnTest;
        private llm_agent.UI.Controls.SimpleMessageDisplay.SimpleMessageDisplay txtLog;
        private System.Windows.Forms.Label lblLog;
        private System.Windows.Forms.CheckedListBox channelListBox;
    }
} 