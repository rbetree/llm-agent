using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text.Json;
using llm_agent.Model;

namespace llm_agent.DAL
{
    public class ChatRepository
    {
        private static readonly string DbName = "llm_agent.db";
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbName);
        private static readonly string ConnectionString = $"Data Source={DbPath};Version=3;";

        public ChatRepository()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(DbPath))
            {
                SQLiteConnection.CreateFile(DbPath);
            }

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // 创建聊天会话表
                string createSessionTableSql = @"
                    CREATE TABLE IF NOT EXISTS ChatSessions (
                        Id TEXT PRIMARY KEY,
                        Title TEXT NOT NULL,
                        CreatedAt TEXT NOT NULL,
                        UpdatedAt TEXT NOT NULL,
                        OrderIndex INTEGER DEFAULT 0,
                        UserId TEXT DEFAULT NULL REFERENCES Users(Id)
                    );";

                // 创建聊天消息表
                string createMessagesTableSql = @"
                    CREATE TABLE IF NOT EXISTS ChatMessages (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        SessionId TEXT NOT NULL,
                        Role TEXT NOT NULL,
                        Content TEXT NOT NULL,
                        Timestamp TEXT NOT NULL,
                        FOREIGN KEY (SessionId) REFERENCES ChatSessions(Id) ON DELETE CASCADE
                    );";

                using (var command = new SQLiteCommand(createSessionTableSql, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SQLiteCommand(createMessagesTableSql, connection))
                {
                    command.ExecuteNonQuery();
                }

                // 检查ChatSessions表中是否存在OrderIndex字段
                bool orderIndexExists = false;
                using (var command = new SQLiteCommand("PRAGMA table_info(ChatSessions);", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader["name"].ToString();
                            if (columnName == "OrderIndex")
                            {
                                orderIndexExists = true;
                                break;
                            }
                        }
                    }
                }

                // 如果OrderIndex字段不存在，则添加该字段并初始化值
                if (!orderIndexExists)
                {
                    // 添加OrderIndex字段
                    using (var command = new SQLiteCommand("ALTER TABLE ChatSessions ADD COLUMN OrderIndex INTEGER DEFAULT 0;", connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // 根据UpdatedAt初始化OrderIndex值（越新的会话排序值越小，显示在越前面）
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 获取按UpdatedAt倒序排列的会话ID
                            var sessionIds = new List<string>();
                            using (var command = new SQLiteCommand("SELECT Id FROM ChatSessions ORDER BY UpdatedAt DESC;", connection, transaction))
                            {
                                using (var reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        sessionIds.Add(reader["Id"].ToString());
                                    }
                                }
                            }

                            // 更新每个会话的OrderIndex
                            for (int i = 0; i < sessionIds.Count; i++)
                            {
                                using (var command = new SQLiteCommand("UPDATE ChatSessions SET OrderIndex = @orderIndex WHERE Id = @id;", connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@orderIndex", i);
                                    command.Parameters.AddWithValue("@id", sessionIds[i]);
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

                // 检查ChatSessions表中是否存在UserId字段
                bool userIdExists = false;
                using (var command = new SQLiteCommand("PRAGMA table_info(ChatSessions);", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader["name"].ToString();
                            if (columnName == "UserId")
                            {
                                userIdExists = true;
                                break;
                            }
                        }
                    }
                }

                // 如果UserId字段不存在，则添加该字段
                if (!userIdExists)
                {
                    using (var command = new SQLiteCommand("ALTER TABLE ChatSessions ADD COLUMN UserId TEXT DEFAULT NULL REFERENCES Users(Id);", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void SaveChatSession(ChatSession session, string userId = null)
        {
            if (session == null) return;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 检查是否为新会话
                        bool isNewSession = false;
                        using (var command = new SQLiteCommand("SELECT COUNT(*) FROM ChatSessions WHERE Id = @id", connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", session.Id);
                            isNewSession = Convert.ToInt32(command.ExecuteScalar()) == 0;
                        }

                        // 如果是新会话，获取当前最小的OrderIndex值
                        int orderIndex = 0;
                        if (isNewSession)
                        {
                            string orderIndexSql = "SELECT MIN(OrderIndex) FROM ChatSessions";
                            if (!string.IsNullOrEmpty(userId))
                            {
                                orderIndexSql += " WHERE UserId = @userId";
                            }

                            using (var command = new SQLiteCommand(orderIndexSql, connection, transaction))
                            {
                                if (!string.IsNullOrEmpty(userId))
                                {
                                    command.Parameters.AddWithValue("@userId", userId);
                                }

                                var result = command.ExecuteScalar();
                                if (result != DBNull.Value && result != null)
                                {
                                    // 将新会话的OrderIndex设为最小值减1，这样它会显示在最前面
                                    orderIndex = Convert.ToInt32(result) - 1;
                                }
                                else
                                {
                                    // 如果没有现有会话，设置为0
                                    orderIndex = 0;
                                }
                            }
                        }

                        // 保存或更新会话信息
                        string upsertSessionSql;
                        if (isNewSession)
                        {
                            upsertSessionSql = @"
                                INSERT INTO ChatSessions (Id, Title, CreatedAt, UpdatedAt, OrderIndex, UserId)
                                VALUES (@id, @title, @createdAt, @updatedAt, @orderIndex, @userId)";
                        }
                        else
                        {
                            upsertSessionSql = @"
                                UPDATE ChatSessions SET 
                                Title = @title,
                                UpdatedAt = @updatedAt
                                WHERE Id = @id";
                        }

                        using (var command = new SQLiteCommand(upsertSessionSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", session.Id);
                            command.Parameters.AddWithValue("@title", session.Title);
                            command.Parameters.AddWithValue("@createdAt", session.CreatedAt.ToString("o"));
                            command.Parameters.AddWithValue("@updatedAt", session.UpdatedAt.ToString("o"));
                            
                            if (isNewSession)
                            {
                                command.Parameters.AddWithValue("@orderIndex", orderIndex);
                                command.Parameters.AddWithValue("@userId", userId == null ? DBNull.Value : userId);
                            }
                            
                            command.ExecuteNonQuery();
                        }

                        // 删除该会话的所有现有消息
                        string deleteMessagesSql = "DELETE FROM ChatMessages WHERE SessionId = @sessionId";
                        using (var command = new SQLiteCommand(deleteMessagesSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@sessionId", session.Id);
                            command.ExecuteNonQuery();
                        }

                        // 插入所有消息
                        string insertMessageSql = @"
                            INSERT INTO ChatMessages (SessionId, Role, Content, Timestamp)
                            VALUES (@sessionId, @role, @content, @timestamp)";

                        foreach (var message in session.Messages)
                        {
                            using (var command = new SQLiteCommand(insertMessageSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@sessionId", session.Id);
                                command.Parameters.AddWithValue("@role", message.Role.ToString());
                                command.Parameters.AddWithValue("@content", message.Content);
                                command.Parameters.AddWithValue("@timestamp", message.Timestamp.ToString("o"));
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

        public ChatSession LoadChatSession(string sessionId, string userId = null)
        {
            if (string.IsNullOrEmpty(sessionId))
                return null;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // 获取会话信息，如果提供了用户ID，则验证会话所有权
                string selectSessionSql = "SELECT * FROM ChatSessions WHERE Id = @sessionId";
                if (!string.IsNullOrEmpty(userId))
                {
                    selectSessionSql += " AND (UserId = @userId OR UserId IS NULL)";
                }

                ChatSession session = null;

                using (var command = new SQLiteCommand(selectSessionSql, connection))
                {
                    command.Parameters.AddWithValue("@sessionId", sessionId);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            session = new ChatSession
                            {
                                Id = reader["Id"].ToString(),
                                Title = reader["Title"].ToString(),
                                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()),
                                UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString()),
                                Messages = new List<ChatMessage>()
                            };
                        }
                        else
                        {
                            return null; // 会话不存在或不属于该用户
                        }
                    }
                }

                // 获取会话的所有消息
                if (session != null)
                {
                    string selectMessagesSql = "SELECT * FROM ChatMessages WHERE SessionId = @sessionId ORDER BY Timestamp";

                    using (var command = new SQLiteCommand(selectMessagesSql, connection))
                    {
                        command.Parameters.AddWithValue("@sessionId", sessionId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var message = new ChatMessage
                                {
                                    Role = Enum.Parse<ChatRole>(reader["Role"].ToString()),
                                    Content = reader["Content"].ToString(),
                                    Timestamp = DateTime.Parse(reader["Timestamp"].ToString())
                                };

                                session.Messages.Add(message);
                            }
                        }
                    }
                }

                return session;
            }
        }

        public List<ChatSession> GetAllSessions(string userId = null)
        {
            List<ChatSession> sessions = new List<ChatSession>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // 获取所有会话的基本信息，如果提供了用户ID，则只获取该用户的会话
                string selectSessionsSql = "SELECT * FROM ChatSessions";
                if (!string.IsNullOrEmpty(userId))
                {
                    selectSessionsSql += " WHERE UserId = @userId OR UserId IS NULL";
                }
                selectSessionsSql += " ORDER BY OrderIndex ASC";

                using (var command = new SQLiteCommand(selectSessionsSql, connection))
                {
                    if (!string.IsNullOrEmpty(userId))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var session = new ChatSession
                            {
                                Id = reader["Id"].ToString(),
                                Title = reader["Title"].ToString(),
                                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()),
                                UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString()),
                                Messages = new List<ChatMessage>()
                            };

                            sessions.Add(session);
                        }
                    }
                }

                // 对于每个会话，获取最后一条消息作为预览
                foreach (var session in sessions)
                {
                    string selectLastMessageSql = "SELECT * FROM ChatMessages WHERE SessionId = @sessionId ORDER BY Timestamp DESC LIMIT 1";

                    using (var command = new SQLiteCommand(selectLastMessageSql, connection))
                    {
                        command.Parameters.AddWithValue("@sessionId", session.Id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var message = new ChatMessage
                                {
                                    Role = Enum.Parse<ChatRole>(reader["Role"].ToString()),
                                    Content = reader["Content"].ToString(),
                                    Timestamp = DateTime.Parse(reader["Timestamp"].ToString())
                                };

                                session.Messages.Add(message);
                            }
                        }
                    }
                }
            }

            return sessions;
        }

        public void DeleteSession(string sessionId, string userId = null)
        {
            if (string.IsNullOrEmpty(sessionId))
                return;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 如果提供了用户ID，验证会话所有权
                        if (!string.IsNullOrEmpty(userId))
                        {
                            string checkOwnershipSql = "SELECT COUNT(*) FROM ChatSessions WHERE Id = @sessionId AND (UserId = @userId OR UserId IS NULL)";
                            using (var command = new SQLiteCommand(checkOwnershipSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@sessionId", sessionId);
                                command.Parameters.AddWithValue("@userId", userId);
                                int count = Convert.ToInt32(command.ExecuteScalar());
                                if (count == 0)
                                {
                                    // 会话不属于该用户，不执行删除
                                    return;
                                }
                            }
                        }

                        // 删除消息
                        string deleteMessagesSql = "DELETE FROM ChatMessages WHERE SessionId = @sessionId";
                        using (var command = new SQLiteCommand(deleteMessagesSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@sessionId", sessionId);
                            command.ExecuteNonQuery();
                        }

                        // 删除会话
                        string deleteSessionSql = "DELETE FROM ChatSessions WHERE Id = @sessionId";
                        using (var command = new SQLiteCommand(deleteSessionSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@sessionId", sessionId);
                            command.ExecuteNonQuery();
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

        public void DeleteAllSessions(string userId = null)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(userId))
                        {
                            // 删除所有消息
                            string deleteAllMessagesSql = "DELETE FROM ChatMessages";
                            using (var command = new SQLiteCommand(deleteAllMessagesSql, connection, transaction))
                            {
                                command.ExecuteNonQuery();
                            }

                            // 删除所有会话
                            string deleteAllSessionsSql = "DELETE FROM ChatSessions";
                            using (var command = new SQLiteCommand(deleteAllSessionsSql, connection, transaction))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // 获取该用户的所有会话ID
                            List<string> sessionIds = new List<string>();
                            string selectSessionIdsSql = "SELECT Id FROM ChatSessions WHERE UserId = @userId OR UserId IS NULL";
                            using (var command = new SQLiteCommand(selectSessionIdsSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@userId", userId);
                                using (var reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        sessionIds.Add(reader["Id"].ToString());
                                    }
                                }
                            }

                            // 删除这些会话的所有消息
                            foreach (var sessionId in sessionIds)
                            {
                                string deleteMessagesSql = "DELETE FROM ChatMessages WHERE SessionId = @sessionId";
                                using (var command = new SQLiteCommand(deleteMessagesSql, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@sessionId", sessionId);
                                    command.ExecuteNonQuery();
                                }
                            }

                            // 删除该用户的所有会话
                            string deleteSessionsSql = "DELETE FROM ChatSessions WHERE UserId = @userId OR UserId IS NULL";
                            using (var command = new SQLiteCommand(deleteSessionsSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@userId", userId);
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
        
        /// <summary>
        /// 更新会话的排序顺序
        /// </summary>
        /// <param name="sessions">会话列表，按照期望的显示顺序排列</param>
        /// <param name="userId">用户ID，用于验证会话所有权</param>
        public void UpdateSessionOrder(List<ChatSession> sessions, string userId = null)
        {
            if (sessions == null || sessions.Count == 0)
                return;
                
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string updateOrderSql = "UPDATE ChatSessions SET OrderIndex = @orderIndex WHERE Id = @id";
                        if (!string.IsNullOrEmpty(userId))
                        {
                            updateOrderSql += " AND (UserId = @userId OR UserId IS NULL)";
                        }
                        
                        for (int i = 0; i < sessions.Count; i++)
                        {
                            using (var command = new SQLiteCommand(updateOrderSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@orderIndex", i);
                                command.Parameters.AddWithValue("@id", sessions[i].Id);
                                if (!string.IsNullOrEmpty(userId))
                                {
                                    command.Parameters.AddWithValue("@userId", userId);
                                }
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

        /// <summary>
        /// 将无主会话分配给指定用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        public void AssignOrphanedSessionsToUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string updateSessionsSql = "UPDATE ChatSessions SET UserId = @userId WHERE UserId IS NULL";
                        using (var command = new SQLiteCommand(updateSessionsSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@userId", userId);
                            command.ExecuteNonQuery();
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
    }
}