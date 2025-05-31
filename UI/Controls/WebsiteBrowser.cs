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
                        * {
                            margin: 0;
                            padding: 0;
                            box-sizing: border-box;
                        }

                        body {
                            font-family: 'Microsoft YaHei UI', 'Microsoft YaHei', sans-serif;
                            background-color: #2d2d2d;
                            color: #f5f5f5;
                            min-height: 100vh;
                            display: flex;
                            align-items: center;
                            justify-content: center;
                            padding: 20px;
                        }

                        .welcome-container {
                            max-width: 480px;
                            width: 100%;
                            text-align: center;
                            padding: 24px 20px;
                        }

                        .welcome-header {
                            margin-bottom: 24px;
                        }

                        .welcome-icon {
                            font-size: 2.4em;
                            margin-bottom: 12px;
                            display: block;
                        }

                        .welcome-title {
                            font-size: 1.5em;
                            font-weight: 600;
                            color: #f5f5f5;
                            margin-bottom: 8px;
                        }

                        .welcome-subtitle {
                            font-size: 1em;
                            color: #b0b0b0;
                            line-height: 1.4;
                        }

                        .features-grid {
                            display: grid;
                            grid-template-columns: 1fr 1fr;
                            gap: 12px;
                            margin: 20px 0;
                        }

                        .feature-item {
                            padding: 12px 8px;
                            text-align: center;
                        }

                        .feature-icon {
                            font-size: 1.4em;
                            margin-bottom: 4px;
                            display: block;
                        }

                        .feature-text {
                            font-size: 0.9em;
                            color: #b0b0b0;
                            font-weight: 400;
                        }

                        .welcome-footer {
                            margin-top: 20px;
                            padding-top: 16px;
                            border-top: 1px solid #4a4a4a;
                        }

                        .footer-text {
                            font-size: 0.95em;
                            color: #b0b0b0;
                            line-height: 1.3;
                        }

                        .highlight {
                            color: #f5f5f5;
                            font-weight: 600;
                        }

                        @media (max-width: 480px) {
                            .welcome-container {
                                padding: 20px 16px;
                            }

                            .features-grid {
                                grid-template-columns: 1fr;
                                gap: 8px;
                            }

                            .welcome-icon {
                                font-size: 2em;
                            }

                            .welcome-title {
                                font-size: 1.3em;
                            }
                        }
                    </style>
                </head>
                <body>
                    <div class='welcome-container'>
                        <div class='welcome-header'>
                            <span class='welcome-icon'>🌐</span>
                            <h1 class='welcome-title'>AI网站导航</h1>
                            <p class='welcome-subtitle'>集成化AI工具浏览平台，一站式访问主流AI服务</p>
                        </div>

                        <div class='features-grid'>
                            <div class='feature-item'>
                                <span class='feature-icon'>🚀</span>
                                <div class='feature-text'>快速访问</div>
                            </div>
                            <div class='feature-item'>
                                <span class='feature-icon'>🔐</span>
                                <div class='feature-text'>安全管理</div>
                            </div>
                            <div class='feature-item'>
                                <span class='feature-icon'>📊</span>
                                <div class='feature-text'>智能收藏</div>
                            </div>
                            <div class='feature-item'>
                                <span class='feature-icon'>⚡</span>
                                <div class='feature-text'>高效浏览</div>
                            </div>
                        </div>

                        <div class='welcome-footer'>
                            <p class='footer-text'>
                                从左侧选择 <span class='highlight'>AI网站卡片</span> 开始您的智能浏览之旅
                            </p>
                        </div>
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
