using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.IO;
using llm_agent.Model;
using llm_agent.Common.Exceptions;
using llm_agent.DAL;
using llm_agent.Common.Utils;
using System.Net.Http;

namespace llm_agent.BLL
{
    /// <summary>
    /// 渠道管理器，负责渠道的CRUD操作
    /// </summary>
    public class ChannelManager
    {
        private List<Channel> _channels = new List<Channel>();
        private static readonly string DbName = "llm_agent.db";
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbName);
        private static readonly string ConnectionString = $"Data Source={DbPath};Version=3;";

        public ChannelManager()
        {
            // 确保数据库和表已初始化
            var databaseManager = new DatabaseManager();

            // 加载渠道数据
            LoadChannels();
            
            // 如果没有任何渠道，创建默认渠道
            if (_channels.Count == 0)
            {
                CreateDefaultChannels();
            }
        }

        /// <summary>
        /// 获取所有渠道
        /// </summary>
        public List<Channel> GetAllChannels()
        {
            // 确保从数据库重新加载最新数据
            LoadChannels();
            return _channels.OrderBy(c => c.Name).ToList();
        }

        /// <summary>
        /// 获取所有启用的渠道
        /// </summary>
        public List<Channel> GetEnabledChannels()
        {
            // 确保从数据库重新加载最新数据
            LoadChannels();
            return _channels.Where(c => c.IsEnabled).OrderBy(c => c.Name).ToList();
        }

