using System;
using System.Collections.Generic;
using llm_agent.Model;

namespace llm_agent.UI.Controls.ChatForm
{
    /// <summary>
    /// 适配器类，负责将ChatMessage转换为TextChatModel
    /// </summary>
    public class ChatModelAdapter
    {
        /// <summary>
        /// 将ChatMessage转换为TextChatModel
        /// </summary>
        /// <param name="message">源ChatMessage对象</param>
        /// <returns>转换后的TextChatModel对象</returns>
        public static TextChatModel ToTextChatModel(ChatMessage message)
        {
            if (message == null)
                return null;

            return new TextChatModel
            {
                Body = message.Content,
                Author = GetAuthorFromRole(message.Role),
                Time = message.CreatedAt,
                Inbound = IsInbound(message.Role),
                Read = true // 默认已读
            };
        }

        /// <summary>
        /// 将多个ChatMessage转换为对应的TextChatModel列表
        /// </summary>
        /// <param name="messages">ChatMessage列表</param>
        /// <returns>转换后的TextChatModel列表</returns>
        public static List<TextChatModel> ToTextChatModels(IEnumerable<ChatMessage> messages)
        {
            if (messages == null)
                return new List<TextChatModel>();

            var result = new List<TextChatModel>();
            foreach (var message in messages)
            {
                var model = ToTextChatModel(message);
                if (model != null)
                {
                    result.Add(model);
                }
            }
            return result;
        }

        /// <summary>
        /// 根据ChatRole获取对应的Author名称
        /// </summary>
        /// <param name="role">聊天角色</param>
        /// <returns>对应的作者名称</returns>
        private static string GetAuthorFromRole(ChatRole role)
        {
            return role switch
            {
                ChatRole.User => "用户",
                ChatRole.Assistant => "助手",
                ChatRole.System => "系统",
                _ => "未知"
            };
        }

        /// <summary>
        /// 判断消息是否为入站消息（非用户发送的消息）
        /// </summary>
        /// <param name="role">聊天角色</param>
        /// <returns>如果是Assistant或System则返回true，否则返回false</returns>
        private static bool IsInbound(ChatRole role)
        {
            // 在winforms-chat中，Inbound=true表示接收到的消息（右侧气泡）
            // Inbound=false表示发送的消息（左侧气泡）
            // 因此User消息为false，其他为true
            return role != ChatRole.User;
        }

        /// <summary>
        /// 从TextChatModel创建ChatMessage
        /// </summary>
        /// <param name="model">TextChatModel对象</param>
        /// <returns>转换后的ChatMessage对象</returns>
        public static ChatMessage ToChatMessage(TextChatModel model)
        {
            if (model == null)
                return null;

            return new ChatMessage
            {
                Content = model.Body,
                Role = GetRoleFromAuthor(model.Author, model.Inbound),
                CreatedAt = model.Time,
                UpdatedAt = model.Time,
                Timestamp = model.Time
            };
        }

        /// <summary>
        /// 根据Author名称和Inbound属性获取对应的ChatRole
        /// </summary>
        /// <param name="author">作者名称</param>
        /// <param name="inbound">是否为入站消息</param>
        /// <returns>对应的ChatRole</returns>
        private static ChatRole GetRoleFromAuthor(string author, bool inbound)
        {
            // 优先根据inbound判断
            if (!inbound)
                return ChatRole.User;

            // 然后根据author名称判断具体是哪类角色
            return author.ToLower() switch
            {
                "系统" => ChatRole.System,
                "助手" => ChatRole.Assistant,
                _ => ChatRole.Assistant // 默认为助手
            };
        }
    }
} 