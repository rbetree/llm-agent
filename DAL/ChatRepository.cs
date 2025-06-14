using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.IO;
using System.Text.Json;
using llm_agent.Model;
using MySql.Data.MySqlClient;

namespace llm_agent.DAL
{
    public class ChatRepository
    {
        public ChatRepository()
        {
            // 数据库初始化由DatabaseManager处理
        }

        public void SaveChatSession(ChatSession session)
        {
            if (session == null) return;

            if (DatabaseConfig.DatabaseType == DatabaseType.MySQL)
            {
                SaveChatSessionToMySql(session);
            }
            else
            {
                SaveChatSessionToSqlite(session);
            }
        }

        private void SaveChatSessionToMySql(ChatSession session)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.MySqlConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 检查是否为新会话
                        bool isNewSession = false;
                        using (var command = new MySqlCommand("SELECT COUNT(*) FROM `ChatSessions` WHERE `Id` = @id", connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", session.Id);
                            isNewSession = Convert.ToInt32(command.ExecuteScalar()) == 0;
                        }

                        // 如果是新会话，获取当前最小的OrderIndex值
                        int orderIndex = 0;
                        if (isNewSession)
                        {
                            using (var command = new MySqlCommand("SELECT MIN(`OrderIndex`) FROM `ChatSessions`", connection, transaction))
                            {
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
                                INSERT INTO `ChatSessions` (`Id`, `Title`, `CreatedAt`, `UpdatedAt`, `OrderIndex`)
                                VALUES (@id, @title, @createdAt, @updatedAt, @orderIndex)";
                        }
                        else
                        {
                            upsertSessionSql = @"
                                UPDATE `ChatSessions` SET 
                                `Title` = @title,
                                `UpdatedAt` = @updatedAt
                                WHERE `Id` = @id";
                        }

                        using (var command = new MySqlCommand(upsertSessionSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", session.Id);
                            command.Parameters.AddWithValue("@title", session.Title);
                            command.Parameters.AddWithValue("@createdAt", session.CreatedAt);
                            command.Parameters.AddWithValue("@updatedAt", session.UpdatedAt);
                            
                            if (isNewSession)
                            {
                                command.Parameters.AddWithValue("@orderIndex", orderIndex);
                            }
                            
                            command.ExecuteNonQuery();
                        }

                        // 删除该会话的所有现有消息
                        string deleteMessagesSql = "DELETE FROM `ChatMessages` WHERE `SessionId` = @sessionId";
                        using (var command = new MySqlCommand(deleteMessagesSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@sessionId", session.Id);
                            command.ExecuteNonQuery();
                        }

                        // 插入消息
                        if (session.Messages != null && session.Messages.Count > 0)
                        {
                            string insertMessageSql = @"
                                INSERT INTO `ChatMessages` (`SessionId`, `Role`, `Content`, `Timestamp`)
                                VALUES (@sessionId, @role, @content, @timestamp)";

                            foreach (var message in session.Messages)
                            {
                                using (var command = new MySqlCommand(insertMessageSql, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@sessionId", session.Id);
                                    command.Parameters.AddWithValue("@role", message.Role.ToString());
                                    command.Parameters.AddWithValue("@content", message.Content);
                                    command.Parameters.AddWithValue("@timestamp", message.Timestamp);
                                    command.ExecuteNonQuery();
                                }
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

        private void SaveChatSessionToSqlite(ChatSession session)
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.SqliteConnectionString))
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
                            using (var command = new SQLiteCommand("SELECT MIN(OrderIndex) FROM ChatSessions", connection, transaction))
                            {
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
                                INSERT INTO ChatSessions (Id, Title, CreatedAt, UpdatedAt, OrderIndex)
                                VALUES (@id, @title, @createdAt, @updatedAt, @orderIndex)";
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

                        // 插入消息
                        if (session.Messages != null && session.Messages.Count > 0)
                        {
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

        public ChatSession LoadChatSession(string sessionId)
        {
            return DatabaseConfig.DatabaseType == DatabaseType.MySQL ? 
                LoadChatSessionFromMySql(sessionId) : 
                LoadChatSessionFromSqlite(sessionId);
        }

        private ChatSession LoadChatSessionFromMySql(string sessionId)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.MySqlConnectionString))
            {
                connection.Open();

                // 获取会话信息
                string sessionSql = "SELECT * FROM `ChatSessions` WHERE `Id` = @id";
                ChatSession session = null;

                using (var command = new MySqlCommand(sessionSql, connection))
                {
                    command.Parameters.AddWithValue("@id", sessionId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            session = new ChatSession
                            {
                                Id = reader["Id"].ToString(),
                                Title = reader["Title"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]),
                                OrderIndex = Convert.ToInt32(reader["OrderIndex"])
                            };
                        }
                    }
                }

                if (session == null)
                    return null;

                // 获取消息
                string messagesSql = "SELECT * FROM `ChatMessages` WHERE `SessionId` = @sessionId ORDER BY `Id`";
                session.Messages = new List<ChatMessage>();

                using (var command = new MySqlCommand(messagesSql, connection))
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
                                Timestamp = Convert.ToDateTime(reader["Timestamp"])
                            };
                            session.Messages.Add(message);
                        }
                    }
                }

