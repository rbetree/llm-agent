using System;
using System.Collections.Generic;
using System.Linq;
using llm_agent.Model;
using llm_agent.DAL;
using llm_agent.Common;

namespace llm_agent.BLL
{
    public class ChatHistoryManager
    {
        private readonly ChatRepository _repository;
        private ChatSession? _currentSession;
        
        public ChatHistoryManager(ChatRepository? repository = null)
        {
            _repository = repository ?? new ChatRepository();
            _currentSession = null;
        }
        
        /// <summary>
        /// 获取所有聊天会话
        /// </summary>
        /// <param name="userId">用户ID，如果提供则只返回该用户的会话</param>
        /// <returns>聊天会话列表</returns>
        public List<ChatSession> GetAllSessions(string userId = null)
        {
            return _repository.GetAllSessions(userId);
        }
        
        /// <summary>
        /// 获取当前会话
        /// </summary>
        /// <param name="userId">用户ID，用于验证会话所有权</param>
        /// <returns>当前聊天会话</returns>
        public ChatSession GetCurrentSession(string userId = null)
        {
            if (_currentSession == null)
            {
                // 不自动创建新会话，而是尝试获取已有会话列表中的第一个
                var sessions = GetAllSessions(userId);
                if (sessions.Count > 0)
                {
                    _currentSession = sessions[0];
                }
                // 如果没有会话，返回null
            }
            else if (userId != null)
            {
                // 验证当前会话是否属于该用户
                var session = _repository.LoadChatSession(_currentSession.Id, userId);
                if (session == null)
                {
                    // 如果当前会话不属于该用户，尝试获取该用户的第一个会话
                    var sessions = GetAllSessions(userId);
                    if (sessions.Count > 0)
                    {
                        _currentSession = sessions[0];
                    }
                    else
                    {
                        _currentSession = null;
                    }
                }
            }
            
            return _currentSession;
        }
        
        /// <summary>
        /// 获取或创建会话
        /// </summary>
        /// <param name="sessionId">会话ID</param>
        /// <param name="userId">用户ID，用于验证会话所有权</param>
        /// <returns>聊天会话</returns>
        public ChatSession GetOrCreateSession(string? sessionId = null, string userId = null)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return CreateNewSession(userId);
            }
            
            var session = _repository.LoadChatSession(sessionId, userId);
            if (session == null)
            {
                session = new ChatSession { Id = sessionId };
            }
            
            _currentSession = session;
            return session;
        }
        
        /// <summary>
        /// 创建新会话
        /// </summary>
        /// <param name="userId">用户ID，用于关联会话所有权</param>
        /// <returns>新创建的聊天会话</returns>
        public ChatSession CreateNewSession(string userId = null)
        {
            _currentSession = new ChatSession();
            _repository.SaveChatSession(_currentSession, userId);
            return _currentSession;
        }
        
        /// <summary>
        /// 保存会话
        /// </summary>
        /// <param name="session">要保存的会话</param>
        /// <param name="userId">用户ID，用于关联会话所有权</param>
        public void SaveSession(ChatSession session, string userId = null)
        {
            if (session == null) return;
            
            session.UpdatedAt = DateTime.Now;
            _repository.SaveChatSession(session, userId);
        }
        
        /// <summary>
        /// 向会话添加消息
        /// </summary>
        /// <param name="session">目标会话</param>
        /// <param name="message">要添加的消息</param>
        /// <param name="userId">用户ID，用于关联会话所有权</param>
        public void AddMessageToSession(ChatSession session, ChatMessage message, string userId = null)
        {
            if (session == null || message == null) return;
            
            session.Messages.Add(message);
            session.UpdatedAt = DateTime.Now;
            _repository.SaveChatSession(session, userId);
        }
        
        /// <summary>
        /// 更新会话标题
        /// </summary>
        /// <param name="session">目标会话</param>
        /// <param name="title">新标题</param>
        /// <param name="userId">用户ID，用于关联会话所有权</param>
        public void UpdateSessionTitle(ChatSession session, string title, string userId = null)
        {
            if (session == null) return;
            
            session.Title = title ?? string.Empty;
            session.UpdatedAt = DateTime.Now;
            _repository.SaveChatSession(session, userId);
        }
        
        /// <summary>
        /// 清除所有聊天记录
        /// </summary>
        /// <param name="userId">用户ID，如果提供则只清除该用户的会话</param>
        public void ClearAllSessions(string userId = null)
        {
            // 删除所有聊天记录
            _repository.DeleteAllSessions(userId);
            
            // 创建一个新的会话
            CreateNewSession(userId);
        }

        /// <summary>
        /// 删除指定会话
        /// </summary>
        /// <param name="sessionId">会话ID</param>
        /// <param name="userId">用户ID，用于验证会话所有权</param>
        public void DeleteChat(string sessionId, string userId = null)
        {
            if (string.IsNullOrEmpty(sessionId)) return;

            // 从数据库中删除会话
            _repository.DeleteSession(sessionId, userId);

            // 如果当前会话被删除，清空当前会话
            if (_currentSession?.Id == sessionId)
            {
                _currentSession = null;
            }
        }
        
        /// <summary>
        /// 更新会话的排序顺序
        /// </summary>
        /// <param name="sessions">会话列表，按照期望的显示顺序排列</param>
        /// <param name="userId">用户ID，用于验证会话所有权</param>
        public void UpdateSessionOrder(List<ChatSession> sessions, string userId = null)
        {
            if (sessions == null || sessions.Count == 0)
                return;
                
            _repository.UpdateSessionOrder(sessions, userId);
        }
        
        /// <summary>
        /// 将无主会话分配给指定用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        public void AssignOrphanedSessionsToUser(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return;
            
            _repository.AssignOrphanedSessionsToUser(userId);
        }
    }
}