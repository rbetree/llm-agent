using System;
using System.Collections.Generic;
using llm_agent.DAL;
using llm_agent.Model;

namespace llm_agent.BLL
{
    /// <summary>
    /// 提示词管理类，处理提示词的业务逻辑
    /// </summary>
    public class PromptManager
    {
        private readonly PromptRepository _promptRepository;

        /// <summary>
        /// 初始化提示词管理器
        /// </summary>
        public PromptManager()
        {
            _promptRepository = new PromptRepository();
        }

        /// <summary>
        /// 创建新提示词
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="category">分类</param>
        /// <returns>创建的提示词</returns>
        public Prompt CreatePrompt(string title, string content, string category)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("提示词标题不能为空", nameof(title));
            
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("提示词内容不能为空", nameof(content));
            
            if (string.IsNullOrWhiteSpace(category))
                category = "未分类"; // 设置默认分类
            
            var prompt = new Prompt(title, content, category);
            _promptRepository.SavePrompt(prompt);
            return prompt;
        }

        /// <summary>
        /// 更新提示词
        /// </summary>
        /// <param name="promptId">提示词ID</param>
        /// <param name="title">新标题</param>
        /// <param name="content">新内容</param>
        /// <param name="category">新分类</param>
        /// <returns>更新后的提示词，如果不存在则返回null</returns>
        public Prompt UpdatePrompt(string promptId, string title, string content, string category)
        {
            var prompt = _promptRepository.GetPrompt(promptId);
            if (prompt == null)
                return null;
            
            if (!string.IsNullOrWhiteSpace(title))
                prompt.Title = title;
            
            if (!string.IsNullOrWhiteSpace(content))
                prompt.Content = content;
            
            if (!string.IsNullOrWhiteSpace(category))
                prompt.Category = category;
            
            prompt.UpdatedAt = DateTime.Now;
            _promptRepository.SavePrompt(prompt);
            return prompt;
        }

        /// <summary>
        /// 获取提示词
        /// </summary>
        /// <param name="promptId">提示词ID</param>
        /// <returns>提示词对象，如果不存在则返回null</returns>
        public Prompt GetPrompt(string promptId)
        {
            return _promptRepository.GetPrompt(promptId);
        }

        /// <summary>
        /// 获取所有提示词
        /// </summary>
        /// <returns>提示词列表</returns>
        public List<Prompt> GetAllPrompts()
        {
            return _promptRepository.GetAllPrompts();
        }

        /// <summary>
        /// 按分类获取提示词
        /// </summary>
        /// <param name="category">分类名称</param>
        /// <returns>属于指定分类的提示词列表</returns>
        public List<Prompt> GetPromptsByCategory(string category)
        {
            return _promptRepository.GetPromptsByCategory(category);
        }

        /// <summary>
        /// 搜索提示词
        /// </summary>
        /// <param name="searchText">搜索文本</param>
        /// <returns>匹配的提示词列表</returns>
        public List<Prompt> SearchPrompts(string searchText)
        {
            return _promptRepository.SearchPrompts(searchText);
        }

        /// <summary>
        /// 删除提示词
        /// </summary>
        /// <param name="promptId">要删除的提示词ID</param>
        /// <returns>是否成功删除</returns>
        public bool DeletePrompt(string promptId)
        {
            var prompt = _promptRepository.GetPrompt(promptId);
            if (prompt == null)
                return false;
            
            _promptRepository.DeletePrompt(promptId);
            return true;
        }

        /// <summary>
        /// 使用提示词（增加使用次数）
        /// </summary>
        /// <param name="promptId">提示词ID</param>
        /// <returns>是否成功更新</returns>
        public bool UsePrompt(string promptId)
        {
            var prompt = _promptRepository.GetPrompt(promptId);
            if (prompt == null)
                return false;
            
            _promptRepository.IncrementUsageCount(promptId);
            return true;
        }

        /// <summary>
        /// 获取所有可用的提示词分类
        /// </summary>
        /// <returns>分类列表</returns>
        public List<string> GetAllCategories()
        {
            return _promptRepository.GetAllCategories();
        }

        /// <summary>
        /// 创建默认提示词示例
        /// </summary>
        /// <returns>创建的示例提示词列表</returns>
        public List<Prompt> CreateDefaultPrompts()
        {
            var defaultPrompts = new List<Prompt>
            {
                new Prompt
                {
                    Title = "自我介绍",
                    Content = "请介绍一下你自己，包括你的能力和局限性。",
                    Category = "常用"
                },
                new Prompt
                {
                    Title = "代码解释",
                    Content = "请解释以下代码的功能和工作原理：\n\n```\n[将代码粘贴在这里]\n```",
                    Category = "编程"
                },
                new Prompt
                {
                    Title = "问题排查",
                    Content = "我在使用以下代码时遇到了问题：\n\n```\n[将代码粘贴在这里]\n```\n\n错误信息如下：\n[错误信息]\n\n请帮我分析问题并提供解决方案。",
                    Category = "编程"
                },
                new Prompt
                {
                    Title = "内容总结",
                    Content = "请用简洁的语言总结以下内容的要点：\n\n[将内容粘贴在这里]",
                    Category = "写作"
                },
                new Prompt
                {
                    Title = "头脑风暴",
                    Content = "我正在考虑关于[主题]的创意。请提供10个有创意的想法，涵盖不同角度和可能性。",
                    Category = "创意"
                }
            };

            foreach (var prompt in defaultPrompts)
            {
                _promptRepository.SavePrompt(prompt);
            }

            return defaultPrompts;
        }
    }
} 