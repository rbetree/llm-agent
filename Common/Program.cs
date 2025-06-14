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
            if (!DatabaseConfig.ConfigureMySqlConnection())
            {
                // 如果用户取消了MySQL配置，切换回SQLite
                DatabaseConfig.DatabaseType = DatabaseType.SQLite;
                MessageBox.Show("已切换到SQLite数据库模式。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // 检查并执行数据迁移
            Task.Run(async () => await MigrateDatabaseIfNeededAsync()).Wait();

            Application.Run(new llm_agent.UI.Forms.LlmAgentMainForm());
        }

        /// <summary>
        /// 检查并执行数据库迁移
        /// </summary>
        private static async Task MigrateDatabaseIfNeededAsync()
        {
            try
            {
                var migrationTool = new DatabaseMigrationTool();
                if (migrationTool.NeedsMigration())
                {
                    var result = MessageBox.Show(
                        "检测到SQLite数据库中存在数据，需要迁移到MySQL数据库。是否立即执行数据迁移？",
                        "数据迁移",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        using (var form = new System.Windows.Forms.Form())
                        {
                            form.StartPosition = FormStartPosition.CenterScreen;
                            form.Size = new System.Drawing.Size(300, 100);
                            form.Text = "数据迁移中";
                            form.FormBorderStyle = FormBorderStyle.FixedDialog;
                            form.MaximizeBox = false;
                            form.MinimizeBox = false;
                            form.ControlBox = false;

                            var label = new Label();
                            label.Text = "正在迁移数据，请稍候...";
                            label.AutoSize = true;
                            label.Location = new System.Drawing.Point(20, 20);
                            form.Controls.Add(label);

                            var progressBar = new ProgressBar();
                            progressBar.Style = ProgressBarStyle.Marquee;
                            progressBar.Location = new System.Drawing.Point(20, 40);
                            progressBar.Size = new System.Drawing.Size(260, 20);
                            form.Controls.Add(progressBar);

                            // 在后台执行迁移
                            Task.Run(async () =>
                            {
                                var migrationResult = await migrationTool.MigrateAsync();
                                form.Invoke(new Action(() => form.Close()));

                                MessageBox.Show(
                                    migrationResult.ToString(),
                                    migrationResult.Success ? "迁移成功" : "迁移失败",
                                    MessageBoxButtons.OK,
                                    migrationResult.Success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                            });

                            form.ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"检查数据迁移时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}