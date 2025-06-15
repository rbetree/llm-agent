using System;
using System.Windows.Forms;
using llm_agent.BLL;
using llm_agent.Common;
using llm_agent.Model;

namespace llm_agent.UI.Forms
{
    public partial class LoginForm : Form
    {
        private readonly UserService _userService;
        private readonly LoggedInUserService _loggedInUserService;
        private bool _isClosingByLogin = false;

        public LoginForm()
        {
            InitializeComponent();
            _userService = new UserService();
            _loggedInUserService = new LoggedInUserService();
        }
        
        /// <summary>
        /// 登录按钮点击事件
        /// </summary>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowMessage("用户名和密码不能为空");
                return;
            }

            try
            {
                // 尝试登录
                var user = _userService.Login(username, password);
                if (user != null)
                {
                    // 登录成功，设置当前用户会话
                    UserSession.Instance.SetCurrentUser(user);
                    
                    _isClosingByLogin = true;
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    ShowMessage("用户名或密码错误");
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"登录失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 注册按钮点击事件
        /// </summary>
        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowMessage("用户名和密码不能为空");
                return;
            }

            if (password.Length < 6)
            {
                ShowMessage("密码长度不能少于6个字符");
                return;
            }

            try
            {
                // 尝试注册新用户
                var user = _userService.RegisterUser(username, password);
                
                // 注册成功后自动登录
                UserSession.Instance.SetCurrentUser(user);
                
                ShowMessage("注册成功，即将进入应用...");
                
                // 延迟关闭窗口，让用户看到成功消息
                Timer timer = new Timer();
                timer.Interval = 1500;
                timer.Tick += (s, args) => 
                {
                    timer.Stop();
                    _isClosingByLogin = true;
                    DialogResult = DialogResult.OK;
                    Close();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                ShowMessage($"注册失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 显示消息
        /// </summary>
        private void ShowMessage(string message)
        {
            lblMessage.Text = message;
        }

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 如果不是通过登录按钮关闭窗口，则认为用户取消了登录，退出应用
            if (!_isClosingByLogin)
            {
                Application.Exit();
            }
        }
        
        /// <summary>
        /// 窗体加载事件
        /// </summary>
        private void LoginForm_Load(object sender, EventArgs e)
        {
            // 检查是否已经有登录用户
            var loggedInUsers = _loggedInUserService.GetLoggedInUsers();
            if (loggedInUsers.Count > 0)
            {
                // 显示已登录用户列表
                ShowLoggedInUsersList(loggedInUsers);
            }
        }
        
        /// <summary>
        /// 显示已登录用户列表
        /// </summary>
        private void ShowLoggedInUsersList(System.Collections.Generic.List<User> loggedInUsers)
        {
            // 在这里可以实现显示已登录用户列表的逻辑
            // 为简化实现，这里不展示复杂的UI，只是在消息标签中提示
            if (loggedInUsers.Count > 0)
            {
                ShowMessage($"已有 {loggedInUsers.Count} 个已登录用户，正在尝试自动登录...");
            }
        }
    }
}