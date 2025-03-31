using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using llm_agent.Model;
using llm_agent.Common.Exceptions;

namespace llm_agent.BLL
{
    /// <summary>
    /// 渠道管理器，负责渠道的CRUD操作
    /// </summary>
    public class ChannelManager
    {
        private List<Channel> _channels = new List<Channel>();
        private readonly string _dataPath;
        private readonly string _channelsFile;

        public ChannelManager()
        {
            // 获取应用数据目录
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _dataPath = Path.Combine(appDataPath, "LlmAgent");
            _channelsFile = Path.Combine(_dataPath, "channels.json");

            // 确保目录存在
            if (!Directory.Exists(_dataPath))
            {
                Directory.CreateDirectory(_dataPath);
            }

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
            return _channels.OrderBy(c => c.Name).ToList();
        }

        /// <summary>
        /// 获取所有启用的渠道
        /// </summary>
        public List<Channel> GetEnabledChannels()
        {
            return _channels.Where(c => c.IsEnabled).OrderBy(c => c.Name).ToList();
        }

        /// <summary>
        /// 根据ID获取渠道
        /// </summary>
        public Channel GetChannelById(Guid id)
        {
            var channel = _channels.FirstOrDefault(c => c.Id == id);
            if (channel == null)
            {
                throw new DataAccessException($"找不到ID为 {id} 的渠道");
            }
            return channel;
        }

        /// <summary>
        /// 根据名称获取渠道
        /// </summary>
        public Channel GetChannelByName(string name)
        {
            return _channels.FirstOrDefault(c => c.Name == name);
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
            if (_channels.Any(c => c.Name.Equals(channel.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DataAccessException($"渠道名称 '{channel.Name}' 已存在");
            }

            // 设置时间戳
            channel.CreatedAt = DateTime.Now;
            channel.UpdatedAt = DateTime.Now;

            _channels.Add(channel);
            SaveChannels();
            return channel;
        }

        /// <summary>
        /// 更新渠道
        /// </summary>
        public Channel UpdateChannel(Channel channel)
        {
            var existingChannel = GetChannelById(channel.Id);
            var index = _channels.IndexOf(existingChannel);

            // 检查名称是否与其他渠道冲突
            if (_channels.Any(c => c.Id != channel.Id && c.Name.Equals(channel.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DataAccessException($"渠道名称 '{channel.Name}' 已存在");
            }

            // 更新时间戳
            channel.CreatedAt = existingChannel.CreatedAt;
            channel.UpdatedAt = DateTime.Now;

            _channels[index] = channel;
            SaveChannels();
            return channel;
        }

        /// <summary>
        /// 删除渠道
        /// </summary>
        public void DeleteChannel(Guid id)
        {
            var channel = GetChannelById(id);
            _channels.Remove(channel);
            SaveChannels();
        }

        /// <summary>
        /// 设置渠道启用状态
        /// </summary>
        public Channel SetChannelEnabledState(Guid id, bool isEnabled)
        {
            var channel = GetChannelById(id);
            channel.IsEnabled = isEnabled;
            channel.UpdatedAt = DateTime.Now;
            SaveChannels();
            return channel;
        }

        /// <summary>
        /// 更新渠道支持的模型列表
        /// </summary>
        public Channel UpdateChannelModels(Guid id, List<string> models)
        {
            var channel = GetChannelById(id);
            channel.SupportedModels = models;
            channel.UpdatedAt = DateTime.Now;
            SaveChannels();
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
            return !_channels.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && (!currentId.HasValue || c.Id != currentId.Value));
        }

        /// <summary>
        /// 加载渠道数据
        /// </summary>
        private void LoadChannels()
        {
            if (File.Exists(_channelsFile))
            {
                try
                {
                    string json = File.ReadAllText(_channelsFile);
                    var channels = JsonSerializer.Deserialize<List<Channel>>(json);
                    if (channels != null)
                    {
                        _channels = channels;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"加载渠道数据失败: {ex.Message}");
                    // 如果加载失败，使用空列表
                    _channels = new List<Channel>();
                }
            }
        }

        /// <summary>
        /// 保存渠道数据
        /// </summary>
        private void SaveChannels()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_channels, options);
                File.WriteAllText(_channelsFile, json);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"保存渠道数据失败: {ex.Message}");
                throw new DataAccessException($"保存渠道数据失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 创建默认渠道
        /// </summary>
        private void CreateDefaultChannels()
        {
            // 添加默认OpenAI渠道
            var openAIChannel = new Channel
            {
                Name = "OpenAI",
                ProviderType = ProviderType.OpenAI,
                ApiHost = "https://api.openai.com/v1",
                IsEnabled = true,
                UseStreamResponse = true,
                SupportedModels = new List<string>
                {
                    "gpt-3.5-turbo",
                    "gpt-3.5-turbo-16k",
                    "gpt-4",
                    "gpt-4-turbo",
                    "gpt-4o"
                }
            };
            _channels.Add(openAIChannel);

            // 添加默认Azure OpenAI渠道
            var azureOpenAIChannel = new Channel
            {
                Name = "Azure OpenAI",
                ProviderType = ProviderType.AzureOpenAI,
                ApiHost = "https://{resource-name}.openai.azure.com/",
                IsEnabled = false,
                UseStreamResponse = true,
                SupportedModels = new List<string>
                {
                    "gpt-35-turbo",
                    "gpt-4",
                    "gpt-4-turbo"
                }
            };
            _channels.Add(azureOpenAIChannel);

            // 添加默认Anthropic渠道
            var anthropicChannel = new Channel
            {
                Name = "Anthropic",
                ProviderType = ProviderType.Anthropic,
                ApiHost = "https://api.anthropic.com",
                IsEnabled = false,
                UseStreamResponse = true,
                SupportedModels = new List<string>
                {
                    "claude-3-opus-20240229",
                    "claude-3-sonnet-20240229",
                    "claude-3-haiku-20240307"
                }
            };
            _channels.Add(anthropicChannel);

            // 保存默认渠道
            SaveChannels();
        }
    }
} 