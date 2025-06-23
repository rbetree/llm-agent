using System;
using llm_agent.Properties;

namespace llm_agent.Common.Utils
{
    /// <summary>
    /// 设置助手类，用于安全地存储和读取配置信息
    /// 自动处理敏感数据的加密存储
    /// </summary>
    public static class SettingsHelper
    {
        /// <summary>
        /// 获取OpenAI API密钥
        /// </summary>
        public static string GetOpenAIApiKey()
        {
            var encryptedKey = Settings.Default.OpenAIApiKey;
            return EncryptionHelper.DecryptIfNeeded(encryptedKey);
        }

        /// <summary>
        /// 设置OpenAI API密钥
        /// </summary>
        /// <param name="apiKey">API密钥</param>
        public static void SetOpenAIApiKey(string apiKey)
        {
            Settings.Default.OpenAIApiKey = EncryptionHelper.EncryptIfNeeded(apiKey ?? "");
            Settings.Default.Save();
        }

        /// <summary>
        /// 获取Azure API密钥
        /// </summary>
        public static string GetAzureApiKey()
        {
            var encryptedKey = Settings.Default.AzureApiKey;
            return EncryptionHelper.DecryptIfNeeded(encryptedKey);
        }

        /// <summary>
        /// 设置Azure API密钥
        /// </summary>
        /// <param name="apiKey">API密钥</param>
        public static void SetAzureApiKey(string apiKey)
        {
            Settings.Default.AzureApiKey = EncryptionHelper.EncryptIfNeeded(apiKey ?? "");
            Settings.Default.Save();
        }

        /// <summary>
        /// 获取Anthropic API密钥
        /// </summary>
        public static string GetAnthropicApiKey()
        {
            var encryptedKey = Settings.Default.AnthropicApiKey;
            return EncryptionHelper.DecryptIfNeeded(encryptedKey);
        }

        /// <summary>
        /// 设置Anthropic API密钥
        /// </summary>
        /// <param name="apiKey">API密钥</param>
        public static void SetAnthropicApiKey(string apiKey)
        {
            Settings.Default.AnthropicApiKey = EncryptionHelper.EncryptIfNeeded(apiKey ?? "");
            Settings.Default.Save();
        }

        /// <summary>
        /// 获取Google API密钥
        /// </summary>
        public static string GetGoogleApiKey()
        {
            var encryptedKey = Settings.Default.GeminiApiKey;
            return EncryptionHelper.DecryptIfNeeded(encryptedKey);
        }

        /// <summary>
        /// 设置Google API密钥
        /// </summary>
        /// <param name="apiKey">API密钥</param>
        public static void SetGoogleApiKey(string apiKey)
        {
            Settings.Default.GeminiApiKey = EncryptionHelper.EncryptIfNeeded(apiKey ?? "");
            Settings.Default.Save();
        }

        /// <summary>
        /// 获取OpenAI API主机
        /// </summary>
        public static string GetOpenAIApiHost()
        {
            return Settings.Default.OpenAIApiHost ?? "https://api.openai.com/v1";
        }

        /// <summary>
        /// 设置OpenAI API主机
        /// </summary>
        /// <param name="apiHost">API主机地址</param>
        public static void SetOpenAIApiHost(string apiHost)
        {
            Settings.Default.OpenAIApiHost = apiHost ?? "https://api.openai.com/v1";
            Settings.Default.Save();
        }

        /// <summary>
        /// 获取Azure API主机
        /// </summary>
        public static string GetAzureApiHost()
        {
            return Settings.Default.AzureApiHost ?? "";
        }

        /// <summary>
        /// 设置Azure API主机
        /// </summary>
        /// <param name="apiHost">API主机地址</param>
        public static void SetAzureApiHost(string apiHost)
        {
            Settings.Default.AzureApiHost = apiHost ?? "";
            Settings.Default.Save();
        }

        /// <summary>
        /// 获取Anthropic API主机
        /// </summary>
        public static string GetAnthropicApiHost()
        {
            return Settings.Default.AnthropicApiHost ?? "https://api.anthropic.com";
        }

        /// <summary>
        /// 设置Anthropic API主机
        /// </summary>
        /// <param name="apiHost">API主机地址</param>
        public static void SetAnthropicApiHost(string apiHost)
        {
            Settings.Default.AnthropicApiHost = apiHost ?? "https://api.anthropic.com";
            Settings.Default.Save();
        }

