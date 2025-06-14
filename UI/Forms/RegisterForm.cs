using System;
using System.Windows.Forms;
using llm_agent.BLL;
using llm_agent.Model;

namespace llm_agent.UI.Forms
{
    public partial class RegisterForm : Form
    {
        private readonly UserService _userService;

        public RegisterForm()
        {
            InitializeComponent();
            _userService = new UserService();
        }

        /// <summary>
        /// 创建按钮点击事件
        /// </summary>
        private void btnCreate_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            // 验证输入
            if (string.IsNullOrWhiteSpace(username))
            {
                ShowMessage("用户名不能为空");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowMessage("密码不能为空");
                return;
            }

            if (password.Length < 6)
            {
                ShowMessage("密码长度不能少于6个字符");
                return;
            }

            if (password != confirmPassword)
            {
                ShowMessage("两次输入的密码不一致");
                return;
            }

            try
            {
                // 尝试创建用户
                var user = _userService.RegisterUser(username, password);
                if (user != null)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"创建用户失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 取消按钮点击事件
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// 显示消息
        /// </summary>
        private void ShowMessage(string message)
        {
            MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
} 