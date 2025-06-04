import { withMermaid } from "vitepress-plugin-mermaid";

export default withMermaid({
  title: 'LLM-Agent',
  description: '支持多个大语言模型服务商的Windows桌面客户端应用程序',
  lang: 'zh-CN',
  base: '/llm-agent/', // 根据GitHub仓库名设置
  lastUpdated: true,
  
  head: [
    ['link', { rel: 'icon', type: 'image/x-icon', href: '/llm-agent/favicon.ico' }]
  ],
  
  themeConfig: {
    logo: '/images/logo.png',
    
    // 导航栏
    nav: [
      { text: '首页', link: '/' },
      { text: '指南', link: '/guide/' },
      { text: '技术设计', link: '/architecture/' }
    ],
    
    // 侧边栏
    sidebar: {
      '/guide/': [
        {
          text: '指南',
          items: [
            { text: '介绍', link: '/guide/' },
            { text: '安装&卸载', link: '/guide/installation' },
            { text: '使用', link: '/guide/usage' },
            { text: '故障排除', link: '/guide/debug' }
          ]
        }
      ],
      '/architecture/': [
        {
          text: '技术设计',
          items: [
            { text: '概述', link: '/architecture/' },
            { text: '项目结构', link: '/architecture/structure' },
            { text: '技术栈', link: '/architecture/tech-stack' },
            { text: '数据存储', link: '/architecture/data-storage' },
            { text: 'UI设计', link: '/architecture/ui-design' },
            { text: '交互逻辑', link: '/architecture/interaction' },
            { text: 'API', link: '/architecture/api-overview' }
          ]
        }
      ]
    },
    
    // 社交链接
    socialLinks: [
      { icon: 'github', link: 'https://github.com/rbetree/llm-agent' }
    ],
    
    // 页脚
    footer: {
      message: '基于 MIT 许可发布',
      copyright: 'Copyright © 2025 rbetree. All rights reserved'
    },
    
    // 搜索
    search: {
      provider: 'local'
    },
    
    // 编辑链接
    editLink: {
      pattern: 'https://github.com/rbetree/llm-agent/edit/main/Docs/:path',
      text: '在 GitHub 上编辑此页'
    }
  },
  
  // Mermaid配置
  mermaid: {
    // Mermaid配置选项，参考 https://mermaid.js.org/config/setup/modules/mermaidAPI.html
    theme: 'default',
    darkTheme: 'dark'
  }
}); 