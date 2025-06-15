using System;
using System.Windows.Forms;
using llm_agent.UI.Forms;
using llm_agent.BLL;

namespace llm_agent.Common
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            // 初始化管理员账号
            InitializeAdminAccount();

            bool autoLoginSuccess = false;
            try
            {
                // 尝试自动登录
                autoLoginSuccess = UserSession.Instance.TryAutoLogin();
            }
            catch (Exception ex)
            {
                // 自动登录失败时显示错误，但程序继续执行以显示登录窗体
                MessageBox.Show($"自动登录时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // 如果自动登录成功，直接启动主窗体
            if (autoLoginSuccess)
            {
                Application.Run(new llm_agent.UI.Forms.LlmAgentMainForm());
            }
            else
            {
                // 显示登录窗体
                using (var loginForm = new LoginForm())
                {
                    // 如果登录成功，则启动主窗体
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        Application.Run(new llm_agent.UI.Forms.LlmAgentMainForm());
                    }
                    // 如果登录取消，应用会在LoginForm的FormClosing事件中退出
                }
            }
        }

        /// <summary>
        /// 初始化管理员账号
        /// </summary>
        private static void InitializeAdminAccount()
        {
            try
            {
                // 创建用户服务
                var userService = new UserService();
                
                // 确保管理员账号存在
                userService.EnsureAdminExists("admin", "admin9");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"初始化管理员账号时出错: {ex.Message}");
                MessageBox.Show($"初始化管理员账号时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}