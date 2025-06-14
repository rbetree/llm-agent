using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using llm_agent.DAL;

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

            // 配置MySQL连接
            DatabaseConfig.ConfigureMySqlConnection();

            Application.Run(new llm_agent.UI.Forms.LlmAgentMainForm());
        }
    }
}