using System;
using System.Windows.Forms;

namespace llm_agent.UI.Forms
{
    public partial class ModelTestForm : Form
    {
        public ModelTestForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.lblModelInfo = new System.Windows.Forms.Label();
            this.txtPrompt = new System.Windows.Forms.TextBox();
            this.lblPrompt = new System.Windows.Forms.Label();
            this.btnRunTest = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.RichTextBox();
            this.lblOutput = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnClose = new System.Windows.Forms.Button();
            this.chkStreamResponse = new System.Windows.Forms.CheckBox();
            this.panelPerformance = new System.Windows.Forms.Panel();
            this.lblOutputSpeed = new System.Windows.Forms.Label();
            this.lblResponseTime = new System.Windows.Forms.Label();
            this.panelPerformance.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblModelInfo
            // 
            this.lblModelInfo.BackColor = System.Drawing.SystemColors.Info;
            this.lblModelInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblModelInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblModelInfo.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblModelInfo.Location = new System.Drawing.Point(0, 0);
            this.lblModelInfo.Name = "lblModelInfo";
            this.lblModelInfo.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.lblModelInfo.Size = new System.Drawing.Size(854, 30);
            this.lblModelInfo.TabIndex = 0;
            this.lblModelInfo.Text = "模型信息";
            this.lblModelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPrompt
            // 
            this.txtPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPrompt.Location = new System.Drawing.Point(12, 53);
            this.txtPrompt.Multiline = true;
            this.txtPrompt.Name = "txtPrompt";
            this.txtPrompt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPrompt.Size = new System.Drawing.Size(830, 150);
            this.txtPrompt.TabIndex = 1;
            // 
            // lblPrompt
            // 
            this.lblPrompt.AutoSize = true;
            this.lblPrompt.Location = new System.Drawing.Point(12, 33);
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Size = new System.Drawing.Size(77, 17);
            this.lblPrompt.TabIndex = 2;
            this.lblPrompt.Text = "测试提示词:";
            // 
            // btnRunTest
            // 
            this.btnRunTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRunTest.Location = new System.Drawing.Point(725, 210);
            this.btnRunTest.Name = "btnRunTest";
            this.btnRunTest.Size = new System.Drawing.Size(117, 28);
            this.btnRunTest.TabIndex = 3;
            this.btnRunTest.Text = "开始性能测试";
            this.btnRunTest.UseVisualStyleBackColor = true;
            this.btnRunTest.Click += new System.EventHandler(this.btnRunTest_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.BackColor = System.Drawing.Color.White;
            this.txtOutput.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOutput.Location = new System.Drawing.Point(12, 260);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.Size = new System.Drawing.Size(830, 260);
            this.txtOutput.TabIndex = 4;
            this.txtOutput.Text = "";
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(12, 240);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(77, 17);
            this.lblOutput.TabIndex = 5;
            this.lblOutput.Text = "输出结果:";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(95, 239);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(747, 15);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 6;
            this.progressBar.Visible = false;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(725, 529);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(117, 28);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // chkStreamResponse
            // 
            this.chkStreamResponse.AutoSize = true;
            this.chkStreamResponse.Checked = true;
            this.chkStreamResponse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkStreamResponse.Location = new System.Drawing.Point(12, 210);
            this.chkStreamResponse.Name = "chkStreamResponse";
            this.chkStreamResponse.Size = new System.Drawing.Size(105, 21);
            this.chkStreamResponse.TabIndex = 8;
            this.chkStreamResponse.Text = "使用流式响应";
            this.chkStreamResponse.UseVisualStyleBackColor = true;
            // 
            // panelPerformance
            // 
            this.panelPerformance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPerformance.BackColor = System.Drawing.SystemColors.Info;
            this.panelPerformance.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPerformance.Controls.Add(this.lblOutputSpeed);
            this.panelPerformance.Controls.Add(this.lblResponseTime);
            this.panelPerformance.Location = new System.Drawing.Point(12, 529);
            this.panelPerformance.Name = "panelPerformance";
            this.panelPerformance.Size = new System.Drawing.Size(707, 28);
            this.panelPerformance.TabIndex = 16;
            // 
            // lblResponseTime
            // 
            this.lblResponseTime.AutoSize = true;
            this.lblResponseTime.Location = new System.Drawing.Point(3, 5);
            this.lblResponseTime.Name = "lblResponseTime";
            this.lblResponseTime.Size = new System.Drawing.Size(92, 17);
            this.lblResponseTime.TabIndex = 0;
            this.lblResponseTime.Text = "响应时间: 等待";
            // 
            // lblOutputSpeed
            // 
            this.lblOutputSpeed.AutoSize = true;
            this.lblOutputSpeed.Location = new System.Drawing.Point(280, 5);
            this.lblOutputSpeed.Name = "lblOutputSpeed";
            this.lblOutputSpeed.Size = new System.Drawing.Size(92, 17);
            this.lblOutputSpeed.TabIndex = 1;
            this.lblOutputSpeed.Text = "输出速度: 等待";
            // 
            // ModelTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 569);
            this.Controls.Add(this.panelPerformance);
            this.Controls.Add(this.chkStreamResponse);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.btnRunTest);
            this.Controls.Add(this.lblPrompt);
            this.Controls.Add(this.txtPrompt);
            this.Controls.Add(this.lblModelInfo);
            this.MinimumSize = new System.Drawing.Size(500, 500);
            this.Name = "ModelTestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "模型性能测试";
            this.panelPerformance.ResumeLayout(false);
            this.panelPerformance.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblModelInfo;
        private System.Windows.Forms.TextBox txtPrompt;
        private System.Windows.Forms.Label lblPrompt;
        private System.Windows.Forms.Button btnRunTest;
        private System.Windows.Forms.RichTextBox txtOutput;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.CheckBox chkStreamResponse;
        private System.Windows.Forms.Panel panelPerformance;
        private System.Windows.Forms.Label lblOutputSpeed;
        private System.Windows.Forms.Label lblResponseTime;
    }
} 