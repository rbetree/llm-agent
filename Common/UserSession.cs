using System;
using llm_agent.Model;
using llm_agent.BLL;

namespace llm_agent.Common
{
    /// <summary>
    /// 用户会话管理类，管理当前登录用户的信息
    /// 使用单例模式确保应用中只有一个用户会话实例
    /// </summary>
    public class UserSession
    {
        private static readonly Lazy<UserSession> _instance = new Lazy<UserSession>(() => new UserSession());
        private readonly LoggedInUserService _loggedInUserService;

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
            _loggedInUserService = new LoggedInUserService();
        }

        /// <summary>
        /// 设置当前登录用户
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <param name="rememberLogin">是否记住登录状态</param>
        public void SetCurrentUser(User user, bool rememberLogin = true)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "用户对象不能为空");
            }
            
            CurrentUser = user;
            LoginTime = DateTime.Now;

            // 如果需要记住登录状态，则将用户添加到已登录用户列表
            if (rememberLogin)
            {
                _loggedInUserService.AddLoggedInUser(user.Id);
            }
        }

        /// <summary>
        /// 清除当前登录用户信息（登出）
        /// </summary>
        /// <param name="removeFromLoggedIn">是否从已登录用户列表中移除</param>
        public void Logout(bool removeFromLoggedIn = true)
        {
            if (CurrentUser != null && removeFromLoggedIn)
            {
                _loggedInUserService.RemoveLoggedInUser(CurrentUser.Id);
            }
            
            CurrentUser = null;
            LoginTime = null;
        }

        /// <summary>
        /// 尝试自动登录
        /// </summary>
        /// <returns>是否成功自动登录</returns>
        public bool TryAutoLogin()
        {
            if (IsLoggedIn)
                return true;

            var loggedInUser = _loggedInUserService.GetFirstLoggedInUser();
            if (loggedInUser != null)
            {
                SetCurrentUser(loggedInUser, false); // 不需要再次添加到已登录用户列表
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取所有已登录用户
        /// </summary>
        /// <returns>已登录用户列表</returns>
        public System.Collections.Generic.List<User> GetLoggedInUsers()
        {
            return _loggedInUserService.GetLoggedInUsers();
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

        /// <summary>
        /// 检查当前用户是否为管理员
        /// </summary>
        /// <returns>是否为管理员</returns>
        public bool IsCurrentUserAdmin()
        {
            return IsLoggedIn && CurrentUser.IsAdmin;
        }
    }
} 