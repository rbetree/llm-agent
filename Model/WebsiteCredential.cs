using System;

namespace llm_agent.Model
{
    /// <summary>
    /// 网站凭据实体类
    /// </summary>
    public class WebsiteCredential
    {
        /// <summary>
        /// 凭据唯一标识符
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 关联的网站ID
        /// </summary>
        public string WebsiteId { get; set; } = string.Empty;

        /// <summary>
        /// 用户名（加密存储）
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 密码（强加密存储）
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 备注信息（加密存储）
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 关联的网站信息（导航属性，不存储在数据库中）
        /// </summary>
        public AiWebsite Website { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WebsiteCredential()
        {
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="notes">备注</param>
        public WebsiteCredential(string websiteId, string username = "", string password = "", string notes = "")
        {
            WebsiteId = websiteId;
            Username = username;
            Password = password;
            Notes = notes;
        }

        /// <summary>
        /// 是否有用户名
        /// </summary>
        public bool HasUsername => !string.IsNullOrWhiteSpace(Username);

        /// <summary>
        /// 是否有密码
        /// </summary>
        public bool HasPassword => !string.IsNullOrWhiteSpace(Password);

        /// <summary>
        /// 是否有备注
        /// </summary>
        public bool HasNotes => !string.IsNullOrWhiteSpace(Notes);

        /// <summary>
        /// 是否有任何凭据信息
        /// </summary>
        public bool HasAnyCredentials => HasUsername || HasPassword || HasNotes;

        /// <summary>
        /// 获取用户名显示文本（用于UI显示，不显示实际内容）
        /// </summary>
        public string UsernameDisplay
        {
            get
            {
                if (!HasUsername)
                    return "未设置";

                // 显示前几个字符，其余用*代替
                if (Username.Length <= 3)
                    return new string('*', Username.Length);

                return Username.Substring(0, Math.Min(3, Username.Length)) + new string('*', Math.Max(0, Username.Length - 3));
            }
        }

        /// <summary>
        /// 获取密码显示文本（用于UI显示，不显示实际内容）
        /// </summary>
        public string PasswordDisplay
        {
            get
            {
                if (!HasPassword)
                    return "未设置";

                return new string('*', Math.Min(8, Password.Length));
            }
        }

        /// <summary>
        /// 验证凭据数据是否有效
        /// </summary>
        /// <returns>验证结果</returns>
        public bool IsValid()
        {
            // 网站ID不能为空
            if (string.IsNullOrWhiteSpace(WebsiteId))
                return false;

            // 至少要有用户名或密码中的一个
            return HasUsername || HasPassword;
        }

        /// <summary>
        /// 获取验证错误信息
        /// </summary>
        /// <returns>错误信息</returns>
        public string GetValidationErrors()
        {
            var errors = new System.Collections.Generic.List<string>();

            if (string.IsNullOrWhiteSpace(WebsiteId))
                errors.Add("网站ID不能为空");

            if (!HasUsername && !HasPassword)
                errors.Add("用户名和密码至少需要设置一个");

            return string.Join("; ", errors);
        }

        /// <summary>
        /// 更新凭据信息
        /// </summary>
        /// <param name="username">新用户名</param>
        /// <param name="password">新密码</param>
        /// <param name="notes">新备注</param>
        public void UpdateCredentials(string username = null, string password = null, string notes = null)
        {
            if (username != null)
                Username = username;

            if (password != null)
                Password = password;

            if (notes != null)
                Notes = notes;

            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// 清空所有凭据信息
        /// </summary>
        public void ClearCredentials()
        {
            Username = string.Empty;
            Password = string.Empty;
            Notes = string.Empty;
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// 获取凭据摘要信息（用于日志记录等）
        /// </summary>
        /// <returns>凭据摘要</returns>
        public string GetCredentialSummary()
        {
            var parts = new System.Collections.Generic.List<string>();

            if (HasUsername)
                parts.Add($"用户名: {UsernameDisplay}");

            if (HasPassword)
                parts.Add($"密码: {PasswordDisplay}");

            if (HasNotes)
                parts.Add("备注: 已设置");

            return parts.Count > 0 ? string.Join(", ", parts) : "无凭据信息";
        }

        /// <summary>
        /// 重写ToString方法
        /// </summary>
        /// <returns>凭据摘要</returns>
        public override string ToString()
        {
            return GetCredentialSummary();
        }

        /// <summary>
        /// 重写Equals方法
        /// </summary>
        /// <param name="obj">比较对象</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object obj)
        {
            if (obj is WebsiteCredential other)
            {
                return Id == other.Id;
            }
            return false;
        }

        /// <summary>
        /// 重写GetHashCode方法
        /// </summary>
        /// <returns>哈希码</returns>
        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }
    }
}
