using System;
using System.Collections.Generic;
using llm_agent.DAL;
using llm_agent.Model;

namespace llm_agent.BLL
{
    /// <summary>
    /// 用户业务逻辑层，封装用户相关的业务操作
    /// </summary>
    public class UserService
    {
        private readonly UserRepository _userRepository;

        /// <summary>
        /// 初始化UserService
        /// </summary>
        /// <param name="repository">用户数据访问对象，如果为null则创建新实例</param>
        public UserService(UserRepository repository = null)
        {
            _userRepository = repository ?? new UserRepository();
        }

        /// <summary>
        /// 注册新用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>注册成功返回用户对象，失败抛出异常</returns>
        public User RegisterUser(string username, string password)
        {
            // 参数验证
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("用户名不能为空");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("密码不能为空");

            if (password.Length < 6)
                throw new ArgumentException("密码长度不能少于6个字符");

            // 创建用户
            return _userRepository.CreateUser(username, password);
        }

        /// <summary>
        /// 用户登录验证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>验证成功返回用户对象，失败返回null</returns>
        public User Login(string username, string password)
        {
            // 参数验证
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            // 验证用户
            var user = _userRepository.ValidateUser(username, password);
            if (user != null)
            {
                // 更新最后登录时间
                _userRepository.UpdateLastLoginTime(user.Id);
                
                // 重新获取用户信息，包含更新后的最后登录时间
                return _userRepository.GetUserById(user.Id);
            }

            return null;
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns>用户列表</returns>
        public List<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        /// <summary>
        /// 根据ID获取用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户对象，如果不存在则返回null</returns>
        public User GetUserById(string userId)
        {
            return _userRepository.GetUserById(userId);
        }

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>用户对象，如果不存在则返回null</returns>
        public User GetUserByUsername(string username)
        {
            return _userRepository.GetUserByUsername(username);
        }

        /// <summary>
        /// 更改用户密码
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>操作成功返回true，失败返回false</returns>
        public bool ChangePassword(string userId, string oldPassword, string newPassword)
        {
            // 参数验证
            if (string.IsNullOrWhiteSpace(userId) || 
                string.IsNullOrWhiteSpace(oldPassword) || 
                string.IsNullOrWhiteSpace(newPassword))
                return false;

            if (newPassword.Length < 6)
                throw new ArgumentException("新密码长度不能少于6个字符");

            // 获取用户信息
            var user = _userRepository.GetUserById(userId);
            if (user == null)
                return false;

            // 验证旧密码
            var validUser = _userRepository.ValidateUser(user.Username, oldPassword);
            if (validUser == null)
                return false;

            // 更新密码
            _userRepository.UpdatePassword(userId, newPassword);
            return true;
        }

        /// <summary>
        /// 重置用户密码（管理员功能）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="newPassword">新密码</param>
        public void ResetPassword(string userId, string newPassword)
        {
            // 参数验证
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("用户ID和新密码不能为空");

            if (newPassword.Length < 6)
                throw new ArgumentException("新密码长度不能少于6个字符");

            _userRepository.UpdatePassword(userId, newPassword);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        public void DeleteUser(string userId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                _userRepository.DeleteUser(userId);
            }
        }
        
        /// <summary>
        /// 更新用户最后登录时间
        /// </summary>
        /// <param name="userId">用户ID</param>
        public void UpdateLastLoginTime(string userId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                _userRepository.UpdateLastLoginTime(userId);
            }
        }
    }
} 