using System;

namespace llm_agent.Model
{
    /// <summary>
    /// 提示词实体类
    /// </summary>
    public class Prompt
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 提示词标题
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 提示词内容
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        
        /// <summary>
        /// 使用次数
        /// </summary>
        public int UsageCount { get; set; }
        
        /// <summary>
        /// 创建新的提示词实例
        /// </summary>
        public Prompt()
        {
            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            UsageCount = 0;
        }
        
        /// <summary>
        /// 创建新的提示词实例（带参数）
        /// </summary>
        public Prompt(string title, string content, string category)
        {
            Id = Guid.NewGuid().ToString();
            Title = title;
            Content = content;
            Category = category;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            UsageCount = 0;
        }
        
        /// <summary>
        /// 增加使用次数
        /// </summary>
        public void IncrementUsageCount()
        {
            UsageCount++;
            UpdatedAt = DateTime.Now;
        }
    }
} 