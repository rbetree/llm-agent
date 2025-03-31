using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace llm_agent.Properties 
{
    public class Settings
    {
        private static Settings _default;
        private static readonly string DbName = "llm_agent.db";
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbName);
        private static readonly string ConnectionString = $"Data Source={DbPath};Version=3;";
        
        public static Settings Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new Settings();
                    _default.InitializeDatabase();
                    _default.LoadSettings();
                }
                return _default;
            }
        }
        
        // 配置项
        public string SystemPrompt { get; set; } = "";
        public bool EnableStreamResponse { get; set; } = true;
        public bool EnableMarkdown { get; set; } = true;
        public string LastSelectedProvider { get; set; } = "OpenAI";
        public string ProviderType { get; set; } = "OpenAI";
        public string LastSelectedModel { get; set; } = "";
        public string OpenAIApiKey { get; set; } = "";
        public string OpenAIApiHost { get; set; } = "";
        public string AzureApiKey { get; set; } = "";
        public string AzureApiHost { get; set; } = "";
        public string AnthropicApiKey { get; set; } = "";
        public string AnthropicApiHost { get; set; } = "";
        public string GeminiApiKey { get; set; } = "";
        public string GeminiApiHost { get; set; } = "";
        public string ZhipuApiKey { get; set; } = "";
        public string ZhipuApiHost { get; set; } = "";
        public string SiliconFlowApiKey { get; set; } = "";
        public string SiliconFlowApiHost { get; set; } = "";
        public string OtherApiKey { get; set; } = "";
        public string OtherApiHost { get; set; } = "";
        
        private void InitializeDatabase()
        {
            try
            {
                if (!File.Exists(DbPath))
                {
                    SQLiteConnection.CreateFile(DbPath);
                }

                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    // 创建设置表
                    string createSettingsTableSql = @"
                        CREATE TABLE IF NOT EXISTS UserSettings (
                            Key TEXT PRIMARY KEY,
                            Value TEXT
                        );";

                    using (var command = new SQLiteCommand(createSettingsTableSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"初始化设置数据库时出错: {ex.Message}");
            }
        }
        
        private void LoadSettings()
        {
            try
            {
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    string selectAllSql = "SELECT Key, Value FROM UserSettings";

                    using (var command = new SQLiteCommand(selectAllSql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string key = reader["Key"].ToString();
                                string value = reader["Value"].ToString();
                                
                                // 使用反射设置属性值
                                PropertyInfo prop = GetType().GetProperty(key);
                                if (prop != null)
                                {
                                    if (prop.PropertyType == typeof(string))
                                    {
                                        prop.SetValue(this, value);
                                    }
                                    else if (prop.PropertyType == typeof(bool))
                                    {
                                        prop.SetValue(this, bool.Parse(value));
                                    }
                                    else if (prop.PropertyType == typeof(int))
                                    {
                                        prop.SetValue(this, int.Parse(value));
                                    }
                                    else if (prop.PropertyType == typeof(double))
                                    {
                                        prop.SetValue(this, double.Parse(value));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"加载设置时出错: {ex.Message}");
            }
        }
        
        // 保存设置
        public void Save()
        {
            try
            {
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 获取所有公共属性
                            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                            
                            // 为每个属性保存值
                            foreach (var prop in properties)
                            {
                                string key = prop.Name;
                                object value = prop.GetValue(this);
                                
                                // 跳过null值
                                if (value == null)
                                    continue;
                                
                                string stringValue = value.ToString();
                                
                                // Upsert设置
                                string upsertSql = @"
                                    INSERT INTO UserSettings (Key, Value)
                                    VALUES (@key, @value)
                                    ON CONFLICT(Key) DO UPDATE SET 
                                    Value = @value";
                                
                                using (var command = new SQLiteCommand(upsertSql, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@key", key);
                                    command.Parameters.AddWithValue("@value", stringValue);
                                    command.ExecuteNonQuery();
                                }
                            }
                            
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"保存设置时出错: {ex.Message}");
            }
        }
    }
} 