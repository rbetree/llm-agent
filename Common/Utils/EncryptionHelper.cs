using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace llm_agent.Common.Utils
{
    /// <summary>
    /// 加密工具类，用于网站凭据的安全存储
    /// </summary>
    public static class EncryptionHelper
    {
        // 固定的盐值，用于增强安全性
        private static readonly byte[] Salt = Encoding.UTF8.GetBytes("LlmAgent2024Salt");
        
        // 基于设备特征生成的密钥
        private static string _deviceKey;

        /// <summary>
        /// 获取设备密钥
        /// </summary>
        private static string DeviceKey
        {
            get
            {
                if (string.IsNullOrEmpty(_deviceKey))
                {
                    _deviceKey = GenerateDeviceKey();
                }
                return _deviceKey;
            }
        }

        /// <summary>
        /// 生成基于设备特征的密钥
        /// </summary>
        /// <returns>设备密钥</returns>
        private static string GenerateDeviceKey()
        {
            try
            {
                // 使用机器名、用户名和应用程序路径生成设备特征
                var machineInfo = Environment.MachineName + Environment.UserName + 
                                 Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                
                using (var sha256 = SHA256.Create())
                {
                    var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(machineInfo));
                    return Convert.ToBase64String(hash);
                }
            }
            catch
            {
                // 如果获取设备信息失败，使用默认密钥
                return "DefaultLlmAgentKey2024";
            }
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="plainText">要加密的明文</param>
        /// <returns>加密后的密文（Base64编码）</returns>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            try
            {
                using (var aes = Aes.Create())
                {
                    // 使用PBKDF2从设备密钥生成加密密钥
                    var key = new Rfc2898DeriveBytes(DeviceKey, Salt, 10000);
                    aes.Key = key.GetBytes(32); // 256位密钥
                    aes.IV = key.GetBytes(16);  // 128位IV

                    using (var encryptor = aes.CreateEncryptor())
                    using (var msEncrypt = new MemoryStream())
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                        swEncrypt.Close();
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录错误但不抛出异常，返回空字符串
                Console.Error.WriteLine($"加密失败: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="cipherText">要解密的密文（Base64编码）</param>
        /// <returns>解密后的明文</returns>
        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            try
            {
                var cipherBytes = Convert.FromBase64String(cipherText);

                using (var aes = Aes.Create())
                {
                    // 使用相同的方法生成解密密钥
                    var key = new Rfc2898DeriveBytes(DeviceKey, Salt, 10000);
                    aes.Key = key.GetBytes(32); // 256位密钥
                    aes.IV = key.GetBytes(16);  // 128位IV

                    using (var decryptor = aes.CreateDecryptor())
                    using (var msDecrypt = new MemoryStream(cipherBytes))
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录错误但不抛出异常，返回空字符串
                Console.Error.WriteLine($"解密失败: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 验证加密功能是否正常工作
        /// </summary>
        /// <returns>验证结果</returns>
        public static bool ValidateEncryption()
        {
            try
            {
                const string testText = "TestEncryption123!@#";
                var encrypted = Encrypt(testText);
                var decrypted = Decrypt(encrypted);
                return testText == decrypted;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 生成安全的随机密码
        /// </summary>
        /// <param name="length">密码长度</param>
        /// <param name="includeSpecialChars">是否包含特殊字符</param>
        /// <returns>生成的密码</returns>
        public static string GenerateSecurePassword(int length = 12, bool includeSpecialChars = true)
        {
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            var chars = lowercase + uppercase + digits;
            if (includeSpecialChars)
                chars += specialChars;

            var random = new Random();
            var password = new StringBuilder();

            // 确保至少包含一个小写字母、大写字母和数字
            password.Append(lowercase[random.Next(lowercase.Length)]);
            password.Append(uppercase[random.Next(uppercase.Length)]);
            password.Append(digits[random.Next(digits.Length)]);

            if (includeSpecialChars && length > 3)
                password.Append(specialChars[random.Next(specialChars.Length)]);

            // 填充剩余长度
            for (int i = password.Length; i < length; i++)
            {
                password.Append(chars[random.Next(chars.Length)]);
            }

            // 打乱字符顺序
            var result = password.ToString().ToCharArray();
            for (int i = result.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (result[i], result[j]) = (result[j], result[i]);
            }

            return new string(result);
        }

        /// <summary>
        /// 评估密码强度
        /// </summary>
        /// <param name="password">要评估的密码</param>
        /// <returns>密码强度等级（0-4，4为最强）</returns>
        public static int EvaluatePasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
                return 0;

            int score = 0;

            // 长度评分
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;

            // 字符类型评分
            if (password.Any(char.IsLower)) score++;
            if (password.Any(char.IsUpper)) score++;
            if (password.Any(char.IsDigit)) score++;
            if (password.Any(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c))) score++;

            return Math.Min(score, 4);
        }

        /// <summary>
        /// 获取密码强度描述
        /// </summary>
        /// <param name="password">要评估的密码</param>
        /// <returns>密码强度描述</returns>
        public static string GetPasswordStrengthDescription(string password)
        {
            var strength = EvaluatePasswordStrength(password);
            return strength switch
            {
                0 => "无密码",
                1 => "极弱",
                2 => "弱",
                3 => "中等",
                4 => "强",
                _ => "未知"
            };
        }
    }
}
