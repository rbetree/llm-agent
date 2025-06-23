using System;
using System.Windows.Forms;
using llm_agent.UI.Forms;
using llm_agent.BLL;
using llm_agent.Common.Utils;

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

            // 迁移现有API密钥到加密存储
            MigrateApiKeysToEncrypted();

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

        /// <summary>
        /// 迁移现有API密钥到加密存储
        /// </summary>
        private static void MigrateApiKeysToEncrypted()
        {
            try
            {
                // 验证加密功能是否正常
                if (!EncryptionHelper.ValidateEncryption())
                {
                    Console.Error.WriteLine("警告: 加密功能验证失败，API密钥可能无法正确加密");
                    return;
                }

                // 迁移Settings中的API密钥
                SettingsHelper.MigrateApiKeysToEncrypted();

                // 验证设置加密功能
                if (!SettingsHelper.ValidateEncryptionSettings())
                {
                    Console.Error.WriteLine("警告: 设置加密功能验证失败");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"迁移API密钥到加密存储时出错: {ex.Message}");
                // 不显示MessageBox，因为这不是致命错误，应用程序可以继续运行
            }
        }
    }
}