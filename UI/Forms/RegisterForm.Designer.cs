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
            txtUsername.Location = new System.Drawing.Point(50, 45);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new System.Drawing.Size(280, 23);
            txtUsername.TabIndex = 1;
            txtUsername.BackColor = System.Drawing.Color.WhiteSmoke;
            txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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
            txtPassword.Location = new System.Drawing.Point(50, 105);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new System.Drawing.Size(280, 23);
            txtPassword.TabIndex = 3;
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.BackColor = System.Drawing.Color.WhiteSmoke;
            txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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
            txtConfirmPassword.Location = new System.Drawing.Point(50, 165);
            txtConfirmPassword.Name = "txtConfirmPassword";
            txtConfirmPassword.Size = new System.Drawing.Size(280, 23);
            txtConfirmPassword.TabIndex = 5;
            txtConfirmPassword.UseSystemPasswordChar = true;
            txtConfirmPassword.BackColor = System.Drawing.Color.WhiteSmoke;
            txtConfirmPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // 
            // btnCreate
            // 
            btnCreate.BackColor = System.Drawing.Color.FromArgb(100, 101, 165);
            btnCreate.FlatAppearance.BorderSize = 0;
            btnCreate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(128)))));
            btnCreate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(128)))));
            btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnCreate.ForeColor = System.Drawing.Color.White;
            btnCreate.Location = new System.Drawing.Point(140, 220);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new System.Drawing.Size(90, 30);
            btnCreate.TabIndex = 6;
            btnCreate.Text = "创建";
            btnCreate.UseVisualStyleBackColor = false;
            btnCreate.Click += btnCreate_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = System.Drawing.Color.Transparent;
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnCancel.ForeColor = System.Drawing.Color.Gray;
            btnCancel.Location = new System.Drawing.Point(240, 220);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(90, 30);
            btnCancel.TabIndex = 7;
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
            this.AcceptButton = this.btnCreate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(384, 281);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.txtConfirmPassword);
            this.Controls.Add(this.lblConfirmPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RegisterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "创建新用户";
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