namespace llm_agent.UI.Controls
{
    partial class WebsiteBrowser
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            panelToolbar = new System.Windows.Forms.Panel();
            btnGo = new System.Windows.Forms.Button();
            txtUrl = new System.Windows.Forms.TextBox();
            btnStop = new System.Windows.Forms.Button();
            btnRefresh = new System.Windows.Forms.Button();
            btnForward = new System.Windows.Forms.Button();
            btnBack = new System.Windows.Forms.Button();
            panelBrowser = new System.Windows.Forms.Panel();
            panelToolbar.SuspendLayout();
            SuspendLayout();
            // 
            // panelToolbar
            // 
            panelToolbar.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            panelToolbar.Controls.Add(btnGo);
            panelToolbar.Controls.Add(txtUrl);
            panelToolbar.Controls.Add(btnStop);
            panelToolbar.Controls.Add(btnRefresh);
            panelToolbar.Controls.Add(btnForward);
            panelToolbar.Controls.Add(btnBack);
            panelToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            panelToolbar.Location = new System.Drawing.Point(0, 0);
            panelToolbar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            panelToolbar.Name = "panelToolbar";
            panelToolbar.Size = new System.Drawing.Size(1029, 52);
            panelToolbar.TabIndex = 0;
            // 
            // btnGo
            // 
            btnGo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnGo.BackColor = System.Drawing.Color.FromArgb(76, 76, 128);
            btnGo.FlatAppearance.BorderSize = 0;
            btnGo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnGo.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            btnGo.ForeColor = System.Drawing.Color.White;
            btnGo.Location = new System.Drawing.Point(953, 9);
            btnGo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnGo.Name = "btnGo";
            btnGo.Size = new System.Drawing.Size(62, 33);
            btnGo.TabIndex = 5;
            btnGo.Text = "转到";
            btnGo.UseVisualStyleBackColor = false;
            btnGo.Click += btnGo_Click;
            // 
            // txtUrl
            // 
            txtUrl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtUrl.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            txtUrl.Location = new System.Drawing.Point(196, 12);
            txtUrl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            txtUrl.Name = "txtUrl";
            txtUrl.PlaceholderText = "输入网址或搜索内容...";
            txtUrl.Size = new System.Drawing.Size(749, 27);
            txtUrl.TabIndex = 4;
            txtUrl.KeyDown += txtUrl_KeyDown;
            // 
            // btnStop
            // 
            btnStop.BackColor = System.Drawing.Color.FromArgb(217, 83, 79);
            btnStop.FlatAppearance.BorderSize = 0;
            btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnStop.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            btnStop.ForeColor = System.Drawing.Color.White;
            btnStop.Location = new System.Drawing.Point(149, 9);
            btnStop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnStop.Name = "btnStop";
            btnStop.Size = new System.Drawing.Size(39, 33);
            btnStop.TabIndex = 3;
            btnStop.Text = "⏹";
            btnStop.UseVisualStyleBackColor = false;
            btnStop.Click += btnStop_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = System.Drawing.Color.FromArgb(92, 184, 92);
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnRefresh.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            btnRefresh.ForeColor = System.Drawing.Color.White;
            btnRefresh.Location = new System.Drawing.Point(103, 9);
            btnRefresh.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new System.Drawing.Size(39, 33);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "🔄";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnForward
            // 
            btnForward.BackColor = System.Drawing.Color.FromArgb(108, 117, 125);
            btnForward.FlatAppearance.BorderSize = 0;
            btnForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnForward.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            btnForward.ForeColor = System.Drawing.Color.White;
            btnForward.Location = new System.Drawing.Point(57, 9);
            btnForward.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnForward.Name = "btnForward";
            btnForward.Size = new System.Drawing.Size(39, 33);
            btnForward.TabIndex = 1;
            btnForward.Text = "▶";
            btnForward.UseVisualStyleBackColor = false;
            btnForward.Click += btnForward_Click;
            // 
            // btnBack
            // 
            btnBack.BackColor = System.Drawing.Color.FromArgb(108, 117, 125);
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnBack.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            btnBack.ForeColor = System.Drawing.Color.White;
            btnBack.Location = new System.Drawing.Point(10, 9);
            btnBack.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnBack.Name = "btnBack";
            btnBack.Size = new System.Drawing.Size(39, 33);
            btnBack.TabIndex = 0;
            btnBack.Text = "◀";
            btnBack.UseVisualStyleBackColor = false;
            btnBack.Click += btnBack_Click;
            // 
            // panelBrowser
            // 
            panelBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            panelBrowser.Location = new System.Drawing.Point(0, 52);
            panelBrowser.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            panelBrowser.Name = "panelBrowser";
            panelBrowser.Size = new System.Drawing.Size(1029, 654);
            panelBrowser.TabIndex = 1;
            // 
            // WebsiteBrowser
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panelBrowser);
            Controls.Add(panelToolbar);
            Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Name = "WebsiteBrowser";
            Size = new System.Drawing.Size(1029, 706);
            panelToolbar.ResumeLayout(false);
            panelToolbar.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelToolbar;
        private System.Windows.Forms.Panel panelBrowser;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnForward;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Button btnGo;
    }
}
