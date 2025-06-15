using System;
using System.Windows.Forms;

namespace llm_agent.UI.Forms
{
    /// <summary>
    /// 密码验证窗体，用于验证用户密码
    /// </summary>
    public partial class PasswordVerificationForm : Form
    {
        /// <summary>
        /// 获取用户输入的密码
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// 初始化密码验证窗体
        /// </summary>
        /// <param name="username">要验证的用户名</param>
        public PasswordVerificationForm(string username)
        {
            InitializeComponent();
            lblUsername.Text = username;
        }

        /// <summary>
        /// 确认按钮点击事件
        /// </summary>
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblMessage.Text = "请输入密码";
                return;
            }

            Password = txtPassword.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// 取消按钮点击事件
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
} 