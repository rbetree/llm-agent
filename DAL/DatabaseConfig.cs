using System;
using System.IO;
using System.Windows.Forms;

namespace llm_agent.DAL
{
    /// <summary>
    /// 数据库配置类，用于管理数据库连接信息
    /// </summary>
    public static class DatabaseConfig
    {
        // 数据库类型：MySQL或SQLite
        public static DatabaseType DatabaseType { get; set; } = DatabaseType.MySQL;
        
        // SQLite配置
        private static readonly string SqliteDbName = "llm_agent.db";
        private static readonly string SqliteDbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SqliteDbName);
        public static string SqliteConnectionString => $"Data Source={SqliteDbPath};Version=3;";
        
        // MySQL配置
        private static string _mysqlServer = "localhost";
        private static string _mysqlPort = "3306";
        private static string _mysqlDatabase = "llm_agent";
        private static string _mysqlUsername = "root";
        private static string _mysqlPassword = "13826861561"; // 用户设置的密码
        
        public static string MySqlServer 
        { 
            get => _mysqlServer; 
            set => _mysqlServer = value; 
        }
        
        public static string MySqlPort 
        { 
            get => _mysqlPort; 
            set => _mysqlPort = value; 
        }
        
        public static string MySqlDatabase 
        { 
            get => _mysqlDatabase; 
            set => _mysqlDatabase = value; 
        }
        
        public static string MySqlUsername 
        { 
            get => _mysqlUsername; 
            set => _mysqlUsername = value; 
        }
        
        public static string MySqlPassword 
        { 
            get => _mysqlPassword; 
            set => _mysqlPassword = value; 
        }
        
        public static string MySqlConnectionString => 
            $"Server={_mysqlServer};Port={_mysqlPort};Database={_mysqlDatabase};Uid={_mysqlUsername};Pwd={_mysqlPassword};CharSet=utf8mb4;";
        
        // 获取当前使用的数据库连接字符串
        public static string GetConnectionString()
        {
            return DatabaseType == DatabaseType.MySQL ? MySqlConnectionString : SqliteConnectionString;
        }
        
        // 获取SQLite数据库文件路径
        public static string GetSqliteDbPath()
        {
            return SqliteDbPath;
        }
        
        /// <summary>
        /// 提示用户输入MySQL连接信息
        /// </summary>
        /// <returns>是否成功配置</returns>
        public static bool ConfigureMySqlConnection()
        {
            if (DatabaseType != DatabaseType.MySQL)
                return true;
                
            // 已经设置了密码，不需要再次提示
            return true;
        }
    }
    
    /// <summary>
    /// 数据库类型枚举
    /// </summary>
    public enum DatabaseType
    {
        SQLite,
        MySQL
    }
} 