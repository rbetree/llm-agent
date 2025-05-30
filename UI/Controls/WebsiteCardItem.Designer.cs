namespace llm_agent.UI.Controls
{
    partial class WebsiteCardItem
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
            lblWebsiteName = new System.Windows.Forms.Label();
            lblWebsiteUrl = new System.Windows.Forms.Label();
            btnCopyUsername = new System.Windows.Forms.Button();
            btnCopyPassword = new System.Windows.Forms.Button();
            contextMenu = new System.Windows.Forms.ContextMenuStrip();
            editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            contextMenu.SuspendLayout();
            SuspendLayout();
            //
            // lblWebsiteName
            //
            lblWebsiteName.AutoSize = true;
            lblWebsiteName.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            lblWebsiteName.ForeColor = System.Drawing.Color.FromArgb(51, 51, 51);
            lblWebsiteName.Location = new System.Drawing.Point(15, 9);
            lblWebsiteName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblWebsiteName.Name = "lblWebsiteName";
            lblWebsiteName.Size = new System.Drawing.Size(78, 24);
            lblWebsiteName.TabIndex = 0;
            lblWebsiteName.Text = "网站名称";
            //
            // lblWebsiteUrl
            //
            lblWebsiteUrl.AutoSize = true;
            lblWebsiteUrl.ForeColor = System.Drawing.Color.FromArgb(51, 122, 183);
            lblWebsiteUrl.Location = new System.Drawing.Point(15, 38);
            lblWebsiteUrl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblWebsiteUrl.MaximumSize = new System.Drawing.Size(386, 0);
            lblWebsiteUrl.Name = "lblWebsiteUrl";
            lblWebsiteUrl.Size = new System.Drawing.Size(68, 20);
            lblWebsiteUrl.TabIndex = 2;
            lblWebsiteUrl.Text = "网站URL";

            //
            // btnCopyUsername
            //
            btnCopyUsername.BackColor = System.Drawing.Color.FromArgb(51, 122, 183);
            btnCopyUsername.FlatAppearance.BorderSize = 0;
            btnCopyUsername.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnCopyUsername.Font = new System.Drawing.Font("Microsoft YaHei UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnCopyUsername.ForeColor = System.Drawing.Color.White;
            btnCopyUsername.Location = new System.Drawing.Point(180, 15);
            btnCopyUsername.Name = "btnCopyUsername";
            btnCopyUsername.Size = new System.Drawing.Size(40, 20);
            btnCopyUsername.TabIndex = 3;
            btnCopyUsername.Text = "账号";
            btnCopyUsername.UseVisualStyleBackColor = false;
            btnCopyUsername.Visible = false;
            //
            // btnCopyPassword
            //
            btnCopyPassword.BackColor = System.Drawing.Color.FromArgb(40, 167, 69);
            btnCopyPassword.FlatAppearance.BorderSize = 0;
            btnCopyPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnCopyPassword.Font = new System.Drawing.Font("Microsoft YaHei UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnCopyPassword.ForeColor = System.Drawing.Color.White;
            btnCopyPassword.Location = new System.Drawing.Point(180, 40);
            btnCopyPassword.Name = "btnCopyPassword";
            btnCopyPassword.Size = new System.Drawing.Size(40, 20);
            btnCopyPassword.TabIndex = 4;
            btnCopyPassword.Text = "密码";
            btnCopyPassword.UseVisualStyleBackColor = false;
            btnCopyPassword.Visible = false;

            //
            // contextMenu
            //
            contextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { editToolStripMenuItem, deleteToolStripMenuItem });
            contextMenu.Name = "contextMenu";
            contextMenu.Size = new System.Drawing.Size(139, 52);
            //
            // editToolStripMenuItem
            //
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new System.Drawing.Size(138, 24);
            editToolStripMenuItem.Text = "编辑网站";
            editToolStripMenuItem.Click += editToolStripMenuItem_Click;
            //
            // deleteToolStripMenuItem
            //
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new System.Drawing.Size(138, 24);
            deleteToolStripMenuItem.Text = "删除网站";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            //
            // WebsiteCardItem
            //
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            ContextMenuStrip = contextMenu;
            Controls.Add(btnCopyPassword);
            Controls.Add(btnCopyUsername);
            Controls.Add(lblWebsiteUrl);
            Controls.Add(lblWebsiteName);
            Cursor = System.Windows.Forms.Cursors.Hand;
            Margin = new System.Windows.Forms.Padding(4);
            Name = "WebsiteCardItem";
            Size = new System.Drawing.Size(269, 104);
            contextMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWebsiteName;
        private System.Windows.Forms.Label lblWebsiteUrl;
        private System.Windows.Forms.Button btnCopyUsername;
        private System.Windows.Forms.Button btnCopyPassword;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    }
}
