namespace llm_agent.UI.Forms
{
    partial class RegisterForm
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
            lblUsername = new System.Windows.Forms.Label();
            txtUsername = new System.Windows.Forms.TextBox();
            lblPassword = new System.Windows.Forms.Label();
            txtPassword = new System.Windows.Forms.TextBox();
            lblConfirmPassword = new System.Windows.Forms.Label();
            txtConfirmPassword = new System.Windows.Forms.TextBox();
            btnCreate = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            lblTitle = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Location = new System.Drawing.Point(34, 70);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new System.Drawing.Size(69, 20);
            lblUsername.TabIndex = 1;
            lblUsername.Text = "用户名：";
            // 
            // txtUsername
            // 
            txtUsername.Location = new System.Drawing.Point(34, 95);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new System.Drawing.Size(337, 27);
            txtUsername.TabIndex = 2;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new System.Drawing.Point(34, 135);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new System.Drawing.Size(54, 20);
            lblPassword.TabIndex = 3;
            lblPassword.Text = "密码：";
            // 
            // txtPassword
            // 
            txtPassword.Location = new System.Drawing.Point(34, 160);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new System.Drawing.Size(337, 27);
            txtPassword.TabIndex = 4;
            // 
            // lblConfirmPassword
            // 
            lblConfirmPassword.AutoSize = true;
            lblConfirmPassword.Location = new System.Drawing.Point(34, 200);
            lblConfirmPassword.Name = "lblConfirmPassword";
            lblConfirmPassword.Size = new System.Drawing.Size(84, 20);
            lblConfirmPassword.TabIndex = 5;
            lblConfirmPassword.Text = "确认密码：";
            // 
            // txtConfirmPassword
            // 
            txtConfirmPassword.Location = new System.Drawing.Point(34, 225);
            txtConfirmPassword.Name = "txtConfirmPassword";
            txtConfirmPassword.PasswordChar = '*';
            txtConfirmPassword.Size = new System.Drawing.Size(337, 27);
            txtConfirmPassword.TabIndex = 6;
            // 
            // btnCreate
            // 
            btnCreate.Location = new System.Drawing.Point(79, 275);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new System.Drawing.Size(106, 29);
            btnCreate.TabIndex = 7;
            btnCreate.Text = "创建";
            btnCreate.UseVisualStyleBackColor = true;
            btnCreate.Click += btnCreate_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(214, 275);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(106, 29);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(34, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new System.Drawing.Size(112, 27);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "创建新用户";
            // 
            // RegisterForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(405, 330);
            Controls.Add(btnCancel);
            Controls.Add(btnCreate);
            Controls.Add(txtConfirmPassword);
            Controls.Add(lblConfirmPassword);
            Controls.Add(txtPassword);
            Controls.Add(lblPassword);
            Controls.Add(txtUsername);
            Controls.Add(lblUsername);
            Controls.Add(lblTitle);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RegisterForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "创建新用户";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblConfirmPassword;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnCancel;
    }
} 