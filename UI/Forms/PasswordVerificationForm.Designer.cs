namespace llm_agent.UI.Forms
{
    partial class PasswordVerificationForm
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
            lblTitle = new System.Windows.Forms.Label();
            lblUsernameTitle = new System.Windows.Forms.Label();
            lblUsername = new System.Windows.Forms.Label();
            lblPasswordTitle = new System.Windows.Forms.Label();
            txtPassword = new System.Windows.Forms.TextBox();
            btnConfirm = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            lblMessage = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(43, 18);
            lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new System.Drawing.Size(92, 27);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "密码验证";
            // 
            // lblUsernameTitle
            // 
            lblUsernameTitle.AutoSize = true;
            lblUsernameTitle.Location = new System.Drawing.Point(90, 61);
            lblUsernameTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblUsernameTitle.Name = "lblUsernameTitle";
            lblUsernameTitle.Size = new System.Drawing.Size(69, 20);
            lblUsernameTitle.TabIndex = 1;
            lblUsernameTitle.Text = "用户名：";
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            lblUsername.Location = new System.Drawing.Point(154, 62);
            lblUsername.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new System.Drawing.Size(54, 19);
            lblUsername.TabIndex = 2;
            lblUsername.Text = "用户名";
            // 
            // lblPasswordTitle
            // 
            lblPasswordTitle.AutoSize = true;
            lblPasswordTitle.Location = new System.Drawing.Point(90, 123);
            lblPasswordTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPasswordTitle.Name = "lblPasswordTitle";
            lblPasswordTitle.Size = new System.Drawing.Size(54, 20);
            lblPasswordTitle.TabIndex = 3;
            lblPasswordTitle.Text = "密码：";
            // 
            // txtPassword
            // 
            txtPassword.BackColor = System.Drawing.Color.WhiteSmoke;
            txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            txtPassword.Location = new System.Drawing.Point(154, 121);
            txtPassword.Margin = new System.Windows.Forms.Padding(4);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new System.Drawing.Size(245, 27);
            txtPassword.TabIndex = 1;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // btnConfirm
            // 
            btnConfirm.BackColor = System.Drawing.Color.FromArgb(100, 101, 165);
            btnConfirm.FlatAppearance.BorderSize = 0;
            btnConfirm.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(76, 76, 128);
            btnConfirm.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(76, 76, 128);
            btnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnConfirm.ForeColor = System.Drawing.Color.White;
            btnConfirm.Location = new System.Drawing.Point(154, 176);
            btnConfirm.Margin = new System.Windows.Forms.Padding(4);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new System.Drawing.Size(116, 35);
            btnConfirm.TabIndex = 2;
            btnConfirm.Text = "确认";
            btnConfirm.UseVisualStyleBackColor = false;
            btnConfirm.Click += btnConfirm_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = System.Drawing.Color.Transparent;
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnCancel.ForeColor = System.Drawing.Color.Gray;
            btnCancel.Location = new System.Drawing.Point(296, 176);
            btnCancel.Margin = new System.Windows.Forms.Padding(4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(116, 35);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Location = new System.Drawing.Point(90, 143);
            lblMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new System.Drawing.Size(0, 20);
            lblMessage.TabIndex = 7;
            // 
            // PasswordVerificationForm
            // 
            AcceptButton = btnConfirm;
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(492, 224);
            Controls.Add(lblMessage);
            Controls.Add(btnCancel);
            Controls.Add(btnConfirm);
            Controls.Add(txtPassword);
            Controls.Add(lblPasswordTitle);
            Controls.Add(lblUsername);
            Controls.Add(lblUsernameTitle);
            Controls.Add(lblTitle);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PasswordVerificationForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "密码验证";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblUsernameTitle;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblPasswordTitle;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblMessage;
    }
}