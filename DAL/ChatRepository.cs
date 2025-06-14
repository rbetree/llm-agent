using System;
using System.Collections.Generic;
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

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
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

        public ChatSession LoadChatSession(string sessionId)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();
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
                                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()),
                                UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString()),
                                Messages = new List<ChatMessage>()
                            };
                        }
                    }
                }

                if (session != null)
                {
                    // 加载会话消息
                    string messagesSql = "SELECT * FROM `ChatMessages` WHERE `SessionId` = @sessionId ORDER BY `Id`";
                    using (var command = new MySqlCommand(messagesSql, connection))
                    {
                        command.Parameters.AddWithValue("@sessionId", sessionId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var message = new ChatMessage
                                {
                                    Role = (ChatRole)Enum.Parse(typeof(ChatRole), reader["Role"].ToString()),
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

        public List<ChatSession> GetAllSessions()
        {
            List<ChatSession> sessions = new List<ChatSession>();

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
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
                                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()),
                                UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString()),
                                Messages = new List<ChatMessage>()
                            };
                            sessions.Add(session);
                        }
                    }
                }

                // 加载每个会话的最后一条消息作为预览
                foreach (var session in sessions)
                {
                    string lastMessageSql = @"
                        SELECT * FROM `ChatMessages` 
                        WHERE `SessionId` = @sessionId 
                        ORDER BY `Id` DESC LIMIT 1";

                    using (var command = new MySqlCommand(lastMessageSql, connection))
                    {
                        command.Parameters.AddWithValue("@sessionId", session.Id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var message = new ChatMessage
                                {
                                    Role = (ChatRole)Enum.Parse(typeof(ChatRole), reader["Role"].ToString()),
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

        public void DeleteSession(string sessionId)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 删除会话消息
                        string deleteMessagesSql = "DELETE FROM `ChatMessages` WHERE `SessionId` = @sessionId";
                        using (var command = new MySqlCommand(deleteMessagesSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@sessionId", sessionId);
                            command.ExecuteNonQuery();
                        }

                        // 删除会话
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

        public void DeleteAllSessions()
        {
            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 删除所有消息
                        string deleteMessagesSql = "DELETE FROM `ChatMessages`";
                        using (var command = new MySqlCommand(deleteMessagesSql, connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        // 删除所有会话
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

        public void UpdateSessionOrder(List<ChatSession> sessions)
        {
            if (sessions == null || sessions.Count == 0)
                return;

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
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
    }
}