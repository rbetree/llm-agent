using System;
using llm_agent.Model;

namespace llm_agent.Common
{
    /// <summary>
    /// 用户会话管理类，管理当前登录用户的信息
    /// 使用单例模式确保应用中只有一个用户会话实例
    /// </summary>
    public class UserSession
    {
        private static readonly Lazy<UserSession> _instance = new Lazy<UserSession>(() => new UserSession());

        /// <summary>
        /// 获取UserSession的单例实例
        /// </summary>
        public static UserSession Instance => _instance.Value;

        /// <summary>
        /// 当前登录用户
        /// </summary>
        public User CurrentUser { get; private set; }

        /// <summary>
        /// 用户是否已登录
        /// </summary>
        public bool IsLoggedIn => CurrentUser != null;

        /// <summary>
        /// 用户登录时间
        /// </summary>
        public DateTime? LoginTime { get; private set; }

        /// <summary>
        /// 私有构造函数，防止外部实例化
        /// </summary>
        private UserSession()
        {
            CurrentUser = null;
            LoginTime = null;
        }

        /// <summary>
        /// 设置当前登录用户
        /// </summary>
        /// <param name="user">用户对象</param>
        public void SetCurrentUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "用户对象不能为空");
            }
            
            CurrentUser = user;
            LoginTime = DateTime.Now;
        }

        /// <summary>
        /// 清除当前登录用户信息（登出）
        /// </summary>
        public void Logout()
        {
            CurrentUser = null;
            LoginTime = null;
        }

        /// <summary>
        /// 获取当前用户ID
        /// </summary>
        /// <returns>当前用户ID，如果未登录则返回空字符串</returns>
        public string GetCurrentUserId()
        {
            return IsLoggedIn ? CurrentUser.Id : string.Empty;
        }

        /// <summary>
        /// 获取当前用户名
        /// </summary>
        /// <returns>用户名，未登录则返回空字符串</returns>
        public string GetCurrentUsername()
        {
            return IsLoggedIn ? CurrentUser.Username : string.Empty;
        }

        /// <summary>
        /// 获取用户会话持续时间
        /// </summary>
        /// <returns>会话持续时间，未登录则返回TimeSpan.Zero</returns>
        public TimeSpan GetSessionDuration()
        {
            return LoginTime.HasValue ? DateTime.Now - LoginTime.Value : TimeSpan.Zero;
        }
    }
} 