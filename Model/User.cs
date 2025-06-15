using System;

namespace llm_agent.Model
{
    /// <summary>
    /// 用户模型类，存储用户基本信息
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// 用户名，用于登录
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// 密码哈希值，不存储明文密码
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;
        
        /// <summary>
        /// 密码加盐，提高安全性
        /// </summary>
        public string Salt { get; set; } = string.Empty;
        
        /// <summary>
        /// 用户创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 用户最后登录时间
        /// </summary>
        public DateTime? LastLoginAt { get; set; } = null;
        
        /// <summary>
        /// 是否为管理员账号
        /// </summary>
        public bool IsAdmin { get; set; } = false;
    }
} 