        /// <summary>
        /// 根据ID获取渠道
        /// </summary>
        public Channel GetChannelById(Guid id)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Channels WHERE Id = @id";
                
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id.ToString());
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var channel = new Channel
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                Name = reader["Name"].ToString(),
                                ProviderType = (ProviderType)Enum.Parse(typeof(ProviderType), reader["ProviderType"].ToString()),
                                ApiKey = EncryptionHelper.DecryptIfNeeded(reader["ApiKey"].ToString()),
                                ApiHost = reader["ApiHost"].ToString(),
                                IsEnabled = Convert.ToBoolean(Convert.ToInt32(reader["IsEnabled"])),
                                UseStreamResponse = Convert.ToBoolean(Convert.ToInt32(reader["UseStreamResponse"])),
                                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()),
                                UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString()),
                                SupportedModels = new List<string>()
                            };
                            
                            // 加载渠道支持的模型列表
                            LoadChannelModels(channel, connection);
                            
                            return channel;
                        }
                    }
                }
            }
            
            throw new DataAccessException($"找不到ID为 {id} 的渠道");
        }

        /// <summary>
        /// 根据名称获取渠道
        /// </summary>
        public Channel GetChannelByName(string name)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Channels WHERE Name = @name";
                
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var channel = new Channel
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                Name = reader["Name"].ToString(),
                                ProviderType = (ProviderType)Enum.Parse(typeof(ProviderType), reader["ProviderType"].ToString()),
                                ApiKey = EncryptionHelper.DecryptIfNeeded(reader["ApiKey"].ToString()),
                                ApiHost = reader["ApiHost"].ToString(),
                                IsEnabled = Convert.ToBoolean(Convert.ToInt32(reader["IsEnabled"])),
                                UseStreamResponse = Convert.ToBoolean(Convert.ToInt32(reader["UseStreamResponse"])),
                                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()),
                                UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString()),
                                SupportedModels = new List<string>()
                            };
                            
                            // 加载渠道支持的模型列表
                            LoadChannelModels(channel, connection);
                            
                            return channel;
                        }
                    }
                }
            }
            
            return null;
        }

        /// <summary>
        /// 添加新渠道
        /// </summary>
        public Channel AddChannel(Channel channel)
        {
            // 确保渠道有一个唯一的ID
            if (channel.Id == Guid.Empty)
            {
                channel.Id = Guid.NewGuid();
            }

            // 检查名称是否已存在
            if (!IsChannelNameAvailable(channel.Name))
            {
                throw new DataAccessException($"渠道名称 '{channel.Name}' 已存在");
            }

            // 设置时间戳
            channel.CreatedAt = DateTime.Now;
            channel.UpdatedAt = DateTime.Now;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 插入渠道基本信息
                        string insertChannelSql = @"
                            INSERT INTO Channels (Id, Name, ProviderType, ApiKey, ApiHost, IsEnabled, UseStreamResponse, CreatedAt, UpdatedAt)
                            VALUES (@id, @name, @providerType, @apiKey, @apiHost, @isEnabled, @useStreamResponse, @createdAt, @updatedAt)";
                        
                        using (var command = new SQLiteCommand(insertChannelSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", channel.Id.ToString());
                            command.Parameters.AddWithValue("@name", channel.Name);
                            command.Parameters.AddWithValue("@providerType", channel.ProviderType.ToString());
                            command.Parameters.AddWithValue("@apiKey", EncryptionHelper.EncryptIfNeeded(channel.ApiKey ?? ""));
                            command.Parameters.AddWithValue("@apiHost", channel.ApiHost ?? "");
                            command.Parameters.AddWithValue("@isEnabled", channel.IsEnabled ? 1 : 0);
                            command.Parameters.AddWithValue("@useStreamResponse", channel.UseStreamResponse ? 1 : 0);
                            command.Parameters.AddWithValue("@createdAt", channel.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                            command.Parameters.AddWithValue("@updatedAt", channel.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                            command.ExecuteNonQuery();
                        }
                        
                        // 插入渠道支持的模型
                        if (channel.SupportedModels != null && channel.SupportedModels.Count > 0)
                        {
                            SaveChannelModels(channel, connection, transaction);
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
            
            // 重新加载渠道列表
            LoadChannels();
            
            return channel;
        }

        /// <summary>
        /// 更新渠道
        /// </summary>
        public Channel UpdateChannel(Channel channel)
        {
            // 检查渠道是否存在
            var existingChannel = GetChannelById(channel.Id);
            
            // 检查名称是否与其他渠道冲突
            if (!IsChannelNameAvailable(channel.Name, channel.Id))
            {
                throw new DataAccessException($"渠道名称 '{channel.Name}' 已存在");
            }

            // 更新时间戳
            channel.CreatedAt = existingChannel.CreatedAt;
            channel.UpdatedAt = DateTime.Now;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 更新渠道基本信息
                        string updateChannelSql = @"
                            UPDATE Channels 
                            SET Name = @name, 
                                ProviderType = @providerType, 
                                ApiKey = @apiKey, 
                                ApiHost = @apiHost, 
                                IsEnabled = @isEnabled, 
                                UseStreamResponse = @useStreamResponse, 
                                UpdatedAt = @updatedAt
                            WHERE Id = @id";
                        
                        using (var command = new SQLiteCommand(updateChannelSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", channel.Id.ToString());
                            command.Parameters.AddWithValue("@name", channel.Name);
                            command.Parameters.AddWithValue("@providerType", channel.ProviderType.ToString());
                            command.Parameters.AddWithValue("@apiKey", EncryptionHelper.EncryptIfNeeded(channel.ApiKey ?? ""));
                            command.Parameters.AddWithValue("@apiHost", channel.ApiHost ?? "");
                            command.Parameters.AddWithValue("@isEnabled", channel.IsEnabled ? 1 : 0);
                            command.Parameters.AddWithValue("@useStreamResponse", channel.UseStreamResponse ? 1 : 0);
                            command.Parameters.AddWithValue("@updatedAt", channel.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                            command.ExecuteNonQuery();
                        }
                        
                        // 删除旧的模型关联
                        string deleteModelsSql = "DELETE FROM ChannelModels WHERE ChannelId = @channelId";
                        using (var command = new SQLiteCommand(deleteModelsSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@channelId", channel.Id.ToString());
                            command.ExecuteNonQuery();
                        }
                        
                        // 插入新的模型关联
                        if (channel.SupportedModels != null && channel.SupportedModels.Count > 0)
                        {
                            SaveChannelModels(channel, connection, transaction);
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
            
            // 重新加载渠道列表
            LoadChannels();
            
            return channel;
        }

        /// <summary>
        /// 删除渠道
        /// </summary>
        public void DeleteChannel(Guid id)
        {
            // 检查渠道是否存在
            GetChannelById(id);

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 删除渠道模型关联（级联删除会自动处理，但为了清晰起见，显式删除）
                        string deleteModelsSql = "DELETE FROM ChannelModels WHERE ChannelId = @channelId";
                        using (var command = new SQLiteCommand(deleteModelsSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@channelId", id.ToString());
                            command.ExecuteNonQuery();
                        }
                        
                        // 删除渠道
                        string deleteChannelSql = "DELETE FROM Channels WHERE Id = @id";
                        using (var command = new SQLiteCommand(deleteChannelSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", id.ToString());
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
            
            // 重新加载渠道列表
            LoadChannels();
        }

        /// <summary>
        /// 设置渠道启用状态
        /// </summary>
        public Channel SetChannelEnabledState(Guid id, bool isEnabled)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                
                string updateSql = "UPDATE Channels SET IsEnabled = @isEnabled, UpdatedAt = @updatedAt WHERE Id = @id";
                using (var command = new SQLiteCommand(updateSql, connection))
                {
                    var now = DateTime.Now;
                    command.Parameters.AddWithValue("@id", id.ToString());
                    command.Parameters.AddWithValue("@isEnabled", isEnabled ? 1 : 0);
                    command.Parameters.AddWithValue("@updatedAt", now.ToString("yyyy-MM-dd HH:mm:ss"));
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new DataAccessException($"找不到ID为 {id} 的渠道");
                    }
                }
            }
            
            // 重新加载渠道列表
            LoadChannels();
            
            return GetChannelById(id);
        }

        /// <summary>
        /// 更新渠道支持的模型列表
        /// </summary>
        public Channel UpdateChannelModels(Guid id, List<string> models)
        {
            // 获取渠道
            var channel = GetChannelById(id);
            channel.SupportedModels = models;
            channel.UpdatedAt = DateTime.Now;
            
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 更新渠道更新时间
                        string updateChannelSql = "UPDATE Channels SET UpdatedAt = @updatedAt WHERE Id = @id";
                        using (var command = new SQLiteCommand(updateChannelSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", id.ToString());
                            command.Parameters.AddWithValue("@updatedAt", channel.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                            command.ExecuteNonQuery();
                        }
                        
                        // 删除旧的模型关联
                        string deleteModelsSql = "DELETE FROM ChannelModels WHERE ChannelId = @channelId";
                        using (var command = new SQLiteCommand(deleteModelsSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@channelId", id.ToString());
                            command.ExecuteNonQuery();
                        }
                        
                        // 插入新的模型关联
                        if (models != null && models.Count > 0)
                        {
                            SaveChannelModels(channel, connection, transaction);
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
            
            // 重新加载渠道列表
            LoadChannels();
            
            return channel;
        }

        /// <summary>
        /// 验证渠道名称是否可用
        /// </summary>
        /// <param name="name">渠道名称</param>
        /// <param name="currentId">当前渠道ID（编辑时使用）</param>
        /// <returns>名称是否可用</returns>
        public bool IsChannelNameAvailable(string name, Guid? currentId = null)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                
                string sql = "SELECT COUNT(*) FROM Channels WHERE Name = @name";
                if (currentId.HasValue)
                {
                    sql += " AND Id != @id";
                }
                
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    if (currentId.HasValue)
                    {
                        command.Parameters.AddWithValue("@id", currentId.Value.ToString());
                    }
                    
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count == 0;
                }
            }
        }

        /// <summary>
        /// 加载渠道数据
        /// </summary>
        private void LoadChannels()
        {
            _channels.Clear();
            
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Channels";
                
                using (var command = new SQLiteCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var channel = new Channel
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                Name = reader["Name"].ToString(),
                                ProviderType = (ProviderType)Enum.Parse(typeof(ProviderType), reader["ProviderType"].ToString()),
                                ApiKey = EncryptionHelper.DecryptIfNeeded(reader["ApiKey"].ToString()),
                                ApiHost = reader["ApiHost"].ToString(),
                                IsEnabled = Convert.ToBoolean(Convert.ToInt32(reader["IsEnabled"])),
                                UseStreamResponse = Convert.ToBoolean(Convert.ToInt32(reader["UseStreamResponse"])),
                                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()),
                                UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString()),
                                SupportedModels = new List<string>()
                            };
                            
                            // 加载渠道支持的模型列表
                            LoadChannelModels(channel, connection);
                            
                            _channels.Add(channel);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 加载渠道支持的模型列表
        /// </summary>
        private void LoadChannelModels(Channel channel, SQLiteConnection connection)
        {
            string sql = "SELECT ModelName FROM ChannelModels WHERE ChannelId = @channelId";
            
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@channelId", channel.Id.ToString());
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        channel.SupportedModels.Add(reader["ModelName"].ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 保存渠道支持的模型列表
        /// </summary>
        private void SaveChannelModels(Channel channel, SQLiteConnection connection, SQLiteTransaction transaction)
        {
            string insertModelSql = "INSERT INTO ChannelModels (ChannelId, ModelName) VALUES (@channelId, @modelName)";
            
            foreach (var model in channel.SupportedModels)
            {
                using (var command = new SQLiteCommand(insertModelSql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@channelId", channel.Id.ToString());
                    command.Parameters.AddWithValue("@modelName", model);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 创建默认渠道
        /// </summary>
        private void CreateDefaultChannels()
        {
            try
            {
                // 创建siliconflow测试渠道
                var channel = new Channel
                {
                    Name = "测试",
                    ProviderType = ProviderType.OpenAI,
                    ApiKey = "sk-tgamwxumswqrsfkzknwscaeqvbzqamkbosxghpbcjhlaxjqt",
                    ApiHost = "https://api.siliconflow.cn/v1",
                    IsEnabled = true,
                    UseStreamResponse = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    SupportedModels = new List<string>() // 使用空列表，不预设模型
                };

                // 添加到系统中
                AddChannel(channel);
            }
            catch (Exception ex)
            {
                // 记录创建默认渠道失败的错误，但不阻止应用程序启动
                Console.WriteLine($"创建默认渠道失败: {ex.Message}");
            }
        }
    }
} 