                return session;
            }
        }

        private ChatSession LoadChatSessionFromSqlite(string sessionId)
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.SqliteConnectionString))
            {
                connection.Open();

                // 获取会话信息
                string sessionSql = "SELECT * FROM ChatSessions WHERE Id = @id";
                ChatSession session = null;

                using (var command = new SQLiteCommand(sessionSql, connection))
                {
                    command.Parameters.AddWithValue("@id", sessionId);
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
                                OrderIndex = Convert.ToInt32(reader["OrderIndex"])
                            };
                        }
                    }
                }

                if (session == null)
                    return null;

                // 获取消息
                string messagesSql = "SELECT * FROM ChatMessages WHERE SessionId = @sessionId ORDER BY Id";
                session.Messages = new List<ChatMessage>();

                using (var command = new SQLiteCommand(messagesSql, connection))
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

                return session;
            }
        }

        public List<ChatSession> GetAllSessions()
        {
            return DatabaseConfig.DatabaseType == DatabaseType.MySQL ? 
                GetAllSessionsFromMySql() : 
                GetAllSessionsFromSqlite();
        }

        private List<ChatSession> GetAllSessionsFromMySql()
        {
            List<ChatSession> sessions = new List<ChatSession>();

            using (var connection = new MySqlConnection(DatabaseConfig.MySqlConnectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM `ChatSessions` ORDER BY `OrderIndex`";

                using (var command = new MySqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var session = new ChatSession
                            {
                                Id = reader["Id"].ToString(),
                                Title = reader["Title"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]),
                                OrderIndex = Convert.ToInt32(reader["OrderIndex"]),
                                Messages = new List<ChatMessage>()
                            };
                            sessions.Add(session);
                        }
                    }
                }
            }

            return sessions;
        }

        private List<ChatSession> GetAllSessionsFromSqlite()
        {
            List<ChatSession> sessions = new List<ChatSession>();

            using (var connection = new SQLiteConnection(DatabaseConfig.SqliteConnectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM ChatSessions ORDER BY OrderIndex";

                using (var command = new SQLiteCommand(sql, connection))
                {
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
                                OrderIndex = Convert.ToInt32(reader["OrderIndex"]),
                                Messages = new List<ChatMessage>()
                            };
                            sessions.Add(session);
                        }
                    }
                }
            }

            return sessions;
        }

        public void DeleteSession(string sessionId)
        {
            if (DatabaseConfig.DatabaseType == DatabaseType.MySQL)
            {
                DeleteSessionFromMySql(sessionId);
            }
            else
            {
                DeleteSessionFromSqlite(sessionId);
            }
        }

        private void DeleteSessionFromMySql(string sessionId)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.MySqlConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string sql = "DELETE FROM `ChatSessions` WHERE `Id` = @id";
                        using (var command = new MySqlCommand(sql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", sessionId);
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

        private void DeleteSessionFromSqlite(string sessionId)
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.SqliteConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string sql = "DELETE FROM ChatSessions WHERE Id = @id";
                        using (var command = new SQLiteCommand(sql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", sessionId);
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

        public void DeleteAllSessions()
        {
            if (DatabaseConfig.DatabaseType == DatabaseType.MySQL)
            {
                DeleteAllSessionsFromMySql();
            }
            else
            {
                DeleteAllSessionsFromSqlite();
            }
        }

        private void DeleteAllSessionsFromMySql()
        {
            using (var connection = new MySqlConnection(DatabaseConfig.MySqlConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 由于外键约束，先删除消息，再删除会话
                        string deleteMessagesSql = "DELETE FROM `ChatMessages`";
                        using (var command = new MySqlCommand(deleteMessagesSql, connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        string deleteSessionsSql = "DELETE FROM `ChatSessions`";
                        using (var command = new MySqlCommand(deleteSessionsSql, connection, transaction))
                        {
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

        private void DeleteAllSessionsFromSqlite()
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.SqliteConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 由于外键约束，先删除消息，再删除会话
                        string deleteMessagesSql = "DELETE FROM ChatMessages";
                        using (var command = new SQLiteCommand(deleteMessagesSql, connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        string deleteSessionsSql = "DELETE FROM ChatSessions";
                        using (var command = new SQLiteCommand(deleteSessionsSql, connection, transaction))
                        {
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

        public void UpdateSessionOrder(List<ChatSession> sessions)
        {
            if (sessions == null || sessions.Count == 0)
                return;

            if (DatabaseConfig.DatabaseType == DatabaseType.MySQL)
            {
                UpdateSessionOrderInMySql(sessions);
            }
            else
            {
                UpdateSessionOrderInSqlite(sessions);
            }
        }

        private void UpdateSessionOrderInMySql(List<ChatSession> sessions)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.MySqlConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string sql = "UPDATE `ChatSessions` SET `OrderIndex` = @orderIndex WHERE `Id` = @id";

                        for (int i = 0; i < sessions.Count; i++)
                        {
                            using (var command = new MySqlCommand(sql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@orderIndex", i);
                                command.Parameters.AddWithValue("@id", sessions[i].Id);
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

        private void UpdateSessionOrderInSqlite(List<ChatSession> sessions)
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.SqliteConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string sql = "UPDATE ChatSessions SET OrderIndex = @orderIndex WHERE Id = @id";

                        for (int i = 0; i < sessions.Count; i++)
                        {
                            using (var command = new SQLiteCommand(sql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@orderIndex", i);
                                command.Parameters.AddWithValue("@id", sessions[i].Id);
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
    }
}