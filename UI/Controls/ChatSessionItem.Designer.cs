namespace llm_agent.UI.Controls
{
    partial class ChatSessionItem
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
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblPreview = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoEllipsis = true;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.Black;
            this.lblTitle.Location = new System.Drawing.Point(6, 5);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(280, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "会话标题";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblPreview
            //
            this.lblPreview.AutoEllipsis = true;
            this.lblPreview.Enabled = false;
            this.lblPreview.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblPreview.ForeColor = System.Drawing.Color.Gray;
            this.lblPreview.Location = new System.Drawing.Point(6, 35);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(280, 18);
            this.lblPreview.TabIndex = 1;
            this.lblPreview.Text = "消息预览";
            //
            // lblTime
            //
            this.lblTime.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblTime.Enabled = false;
            this.lblTime.Font = new System.Drawing.Font("Microsoft YaHei UI", 8F);
            this.lblTime.ForeColor = System.Drawing.Color.DarkGray;
            this.lblTime.Location = new System.Drawing.Point(6, 75);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(280, 20);
            this.lblTime.TabIndex = 2;
            this.lblTime.Text = "时间";
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // contextMenu
            //
            this.contextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.deleteToolStripMenuItem });
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(139, 28);
            //
            // deleteToolStripMenuItem
            //
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(138, 24);
            this.deleteToolStripMenuItem.Text = "删除会话";
            this.deleteToolStripMenuItem.Click += DeleteSession;
            //
            // ChatSessionItem
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ContextMenuStrip = this.contextMenu;
            this.Controls.Add(this.lblPreview);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblTitle);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.Name = "ChatSessionItem";
            this.Padding = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Size = new System.Drawing.Size(292, 85);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblPreview;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    }
}
