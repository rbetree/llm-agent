namespace llm_agent.UI.Controls
{
    partial class CustomTitleBar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomTitleBar));
            lblTitle = new System.Windows.Forms.Label();
            btnClose = new System.Windows.Forms.Button();
            btnMaximize = new System.Windows.Forms.Button();
            btnMinimize = new System.Windows.Forms.Button();
            picLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.Location = new System.Drawing.Point(79, 19);
            lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new System.Drawing.Size(104, 24);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "LLM Agent";
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnClose
            // 
            btnClose.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnClose.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            btnClose.ForeColor = System.Drawing.Color.White;
            btnClose.Location = new System.Drawing.Point(1136, 5);
            btnClose.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(60, 50);
            btnClose.TabIndex = 1;
            btnClose.Text = "✕";
            btnClose.UseVisualStyleBackColor = true;
            // 
            // btnMaximize
            // 
            btnMaximize.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnMaximize.FlatAppearance.BorderSize = 0;
            btnMaximize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnMaximize.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            btnMaximize.ForeColor = System.Drawing.Color.White;
            btnMaximize.Location = new System.Drawing.Point(1082, 5);
            btnMaximize.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnMaximize.Name = "btnMaximize";
            btnMaximize.Size = new System.Drawing.Size(60, 50);
            btnMaximize.TabIndex = 2;
            btnMaximize.Text = "☐";
            btnMaximize.UseVisualStyleBackColor = true;
            // 
            // btnMinimize
            // 
            btnMinimize.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnMinimize.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            btnMinimize.ForeColor = System.Drawing.Color.White;
            btnMinimize.Location = new System.Drawing.Point(1026, 5);
            btnMinimize.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnMinimize.Name = "btnMinimize";
            btnMinimize.Size = new System.Drawing.Size(60, 50);
            btnMinimize.TabIndex = 3;
            btnMinimize.Text = "—";
            btnMinimize.UseVisualStyleBackColor = true;
            // 
            // picLogo
            // 
            picLogo.Image = (System.Drawing.Image)resources.GetObject("picLogo.Image");
            picLogo.Location = new System.Drawing.Point(3, 2);
            picLogo.Name = "picLogo";
            picLogo.Size = new System.Drawing.Size(69, 55);
            picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 4;
            picLogo.TabStop = false;
            // 
            // CustomTitleBar
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(100, 101, 165);
            Controls.Add(picLogo);
            Controls.Add(btnMinimize);
            Controls.Add(btnMaximize);
            Controls.Add(btnClose);
            Controls.Add(lblTitle);
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "CustomTitleBar";
            Size = new System.Drawing.Size(1200, 60);
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnMaximize;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.PictureBox picLogo;
    }
} 