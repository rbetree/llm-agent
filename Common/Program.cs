using System;
using System.Windows.Forms;
using llm_agent.UI.Forms;

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
}