using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using llm_agent.Model;

namespace llm_agent.UI.Controls
{
    /// <summary>
    /// 内置浏览器用户控件
    /// </summary>
    public partial class WebsiteBrowser : UserControl
    {
        private WebView2 webView;
        private AiWebsite _currentWebsite;
        private bool _isNavigating = false;

        /// <summary>
        /// 导航开始事件
        /// </summary>
        public event EventHandler<CoreWebView2NavigationStartingEventArgs> NavigationStarting;

        /// <summary>
        /// 导航完成事件
        /// </summary>
        public event EventHandler<CoreWebView2NavigationCompletedEventArgs> NavigationCompleted;

        /// <summary>
        /// 页面标题改变事件
        /// </summary>
        public event EventHandler<object> DocumentTitleChanged;

        /// <summary>
        /// 当前访问的网站
        /// </summary>
        public AiWebsite CurrentWebsite
        {
            get => _currentWebsite;
            private set => _currentWebsite = value;
        }

        /// <summary>
        /// 当前页面URL
        /// </summary>
        public string CurrentUrl => webView?.Source?.ToString() ?? string.Empty;

        /// <summary>
        /// 当前页面标题
        /// </summary>
        public string CurrentTitle => webView?.CoreWebView2?.DocumentTitle ?? string.Empty;

        /// <summary>
        /// 是否正在导航
        /// </summary>
        public bool IsNavigating => _isNavigating;

        /// <summary>
        /// 是否可以后退
        /// </summary>
        public bool CanGoBack => webView?.CanGoBack ?? false;

        /// <summary>
        /// 是否可以前进
        /// </summary>
        public bool CanGoForward => webView?.CanGoForward ?? false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WebsiteBrowser()
        {
            InitializeComponent();
            InitializeWebView();
        }

