using System;

namespace llm_agent.Model
{
    public class ChatMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public ChatRole Role { get; set; } = ChatRole.User;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string ModelId { get; set; } = string.Empty;
        
        public string RoleString 
        { 
            get
            {
                return Role switch
                {
                    ChatRole.User => "用户",
                    ChatRole.Assistant => "助手",
                    ChatRole.System => "系统",
                    _ => "未知"
                };
            }
        }
    }
}