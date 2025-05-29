using System;

namespace llm_agent.Model
{
    /// <summary>
    /// AI网站实体类
    /// </summary>
    public class AiWebsite
    {
        /// <summary>
        /// 网站唯一标识符
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 网站名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 网站描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 网站URL
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 网站图标URL
        /// </summary>
        public string IconUrl { get; set; } = string.Empty;

        /// <summary>
        /// 分类名称
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int SortOrder { get; set; } = 0;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 最后访问时间
        /// </summary>
        public DateTime? LastVisitedAt { get; set; }



        /// <summary>
        /// 网站凭据信息（导航属性，不存储在数据库中）
        /// </summary>
        public WebsiteCredential Credential { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AiWebsite()
        {
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        /// <param name="name">网站名称</param>
        /// <param name="url">网站URL</param>
        /// <param name="description">网站描述</param>
        /// <param name="category">分类名称</param>
        public AiWebsite(string name, string url, string description = "", string category = "")
        {
            Name = name;
            Url = url;
            Description = description;
            Category = category;
        }

        /// <summary>
        /// 更新最后访问时间
        /// </summary>
        public void UpdateLastVisitedTime()
        {
            LastVisitedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// 获取显示名称（如果名称为空则返回URL的域名部分）
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(Name))
                    return Name;

                try
                {
                    var uri = new Uri(Url);
                    return uri.Host;
                }
                catch
                {
                    return Url;
                }
            }
        }

        /// <summary>
        /// 获取格式化的最后访问时间
        /// </summary>
        public string LastVisitedDisplay
        {
            get
            {
                if (!LastVisitedAt.HasValue)
                    return "从未访问";

                var timeSpan = DateTime.Now - LastVisitedAt.Value;
                if (timeSpan.TotalMinutes < 1)
                    return "刚刚访问";
                else if (timeSpan.TotalHours < 1)
                    return $"{(int)timeSpan.TotalMinutes}分钟前";
                else if (timeSpan.TotalDays < 1)
                    return $"{(int)timeSpan.TotalHours}小时前";
                else if (timeSpan.TotalDays < 30)
                    return $"{(int)timeSpan.TotalDays}天前";
                else
                    return LastVisitedAt.Value.ToString("yyyy-MM-dd");
            }
        }

        /// <summary>
        /// 验证网站数据是否有效
        /// </summary>
        /// <returns>验证结果</returns>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return false;

            if (string.IsNullOrWhiteSpace(Url))
                return false;

            try
            {
                var uri = new Uri(Url);
                return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取验证错误信息
        /// </summary>
        /// <returns>错误信息列表</returns>
        public string GetValidationErrors()
        {
            var errors = new System.Collections.Generic.List<string>();

            if (string.IsNullOrWhiteSpace(Name))
                errors.Add("网站名称不能为空");

            if (string.IsNullOrWhiteSpace(Url))
                errors.Add("网站URL不能为空");
            else
            {
                try
                {
                    var uri = new Uri(Url);
                    if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                        errors.Add("网站URL必须是有效的HTTP或HTTPS地址");
                }
                catch
                {
                    errors.Add("网站URL格式无效");
                }
            }

            return string.Join("; ", errors);
        }
    }
}
