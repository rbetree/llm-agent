# 开发笔记

## 2025-04-16 文档结构调整：API整合到技术设计

### 问题
API参考文档作为独立的导航项存在，但其内容本质上是技术设计文档的一部分，与项目结构、数据存储等技术文档放在一起更加合理。

### 解决方案
1. 将API文档从api目录移动到architecture目录：
   - `api/index.md` → `architecture/api-overview.md`
   - `api/models.md` → `architecture/api-models.md`

2. 更新VitePress配置文件（`.vitepress/config.mjs`）：
   - 从导航栏中移除API项
   - 在技术设计侧边栏中添加API相关条目
   - 移除独立的API侧边栏配置

3. 更新API文档中的内部链接，确保它们指向新的路径。

4. 删除api目录下的原始文件。

这样调整后，文档结构更加合理，所有技术文档都集中在技术设计部分，简化了导航层级，使文档结构更加清晰。 