        /// <summary>
        /// 初始化WebView2控件
        /// </summary>
        private async void InitializeWebView()
        {
            try
            {
                webView = new WebView2()
                {
                    Dock = DockStyle.Fill
                };

                // 添加到浏览器面板
                panelBrowser.Controls.Add(webView);

                // 等待WebView2初始化完成
                await webView.EnsureCoreWebView2Async(null);

                // 设置事件处理
                SetupWebViewEvents();

                // 更新导航按钮状态
                UpdateNavigationButtons();

                // 设置默认页面
                _ = NavigateToDefaultPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"初始化浏览器失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 设置WebView事件处理
        /// </summary>
        private void SetupWebViewEvents()
        {
            if (webView?.CoreWebView2 == null) return;

            // 导航开始事件
            webView.CoreWebView2.NavigationStarting += (sender, e) =>
            {
                _isNavigating = true;
                txtUrl.Text = e.Uri;
                UpdateNavigationButtons();
                NavigationStarting?.Invoke(this, e);
            };

            // 导航完成事件
            webView.CoreWebView2.NavigationCompleted += (sender, e) =>
            {
                _isNavigating = false;
                txtUrl.Text = webView.Source?.ToString() ?? string.Empty;
                UpdateNavigationButtons();
                NavigationCompleted?.Invoke(this, e);
            };

            // 页面标题改变事件
            webView.CoreWebView2.DocumentTitleChanged += (sender, e) =>
            {
                DocumentTitleChanged?.Invoke(this, e);
            };

            // 源改变事件
            webView.CoreWebView2.SourceChanged += (sender, e) =>
            {
                txtUrl.Text = webView.Source?.ToString() ?? string.Empty;
            };
        }

        /// <summary>
        /// 导航到指定URL
        /// </summary>
        /// <param name="url">目标URL</param>
        public async void NavigateToUrl(string url)
        {
            try
            {
                if (webView?.CoreWebView2 == null)
                {
                    await webView.EnsureCoreWebView2Async(null);
                }

                if (!string.IsNullOrEmpty(url))
                {
                    // 确保URL格式正确
                    if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                    {
                        url = "https://" + url;
                    }

                    webView.CoreWebView2.Navigate(url);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导航失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 导航到网站
        /// </summary>
        /// <param name="website">目标网站</param>
        public void NavigateToWebsite(AiWebsite website)
        {
            if (website == null) return;

            _currentWebsite = website;
            NavigateToUrl(website.Url);
        }

        /// <summary>
        /// 后退
        /// </summary>
        public void GoBack()
        {
            if (CanGoBack)
            {
                webView.GoBack();
            }
        }

        /// <summary>
        /// 前进
        /// </summary>
        public void GoForward()
        {
            if (CanGoForward)
            {
                webView.GoForward();
            }
        }

        /// <summary>
        /// 刷新页面
        /// </summary>
        public new void Refresh()
        {
            webView?.Reload();
        }

        /// <summary>
        /// 停止加载
        /// </summary>
        public void Stop()
        {
            webView?.Stop();
        }

        /// <summary>
        /// 导航到默认页面
        /// </summary>
        private async System.Threading.Tasks.Task NavigateToDefaultPage()
        {
            try
            {
                string defaultHtml = @"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>AI网站导航</title>
                    <style>
                        body {
                            font-family: 'Microsoft YaHei', sans-serif;
                            margin: 0;
                            padding: 40px;
                            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                            color: white;
                            text-align: center;
                        }
                        .container {
                            max-width: 600px;
                            margin: 0 auto;
                            background: rgba(255,255,255,0.1);
                            padding: 40px;
                            border-radius: 15px;
                            backdrop-filter: blur(10px);
                        }
                        h1 {
                            font-size: 2.5em;
                            margin-bottom: 20px;
                            text-shadow: 2px 2px 4px rgba(0,0,0,0.3);
                        }
                        p {
                            font-size: 1.2em;
                            line-height: 1.6;
                            margin-bottom: 30px;
                        }
                        .features {
                            text-align: left;
                            margin: 30px 0;
                        }
                        .feature {
                            margin: 15px 0;
                            padding: 10px;
                            background: rgba(255,255,255,0.1);
                            border-radius: 8px;
                        }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h1>🌐 AI网站导航</h1>
                        <p>欢迎使用内置浏览器！从左侧选择一个AI网站开始浏览。</p>
                        <div class='features'>
                            <div class='feature'>📱 现代化浏览体验</div>
                            <div class='feature'>🔍 智能网站管理</div>
                            <div class='feature'>⚡ 快速访问收藏</div>
                            <div class='feature'>🛡️ 安全浏览环境</div>
                        </div>
                        <p>选择左侧的网站卡片即可开始浏览！</p>
                    </div>
                </body>
                </html>";

                webView.CoreWebView2.NavigateToString(defaultHtml);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载默认页面失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新导航按钮状态
        /// </summary>
        private void UpdateNavigationButtons()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateNavigationButtons));
                return;
            }

            btnBack.Enabled = CanGoBack;
            btnForward.Enabled = CanGoForward;
            btnRefresh.Enabled = !_isNavigating;
            btnStop.Enabled = _isNavigating;
        }

        /// <summary>
        /// 后退按钮点击事件
        /// </summary>
        private void btnBack_Click(object sender, EventArgs e)
        {
            GoBack();
        }

        /// <summary>
        /// 前进按钮点击事件
        /// </summary>
        private void btnForward_Click(object sender, EventArgs e)
        {
            GoForward();
        }

        /// <summary>
        /// 刷新按钮点击事件
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// 停止按钮点击事件
        /// </summary>
        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        /// <summary>
        /// 地址栏回车事件
        /// </summary>
        private void txtUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NavigateToUrl(txtUrl.Text);
            }
        }

        /// <summary>
        /// 转到按钮点击事件
        /// </summary>
        private void btnGo_Click(object sender, EventArgs e)
        {
            NavigateToUrl(txtUrl.Text);
        }
    }
}
