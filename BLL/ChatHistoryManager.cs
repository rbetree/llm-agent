using System;
using System.Collections.Generic;
using System.Linq;
using llm_agent.Model;
using llm_agent.DAL;

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
        
        public List<ChatSession> GetAllSessions()
        {
            return _repository.GetAllSessions();
        }
        
        public ChatSession GetCurrentSession()
        {
            if (_currentSession == null)
            {
                // 不自动创建新会话，而是尝试获取已有会话列表中的第一个
                var sessions = GetAllSessions();
                if (sessions.Count > 0)
                {
                    _currentSession = sessions[0];
                }
                // 如果没有会话，返回null
            }
            return _currentSession;
        }
        
        public ChatSession GetOrCreateSession(string? sessionId = null)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return CreateNewSession();
            }
            
            var session = _repository.LoadChatSession(sessionId);
            if (session == null)
            {
                session = new ChatSession { Id = sessionId };
            }
            
            _currentSession = session;
            return session;
        }
        
        public ChatSession CreateNewSession()
        {
            _currentSession = new ChatSession();
            _repository.SaveChatSession(_currentSession);
            return _currentSession;
        }
        
        public void SaveSession(ChatSession session)
        {
            if (session == null) return;
            
            session.UpdatedAt = DateTime.Now;
            _repository.SaveChatSession(session);
        }
        
        public void AddMessageToSession(ChatSession session, ChatMessage message)
        {
            if (session == null || message == null) return;
            
            session.Messages.Add(message);
            session.UpdatedAt = DateTime.Now;
            _repository.SaveChatSession(session);
        }
        
        public void UpdateSessionTitle(ChatSession session, string title)
        {
            if (session == null) return;
            
            session.Title = title ?? string.Empty;
            session.UpdatedAt = DateTime.Now;
            _repository.SaveChatSession(session);
        }
        
        public void ClearAllChatHistory()
        {
            // 删除所有聊天记录
            _repository.DeleteAllSessions();
            
            // 创建一个新的会话
            CreateNewSession();
        }

        public void DeleteChat(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId)) return;

            // 从数据库中删除会话
            _repository.DeleteSession(sessionId);

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
        public void UpdateSessionOrder(List<ChatSession> sessions)
        {
            if (sessions == null || sessions.Count == 0)
                return;
                
            _repository.UpdateSessionOrder(sessions);
        }
    }
}