        /// <summary>
        /// 获取Google API主机
        /// </summary>
        public static string GetGoogleApiHost()
        {
            return Settings.Default.GeminiApiHost ?? "https://generativelanguage.googleapis.com/v1beta";
        }

        /// <summary>
        /// 设置Google API主机
        /// </summary>
        /// <param name="apiHost">API主机地址</param>
        public static void SetGoogleApiHost(string apiHost)
        {
            Settings.Default.GeminiApiHost = apiHost ?? "https://generativelanguage.googleapis.com/v1beta";
            Settings.Default.Save();
        }

        /// <summary>
        /// 迁移现有的明文API密钥到加密存储
        /// 这个方法应该在应用程序启动时调用一次
        /// </summary>
        public static void MigrateApiKeysToEncrypted()
        {
            bool needsSave = false;

            // 检查并迁移OpenAI API密钥
            if (!string.IsNullOrEmpty(Settings.Default.OpenAIApiKey) && 
                !EncryptionHelper.IsEncrypted(Settings.Default.OpenAIApiKey))
            {
                Settings.Default.OpenAIApiKey = EncryptionHelper.Encrypt(Settings.Default.OpenAIApiKey);
                needsSave = true;
            }

            // 检查并迁移Azure API密钥
            if (!string.IsNullOrEmpty(Settings.Default.AzureApiKey) && 
                !EncryptionHelper.IsEncrypted(Settings.Default.AzureApiKey))
            {
                Settings.Default.AzureApiKey = EncryptionHelper.Encrypt(Settings.Default.AzureApiKey);
                needsSave = true;
            }

            // 检查并迁移Anthropic API密钥
            if (!string.IsNullOrEmpty(Settings.Default.AnthropicApiKey) && 
                !EncryptionHelper.IsEncrypted(Settings.Default.AnthropicApiKey))
            {
                Settings.Default.AnthropicApiKey = EncryptionHelper.Encrypt(Settings.Default.AnthropicApiKey);
                needsSave = true;
            }

            // 检查并迁移Google API密钥
            if (!string.IsNullOrEmpty(Settings.Default.GeminiApiKey) &&
                !EncryptionHelper.IsEncrypted(Settings.Default.GeminiApiKey))
            {
                Settings.Default.GeminiApiKey = EncryptionHelper.Encrypt(Settings.Default.GeminiApiKey);
                needsSave = true;
            }

            // 如果有任何更改，保存设置
            if (needsSave)
            {
                Settings.Default.Save();
                Console.WriteLine("已将现有API密钥迁移到加密存储");
            }
        }

        /// <summary>
        /// 验证所有加密功能是否正常工作
        /// </summary>
        /// <returns>验证结果</returns>
        public static bool ValidateEncryptionSettings()
        {
            try
            {
                // 测试加密和解密功能
                const string testKey = "test-api-key-12345";
                
                // 临时保存当前设置
                var originalOpenAIKey = Settings.Default.OpenAIApiKey;
                
                // 测试设置和获取
                SetOpenAIApiKey(testKey);
                var retrievedKey = GetOpenAIApiKey();
                
                // 恢复原始设置
                Settings.Default.OpenAIApiKey = originalOpenAIKey;
                Settings.Default.Save();
                
                // 验证结果
                return testKey == retrievedKey;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"设置加密验证失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 清除所有API密钥
        /// </summary>
        public static void ClearAllApiKeys()
        {
            Settings.Default.OpenAIApiKey = "";
            Settings.Default.AzureApiKey = "";
            Settings.Default.AnthropicApiKey = "";
            Settings.Default.GeminiApiKey = "";
            Settings.Default.Save();
        }

        /// <summary>
        /// 检查是否有任何API密钥已配置
        /// </summary>
        /// <returns>如果有任何API密钥已配置返回true</returns>
        public static bool HasAnyApiKeyConfigured()
        {
            return !string.IsNullOrEmpty(GetOpenAIApiKey()) ||
                   !string.IsNullOrEmpty(GetAzureApiKey()) ||
                   !string.IsNullOrEmpty(GetAnthropicApiKey()) ||
                   !string.IsNullOrEmpty(GetGoogleApiKey());
        }
    }
}
