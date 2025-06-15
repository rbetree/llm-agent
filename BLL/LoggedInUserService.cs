using System;
using System.Collections.Generic;
using llm_agent.DAL;
using llm_agent.Model;

namespace llm_agent.BLL
{
    /// <summary>
    /// 已登录用户业务逻辑层，封装已登录用户相关的业务操作
    /// </summary>
    public class LoggedInUserService
    {
        private readonly LoggedInUserRepository _loggedInUserRepository;
        private readonly UserRepository _userRepository;

        /// <summary>
        /// 初始化LoggedInUserService
        /// </summary>
        public LoggedInUserService()
        {
            _loggedInUserRepository = new LoggedInUserRepository();
            _userRepository = new UserRepository();
        }

        /// <summary>
        /// 添加已登录用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>是否添加成功</returns>
        public bool AddLoggedInUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            // 验证用户是否存在
            var user = _userRepository.GetUserById(userId);
            if (user == null)
                return false;

            // 添加已登录用户
            return _loggedInUserRepository.AddLoggedInUser(userId);
        }

        /// <summary>
        /// 移除已登录用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveLoggedInUser(string userId)
        {
            return _loggedInUserRepository.RemoveLoggedInUser(userId);
        }

        /// <summary>
        /// 清除所有已登录用户
        /// </summary>
        /// <returns>是否清除成功</returns>
        public bool ClearLoggedInUsers()
        {
            return _loggedInUserRepository.ClearLoggedInUsers();
        }

        /// <summary>
        /// 获取所有已登录用户
        /// </summary>
        /// <returns>已登录用户列表</returns>
        public List<User> GetLoggedInUsers()
        {
            return _loggedInUserRepository.GetLoggedInUsers();
        }

        /// <summary>
        /// 检查用户是否已登录
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>是否已登录</returns>
        public bool IsUserLoggedIn(string userId)
        {
            return _loggedInUserRepository.IsUserLoggedIn(userId);
        }

        /// <summary>
        /// 获取第一个已登录用户
        /// </summary>
        /// <returns>第一个已登录用户，如果没有则返回null</returns>
        public User GetFirstLoggedInUser()
        {
            var loggedInUsers = GetLoggedInUsers();
            return loggedInUsers.Count > 0 ? loggedInUsers[0] : null;
        }
    }
} 