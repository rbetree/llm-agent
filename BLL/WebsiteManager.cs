using System;
using System.Collections.Generic;
using System.Linq;
using llm_agent.DAL;
using llm_agent.Model;
using llm_agent.Common.Exceptions;

namespace llm_agent.BLL
{
    /// <summary>
    /// 网站管理器 - 提供AI网站的业务逻辑管理
    /// </summary>
    public class WebsiteManager
    {
        private readonly WebsiteRepository _repository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WebsiteManager()
        {
            _repository = new WebsiteRepository();
            InitializeDefaultWebsitesIfNeeded();
        }

        /// <summary>
        /// 获取所有网站
        /// </summary>
        /// <returns>网站列表</returns>
        public List<AiWebsite> GetAllWebsites()
        {
            try
            {
                return _repository.GetAllWebsites();
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"获取网站列表失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 获取所有网站（包含凭据信息）
        /// </summary>
        /// <returns>包含凭据的网站列表</returns>
        public List<AiWebsite> GetAllWebsitesWithCredentials()
        {
            try
            {
                return _repository.GetAllWebsitesWithCredentials();
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"获取网站列表失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 根据分类获取网站
        /// </summary>
        /// <param name="category">分类名称</param>
        /// <returns>网站列表</returns>
        public List<AiWebsite> GetWebsitesByCategory(string category)
        {
            try
            {
                return _repository.GetWebsitesByCategory(category);
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"获取分类网站失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 搜索网站
        /// </summary>
        /// <param name="searchText">搜索文本</param>
        /// <returns>匹配的网站列表</returns>
        public List<AiWebsite> SearchWebsites(string searchText)
        {
            try
            {
                return _repository.SearchWebsites(searchText);
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"搜索网站失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 搜索网站（包含凭据信息）
        /// </summary>
        /// <param name="searchText">搜索文本</param>
        /// <returns>包含凭据的匹配网站列表</returns>
        public List<AiWebsite> SearchWebsitesWithCredentials(string searchText)
        {
            try
            {
                return _repository.SearchWebsitesWithCredentials(searchText);
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"搜索网站失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 根据ID获取网站
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>网站信息</returns>
        public AiWebsite GetWebsiteById(string websiteId)
        {
            try
            {
                return _repository.GetWebsiteById(websiteId);
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"获取网站信息失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 保存网站
        /// </summary>
        /// <param name="website">网站信息</param>
        public void SaveWebsite(AiWebsite website)
        {
            try
            {
                // 验证网站数据
                var validationErrors = website.GetValidationErrors();
                if (!string.IsNullOrEmpty(validationErrors))
                {
                    throw new ValidationException($"网站数据验证失败: {validationErrors}");
                }

                // 更新时间戳
                website.UpdatedAt = DateTime.Now;

                _repository.SaveWebsite(website);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"保存网站失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 删除网站
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        public void DeleteWebsite(string websiteId)
        {
            try
            {
                _repository.DeleteWebsite(websiteId);
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"删除网站失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 更新网站访问时间
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        public void UpdateWebsiteVisitTime(string websiteId)
        {
            try
            {
                var website = _repository.GetWebsiteById(websiteId);
                if (website != null)
                {
                    website.UpdateLastVisitedTime();
                    _repository.SaveWebsite(website);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"更新访问时间失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <returns>分类列表</returns>
        public List<string> GetAllCategories()
        {
            try
            {
                return _repository.GetAllCategories();
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"获取分类列表失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 获取网站数量统计
        /// </summary>
        /// <returns>网站数量</returns>
        public int GetWebsiteCount()
        {
            try
            {
                return _repository.GetWebsiteCount();
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"获取网站数量失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 获取带凭据的网站完整信息
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>包含凭据的网站信息</returns>
        public AiWebsite GetWebsiteWithCredential(string websiteId)
        {
            try
            {
                return _repository.GetWebsiteWithCredential(websiteId);
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"获取网站完整信息失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 保存网站凭据
        /// </summary>
        /// <param name="credential">凭据信息</param>
        public void SaveWebsiteCredential(WebsiteCredential credential)
        {
            try
            {
                // 验证凭据数据
                if (credential != null && !credential.IsValid())
                {
                    var validationErrors = credential.GetValidationErrors();
                    throw new ValidationException($"凭据数据验证失败: {validationErrors}");
                }

                if (credential != null)
                {
                    // 更新时间戳
                    credential.UpdatedAt = DateTime.Now;
                    _repository.SaveWebsiteCredential(credential);
                }
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"保存网站凭据失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 如果需要，初始化默认网站数据
        /// </summary>
        private void InitializeDefaultWebsitesIfNeeded()
        {
            try
            {
                // 检查是否已有网站数据
                var existingCount = _repository.GetWebsiteCount();
                if (existingCount == 0)
                {
                    // 初始化默认网站数据
                    _repository.InitializeDefaultWebsites();
                }
            }
            catch (Exception ex)
            {
                // 记录错误但不抛出异常，避免影响应用启动
                Console.WriteLine($"初始化默认网站数据时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 创建新网站
        /// </summary>
        /// <param name="name">网站名称</param>
        /// <param name="url">网站URL</param>
        /// <param name="description">网站描述</param>
        /// <param name="category">分类</param>
        /// <returns>创建的网站</returns>
        public AiWebsite CreateWebsite(string name, string url, string description = "", string category = "")
        {
            var website = new AiWebsite(name, url, description, category);
            SaveWebsite(website);
            return website;
        }

        /// <summary>
        /// 批量导入网站
        /// </summary>
        /// <param name="websites">网站列表</param>
        /// <returns>成功导入的数量</returns>
        public int ImportWebsites(List<AiWebsite> websites)
        {
            int successCount = 0;
            foreach (var website in websites)
            {
                try
                {
                    SaveWebsite(website);
                    successCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"导入网站 {website.Name} 失败: {ex.Message}");
                }
            }
            return successCount;
        }

        /// <summary>
        /// 获取最近访问的网站
        /// </summary>
        /// <param name="count">返回数量</param>
        /// <returns>最近访问的网站列表</returns>
        public List<AiWebsite> GetRecentlyVisitedWebsites(int count = 10)
        {
            try
            {
                return _repository.GetRecentlyVisitedWebsites(count);
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"获取最近访问网站失败: {ex.Message}", ex);
            }
        }
    }
}
