using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using llm_agent.Model;
using llm_agent.Common.Exceptions;
using llm_agent.DAL;
using System.Net.Http;
using MySql.Data.MySqlClient;
using System.Data;

namespace llm_agent.BLL
{
    /// <summary>
    /// 渠道管理器，负责渠道的CRUD操作
    /// </summary>
    public class ChannelManager
    {
        private List<Channel> _channels = new List<Channel>();

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
            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();
                string sql = "SELECT * FROM Channels WHERE Id = @id";
                
                using (var command = new MySqlCommand(sql, connection))
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
                                ApiKey = reader["ApiKey"].ToString(),
                                ApiHost = reader["ApiHost"].ToString(),
                                IsEnabled = Convert.ToBoolean(Convert.ToInt32(reader["IsEnabled"])),
                                UseStreamResponse = Convert.ToBoolean(Convert.ToInt32(reader["UseStreamResponse"])),
                                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()),
                                UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString()),
                                SupportedModels = new List<string>()
                            };
                            
                            // 加载渠道支持的模型列表
                            LoadChannelModels(channel);
                            
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
            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();
                string sql = "SELECT * FROM Channels WHERE Name = @name";
                
                using (var command = new MySqlCommand(sql, connection))
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
                                ApiKey = reader["ApiKey"].ToString(),
                                ApiHost = reader["ApiHost"].ToString(),
                                IsEnabled = Convert.ToBoolean(Convert.ToInt32(reader["IsEnabled"])),
                                UseStreamResponse = Convert.ToBoolean(Convert.ToInt32(reader["UseStreamResponse"])),
                                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()),
                                UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString()),
                                SupportedModels = new List<string>()
                            };
                            
                            // 加载渠道支持的模型列表
                            LoadChannelModels(channel);
                            
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

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
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
                        
                        using (var command = new MySqlCommand(insertChannelSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", channel.Id.ToString());
                            command.Parameters.AddWithValue("@name", channel.Name);
                            command.Parameters.AddWithValue("@providerType", channel.ProviderType.ToString());
                            command.Parameters.AddWithValue("@apiKey", channel.ApiKey ?? "");
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

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
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
                        
                        using (var command = new MySqlCommand(updateChannelSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", channel.Id.ToString());
                            command.Parameters.AddWithValue("@name", channel.Name);
                            command.Parameters.AddWithValue("@providerType", channel.ProviderType.ToString());
                            command.Parameters.AddWithValue("@apiKey", channel.ApiKey ?? "");
                            command.Parameters.AddWithValue("@apiHost", channel.ApiHost ?? "");
                            command.Parameters.AddWithValue("@isEnabled", channel.IsEnabled ? 1 : 0);
                            command.Parameters.AddWithValue("@useStreamResponse", channel.UseStreamResponse ? 1 : 0);
                            command.Parameters.AddWithValue("@updatedAt", channel.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                            
                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected == 0)
                            {
                                throw new DataAccessException($"找不到ID为 {channel.Id} 的渠道");
                            }
                        }
                        
                        // 删除旧的模型关联
                        string deleteModelsSql = "DELETE FROM ChannelModels WHERE ChannelId = @channelId";
                        using (var command = new MySqlCommand(deleteModelsSql, connection, transaction))
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
            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 先删除渠道关联的模型
                        string deleteModelsSql = "DELETE FROM ChannelModels WHERE ChannelId = @channelId";
                        using (var command = new MySqlCommand(deleteModelsSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@channelId", id.ToString());
                            command.ExecuteNonQuery();
                        }

                        // 再删除渠道本身
                        string deleteChannelSql = "DELETE FROM Channels WHERE Id = @id";
                        using (var command = new MySqlCommand(deleteChannelSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", id.ToString());
                            int rowsAffected = command.ExecuteNonQuery();
                            
                            if (rowsAffected == 0)
                            {
                                throw new DataAccessException($"找不到ID为 {id} 的渠道");
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
            
            // 重新加载渠道列表
            LoadChannels();
        }

        /// <summary>
        /// 设置渠道启用状态
        /// </summary>
        public Channel SetChannelEnabledState(Guid id, bool isEnabled)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();
                
                string updateSql = "UPDATE Channels SET IsEnabled = @isEnabled, UpdatedAt = @updatedAt WHERE Id = @id";
                using (var command = new MySqlCommand(updateSql, connection))
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
            
            // 更新模型列表
            channel.SupportedModels = models ?? new List<string>();
            
            // 更新时间戳
            channel.UpdatedAt = DateTime.Now;
            
            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 更新渠道时间戳
                        string updateChannelSql = "UPDATE Channels SET UpdatedAt = @updatedAt WHERE Id = @id";
                        using (var command = new MySqlCommand(updateChannelSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", channel.Id.ToString());
                            command.Parameters.AddWithValue("@updatedAt", channel.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                            command.ExecuteNonQuery();
                        }
                        
                        // 删除旧的模型关联
                        string deleteModelsSql = "DELETE FROM ChannelModels WHERE ChannelId = @channelId";
                        using (var command = new MySqlCommand(deleteModelsSql, connection, transaction))
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
        /// 检查渠道名称是否可用
        /// </summary>
        /// <param name="name">要检查的名称</param>
        /// <param name="currentId">当前渠道ID（用于更新时排除自身）</param>
        /// <returns>名称是否可用</returns>
        public bool IsChannelNameAvailable(string name, Guid? currentId = null)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();
                
                string sql = "SELECT COUNT(*) FROM Channels WHERE Name = @name";
                if (currentId.HasValue)
                {
                    sql += " AND Id != @id";
                }
                
                using (var command = new MySqlCommand(sql, connection))
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
        /// 从数据库加载所有渠道
        /// </summary>
        private void LoadChannels()
        {
            _channels.Clear();
            
            try
            {
                using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
                {
                    connection.Open();
                    
                    string sql = "SELECT * FROM Channels";
                    using (var command = new MySqlCommand(sql, connection))
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
                                    ApiKey = reader["ApiKey"].ToString(),
                                    ApiHost = reader["ApiHost"].ToString(),
                                    IsEnabled = Convert.ToBoolean(Convert.ToInt32(reader["IsEnabled"])),
                                    UseStreamResponse = Convert.ToBoolean(Convert.ToInt32(reader["UseStreamResponse"])),
                                    CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()),
                                    UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString()),
                                    SupportedModels = new List<string>()
                                };
                                
                                _channels.Add(channel);
                            }
                        }
                    }
                    
                    // 加载每个渠道支持的模型
                    foreach (var channel in _channels)
                    {
                        LoadChannelModels(channel, connection);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"加载渠道数据失败: {ex.Message}",
                    "数据库错误",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                
                throw;
            }
        }

        /// <summary>
        /// 加载渠道支持的模型列表
        /// </summary>
        private void LoadChannelModels(Channel channel)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();
                LoadChannelModels(channel, connection);
            }
        }

        /// <summary>
        /// 加载渠道支持的模型列表
        /// </summary>
        private void LoadChannelModels(Channel channel, MySqlConnection connection)
        {
            string sql = "SELECT ModelName FROM ChannelModels WHERE ChannelId = @channelId";
            using (var command = new MySqlCommand(sql, connection))
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
        private void SaveChannelModels(Channel channel, MySqlConnection connection, MySqlTransaction transaction)
        {
            string insertSql = "INSERT INTO ChannelModels (ChannelId, ModelName) VALUES (@channelId, @modelName)";
            
            foreach (var model in channel.SupportedModels)
            {
                using (var command = new MySqlCommand(insertSql, connection, transaction))
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
            var defaultChannels = new List<Channel>
            {
                new Channel
                {
                    Id = Guid.NewGuid(),
                    Name = "OpenAI API",
                    ProviderType = ProviderType.OpenAI,
                    ApiKey = "",
                    ApiHost = "https://api.openai.com",
                    IsEnabled = false,
                    UseStreamResponse = true,
                    SupportedModels = new List<string> { "gpt-3.5-turbo", "gpt-4" }
                },
                new Channel
                {
                    Id = Guid.NewGuid(),
                    Name = "Azure OpenAI",
                    ProviderType = ProviderType.AzureOpenAI,
                    ApiKey = "",
                    ApiHost = "https://your-resource-name.openai.azure.com",
                    IsEnabled = false,
                    UseStreamResponse = true,
                    SupportedModels = new List<string> { "gpt-35-turbo", "gpt-4" }
                },
                new Channel
                {
                    Id = Guid.NewGuid(),
                    Name = "Anthropic API",
                    ProviderType = ProviderType.Anthropic,
                    ApiKey = "",
                    ApiHost = "https://api.anthropic.com",
                    IsEnabled = false,
                    UseStreamResponse = true,
                    SupportedModels = new List<string> { "claude-3-opus-20240229", "claude-3-sonnet-20240229", "claude-3-haiku-20240307" }
                }
            };

            foreach (var channel in defaultChannels)
            {
                try
                {
                    AddChannel(channel);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(
                        $"创建默认渠道 '{channel.Name}' 失败: {ex.Message}",
                        "错误",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }
    }
} 