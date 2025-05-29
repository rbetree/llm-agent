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
            btnVisit = new System.Windows.Forms.Button();
            btnEdit = new System.Windows.Forms.Button();
            btnDelete = new System.Windows.Forms.Button();
            panelActions = new System.Windows.Forms.Panel();
            panelActions.SuspendLayout();
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
            // btnVisit
            // 
            btnVisit.BackColor = System.Drawing.Color.FromArgb(51, 122, 183);
            btnVisit.FlatAppearance.BorderSize = 0;
            btnVisit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnVisit.Font = new System.Drawing.Font("Microsoft YaHei UI", 8F);
            btnVisit.ForeColor = System.Drawing.Color.White;
            btnVisit.Location = new System.Drawing.Point(0, 0);
            btnVisit.Margin = new System.Windows.Forms.Padding(4);
            btnVisit.Name = "btnVisit";
            btnVisit.Size = new System.Drawing.Size(64, 28);
            btnVisit.TabIndex = 0;
            btnVisit.Text = "访问";
            btnVisit.UseVisualStyleBackColor = false;
            btnVisit.Click += btnVisit_Click;
            // 
            // btnEdit
            // 
            btnEdit.BackColor = System.Drawing.Color.FromArgb(92, 184, 92);
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnEdit.Font = new System.Drawing.Font("Microsoft YaHei UI", 8F);
            btnEdit.ForeColor = System.Drawing.Color.White;
            btnEdit.Location = new System.Drawing.Point(71, 0);
            btnEdit.Margin = new System.Windows.Forms.Padding(4);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new System.Drawing.Size(64, 28);
            btnEdit.TabIndex = 1;
            btnEdit.Text = "编辑";
            btnEdit.UseVisualStyleBackColor = false;
            btnEdit.Click += btnEdit_Click;
            // 
            // btnDelete
            // 
            btnDelete.BackColor = System.Drawing.Color.FromArgb(217, 83, 79);
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnDelete.Font = new System.Drawing.Font("Microsoft YaHei UI", 8F);
            btnDelete.ForeColor = System.Drawing.Color.White;
            btnDelete.Location = new System.Drawing.Point(143, 0);
            btnDelete.Margin = new System.Windows.Forms.Padding(4);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new System.Drawing.Size(64, 28);
            btnDelete.TabIndex = 2;
            btnDelete.Text = "删除";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += btnDelete_Click;
            // 
            // panelActions
            // 
            panelActions.Anchor = System.Windows.Forms.AnchorStyles.Left;
            panelActions.Controls.Add(btnVisit);
            panelActions.Controls.Add(btnEdit);
            panelActions.Controls.Add(btnDelete);
            panelActions.Location = new System.Drawing.Point(15, 67);
            panelActions.Margin = new System.Windows.Forms.Padding(4);
            panelActions.Name = "panelActions";
            panelActions.Size = new System.Drawing.Size(206, 28);
            panelActions.TabIndex = 5;
            // 
            // WebsiteCardItem
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            Controls.Add(panelActions);
            Controls.Add(lblWebsiteUrl);
            Controls.Add(lblWebsiteName);
            Cursor = System.Windows.Forms.Cursors.Hand;
            Margin = new System.Windows.Forms.Padding(4);
            Name = "WebsiteCardItem";
            Size = new System.Drawing.Size(269, 104);
            panelActions.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWebsiteName;
        private System.Windows.Forms.Label lblWebsiteUrl;
        private System.Windows.Forms.Button btnVisit;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Panel panelActions;
    }
}
