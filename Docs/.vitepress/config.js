export default {
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
      { text: 'API', link: '/api/' },
      { text: '架构', link: '/architecture/' }
    ],
    
    // 侧边栏
    sidebar: {
      '/guide/': [
        {
          text: '指南',
          items: [
            { text: '介绍', link: '/guide/' },
            { text: '安装', link: '/guide/installation' },
            { text: '使用', link: '/guide/usage' },
            { text: 'UI设计', link: '/guide/ui-design' },
            { text: '交互逻辑', link: '/guide/interaction' },
            { text: '开发路线图', link: '/guide/roadmap' }
          ]
        }
      ],
      '/api/': [
        {
          text: 'API参考',
          items: [
            { text: '概述', link: '/api/' },
            { text: '模型接口', link: '/api/models' }
          ]
        }
      ],
      '/architecture/': [
        {
          text: '架构设计',
          items: [
            { text: '概述', link: '/architecture/' },
            { text: '项目结构', link: '/architecture/structure' },
            { text: '技术栈', link: '/architecture/tech-stack' }
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
  }
} 