using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using llm_agent.BLL;
using llm_agent.Model;
using llm_agent.API.Provider;
using llm_agent.Common.Exceptions;
using llm_agent.Properties;
using llm_agent.DAL;
using llm_agent.UI.Controls;
using llm_agent.Common.Utils;
using llm_agent.UI.Controls.ChatForm; // 添加对ChatForm命名空间的引用

namespace llm_agent.UI.Forms
{
    public partial class LlmAgentMainForm : Form
    {
        // 静态集合，用于跟踪所有正在被聊天窗口使用的渠道ID
        private static HashSet<Guid> _activeChannels = new HashSet<Guid>();

        // 添加渠道到活跃列表
        public static void AddActiveChannel(Guid channelId)
        {
            if (channelId != Guid.Empty)
            {
                _activeChannels.Add(channelId);
            }
        }

        // 从活跃列表移除渠道
        public static void RemoveActiveChannel(Guid channelId)
        {
            _activeChannels.Remove(channelId);
        }

        // 检查渠道是否正在被使用
        public static bool IsChannelActive(Guid channelId)
        {
            return _activeChannels.Contains(channelId);
        }

        // 获取活跃渠道数量
        public static int GetActiveChannelCount()
        {
            return _activeChannels.Count;
        }

        private HttpClient _httpClient = null!;
        private ProviderFactory _providerFactory = null!;
        private ChatHistoryManager _chatHistoryManager = null!;
        private ChannelManager _channelManager = null!;
        private ChannelService _channelService = null!;
        private ProviderType _currentProviderType = ProviderType.OpenAI;
        private string _currentModelId = string.Empty;
        private Guid _currentChannelId = Guid.Empty; // 当前使用的渠道ID
        private bool _isProcessingMessage = false;
        private bool _useStreamResponse = true;  // 默认启用流式响应
        private string _systemPrompt = "";      // 系统提示内容
        private bool _isUpdatingChannelDetails = false; // 用于防止界面更新时触发事件处理
        private bool _enableMarkdown = false; // 用于Markdown支持设置
        private PromptManager _promptManager = null!; // 提示词管理器
        private PromptCardItem _selectedPromptCard = null!; // 当前选中的提示词卡片
        private WebsiteManager _websiteManager = null!; // 网站管理器
        private WebsiteCardItem _selectedWebsiteCard = null!; // 当前选中的网站卡片
        private WebsiteBrowser _websiteBrowser = null!; // 内置浏览器控件

        protected llm_agent.UI.Controls.HiddenScrollBarFlowLayoutPanel chatListPanel;
        protected TextBox searchBox;
        private Panel searchPanel;
        private Chatbox chatboxControl; // 新集成的现代化聊天控件

        public LlmAgentMainForm()
        {
            InitializeComponent();
            InitializeHttpClient();
            InitializeProviderFactory();
            InitializeChatHistoryManager();
            InitializeChannelManager();
            InitializeChannelService();
            InitializePromptManager();
            InitializeWebsiteManager();
            LoadSettings();

            // 设置KeyPreview为true，使窗体可以在控件之前处理键盘事件
            this.KeyPreview = true;
            // 添加KeyDown事件处理
            this.KeyDown += LlmAgentMainForm_KeyDown;
            // 添加窗体关闭事件处理
            this.FormClosing += LlmAgentMainForm_FormClosing;

            SetupEvents();
            SetupUI();
        }

        private void InitializeHttpClient()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(300);
        }

        private void InitializeProviderFactory()
        {
            _providerFactory = new ProviderFactory(_httpClient);
        }

        private void InitializeChatHistoryManager()
        {
            _chatHistoryManager = new ChatHistoryManager();
            // 不再自动创建新会话
        }

        private void InitializeChannelManager()
        {
            _channelManager = new ChannelManager();
        }

        private void InitializeChannelService()
        {
            _channelService = new ChannelService(_httpClient);
        }

        /// <summary>
        /// 初始化提示词管理器
        /// </summary>
        private void InitializePromptManager()
        {
            _promptManager = new PromptManager();
        }

        /// <summary>
        /// 初始化网站管理器
        /// </summary>
        private void InitializeWebsiteManager()
        {
            _websiteManager = new WebsiteManager();
        }

        private void LoadSettings()
        {
            try
            {
                _systemPrompt = Properties.Settings.Default.SystemPrompt;

                // 加载流式响应设置
                _useStreamResponse = Properties.Settings.Default.EnableStreamResponse;

                // 加载Markdown支持设置
                _enableMarkdown = Properties.Settings.Default.EnableMarkdown;

                // 加载上次使用的模型
                if (Enum.TryParse<ProviderType>(Properties.Settings.Default.ProviderType, out var providerType))
                {
                    _currentProviderType = providerType;
                }
                else if (Enum.TryParse<ProviderType>(Properties.Settings.Default.LastSelectedProvider, out providerType))
                {
                    _currentProviderType = providerType;
                }

                Properties.Settings.Default.ProviderType = providerType.ToString();
                Properties.Settings.Default.Save();

                // 加载上次使用的模型
                _currentModelId = Properties.Settings.Default.LastSelectedModel;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"加载设置时出错: {ex.Message}");
            }
        }

        private void SetupEvents()
        {
            // 添加缺失的变量声明
            TextBox txtSystemPrompt = settingsContentContainer.Controls.Find("txtSystemPrompt", true).FirstOrDefault() as TextBox;

            // Markdown支持复选框事件处理
            chkEnableMarkdown.CheckedChanged += (s, e) =>
            {
                _enableMarkdown = chkEnableMarkdown.Checked;
                Properties.Settings.Default.EnableMarkdown = _enableMarkdown;
                Properties.Settings.Default.Save();
            };

            // 系统提示输入框事件处理
            if (txtSystemPrompt != null)
            {
                txtSystemPrompt.TextChanged += (s, e) =>
                {
                    _systemPrompt = txtSystemPrompt.Text;
                    Properties.Settings.Default.SystemPrompt = _systemPrompt;
                    Properties.Settings.Default.Save();
                };
            }

            // 导航按钮事件
            avatarButton.Click += (s, e) => SwitchToPanel(userProfilePanel, avatarButton);
            chatNavButton.Click += (s, e) => SwitchToPanel(chatPagePanel, chatNavButton);
            websiteNavButton.Click += (s, e) => SwitchToPanel(aiWebsitePanel, websiteNavButton);
            promptsNavButton.Click += (s, e) => SwitchToPanel(promptsPanel, promptsNavButton);
            filesNavButton.Click += (s, e) => SwitchToPanel(filesPanel, filesNavButton);
            settingsNavButton.Click += (s, e) => SwitchToPanel(settingsPanel, settingsNavButton);
            channelNavButton.Click += channelNavButton_Click;

            // 设置页面事件
            ComboBox cboProvider = settingsContentContainer.Controls.Find("cboProvider", true).FirstOrDefault() as ComboBox;
            Button btnUpdateApiKey = settingsContentContainer.Controls.Find("btnUpdateApiKey", true).FirstOrDefault() as Button;

            // 设置菜单按钮事件
            shortcutSettingsButton.Click += (s, e) => SwitchSettingsPage(shortcutSettingsContainer);
            generalSettingsButton.Click += (s, e) => SwitchSettingsPage(generalSettingsContainer);
            dataSettingsButton.Click += (s, e) => SwitchSettingsPage(dataSettingsContainer);
            aboutSettingsButton.Click += (s, e) => SwitchSettingsPage(aboutContainer);

            // 模型管理按钮事件
            Button manageModelsButton = settingsContentContainer.Controls.Find("manageModelsButton", true).FirstOrDefault() as Button;
            Button testModelButton = settingsContentContainer.Controls.Find("testModelButton", true).FirstOrDefault() as Button;

            if (manageModelsButton != null)
                manageModelsButton.Click += ManageModelsButton_Click;

            if (testModelButton != null)
                testModelButton.Click += TestModelButton_Click;

            if (cboProvider != null)
                cboProvider.SelectedIndexChanged += ProviderChanged;

            if (btnUpdateApiKey != null)
                btnUpdateApiKey.Click += UpdateApiSettings;

            // 系统提示文本框
            if (txtSystemPrompt != null)
            {
                txtSystemPrompt.Leave += (s, e) =>
                {
                    _systemPrompt = txtSystemPrompt.Text.Trim();
                    Properties.Settings.Default.SystemPrompt = _systemPrompt;
                    Properties.Settings.Default.Save();
                };
            }

            // 设置数据页面"清除所有聊天记录"按钮事件
            clearChatHistoryButton.Click += ClearChatHistoryButton_Click;
        }

        private void InitializeChatPageModelSelector()
        {
            // 使用新的方法更新模型列表
            if (chatboxControl != null)
            {
                UpdateChatboxModelList();
            }
        }

        // 切换到指定面板并高亮对应按钮
        private void SwitchToPanel(Control targetPanel, Button activeButton)
        {
            // 隐藏所有内容面板
            foreach (Control control in mainSplitContainer.Panel2.Controls)
            {
                control.Visible = false;
            }

            // 显示目标面板
            targetPanel.Visible = true;

            // 重置所有导航按钮样式
            foreach (Control control in navPanel.Controls)
            {
                if (control is Button button)
                {
                    button.BackColor = Color.Transparent;
                }
            }

            // 高亮当前活动按钮
            activeButton.BackColor = Color.FromArgb(240, 240, 240);

            // 执行特定面板的初始化操作
            InitializePanel(targetPanel);
        }

        // 执行特定面板的初始化操作
        private void InitializePanel(Control targetPanel)
        {
            // 根据目标面板类型执行相应的操作
            if (targetPanel == chatPageSplitContainer || targetPanel == chatPagePanel)
            {
                // 初始化聊天选择器（左侧列表）
                InitializeChatTopics();

                // 初始化聊天模型选择器
                InitializeChatPageModelSelector();

                DisplayChatInterface();
            }
            else if (targetPanel == channelPanel)
            {
                InitializeChannelList();
            }
            else if (targetPanel == settingsPanel)
            {
                // 初始化设置面板状态
                if (generalSettingsGroup != null)
                {
                    // 初始化Markdown支持复选框状态
                    if (chkEnableMarkdown != null)
                    {
                        chkEnableMarkdown.Checked = _enableMarkdown;
                    }

                    // 其他设置页面初始化...
                }

                // 默认选中通用设置按钮
                SwitchSettingsPage(generalSettingsContainer);
                generalSettingsButton.BackColor = Color.FromArgb(230, 230, 230);
            }
            else if (targetPanel == promptsPanel)
            {
                // 初始化提示词库面板
                InitializePromptsPanel();
            }
            else if (targetPanel == filesPanel)
            {
                // 初始化文件管理面板
                // 待实现
            }
            else if (targetPanel == aiWebsitePanel)
            {
                // 初始化AI网站面板
                InitializeAiWebsitePanel();
            }
            else if (targetPanel == userProfilePanel)
            {
                // 初始化用户资料面板
                // 待实现
            }
        }

        private void InitializePromptsPanel()
        {
            // 初始化提示词列表
            InitializePromptsList();

            // 初始化搜索框
            InitializePromptSearchBox();

            // 初始化新建按钮
            InitializeNewPromptButton();

            // 添加大小改变事件处理，与ChatListPanel保持一致
            promptsListPanel.SizeChanged += PromptsListPanel_SizeChanged;
        }

        private void InitializePromptsList()
        {
            try
            {
                // 获取所有提示词
                var prompts = _promptManager.GetAllPrompts();

                // 清空现有的提示词卡片
                promptsListPanel.Controls.Clear();

                // 重置选中状态
                _selectedPromptCard = null;

                // 添加各个提示词的卡片
                foreach (var prompt in prompts)
                {
                    var promptCard = new PromptCardItem
                    {
                        Prompt = prompt,
                        Margin = new Padding(0, 1, 0, 1)
                    };

                    // 设置宽度，与ChatSessionItem保持一致的计算方式
                    promptCard.Width = promptsListPanel.ClientSize.Width - promptCard.Margin.Horizontal;

                    // 添加DPI缩放的高度计算，与ChatSessionItem保持一致
                    float dpiScaleFactor = promptCard.CreateGraphics().DpiX / 96f;
                    int scaledHeight = (int)(85 * dpiScaleFactor);
                    promptCard.Height = scaledHeight;

                    // 添加点击事件
                    promptCard.PromptClicked += (s, e) =>
                    {
                        // 取消之前选中卡片的高亮状态
                        if (_selectedPromptCard != null)
                        {
                            _selectedPromptCard.IsSelected = false;
                        }

                        // 设置当前卡片为选中状态
                        _selectedPromptCard = promptCard;
                        promptCard.IsSelected = true;

                        DisplayPromptDetail(e.Prompt);
                    };

                    // 添加使用提示词事件
                    promptCard.UsePromptClicked += (s, e) =>
                    {
                        UsePrompt(e.Prompt);
                    };

                    promptsListPanel.Controls.Add(promptCard);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"初始化提示词列表时出错: {ex.Message}");
                MessageBox.Show($"加载提示词列表时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializePromptSearchBox()
        {
            // 先解除旧的事件绑定，防止重复绑定
            promptSearchBox.TextChanged -= PromptSearchBox_TextChanged;
            // 设置搜索框事件处理
            promptSearchBox.TextChanged += PromptSearchBox_TextChanged;
        }

        private void InitializeNewPromptButton()
        {
            // 先解除旧的事件绑定，防止重复绑定
            newPromptButton.Click -= NewPromptButton_Click;
            // 设置新建按钮事件处理
            newPromptButton.Click += NewPromptButton_Click;
        }

        private void PromptSearchBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // 获取搜索框文本
                string searchText = promptSearchBox.Text.Trim();

                List<Prompt> prompts;

                // 根据搜索文本获取提示词
                if (string.IsNullOrEmpty(searchText))
                {
                    prompts = _promptManager.GetAllPrompts();
                }
                else
                {
                    prompts = _promptManager.SearchPrompts(searchText);
                }

                // 清空现有的提示词卡片
                promptsListPanel.Controls.Clear();

                // 重置选中状态
                _selectedPromptCard = null;

                // 添加匹配搜索文本的提示词卡片
                foreach (var prompt in prompts)
                {
                    var promptCard = new PromptCardItem
                    {
                        Prompt = prompt,
                        Margin = new Padding(0, 1, 0, 1)
                    };

                    // 设置宽度，与ChatSessionItem保持一致的计算方式
                    promptCard.Width = promptsListPanel.ClientSize.Width - promptCard.Margin.Horizontal;

                    // 添加DPI缩放的高度计算，与ChatSessionItem保持一致
                    float dpiScaleFactor = promptCard.CreateGraphics().DpiX / 96f;
                    int scaledHeight = (int)(85 * dpiScaleFactor);
                    promptCard.Height = scaledHeight;

                    // 添加点击事件
                    promptCard.PromptClicked += (s, args) =>
                    {
                        // 取消之前选中卡片的高亮状态
                        if (_selectedPromptCard != null)
                        {
                            _selectedPromptCard.IsSelected = false;
                        }

                        // 设置当前卡片为选中状态
                        _selectedPromptCard = promptCard;
                        promptCard.IsSelected = true;

                        DisplayPromptDetail(args.Prompt);
                    };

                    // 添加使用提示词事件
                    promptCard.UsePromptClicked += (s, args) =>
                    {
                        UsePrompt(args.Prompt);
                    };

                    promptsListPanel.Controls.Add(promptCard);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"搜索提示词时出错: {ex.Message}");
            }
        }

        private void NewPromptButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 创建新的提示词
                var newPrompt = new Prompt
                {
                    Title = "新提示词",
                    Content = "请在此处输入提示词内容",
                    Category = "默认分类"
                };

                // 添加到提示词管理器
                _promptManager.CreatePrompt(newPrompt.Title, newPrompt.Content, newPrompt.Category);

                // 刷新提示词列表
                InitializePromptsList();

                MessageBox.Show("已创建新提示词，请在右侧编辑其内容。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"创建新提示词时出错: {ex.Message}");
                MessageBox.Show($"创建新提示词时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayPromptDetail(Prompt prompt)
        {
            try
            {
                if (prompt == null) return;

                // 清空现有内容
                promptsContentPanel.Controls.Clear();

                // 创建标题文本框
                var titleLabel = new Label
                {
                    Text = "标题:",
                    AutoSize = true,
                    Location = new Point(10, 10)
                };

                var titleTextBox = new TextBox
                {
                    Text = prompt.Title,
                    Width = promptsContentPanel.Width - 40,
                    Location = new Point(10, 40),
                    Font = new Font("Microsoft YaHei UI", 10F)
                };

                // 创建分类文本框
                var categoryLabel = new Label
                {
                    Text = "分类:",
                    AutoSize = true,
                    Location = new Point(10, 80)
                };

                var categoryTextBox = new TextBox
                {
                    Text = prompt.Category,
                    Width = promptsContentPanel.Width - 40,
                    Location = new Point(10, 110),
                    Font = new Font("Microsoft YaHei UI", 10F)
                };

                // 创建内容文本框
                var contentLabel = new Label
                {
                    Text = "内容:",
                    AutoSize = true,
                    Location = new Point(10, 150)
                };

                var contentTextBox = new TextBox
                {
                    Text = prompt.Content,
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Width = promptsContentPanel.Width - 40,
                    Height = 300,
                    Location = new Point(10, 180),
                    Font = new Font("Microsoft YaHei UI", 9.75F)
                };

                // 创建保存按钮
                var saveButton = new Button
                {
                    Text = "保存",
                    Location = new Point(10, 500),
                    Width = 100,
                    BackColor = Color.LightSlateGray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                saveButton.FlatAppearance.BorderSize = 0;

                // 创建使用按钮
                var useButton = new Button
                {
                    Text = "使用",
                    Location = new Point(120, 500),
                    Width = 100,
                    BackColor = Color.CornflowerBlue,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                useButton.FlatAppearance.BorderSize = 0;

                // 创建删除按钮
                var deleteButton = new Button
                {
                    Text = "删除",
                    Location = new Point(230, 500),
                    Width = 100,
                    BackColor = Color.IndianRed,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                deleteButton.FlatAppearance.BorderSize = 0;

                // 添加保存按钮点击事件
                saveButton.Click += (s, e) =>
                {
                    try
                    {
                        // 更新提示词数据
                        _promptManager.UpdatePrompt(
                            prompt.Id,
                            titleTextBox.Text,
                            contentTextBox.Text,
                            categoryTextBox.Text
                        );

                        // 刷新列表
                        InitializePromptsList();

                        MessageBox.Show("提示词已保存。", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"保存提示词时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                // 添加使用按钮点击事件
                useButton.Click += (s, e) =>
                {
                    UsePrompt(prompt);
                };

                // 添加删除按钮点击事件
                deleteButton.Click += (s, e) =>
                {
                    try
                    {
                        // 确认删除
                        var result = MessageBox.Show(
                            $"确定要删除提示词 \"{prompt.Title}\" 吗？此操作不可恢复。",
                            "确认删除",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        );

                        if (result == DialogResult.Yes)
                        {
                            // 删除提示词
                            _promptManager.DeletePrompt(prompt.Id);

                            // 刷新列表
                            InitializePromptsList();

                            // 清空详情面板
                            promptsContentPanel.Controls.Clear();

                            MessageBox.Show("提示词已删除。", "删除成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"删除提示词时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                // 将控件添加到面板
                promptsContentPanel.Controls.Add(titleLabel);
                promptsContentPanel.Controls.Add(titleTextBox);
                promptsContentPanel.Controls.Add(categoryLabel);
                promptsContentPanel.Controls.Add(categoryTextBox);
                promptsContentPanel.Controls.Add(contentLabel);
                promptsContentPanel.Controls.Add(contentTextBox);
                promptsContentPanel.Controls.Add(saveButton);
                promptsContentPanel.Controls.Add(useButton);
                promptsContentPanel.Controls.Add(deleteButton);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"显示提示词详情时出错: {ex.Message}");
                MessageBox.Show($"显示提示词详情时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UsePrompt(Prompt prompt)
        {
            try
            {
                if (prompt == null) return;

                // 增加使用次数
                _promptManager.UsePrompt(prompt.Id);

                // 切换到聊天面板
                SwitchToPanel(chatPagePanel, chatNavButton);

                // 将提示词内容填入聊天输入框
                if (chatboxControl != null)
                {
                    chatboxControl.SetInputText(prompt.Content);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"使用提示词时出错: {ex.Message}");
                MessageBox.Show($"使用提示词时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupUI()
        {
            // 添加自定义字体支持
            var fontCollection = new System.Drawing.Text.PrivateFontCollection();
            var fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts", "remixicon.ttf");
            try
            {
                if (File.Exists(fontPath))
                {
                    fontCollection.AddFontFile(fontPath);
                    if (fontCollection.Families.Length > 0)
                    {
                        Font remixIconFont = new Font(fontCollection.Families[0], 24);

                        // 为导航按钮添加图标
                        AddIconToNavButton(avatarButton, "\uef7c", remixIconFont);
                        AddIconToNavButton(chatNavButton, "\uec2e", remixIconFont);
                        AddIconToNavButton(promptsNavButton, "\ueda4", remixIconFont);
                        AddIconToNavButton(websiteNavButton, "\ueb7c", remixIconFont);
                        AddIconToNavButton(filesNavButton, "\ueccb", remixIconFont);
                        AddIconToNavButton(channelNavButton, "\ueb5c", remixIconFont);
                        AddIconToNavButton(settingsNavButton, "\uee4a", remixIconFont);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"加载字体时出错: {ex.Message}");
            }

            // 已移除以下代码，不再配置chatModelComboBox
            // // 配置聊天模型下拉框
            // chatModelComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            // chatModelComboBox.Width = 180;
            // chatModelComboBox.Location = new Point(10, 10);

            // 创建并初始化头像图片
            var avatarImage = new PictureBox
            {
                Width = 36,
                Height = 36,
                BackColor = Color.LightGray,
                // 已移除此引用，使用纯色背景替代
                // Image = Properties.Resources.defaultAvatar,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            try
            {
                // 设置窗体标题
                UpdateTitle();

                // 加载模型列表
                UpdateModelList();

                // 填充API密钥和主机
                txtApiKey.Text = GetApiKey();
                txtApiHost.Text = GetApiHost();

                // 添加图标到导航按钮
                AddIconToNavButton(chatNavButton, "💬");
                AddIconToNavButton(websiteNavButton, "🌐");
                AddIconToNavButton(promptsNavButton, "📝");
                AddIconToNavButton(filesNavButton, "📁");
                AddIconToNavButton(settingsNavButton, "⚙️");
                AddIconToNavButton(avatarButton, "👤");
                AddIconToNavButton(channelNavButton, "🔌");

                // 更新导航按钮工具提示
                toolTip1.SetToolTip(avatarButton, "用户");
                toolTip1.SetToolTip(chatNavButton, "聊天");
                toolTip1.SetToolTip(websiteNavButton, "AI网站");
                toolTip1.SetToolTip(promptsNavButton, "提示词库");
                toolTip1.SetToolTip(filesNavButton, "文件");
                toolTip1.SetToolTip(settingsNavButton, "设置");
                toolTip1.SetToolTip(channelNavButton, "渠道管理");

                // 注册渠道页面中模型管理和测试按钮的事件
                manageChannelModelsButton.Click += manageChannelModelsButton_Click;
                testChannelModelButton.Click += testChannelModelButton_Click;

                // 默认显示聊天页面
                SwitchToPanel(chatPagePanel, chatNavButton);

                // 初始化聊天列表面板
                InitializeChatListPanel();

                // 更新API主机地址标签提示文本
                Label lblApiHost = settingsContentContainer.Controls.Find("lblApiHost", true).FirstOrDefault() as Label;
                if (lblApiHost != null)
                {
                    lblApiHost.Text = "API主机地址（已预设，可修改）：";
                }

                // 创建第一个默认会话
                if (_chatHistoryManager.GetAllSessions().Count == 0)
                {
                    CreateNewChat();
                }
                else
                {
                    // 加载第一个会话
                    var sessions = _chatHistoryManager.GetAllSessions();
                    if (sessions.Count > 0)
                    {
                        SwitchToChat(sessions[0]);
                    }
                }

                // 显示欢迎消息
                string welcomeMessage = $"欢迎使用LLM Agent！\n当前使用的模型：{GetCurrentModelName()}";
                // 使用 chatboxControl 显示欢迎消息
                if (chatboxControl == null)
                {
                    InitializeChatbox();
                }
                var systemMessage = new ChatMessage
                {
                    Role = ChatRole.System,
                    Content = welcomeMessage,
                    Timestamp = DateTime.Now
                };
                chatboxControl.AddMessage(ChatModelAdapter.ToTextChatModel(systemMessage));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"设置UI时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 为导航按钮添加图标（修改方法签名，接受Font参数）
        private void AddIconToNavButton(Button button, string iconText, Font? iconFont = null)
        {
            // 使用传入的字体或默认系统字体
            Font font = iconFont ?? new Font("Segoe UI Symbol", 22, FontStyle.Regular);

            // 创建图标标签
            Label iconLabel = new Label
            {
                Text = iconText,
                Font = font,
                Size = new Size(button.Width, button.Height), // 修改为与按钮大小一致
                Location = new Point(0, 0), // 修改为从按钮左上角开始
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(80, 80, 80),
                Enabled = false // 禁用Label以允许事件传递给按钮
            };

            button.Controls.Add(iconLabel);

            // 添加悬停效果
            button.MouseEnter += (s, e) =>
            {
                if (button.BackColor != Color.FromArgb(240, 240, 240)) // 如果不是当前活动按钮
                {
                    button.BackColor = Color.FromArgb(245, 245, 245);
                }
                iconLabel.ForeColor = Color.FromArgb(0, 120, 212);
            };

            button.MouseLeave += (s, e) =>
            {
                if (button.BackColor != Color.FromArgb(240, 240, 240)) // 如果不是当前活动按钮
                {
                    button.BackColor = Color.Transparent;
                }
                iconLabel.ForeColor = Color.FromArgb(80, 80, 80);
            };
        }

        private void InitializeChatTopics()
        {
            try
            {
                var chatTopicPanel = chatListPanel.Controls["chatTopicPanel"] as FlowLayoutPanel;
                if (chatTopicPanel == null)
                    return;

                // 清空现有的对话主题
                chatTopicPanel.Controls.Clear();

                // 获取所有会话
                var sessions = _chatHistoryManager.GetAllSessions();

                // 如果没有会话，只显示新建对话按钮即可，不需要额外处理
                if (sessions.Count == 0)
                    return;

                // 添加各个会话的按钮
                foreach (var session in sessions)
                {
                    var sessionTitle = string.IsNullOrEmpty(session.Title) ? "新建对话" : session.Title;

                    Button sessionButton = new Button();
                    sessionButton.Text = sessionTitle;
                    sessionButton.FlatStyle = FlatStyle.Flat;
                    sessionButton.Margin = new Padding(0, 1, 0, 1);
                    sessionButton.Height = 40;
                    sessionButton.Width = chatTopicPanel.Width - SystemInformation.VerticalScrollBarWidth - 5;
                    sessionButton.TextAlign = ContentAlignment.MiddleLeft;
                    sessionButton.Padding = new Padding(10, 0, 0, 0);
                    sessionButton.UseVisualStyleBackColor = true;
                    sessionButton.Tag = session;

                    // 突出显示当前会话
                    var currentSession = _chatHistoryManager.GetCurrentSession();
                    if (currentSession != null && session.Id == currentSession.Id)
                    {
                        sessionButton.BackColor = Color.FromArgb(230, 230, 230);
                        sessionButton.Font = new Font(sessionButton.Font, FontStyle.Bold);
                    }

                    // 添加点击事件
                    sessionButton.Click += (s, e) =>
                    {
                        SwitchToChat(session);
                    };

                    chatTopicPanel.Controls.Add(sessionButton);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化聊天主题时出错: {ex.Message}");
            }
        }

        private void SwitchToChat(ChatSession session)
        {
            if (session == null)
                return;

            // 将当前会话设为所选会话
            var loadedSession = _chatHistoryManager.GetOrCreateSession(session.Id);
            if (loadedSession == null)
                return;

            // 刷新会话列表
            UpdateChatList();

            // 显示对话内容
            DisplayChatInterface();
        }

        private void DisplayChatInterface()
        {
            // 初始化Chatbox控件
            InitializeChatbox();

            // 获取当前会话
            var currentSession = _chatHistoryManager.GetCurrentSession();
            if (currentSession == null)
            {
                // 显示空界面提示
                InitializeChatboxForEmptySession(chatboxControl);
                return;
            }

            if (currentSession.Messages.Count == 0)
                return;

            // 使用Chatbox显示当前会话的所有消息
            RefreshChatMessages(chatboxControl, currentSession.Messages);

            // 将焦点设置到Chatbox的输入框
            var chatTextbox = chatboxControl.Controls.Find("chatTextbox", true).FirstOrDefault() as TextBox;
            if (chatTextbox != null)
                chatTextbox.Focus();
        }

        private void CreateNewChat()
        {
            // 创建新的聊天会话
            var session = _chatHistoryManager.CreateNewSession();
            if (session == null)
                return;

            // 初始化Chatbox控件
            InitializeChatbox();

            // 添加系统欢迎消息
            string welcomeMessage = "欢迎使用AI助手，我可以帮助您回答问题、提供信息或与您聊天。请告诉我您需要什么帮助？";
            ChatMessage systemMessage = new ChatMessage
            {
                Role = ChatRole.Assistant,
                Content = welcomeMessage,
                Timestamp = DateTime.Now
            };

            // 添加消息到会话并保存
            _chatHistoryManager.AddMessageToSession(session, systemMessage);

            // 使用Chatbox显示消息
            chatboxControl.AddMessage(ChatModelAdapter.ToTextChatModel(systemMessage));

            // 重新初始化聊天列表
            UpdateChatList();

            // 确保切换到聊天界面
            SwitchToPanel(chatPagePanel, chatNavButton);

            // 设置Chatbox输入框焦点
            var chatTextbox = chatboxControl.Controls.Find("chatTextbox", true).FirstOrDefault() as TextBox;
            if (chatTextbox != null)
                chatTextbox.Focus();
        }

        private void ProviderChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox cboProvider && cboProvider.SelectedIndex >= 0)
            {
                string selectedProvider = cboProvider.SelectedItem.ToString();

                // 从显示名称获取提供商类型
                ProviderType selectedProviderType = _providerFactory.GetProviderTypeFromDisplayName(selectedProvider);

                // 更新当前提供商类型
                _currentProviderType = selectedProviderType;

                // 更新聊天页面的模型选择器
                InitializeChatPageModelSelector();

                // 更新设置页面的模型列表
                UpdateModelList();

                // 保存设置
                Properties.Settings.Default.ProviderType = selectedProviderType.ToString();
                Properties.Settings.Default.Save();

                // 更新窗体标题
                UpdateTitle();
            }
        }

        // 根据提供商类型获取默认API主机地址
        private string GetDefaultApiHost(ProviderType providerType)
        {
            return ProviderFactory.GetDefaultApiHost(providerType);
        }

        private void ModelChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox cboModel && cboModel.SelectedIndex >= 0)
            {
                string selectedModel = cboModel.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(selectedModel))
                {
                    _currentModelId = selectedModel;
                    Properties.Settings.Default.LastSelectedModel = _currentModelId;
                    Properties.Settings.Default.Save();
                    UpdateTitle();
                }
            }
        }

        private void UpdateApiSettings(object sender, EventArgs e)
        {
            // 获取当前选中的渠道
            var channel = GetSelectedChannel();
            if (channel == null)
            {
                MessageBox.Show("请选择一个渠道", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 使用渠道的API设置
            string apiKey = channel.ApiKey;
            string apiHost = channel.ApiHost;

            if (string.IsNullOrEmpty(apiKey))
            {
                MessageBox.Show("API密钥不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 更新提供商实例的API密钥和主机
            var provider = _providerFactory.GetProvider(channel.ProviderType);
            if (provider != null)
            {
                provider.UpdateApiKey(apiKey);
                provider.UpdateApiHost(apiHost);
                MessageBox.Show($"{channel.Name}的API设置已更新", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"无法获取{channel.Name}提供商", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateModelList()
        {
            try
            {
                // 此方法现在更新settingsPanel中的模型列表
                // 1. 更新设置面板中的模型列表
                UpdateSettingsModelList();

                // 2. 更新聊天面板中的模型选择下拉框
                UpdateChatProviderModels();

                // 3. 更新聊天页面的模型选择器
                InitializeChatPageModelSelector();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新模型列表时出错: {ex.Message}");
            }
        }

        private void UpdateSettingsModelList()
        {
            // 该方法不再需要，因为已删除modelListBox控件
        }

        private void UpdateTitle()
        {
            // 从选中的聊天模型获取信息
            string modelInfo = string.IsNullOrEmpty(_currentModelId) ? "未选择模型" : _currentModelId;

            // 如果当前模型是带提供商前缀的格式，则分离显示
            if (modelInfo.Contains(":"))
            {
                string[] parts = modelInfo.Split(new[] { ':' }, 2);
                string provider = parts[0].Trim();
                string model = parts[1].Trim();

                this.Text = $"LLM Agent - {provider} - {model}";
            }
            else
            {
                // 否则使用当前提供商和模型
                string provider = GetProviderDisplayName(_currentProviderType);
                this.Text = $"LLM Agent - {provider} - {modelInfo}";
            }
        }

        private string GetProviderDisplayName(ProviderType providerType)
        {
            return providerType switch
            {
                ProviderType.OpenAI => "OpenAI",
                ProviderType.AzureOpenAI => "Azure OpenAI",
                ProviderType.Anthropic => "Anthropic Claude",
                ProviderType.Google => "Google Gemini",
                ProviderType.ZhipuAI => "智谱 GLM",
                ProviderType.Other => "其他",
                _ => "未知提供商"
            };
        }

        private object? GetCurrentProvider()
        {
            try
            {
                var apiKey = GetProviderApiKey(_currentProviderType);
                if (string.IsNullOrEmpty(apiKey))
                    return null;

                return new { ApiKey = apiKey, ApiHost = GetProviderApiHost(_currentProviderType) };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取提供商失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private string GetProviderApiKey(ProviderType providerType)
        {
            switch (providerType)
            {
                case ProviderType.OpenAI:
                    return Properties.Settings.Default.OpenAIApiKey;
                case ProviderType.AzureOpenAI:
                    return Properties.Settings.Default.AzureApiKey;
                case ProviderType.Anthropic:
                    return Properties.Settings.Default.AnthropicApiKey;
                case ProviderType.Google:
                    return Properties.Settings.Default.GeminiApiKey;
                case ProviderType.ZhipuAI:
                    return Properties.Settings.Default.ZhipuApiKey;
                case ProviderType.Other:
                    return Properties.Settings.Default.OtherApiKey;
                default:
                    return string.Empty;
            }
        }

        private string GetProviderApiHost(ProviderType providerType)
        {
            switch (providerType)
            {
                case ProviderType.OpenAI:
                    return Properties.Settings.Default.OpenAIApiHost;
                case ProviderType.AzureOpenAI:
                    return Properties.Settings.Default.AzureApiHost;
                case ProviderType.Anthropic:
                    return Properties.Settings.Default.AnthropicApiHost;
                case ProviderType.Google:
                    return Properties.Settings.Default.GeminiApiHost;
                case ProviderType.ZhipuAI:
                    return Properties.Settings.Default.ZhipuApiHost;
                case ProviderType.Other:
                    return Properties.Settings.Default.OtherApiHost;
                default:
                    return string.Empty;
            }
        }

        private string GetApiKey()
        {
            var provider = GetCurrentProvider();
            return provider?.GetType().GetProperty("ApiKey")?.GetValue(provider) as string ?? string.Empty;
        }

        private string GetApiHost()
        {
            var provider = GetCurrentProvider();
            return provider?.GetType().GetProperty("ApiHost")?.GetValue(provider) as string ?? string.Empty;
        }

        // 获取当前模型显示名称
        private string GetCurrentModelName()
        {
            if (string.IsNullOrEmpty(_currentModelId))
            {
                return GetProviderDisplayName(_currentProviderType);
            }
            return _currentModelId;
        }

        // 辅助方法：截断文本
        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
        }

        // 发送消息并接收回复
        private async Task SendMessage()
        {
            // 防止重复处理或并发请求
            if (_isProcessingMessage)
                return;

            // 获取Chatbox的输入框控件
            var chatTextbox = chatboxControl.Controls.Find("chatTextbox", true).FirstOrDefault() as TextBox;
            if (chatTextbox == null)
                return;

            // 获取用户输入消息，必须和占位符文本不同
            string messageText = chatTextbox.Text.Trim();
            if (string.IsNullOrEmpty(messageText) || messageText == chatboxControl.chatbox_info.ChatPlaceholder)
                return;

            _isProcessingMessage = true;

            try
            {
                // 清空输入框（使用Chatbox的方法）
                chatboxControl.ClearInputText();

                // 确保活跃会话
                var session = _chatHistoryManager.GetCurrentSession();
                if (session == null)
                {
                    session = _chatHistoryManager.CreateNewSession();
                }

                // 创建用户消息
                var userMessage = new ChatMessage
                {
                    Role = ChatRole.User,
                    Content = messageText,
                    Timestamp = DateTime.Now
                };

                // 保存用户消息到会话
                _chatHistoryManager.AddMessageToSession(session, userMessage);

                // 使用Chatbox显示用户消息
                chatboxControl.AddMessage(ChatModelAdapter.ToTextChatModel(userMessage));

                // 创建一个占位的助手响应消息
                var waitingMessage = new ChatMessage
                {
                    Role = ChatRole.Assistant,
                    Content = "思考中...",
                    Timestamp = DateTime.Now
                };

                // 保存占位消息到会话
                _chatHistoryManager.AddMessageToSession(session, waitingMessage);

                // 使用Chatbox显示占位消息
                chatboxControl.AddMessage(ChatModelAdapter.ToTextChatModel(waitingMessage));

                // 准备发送给LLM的消息列表
                var messages = session.Messages
                    .Where(m => m.Role != ChatRole.System || m == session.Messages.FirstOrDefault())
                    .ToList();

                try
                {
                    // 开始API请求
                    string apiKey = GetApiKey();
                    string apiHost = GetApiHost();
                    string modelId = string.Empty;

                    // 从_currentModelId中提取真正的模型名称
                    // _currentModelId的格式为"渠道名: 模型名"
                    if (!string.IsNullOrEmpty(_currentModelId) && _currentModelId.Contains(":"))
                    {
                        string[] parts = _currentModelId.Split(new[] { ':' }, 2);
                        if (parts.Length == 2)
                        {
                            modelId = parts[1].Trim(); // 提取模型名称部分
                        }
                    }

                    // 获取提供商实例
                    var provider = _providerFactory.GetProvider(_currentProviderType);
                    if (provider == null)
                    {
                        throw new InvalidOperationException($"无法创建提供商实例: {_currentProviderType}");
                    }

                    // 处理特定渠道模型
                    if (_currentChannelId != Guid.Empty)
                    {
                        var channel = _channelManager.GetChannelById(_currentChannelId);
                        if (channel != null && channel.ProviderType == _currentProviderType)
                        {
                            // 使用渠道的配置
                            apiKey = channel.ApiKey;
                            apiHost = channel.ApiHost;

                            // 如果未能从_currentModelId提取模型名称，使用第一个可用模型作为备选
                            if (string.IsNullOrEmpty(modelId))
                            {
                                var channelModels = _channelService.GetChannelModels(channel);
                                if (channelModels.Count > 0)
                                {
                                    modelId = channelModels[0]; // 使用第一个可用模型
                                }
                            }
                        }
                    }

                    // 配置提供商
                    provider.UpdateApiKey(apiKey);
                    provider.UpdateApiHost(apiHost);

                    // 更新占位消息的ID（用于后续标识）
                    waitingMessage.Id = Guid.NewGuid().ToString();

                    // 如果是流式响应
                    if (_useStreamResponse)
                    {
                        // 初始化响应内容
                        StringBuilder responseContent = new StringBuilder();

                        // 处理流式响应
                        await foreach (var content in provider.StreamChatAsync(messages, modelId))
                        {
                            responseContent.Append(content);
                            waitingMessage.Content = responseContent.ToString();

                            // 更新UI上的响应内容
                            UpdateLastAssistantMessageContent(chatboxControl, responseContent.ToString());
                        }

                        // 更新会话中的消息内容
                        waitingMessage.UpdatedAt = DateTime.Now;
                        // 重新添加消息（因为没有UpdateMessage方法）
                        _chatHistoryManager.SaveSession(session);
                    }
                    else
                    {
                        // 发送非流式请求
                        string response = await provider.ChatAsync(messages, modelId);

                        // 获取响应结果
                        waitingMessage.Content = response;
                        waitingMessage.UpdatedAt = DateTime.Now;

                        // 更新UI显示和会话记录
                        UpdateLastAssistantMessageContent(chatboxControl, response);
                        // 保存更新后的会话
                        _chatHistoryManager.SaveSession(session);
                    }

                    // 更新会话标题（如果第一个消息）
                    if (session.Messages.Count <= 3)
                    {
                        // 使用用户消息的前20个字符作为会话标题
                        _chatHistoryManager.UpdateSessionTitle(session, TruncateText(userMessage.Content, 20));
                        UpdateChatList();
                    }
                }
                catch (Exception ex)
                {
                    // 处理API请求错误
                    string errorContent = $"请求出错：{ex.Message}";

                    // 使用错误消息替换"思考中..."
                    waitingMessage.Content = errorContent;
                    waitingMessage.UpdatedAt = DateTime.Now;

                    // 更新UI上的错误消息
                    UpdateLastAssistantMessageContent(chatboxControl, errorContent);

                    // 保存更新后的会话
                    _chatHistoryManager.SaveSession(session);
                }
            }
            finally
            {
                _isProcessingMessage = false;
            }
        }

        private void LlmAgentMainForm_Load(object sender, EventArgs e)
        {
            // 初始化HTTP客户端
            InitializeHttpClient();

            // 初始化提供商工厂
            InitializeProviderFactory();

            // 初始化聊天历史管理器
            InitializeChatHistoryManager();

            // 初始化渠道管理器
            InitializeChannelManager();

            // 初始化渠道服务
            InitializeChannelService();

            // 初始化提示词管理器
            InitializePromptManager();

            // 配置界面元素
            SetupUI();

            // 设置事件处理
            SetupEvents();

            // 加载设置
            LoadSettings();

            // 启用Markdown支持选项
            chkEnableMarkdown.Checked = _enableMarkdown;

            // 已删除对streamCheckBox的初始化
            // streamCheckBox.Checked = _useStreamResponse;

            // 默认切换到聊天页面
            SwitchToPanel(chatPagePanel, chatNavButton);

            // 更新界面标题
            UpdateTitle();

            // 初始化聊天会话列表（左侧聊天历史）
            InitializeChatTopics();

            // 初始化聊天界面
            DisplayChatInterface();

            // 绑定表单快捷键
            this.KeyDown += LlmAgentMainForm_KeyDown;
            this.FormClosing += LlmAgentMainForm_FormClosing;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void lblSystemPrompt_Click(object sender, EventArgs e)
        {
            // 此方法不再需要，但保留空实现以避免错误
        }

        // 全局键盘快捷键处理
        private void LlmAgentMainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // 发送消息 - Ctrl+Enter
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                // 查找Chatbox的输入框控件
                var chatTextbox = chatboxControl?.Controls.Find("chatTextbox", true)?.FirstOrDefault() as TextBox;
                if (chatTextbox != null && chatTextbox.Focused)
                {
                    e.SuppressKeyPress = true;  // 阻止默认回车换行行为

                    // 触发发送消息
                    _ = SendMessage();
                }
            }

            // 新建对话 - Ctrl+N
            if (e.Control && e.KeyCode == Keys.N)
            {
                e.SuppressKeyPress = true;
                CreateNewChat();
            }

            // 切换到聊天页面 - Alt+1
            if (e.Alt && e.KeyCode == Keys.D1)
            {
                e.SuppressKeyPress = true;
                SwitchToPanel(chatPagePanel, chatNavButton);
            }

            // 切换到设置页面 - Alt+2
            if (e.Alt && e.KeyCode == Keys.D2)
            {
                e.SuppressKeyPress = true;
                SwitchToPanel(settingsPanel, settingsNavButton);
            }

            // 切换到AI网站页面 - Alt+3
            if (e.Alt && e.KeyCode == Keys.D3)
            {
                e.SuppressKeyPress = true;
                SwitchToPanel(aiWebsitePanel, websiteNavButton);
            }

            // 焦点到搜索框 - Ctrl+F
            if (e.Control && e.KeyCode == Keys.F)
            {
                e.SuppressKeyPress = true;
                TextBox searchBox = chatListPanel.Controls.Find("searchBox", true).FirstOrDefault() as TextBox;
                if (searchBox != null)
                    searchBox.Focus();
            }

            // 清空聊天框 - Ctrl+L
            if (e.Control && e.KeyCode == Keys.L)
            {
                e.SuppressKeyPress = true;
                if (chatboxControl != null)
                {
                    // 清空 chatboxControl 的消息
                    chatboxControl.ClearMessages();
                    // 显示欢迎界面
                    InitializeChatboxForEmptySession(chatboxControl);
                }
            }
        }

        /// <summary>
        /// 窗体关闭事件处理，清理活跃渠道
        /// </summary>
        private void LlmAgentMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 如果当前窗体有使用中的渠道，在关闭窗体时将其从活跃列表中移除
            if (_currentChannelId != Guid.Empty)
            {
                RemoveActiveChannel(_currentChannelId);
            }
        }

        private void SwitchToNextNavPage()
        {
            // 实现在导航页面之间循环切换的逻辑
            Button[] navButtons = { avatarButton, chatNavButton, websiteNavButton, promptsNavButton, filesNavButton, settingsNavButton };

            // 找出当前活动的按钮
            int currentIndex = -1;
            for (int i = 0; i < navButtons.Length; i++)
            {
                if (navButtons[i].ForeColor == Color.White) // 假设活动按钮文本为白色
                {
                    currentIndex = i;
                    break;
                }
            }

            // 计算下一个按钮的索引
            int nextIndex = (currentIndex + 1) % navButtons.Length;
            navButtons[nextIndex].PerformClick();
        }

        // 切换设置页面
        private void SwitchSettingsPage(Control targetContainer)
        {
            // 隐藏所有设置内容面板
            foreach (Control control in settingsContentContainer.Controls)
            {
                if (control is Panel)
                {
                    control.Visible = false;
                }
            }

            // 显示目标设置面板
            targetContainer.Visible = true;

            // 重置所有设置菜单按钮样式
            foreach (Control control in settingsMenuPanel.Controls)
            {
                if (control is Button button)
                {
                    button.BackColor = Color.FromArgb(248, 249, 250);
                }
            }

            // 高亮当前活动按钮
            if (targetContainer == shortcutSettingsContainer)
                shortcutSettingsButton.BackColor = Color.FromArgb(230, 230, 230);
            else if (targetContainer == generalSettingsContainer)
                generalSettingsButton.BackColor = Color.FromArgb(230, 230, 230);
            else if (targetContainer == dataSettingsContainer)
                dataSettingsButton.BackColor = Color.FromArgb(230, 230, 230);
            else if (targetContainer == aboutContainer)
                aboutSettingsButton.BackColor = Color.FromArgb(230, 230, 230);
        }

        // 更新聊天区域的模型提供商选择列表
        private void UpdateChatProviderModels()
        {
            // 此方法已不再使用chatModelComboBox控件
            // 而是使用chatboxControl的模型选择器

            /* 原代码如下：
            if (chatModelComboBox != null)
            {
                chatModelComboBox.Items.Clear();

                // 获取所有已启用的渠道
                var enabledChannels = _channelManager.GetEnabledChannels();

                // 加载所有启用渠道的模型
                foreach (var channel in enabledChannels)
                {
                    // 获取渠道支持的模型列表
                    var availableModels = channel.SupportedModels;

                    foreach (var model in availableModels)
                    {
                        // 添加渠道前缀来区分不同渠道的模型
                        string displayName = $"{channel.Name}: {model}";
                        chatModelComboBox.Items.Add(displayName);
                    }
                }

                // 选择上次使用的模型
                if (!string.IsNullOrEmpty(_currentModelId) && chatModelComboBox.Items.Contains(_currentModelId))
                {
                    chatModelComboBox.SelectedItem = _currentModelId;
                }
                else if (chatModelComboBox.Items.Count > 0)
                {
                    chatModelComboBox.SelectedIndex = 0;
                    _currentModelId = chatModelComboBox.SelectedItem.ToString();
                }
            }
            */

            // 使用新的chatboxControl控件更新模型列表
            if (chatboxControl != null)
            {
                UpdateChatboxModelList();
            }
        }

        // 获取提供商的简称
        private string GetProviderShortName(ProviderType providerType)
        {
            switch (providerType)
            {
                case ProviderType.OpenAI:
                    return "OAI";
                case ProviderType.AzureOpenAI:
                    return "Azure";
                case ProviderType.Anthropic:
                    return "Claude";
                case ProviderType.Google:
                    return "Google";
                case ProviderType.ZhipuAI:
                    return "ZhipuAI";
                case ProviderType.Other:
                    return "Other";
                default:
                    return providerType.ToString();
            }
        }

        // 获取已启用的提供商列表
        private List<ProviderType> GetEnabledProviders()
        {
            // 从渠道管理器中获取所有启用的渠道
            var enabledChannels = _channelManager.GetEnabledChannels();

            // 提取所有启用的渠道的提供商类型
            return enabledChannels
                .Select(c => c.ProviderType)
                .Distinct()
                .ToList();
        }

        // 根据简称获取提供商类型
        private ProviderType GetProviderTypeFromDisplayName(string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
                return ProviderType.OpenAI;

            if (displayName.Contains("OpenAI", StringComparison.OrdinalIgnoreCase))
            {
                if (displayName.Contains("Azure", StringComparison.OrdinalIgnoreCase))
                    return ProviderType.AzureOpenAI;
                else
                    return ProviderType.OpenAI;
            }
            else if (displayName.Contains("Anthropic", StringComparison.OrdinalIgnoreCase) ||
                     displayName.Contains("Claude", StringComparison.OrdinalIgnoreCase))
                return ProviderType.Anthropic;
            else if (displayName.Contains("Google", StringComparison.OrdinalIgnoreCase) ||
                     displayName.Contains("Gemini", StringComparison.OrdinalIgnoreCase))
                return ProviderType.Google;
            else if (displayName.Contains("智谱", StringComparison.OrdinalIgnoreCase) ||
                     displayName.Contains("GLM", StringComparison.OrdinalIgnoreCase))
                return ProviderType.ZhipuAI;
            else if (displayName.Contains("其他", StringComparison.OrdinalIgnoreCase) ||
                     displayName.Contains("Other", StringComparison.OrdinalIgnoreCase))
                return ProviderType.Other;

            // 默认返回OpenAI
            return ProviderType.OpenAI;
        }

        // 模型列表选择事件处理
        // private void ModelListBox_SelectedIndexChanged(object sender, EventArgs e)
        // {
        //     if (modelListBox.SelectedItem != null)
        //     {
        //         string selectedModel = modelListBox.SelectedItem.ToString();
        //
        //         // 设置当前模型ID
        //         _currentModelId = selectedModel;
        //
        //         // 保存设置
        //         Properties.Settings.Default.LastSelectedModel = _currentModelId;
        //         Properties.Settings.Default.Save();
        //
        //         // 更新窗体标题
        //         UpdateTitle();
        //     }
        // }

        // 管理模型按钮点击事件
        private void ManageModelsButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 获取当前选中的提供商和模型
                string providerName = GetProviderDisplayName(_currentProviderType);
                // 不再需要获取选中的模型ID

                // 打开模型管理界面
                using (var modelForm = new ModelManagementForm(providerName, _currentProviderType, _providerFactory))
                {
                    if (modelForm.ShowDialog() == DialogResult.OK)
                    {
                        // 创建新的HttpClient实例，避免重用已发送请求的实例
                        _httpClient.Dispose();
                        _httpClient = new HttpClient();
                        _httpClient.Timeout = TimeSpan.FromMinutes(5);

                        // 重新初始化提供商，确保能获取到最新的模型列表
                        _providerFactory = new ProviderFactory(_httpClient);
                        // 重新创建当前提供商实例
                        var currentProvider = _providerFactory.GetProvider(_currentProviderType);

                        // 重新加载模型列表
                        UpdateModelList();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开模型管理功能时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 测试模型按钮点击事件
        private void TestModelButton_Click(object sender, EventArgs e)
        {
            if (channelModelListBox.SelectedItem == null)
            {
                MessageBox.Show("请先选择要测试的模型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string selectedModelId = channelModelListBox.SelectedItem.ToString();
            var channel = GetSelectedChannel();

            if (channel == null)
            {
                MessageBox.Show("请先选择渠道", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 使用新的构造函数创建ModelTestForm
            using (var testForm = new ModelTestForm(channel, selectedModelId))
            {
                testForm.ShowDialog();
            }
        }

        private void channelNavButton_Click(object sender, EventArgs e)
        {
            SwitchToPanel(channelPanel, channelNavButton);

            // 初始化渠道列表
            InitializeChannelList();
        }

        private void InitializeChannelList()
        {
            try
            {
                // 清空现有列表
                channelListBox.Items.Clear();

                // 获取所有渠道
                var channels = _channelManager.GetAllChannels();

                // 创建字典，用于保存渠道ID和索引的映射关系
                Dictionary<int, Guid> channelIndexMap = new Dictionary<int, Guid>();

                for (int i = 0; i < channels.Count; i++)
                {
                    // 在列表中显示渠道名称，前面加上启用状态标记
                    string displayName = channels[i].IsEnabled ? "✓ " : "✗ ";
                    displayName += channels[i].Name;

                    channelListBox.Items.Add(displayName);

                    // 保存索引和ID的映射
                    channelIndexMap[i] = channels[i].Id;
                }

                // 保存索引映射为Tag
                channelListBox.Tag = channelIndexMap;

                // 如果有项目，默认选择第一个
                if (channelListBox.Items.Count > 0)
                {
                    channelListBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"初始化渠道列表失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void channelListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (channelListBox.SelectedIndex >= 0)
                {
                    // 从Tag中获取索引映射
                    var channelIndexMap = channelListBox.Tag as Dictionary<int, Guid>;

                    if (channelIndexMap != null && channelIndexMap.TryGetValue(channelListBox.SelectedIndex, out Guid channelId))
                    {
                        // 获取渠道对象
                        var channel = _channelManager.GetChannelById(channelId);

                        // 检查渠道模型列表是否为空，如果为空并且API密钥不为空，则尝试从API获取模型列表
                        if ((channel.SupportedModels == null || channel.SupportedModels.Count == 0) && !string.IsNullOrEmpty(channel.ApiKey))
                        {
                            try
                            {
                                // 显示加载提示
                                Cursor = Cursors.WaitCursor;
                                channelTitleLabel.Text = $"渠道详情 - {channel.Name} (正在从API获取模型列表...)";

                                // 在后台线程中执行
                                Task.Run(async () =>
                                {
                                    List<string> modelList = new List<string>();

                                    try
                                    {
                                        // 获取提供商实例
                                        var provider = _providerFactory.GetProvider(channel.ProviderType);

                                        // 设置API密钥和主机
                                        provider.UpdateApiKey(channel.ApiKey);
                                        provider.UpdateApiHost(channel.ApiHost);

                                        // 尝试执行对应的GetModelsFromApiAsync方法
                                        if (provider is OpenAIProvider openAIProvider)
                                        {
                                            modelList = await openAIProvider.GetModelsFromApiAsync();
                                        }
                                        else if (provider is AzureOpenAIProvider azureProvider)
                                        {
                                            modelList = await azureProvider.GetModelsFromApiAsync();
                                        }
                                        else if (provider is AnthropicProvider anthropicProvider)
                                        {
                                            modelList = await anthropicProvider.GetModelsFromApiAsync();
                                        }
                                        else if (provider is GeminiProvider geminiProvider)
                                        {
                                            modelList = await geminiProvider.GetModelsFromApiAsync();
                                        }
                                        else if (provider is ZhipuProvider zhipuProvider)
                                        {
                                            modelList = await zhipuProvider.GetModelsFromApiAsync();
                                        }
                                        else if (provider is SiliconFlowProvider siliconFlowProvider)
                                        {
                                            modelList = await siliconFlowProvider.GetModelsFromApiAsync();
                                        }
                                        else
                                        {
                                            // 如果没有特定的API获取方法，使用默认支持的模型
                                            modelList = provider.GetSupportedModels();
                                        }

                                        // 如果成功获取模型列表
                                        if (modelList.Count > 0)
                                        {
                                            // 更新渠道的支持模型列表
                                            channel.SupportedModels = modelList;
                                            _channelManager.UpdateChannelModels(channelId, modelList);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        BeginInvoke(new Action(() =>
                                        {
                                            MessageBox.Show($"从API获取模型列表失败: {ex.Message}\n将使用默认模型列表。",
                                                "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        }));
                                    }
                                    finally
                                    {
                                        // 在UI线程中更新界面
                                        BeginInvoke(new Action(() =>
                                        {
                                            // 更新渠道详情
                                            UpdateChannelDetails(channel);
                                            Cursor = Cursors.Default;
                                        }));
                                    }
                                });
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"尝试从API获取模型列表失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Cursor = Cursors.Default;

                                // 继续更新渠道详情
                                UpdateChannelDetails(channel);
                            }
                        }
                        else
                        {
                            // 正常更新渠道详情
                            UpdateChannelDetails(channel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载渠道详情失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor = Cursors.Default;
            }
        }

        private void UpdateChannelDetails(Channel channel)
        {
            try
            {
                _isUpdatingChannelDetails = true; // 设置标志，表示正在更新渠道详情

                // 更新渠道详情界面
                channelTitleLabel.Text = $"渠道详情 - {channel.Name}";

                // 填充渠道信息
                txtChannelName.Text = channel.Name;

                // 设置渠道类型下拉框
                if (cboChannelType.Items.Count == 0)
                {
                    // 添加所有提供商类型
                    foreach (ProviderType type in Enum.GetValues(typeof(ProviderType)))
                    {
                        cboChannelType.Items.Add(GetProviderDisplayName(type));
                    }
                }

                // 根据渠道类型选择对应的下拉框项
                string providerName = GetProviderDisplayName(channel.ProviderType);

                // 暂时移除事件处理，避免自动触发API主机地址更新
                cboChannelType.SelectedIndexChanged -= cboChannelType_SelectedIndexChanged;

                // 查找匹配的下拉框项
                cboChannelType.SelectedIndex = -1; // 先清除选择
                for (int i = 0; i < cboChannelType.Items.Count; i++)
                {
                    string item = cboChannelType.Items[i].ToString();
                    if (string.Equals(item, providerName, StringComparison.OrdinalIgnoreCase))
                    {
                        cboChannelType.SelectedIndex = i;
                        break;
                    }
                }

                // 如果没找到匹配项，选择第一项
                if (cboChannelType.SelectedIndex == -1 && cboChannelType.Items.Count > 0)
                {
                    cboChannelType.SelectedIndex = 0;
                }

                // 重新添加事件处理
                cboChannelType.SelectedIndexChanged += cboChannelType_SelectedIndexChanged;

                // 填充API设置
                txtApiKey.Text = channel.ApiKey;

                // 如果API主机地址为空，则使用默认值
                if (string.IsNullOrEmpty(channel.ApiHost))
                {
                    txtApiHost.Text = GetDefaultApiHost(channel.ProviderType);
                }
                else
                {
                    txtApiHost.Text = channel.ApiHost;
                }

                // 设置启用状态
                enableChannelCheckBox.Checked = channel.IsEnabled;

                // 更新模型列表
                UpdateChannelModelList(channel);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新渠道详情失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isUpdatingChannelDetails = false; // 重置标志
            }
        }

        private void UpdateChannelModelList(Channel channel)
        {
            // 清空现有列表
            channelModelListBox.Items.Clear();

            // 添加渠道中的所有模型
            foreach (var model in channel.SupportedModels)
            {
                channelModelListBox.Items.Add(model);
            }
        }

        private void addChannelButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 创建新渠道
                var channel = new Channel
                {
                    Name = $"新渠道_{DateTime.Now.ToString("HHmmss")}",
                    ProviderType = ProviderType.OpenAI,
                    ApiHost = GetDefaultApiHost(ProviderType.OpenAI),
                    IsEnabled = true,
                    SupportedModels = new List<string>() // 使用空列表，不预设模型
                };

                // 添加到渠道管理器
                _channelManager.AddChannel(channel);

                // 刷新渠道列表
                InitializeChannelList();

                // 选中新添加的渠道
                int lastIndex = channelListBox.Items.Count - 1;
                if (lastIndex >= 0)
                {
                    channelListBox.SelectedIndex = lastIndex;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加渠道失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void deleteChannelButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (channelListBox.SelectedIndex >= 0)
                {
                    // 从Tag中获取索引映射
                    var channelIndexMap = channelListBox.Tag as Dictionary<int, Guid>;

                    if (channelIndexMap != null && channelIndexMap.TryGetValue(channelListBox.SelectedIndex, out Guid channelId))
                    {
                        var channel = _channelManager.GetChannelById(channelId);

                        // 检查渠道是否正在被使用
                        if (IsChannelActive(channelId))
                        {
                            // 显示警告信息
                            if (MessageBox.Show($"渠道 '{channel.Name}' 正在被聊天窗口使用，删除可能会导致正在使用此渠道的对话失败。确定要继续删除吗？",
                                "删除确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                            {
                                return; // 用户取消删除
                            }
                        }
                        else
                        {
                            if (MessageBox.Show($"确定要删除渠道 '{channel.Name}' 吗？", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                // 删除渠道
                                _channelManager.DeleteChannel(channelId);

                                // 记住当前选择的索引
                                int selectedIndex = channelListBox.SelectedIndex;

                                // 刷新渠道列表
                                InitializeChannelList();

                                // 选择合适的项目
                                if (channelListBox.Items.Count > 0)
                                {
                                    channelListBox.SelectedIndex = Math.Min(selectedIndex, channelListBox.Items.Count - 1);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"删除渠道失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdateChannel_Click(object sender, EventArgs e)
        {
            try
            {
                if (channelListBox.SelectedIndex >= 0)
                {
                    // 从Tag中获取索引映射
                    var channelIndexMap = channelListBox.Tag as Dictionary<int, Guid>;

                    if (channelIndexMap != null && channelIndexMap.TryGetValue(channelListBox.SelectedIndex, out Guid channelId))
                    {
                        // 获取当前渠道
                        var channel = _channelManager.GetChannelById(channelId);

                        // 获取渠道类型
                        string providerName = cboChannelType.SelectedItem?.ToString() ?? "OpenAI";

                        // 检查selectedItem是否有效
                        if (string.IsNullOrEmpty(providerName))
                        {
                            MessageBox.Show("请选择有效的渠道类型！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // 使用修正的方法获取提供商类型
                        ProviderType providerType = GetProviderTypeFromDisplayName(providerName);

                        // 验证输入
                        string name = txtChannelName.Text.Trim();
                        if (string.IsNullOrEmpty(name))
                        {
                            MessageBox.Show("渠道名称不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // 如果名称变更，检查是否已存在
                        if (name != channel.Name && _channelManager.GetAllChannels().Any(c => c.Id != channelId && c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                        {
                            MessageBox.Show($"渠道名称 '{name}' 已存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // 更新渠道属性
                        channel.Name = name;
                        channel.ProviderType = providerType;
                        channel.ApiKey = txtApiKey.Text.Trim();
                        channel.ApiHost = txtApiHost.Text.Trim();
                        channel.IsEnabled = enableChannelCheckBox.Checked;
                        channel.UpdatedAt = DateTime.Now;

                        // 保存到渠道管理器
                        _channelManager.UpdateChannel(channel);

                        // 如果是启用状态，同步更新应用程序的当前提供商设置
                        if (channel.IsEnabled)
                        {
                            // 更新应用程序的当前提供商类型
                            _currentProviderType = providerType;

                            // 更新应用程序设置
                            Properties.Settings.Default.ProviderType = providerType.ToString();

                            // 根据提供商类型更新API密钥和主机设置
                            switch (providerType)
                            {
                                case ProviderType.OpenAI:
                                    Properties.Settings.Default.OpenAIApiKey = channel.ApiKey;
                                    Properties.Settings.Default.OpenAIApiHost = channel.ApiHost;
                                    break;
                                case ProviderType.AzureOpenAI:
                                    Properties.Settings.Default.AzureApiKey = channel.ApiKey;
                                    Properties.Settings.Default.AzureApiHost = channel.ApiHost;
                                    break;
                                case ProviderType.Anthropic:
                                    Properties.Settings.Default.AnthropicApiKey = channel.ApiKey;
                                    Properties.Settings.Default.AnthropicApiHost = channel.ApiHost;
                                    break;
                                case ProviderType.Google:
                                    Properties.Settings.Default.GeminiApiKey = channel.ApiKey;
                                    Properties.Settings.Default.GeminiApiHost = channel.ApiHost;
                                    break;
                                case ProviderType.ZhipuAI:
                                    Properties.Settings.Default.ZhipuApiKey = channel.ApiKey;
                                    Properties.Settings.Default.ZhipuApiHost = channel.ApiHost;
                                    break;
                                case ProviderType.Other:
                                    Properties.Settings.Default.OtherApiKey = channel.ApiKey;
                                    Properties.Settings.Default.OtherApiHost = channel.ApiHost;
                                    break;
                            }

                            // 更新API设置
                            Properties.Settings.Default.Save();

                            // 更新窗体标题
                            UpdateTitle();

                            // 更新模型列表
                            UpdateModelList();
                        }

                        // 刷新渠道列表
                        int selectedIndex = channelListBox.SelectedIndex;
                        InitializeChannelList();
                        channelListBox.SelectedIndex = selectedIndex;

                        MessageBox.Show($"渠道 '{channel.Name}' 已更新！", "更新成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新渠道失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void testChannelButton_Click(object sender, EventArgs e)
        {
            // 批量渠道测试模式
            using (var tester = new ChannelTestForm())
            {
                tester.ShowDialog();
            }
        }

        private void enableChannelCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // 如果是界面更新触发的，不执行实际的启用/禁用操作
            if (_isUpdatingChannelDetails)
                return;

            try
            {
                if (channelListBox.SelectedIndex >= 0)
                {
                    // 从Tag中获取索引映射
                    var channelIndexMap = channelListBox.Tag as Dictionary<int, Guid>;

                    if (channelIndexMap != null && channelIndexMap.TryGetValue(channelListBox.SelectedIndex, out Guid channelId))
                    {
                        // 获取渠道
                        var channel = _channelManager.GetChannelById(channelId);

                        // 检查是否正在禁用渠道
                        bool isEnabled = enableChannelCheckBox.Checked;

                        // 如果是禁用操作且渠道正在被使用，显示警告
                        if (!isEnabled && IsChannelActive(channelId))
                        {
                            if (MessageBox.Show($"渠道 '{channel.Name}' 正在被聊天窗口使用，禁用可能会导致正在使用此渠道的对话失败。确定要继续禁用吗？",
                                "禁用确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                            {
                                // 用户取消禁用，恢复复选框状态
                                _isUpdatingChannelDetails = true;
                                enableChannelCheckBox.Checked = true;
                                _isUpdatingChannelDetails = false;
                                return;
                            }
                        }

                        // 更新渠道启用状态
                        _channelManager.SetChannelEnabledState(channelId, isEnabled);

                        // 如果渠道被启用，将其设置为当前使用的渠道
                        if (isEnabled)
                        {
                            // 更新应用程序的当前提供商类型
                            _currentProviderType = channel.ProviderType;

                            // 更新应用程序设置
                            Properties.Settings.Default.ProviderType = channel.ProviderType.ToString();

                            // 根据提供商类型更新API密钥和主机设置
                            switch (channel.ProviderType)
                            {
                                case ProviderType.OpenAI:
                                    Properties.Settings.Default.OpenAIApiKey = channel.ApiKey;
                                    Properties.Settings.Default.OpenAIApiHost = channel.ApiHost;
                                    break;
                                case ProviderType.AzureOpenAI:
                                    Properties.Settings.Default.AzureApiKey = channel.ApiKey;
                                    Properties.Settings.Default.AzureApiHost = channel.ApiHost;
                                    break;
                                case ProviderType.Anthropic:
                                    Properties.Settings.Default.AnthropicApiKey = channel.ApiKey;
                                    Properties.Settings.Default.AnthropicApiHost = channel.ApiHost;
                                    break;
                                case ProviderType.Google:
                                    Properties.Settings.Default.GeminiApiKey = channel.ApiKey;
                                    Properties.Settings.Default.GeminiApiHost = channel.ApiHost;
                                    break;
                                case ProviderType.ZhipuAI:
                                    Properties.Settings.Default.ZhipuApiKey = channel.ApiKey;
                                    Properties.Settings.Default.ZhipuApiHost = channel.ApiHost;
                                    break;
                                case ProviderType.Other:
                                    Properties.Settings.Default.OtherApiKey = channel.ApiKey;
                                    Properties.Settings.Default.OtherApiHost = channel.ApiHost;
                                    break;
                            }

                            // 更新API设置
                            Properties.Settings.Default.Save();

                            // 更新窗体标题
                            UpdateTitle();

                            // 删除这行消息提示
                            // MessageBox.Show($"已将 '{channel.Name}' 设置为当前使用的渠道", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        // 无论渠道是启用还是禁用，都更新模型列表
                        UpdateModelList();

                        // 刷新渠道列表
                        int selectedIndex = channelListBox.SelectedIndex;
                        InitializeChannelList();
                        channelListBox.SelectedIndex = selectedIndex;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新渠道启用状态失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void settingsNavButton_Click(object sender, EventArgs e)
        {

        }

        // 添加渠道页面模型管理按钮事件处理
        private void manageChannelModelsButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 确保已选择渠道
                if (channelListBox.SelectedIndex < 0)
                {
                    MessageBox.Show("请先选择一个渠道！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 从Tag中获取索引映射
                var channelIndexMap = channelListBox.Tag as Dictionary<int, Guid>;

                if (channelIndexMap != null && channelIndexMap.TryGetValue(channelListBox.SelectedIndex, out Guid channelId))
                {
                    // 获取当前渠道
                    var channel = _channelManager.GetChannelById(channelId);

                    // 打开模型管理窗体
                    using (var modelManagementForm = new ModelManagementForm(
                        GetProviderDisplayName(channel.ProviderType),
                        channel.ProviderType,
                        _providerFactory))
                    {
                        if (modelManagementForm.ShowDialog() == DialogResult.OK)
                        {
                            // 获取数据库中为该提供商保存的模型
                            // 注意：ModelManagementForm已经将模型保存到数据库中
                            // 现在我们需要从数据库中获取已启用的模型ID列表，并更新到渠道中

                            try
                            {
                                // 获取提供商类型字符串
                                string providerStr = channel.ProviderType.ToString().ToLower();
                                if (providerStr == "azureopenai")
                                    providerStr = "openai"; // 特殊处理Azure OpenAI

                                // 从数据库获取已启用的模型
                                var dbManager = new DAL.DatabaseManager();
                                var dbModels = dbManager.GetModels(providerStr);

                                // 提取已启用的模型ID列表
                                List<string> enabledModelIds = dbModels
                                    .Where(m => m.Enabled)
                                    .Select(m => m.Id)
                                    .ToList();

                                // 如果列表为空（可能出现的异常情况），使用默认模型
                                if (enabledModelIds.Count == 0)
                                {
                                    // 回退到提供商的默认支持模型
                                    var provider = _providerFactory.GetProvider(channel.ProviderType);
                                    enabledModelIds = provider.GetSupportedModels();
                                }

                                // 更新渠道的支持模型列表
                                channel.SupportedModels = enabledModelIds;
                                _channelManager.UpdateChannelModels(channelId, enabledModelIds);

                                // 刷新模型列表显示
                                UpdateChannelModelList(channel);

                                MessageBox.Show($"模型列表已更新！共有 {enabledModelIds.Count} 个启用的模型。",
                                    "更新成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                // 如果获取数据库模型失败，回退到重新获取渠道信息
                                channel = _channelManager.GetChannelById(channelId);
                                UpdateChannelModelList(channel);

                                MessageBox.Show($"从数据库获取模型列表时出错: {ex.Message}\n已恢复到原有模型列表。",
                                    "更新警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开模型管理窗体失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 添加渠道页面模型测试按钮事件处理
        private void testChannelModelButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 确保已选择渠道
                if (channelListBox.SelectedIndex < 0)
                {
                    MessageBox.Show("请先选择一个渠道！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 确保已选择模型
                if (channelModelListBox.SelectedIndex < 0)
                {
                    MessageBox.Show("请先选择一个模型！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 获取当前渠道
                var channel = GetSelectedChannel();

                if (channel != null)
                {
                    // 获取选中的模型名称
                    string modelName = channelModelListBox.SelectedItem?.ToString();

                    if (!string.IsNullOrEmpty(modelName))
                    {
                        // 打开模型测试窗体，使用新的构造函数
                        using (var modelTestForm = new ModelTestForm(channel, modelName))
                        {
                            modelTestForm.ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开模型测试窗体失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void channelModelsGroupBox_Enter(object sender, EventArgs e)
        {

        }

        // 渠道类型下拉框选择变更事件
        private void cboChannelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboChannelType.SelectedIndex >= 0)
                {
                    // 获取选中的提供商名称
                    string providerName = cboChannelType.SelectedItem.ToString();

                    // 转换为提供商类型
                    ProviderType providerType = GetProviderTypeFromDisplayName(providerName);

                    // 获取该提供商类型的默认API主机地址
                    string defaultApiHost = GetDefaultApiHost(providerType);

                    // 更新API主机地址文本框
                    txtApiHost.Text = defaultApiHost;
                }
            }
            catch (Exception ex)
            {
                // 记录错误但不显示给用户，以免打断用户操作
                Console.Error.WriteLine($"更新API主机地址失败: {ex.Message}");
            }
        }

        private Channel GetSelectedChannel()
        {
            if (channelListBox.SelectedIndex >= 0)
            {
                // 从Tag中获取索引映射
                var channelIndexMap = channelListBox.Tag as Dictionary<int, Guid>;

                if (channelIndexMap != null && channelIndexMap.TryGetValue(channelListBox.SelectedIndex, out Guid channelId))
                {
                    // 获取渠道对象
                    return _channelManager.GetChannelById(channelId);
                }
            }
            return null;
        }

        // 清除所有聊天记录按钮点击事件
        private void ClearChatHistoryButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "确定要清除所有聊天记录吗？此操作无法恢复！",
                "确认清除",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // 清除所有聊天记录
                    _chatHistoryManager.ClearAllChatHistory();

                    // 刷新聊天列表
                    UpdateChatList();

                    // 切换到新的空聊天界面
                    SwitchToPanel(chatPagePanel, chatNavButton);

                    MessageBox.Show(
                        "已成功清除所有聊天记录！",
                        "操作成功",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"清除聊天记录失败: {ex.Message}",
                        "操作失败",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void InitializeChatListPanel()
        {
            // 使用设计器已创建的控件，只添加事件处理器

            // 为searchBox绑定事件
            searchBox.TextChanged += SearchBox_TextChanged;

            // 创建searchPanel面板（用于兼容代码中可能的引用）
            searchPanel = new Panel
            {
                Visible = false,
                Width = 0,
                Height = 0
            };

            // 为chatListPanel添加拖放支持
            chatListPanel.AllowDrop = true;
            chatListPanel.DragEnter += ChatListPanel_DragEnter;
            chatListPanel.DragDrop += ChatListPanel_DragDrop;

            // 添加大小改变事件
            chatListPanel.SizeChanged += ChatListPanel_SizeChanged;

            // 清除现有事件并重新绑定
            newChatButton.Click -= NewChatButton_Click;
            newChatButton.Click += NewChatButton_Click;
        }

        // 提取为单独的方法，以便可以明确地添加和移除
        private void NewChatButton_Click(object sender, EventArgs e)
        {
            CreateNewChat();
        }

        private void ChatListPanel_SizeChanged(object sender, EventArgs e)
        {
            // 当面板大小改变时调整所有ChatSessionItem的宽度
            foreach (Control control in chatListPanel.Controls)
            {
                if (control is ChatSessionItem sessionItem)
                {
                    sessionItem.Width = chatListPanel.ClientSize.Width - sessionItem.Margin.Horizontal;
                }
            }
        }

        private void PromptsListPanel_SizeChanged(object sender, EventArgs e)
        {
            // 当面板大小改变时调整所有PromptCardItem的宽度
            foreach (Control control in promptsListPanel.Controls)
            {
                if (control is PromptCardItem promptCard)
                {
                    promptCard.Width = promptsListPanel.ClientSize.Width - promptCard.Margin.Horizontal;
                }
            }
        }

        private void UpdateChatList()
        {
            chatListPanel.Controls.Clear();

            var sessions = _chatHistoryManager.GetAllSessions();
            foreach (var session in sessions)
            {
                var sessionItem = new ChatSessionItem();
                sessionItem.Session = session;
                sessionItem.OnSessionSelected += (s, e) => SwitchToChat(e);
                sessionItem.OnSessionDeleted += (s, e) => DeleteChatSession(e);

                // 添加到Panel之前先调整宽度
                sessionItem.Width = chatListPanel.ClientSize.Width - sessionItem.Margin.Horizontal;

                // 添加到控件集合
                chatListPanel.Controls.Add(sessionItem);

                // 在控件添加后再次调整大小，确保适应当前DPI设置
                float dpiScaleFactor = sessionItem.CreateGraphics().DpiX / 96f;
                int scaledHeight = (int)(85 * dpiScaleFactor);
                sessionItem.Height = scaledHeight;

                // 确保内部控件也正确调整大小
                if (sessionItem.IsHandleCreated)
                {
                    sessionItem.Invoke(new Action(() =>
                    {
                        typeof(ChatSessionItem).GetMethod("AdjustControlSizes",
                            System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Instance)?.Invoke(sessionItem, null);
                    }));
                }
            }
        }

        private void ChatListPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ChatSessionItem)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void ChatListPanel_DragDrop(object sender, DragEventArgs e)
        {
            var draggedItem = (ChatSessionItem)e.Data.GetData(typeof(ChatSessionItem));
            var dropPoint = chatListPanel.PointToClient(new Point(e.X, e.Y));
            var targetIndex = GetDropIndex(dropPoint);

            if (targetIndex > -1)
            {
                chatListPanel.Controls.SetChildIndex(draggedItem, targetIndex);
                // 更新会话顺序
                UpdateSessionOrder();
            }
        }

        private int GetDropIndex(Point dropPoint)
        {
            for (int i = 0; i < chatListPanel.Controls.Count; i++)
            {
                var control = chatListPanel.Controls[i];
                if (control is ChatSessionItem)
                {
                    if (dropPoint.Y < control.Bottom)
                    {
                        return i;
                    }
                }
            }
            return chatListPanel.Controls.Count - 1;
        }

        private void UpdateSessionOrder()
        {
            var sessions = new List<ChatSession>();
            foreach (Control control in chatListPanel.Controls)
            {
                if (control is ChatSessionItem item)
                {
                    sessions.Add(item.Session);
                }
            }
            // 取消注释此行，启用会话顺序更新功能
            _chatHistoryManager.UpdateSessionOrder(sessions);
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            var searchText = searchBox.Text.ToLower();
            foreach (Control control in chatListPanel.Controls)
            {
                if (control is ChatSessionItem item)
                {
                    bool match = item.Session.Title?.ToLower().Contains(searchText) == true ||
                                item.Session.Messages.Any(m => m.Content.ToLower().Contains(searchText));
                    control.Visible = match;
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // 检查是否按下Ctrl键
            bool isCtrlPressed = (keyData & Keys.Control) == Keys.Control;

            // 检查是否按下Alt键
            bool isAltPressed = (keyData & Keys.Alt) == Keys.Alt;

            // 获取主键（去除修饰键）
            Keys mainKey = keyData & ~Keys.Control & ~Keys.Alt;

            // 搜索快捷键：Ctrl+F
            if (isCtrlPressed && mainKey == Keys.F)
            {
                searchBox.Focus();
                return true;
            }

            // 删除会话快捷键：Delete
            if (mainKey == Keys.Delete && chatListPanel.Focused)
            {
                var selectedItem = chatListPanel.Controls.OfType<ChatSessionItem>()
                    .FirstOrDefault(item => item.ClientRectangle.Contains(item.PointToClient(Control.MousePosition)));
                if (selectedItem != null)
                {
                    DeleteChatSession(selectedItem.Session);
                }
                return true;
            }

            // 切换会话快捷键：Alt+数字键
            if (isAltPressed && mainKey >= Keys.D1 && mainKey <= Keys.D9)
            {
                int index = (int)mainKey - (int)Keys.D1;
                var sessions = chatListPanel.Controls.OfType<ChatSessionItem>().ToList();
                if (index < sessions.Count)
                {
                    SwitchToChat(sessions[index].Session);
                }
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void DeleteChatSession(ChatSession session)
        {
            if (session == null) return;

            var result = MessageBox.Show(
                string.Format("确定要删除会话 \"{0}\" 吗？", session.Title),
                "删除确认",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // 从数据库中删除会话
                _chatHistoryManager.DeleteChat(session.Id);

                // 更新会话列表
                UpdateChatList();
            }
        }

        /// <summary>
        /// 初始化聊天面板现代化控件
        /// </summary>
        private void InitializeChatbox()
        {
            // 如果已经初始化，则不再重复初始化
            if (chatboxControl != null && chatContainer.Panel1.Controls.Contains(chatboxControl))
                return;

            // 移除chatContainer中Panel1和Panel2的所有控件
            chatContainer.Panel1.Controls.Clear();
            chatContainer.Panel2.Controls.Clear();

            // 使chatContainer变为普通的Panel（隐藏分割线）
            chatContainer.Panel2Collapsed = true;

            // 创建 ChatboxInfo 对象配置 Chatbox
            var chatboxInfo = new ChatboxInfo
            {
                User = "用户", // 用户名称
                NamePlaceholder = "用户", // 顶部显示的名称
                StatusPlaceholder = "在线", // 状态文本
                PhonePlaceholder = "LLM Agent", // 显示的标识符
                ChatPlaceholder = "在此输入消息..." // 输入框占位符文本
            };

            // 创建并配置 Chatbox 控件
            chatboxControl = new Chatbox(chatboxInfo);
            chatboxControl.Dock = DockStyle.Fill;
            chatboxControl.Name = "chatboxControl";

            // 设置流式响应状态
            chatboxControl.SetStreamResponse(_useStreamResponse);

            // 注册流式响应事件
            chatboxControl.StreamResponseToggled += (s, e) => {
                _useStreamResponse = chatboxControl.UseStreamResponse;
                // 保存设置
                Properties.Settings.Default.EnableStreamResponse = _useStreamResponse;
                Properties.Settings.Default.Save();
            };

            // 注册模型选择事件
            chatboxControl.ModelSelectionChanged += (s, e) => {
                string selectedModel = chatboxControl.GetSelectedModel();
                if (!string.IsNullOrEmpty(selectedModel))
                {
                    // 直接处理模型选择逻辑
                    HandleModelSelection(selectedModel);
                }
            };

            // 重新配置附件上传功能
            // 移除原有的BuildAttachment事件处理
            var attachButton = chatboxControl.Controls.Find("attachButton", true).FirstOrDefault() as Button;
            if (attachButton != null)
            {
                // 清除原有的事件处理
                attachButton.Click -= new EventHandler(chatboxControl.BuildAttachment);

                // 添加新的事件处理
                attachButton.Click += (s, e) => {
                    // 调用文件上传功能
                    UploadAttachment();
                };
            }

            // 配置发送消息按钮
            var sendButton = chatboxControl.Controls.Find("sendButton", true).FirstOrDefault() as Button;
            if (sendButton != null)
            {
                // 先移除原有的SendMessage事件处理程序
                chatboxControl.RemoveSendMessageHandler();

                // 添加新的发送事件处理
                sendButton.Click += async (s, e) => {
                    await SendMessage();
                };
            }

            // 配置输入框的按键事件（Shift+Enter发送）
            var chatTextbox = chatboxControl.Controls.Find("chatTextbox", true).FirstOrDefault() as TextBox;
            if (chatTextbox != null)
            {
                chatTextbox.KeyDown += async (s, e) => {
                    if (e.Shift && e.KeyCode == Keys.Enter)
                    {
                        e.SuppressKeyPress = true; // 阻止Enter键的默认行为
                        await SendMessage();
                    }
                };
            }

            // 将 Chatbox 添加到chatContainer的Panel1（主要面板）
            chatContainer.Panel1.Controls.Add(chatboxControl);

            // 初始化模型列表
            UpdateChatboxModelList();
        }

        /// <summary>
        /// 处理文件上传功能
        /// </summary>
        private void UploadAttachment()
        {
            try
            {
                // 创建文件选择对话框
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "所有文件|*.*|图片文件|*.jpg;*.jpeg;*.png;*.gif|文档文件|*.pdf;*.doc;*.docx;*.txt";
                    openFileDialog.Title = "选择要上传的文件";
                    openFileDialog.Multiselect = false;

                    // 显示文件选择对话框
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // 获取选择的文件路径
                        string filePath = openFileDialog.FileName;
                        string fileName = Path.GetFileName(filePath);

                        // 读取文件内容
                        byte[] fileContent = File.ReadAllBytes(filePath);

                        // 检查文件大小
                        if (fileContent.Length > 1450000) // 限制文件大小为1.45MB
                        {
                            MessageBox.Show($"文件 {fileName} 太大，无法上传。请选择小于1.45MB的文件。", "文件过大", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // 获取文件扩展名
                        string extension = Path.GetExtension(filePath).ToLower();

                        // 根据文件类型处理
                        if (IsImageFile(extension))
                        {
                            // 处理图片文件
                            try
                            {
                                using (MemoryStream ms = new MemoryStream(fileContent))
                                {
                                    Image image = Image.FromStream(ms);

                                    // 创建图片消息模型
                                    var imageModel = new ImageChatModel
                                    {
                                        Author = "用户",
                                        Inbound = false,
                                        Read = true,
                                        Time = DateTime.Now,
                                        Image = image,
                                        ImageName = fileName
                                    };

                                    // 添加到聊天界面
                                    chatboxControl.AddMessage(imageModel);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"处理图片文件时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            // 处理其他类型文件
                            var attachmentModel = new AttachmentChatModel
                            {
                                Author = "用户",
                                Inbound = false,
                                Read = true,
                                Time = DateTime.Now,
                                Attachment = fileContent,
                                Filename = fileName
                            };

                            // 添加到聊天界面
                            chatboxControl.AddMessage(attachmentModel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"上传文件时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 判断文件是否为图片
        /// </summary>
        private bool IsImageFile(string extension)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };
            return imageExtensions.Contains(extension);
        }

        /// <summary>
        /// 更新Chatbox控件的模型列表
        /// </summary>
        private void UpdateChatboxModelList()
        {
            if (chatboxControl == null)
                return;

            // 获取所有已启用的渠道的模型
            var models = new List<string>();
            var enabledChannels = _channelManager.GetEnabledChannels();

            foreach (var channel in enabledChannels)
            {
                foreach (var model in channel.SupportedModels)
                {
                    string displayName = $"{channel.Name}: {model}";
                    models.Add(displayName);
                }
            }

            // 设置模型列表
            chatboxControl.SetModelList(models, _currentModelId);
        }

        /// <summary>
        /// 处理模型选择逻辑
        /// </summary>
        /// <param name="selectedModel">选中的模型</param>
        private void HandleModelSelection(string selectedModel)
        {
            if (string.IsNullOrEmpty(selectedModel))
                return;

            // 解析渠道名称和模型名称
            string[] parts = selectedModel.Split(new[] { ':' }, 2);
            if (parts.Length == 2)
            {
                string channelName = parts[0].Trim();
                string modelName = parts[1].Trim();

                // 从渠道管理器中获取对应的渠道
                var channel = _channelManager.GetEnabledChannels()
                    .FirstOrDefault(c => c.Name.Equals(channelName, StringComparison.OrdinalIgnoreCase));

                if (channel != null)
                {
                    // 如果之前有选择其他渠道，先从活跃列表中移除
                    if (_currentChannelId != Guid.Empty && _currentChannelId != channel.Id)
                    {
                        RemoveActiveChannel(_currentChannelId);
                    }

                    // 更新当前渠道ID和模型
                    _currentChannelId = channel.Id;
                    _currentModelId = selectedModel;
                    // 更新当前提供商类型，确保API调用使用正确的提供商
                    _currentProviderType = channel.ProviderType;

                    // 将新渠道添加到活跃列表
                    AddActiveChannel(_currentChannelId);

                    // 保存设置
                    Properties.Settings.Default.LastSelectedModel = _currentModelId;
                    Properties.Settings.Default.Save();

                    // 更新窗体标题
                    UpdateTitle();
                }
            }
        }

        /// <summary>
        /// 刷新聊天消息，将ChatMessage集合加载到Chatbox控件中显示
        /// </summary>
        /// <param name="chatbox">目标Chatbox控件</param>
        /// <param name="messages">要显示的消息集合</param>
        private void RefreshChatMessages(Chatbox chatbox, IEnumerable<ChatMessage> messages)
        {
            if (chatbox == null || messages == null)
                return;

            // 清空现有消息
            chatbox.ClearMessages();

            // 没有消息的情况，显示欢迎界面
            if (!messages.Any())
            {
                InitializeChatboxForEmptySession(chatbox);
                return;
            }

            // 转换并添加所有消息
            foreach (var message in messages)
            {
                // 使用适配器将ChatMessage转换为TextChatModel
                var chatModel = ChatModelAdapter.ToTextChatModel(message);
                if (chatModel != null)
                {
                    chatbox.AddMessage(chatModel);
                }
            }

            // 确保滚动到最新消息
            if (chatbox.GetMessageCount() > 0)
            {
                var lastMessage = chatbox.GetMessageAt(0);
                chatbox.ScrollToMessage(lastMessage);
            }
        }

        /// <summary>
        /// 查找并刷新最后一条助手消息
        /// </summary>
        /// <param name="chatbox">目标Chatbox控件</param>
        /// <returns>找到的最后一条助手消息对应的ChatItem控件，如果没有找到则返回null</returns>
        private ChatItem RefreshLastAssistantMessage(Chatbox chatbox)
        {
            if (chatbox == null || chatbox.GetMessageCount() == 0)
                return null;

            // 寻找最后一条助手消息
            // 注意：控件是按照添加顺序倒序排列的，所以最新的消息在顶部
            for (int i = 0; i < chatbox.GetMessageCount(); i++)
            {
                var chatItem = chatbox.GetMessageAt(i);
                if (chatItem != null)
                {
                    // 检查是否是助手消息（TextChatModel且Author为助手）
                    if (chatItem.Message is TextChatModel textModel &&
                        textModel.Inbound &&
                        textModel.Author == "助手")
                    {
                        // 找到最后一条助手消息，滚动到该消息
                        chatbox.ScrollToMessage(chatItem);
                        return chatItem;
                    }
                }
            }

            // 未找到助手消息
            return null;
        }

        /// <summary>
        /// 更新最后一条助手消息的内容
        /// </summary>
        /// <param name="chatbox">目标Chatbox控件</param>
        /// <param name="content">新的消息内容</param>
        /// <returns>是否成功更新</returns>
        private bool UpdateLastAssistantMessageContent(Chatbox chatbox, string content)
        {
            // 查找最后一条助手消息
            ChatItem lastAssistantItem = RefreshLastAssistantMessage(chatbox);
            if (lastAssistantItem == null)
                return false;

            // 更新消息内容
            if (lastAssistantItem.Message is TextChatModel textModel)
            {
                // 使用Chatbox的UpdateLastMessage方法更新内容
                return chatbox.UpdateLastMessage("助手", content);
            }

            return false;
        }

        private void InitializeChatboxForEmptySession(Chatbox chatbox)
        {
            if (chatbox == null) return;

            chatbox.ClearMessages();

            var welcomeMessage = new ChatMessage
            {
                Role = ChatRole.System, // 或者 Assistant
                Content = "当前没有活动的聊天。您可以开始新的对话或选择一个现有对话。",
                Timestamp = DateTime.Now
            };
            // 确保 ChatModelAdapter.ToTextChatModel 不会返回 null
            var chatModel = ChatModelAdapter.ToTextChatModel(welcomeMessage);
            if (chatModel != null) // 添加 null 检查
            {
                chatbox.AddMessage(chatModel);
            }

            // 确保 Chatbox 的输入框有焦点，如果适用
            var chatTextbox = chatbox.Controls.Find("chatTextbox", true).FirstOrDefault() as TextBox;
            if (chatTextbox != null)
            {
                chatTextbox.Focus();
            }
        }

        #region AI网站面板相关方法

        /// <summary>
        /// 计算DPI缩放后的高度
        /// </summary>
        /// <param name="baseHeight">基础高度</param>
        /// <param name="control">用于获取DPI的控件</param>
        /// <returns>缩放后的高度</returns>
        private int GetScaledHeight(int baseHeight, Control control)
        {
            float dpiScaleFactor = control.CreateGraphics().DpiX / 96f;
            return (int)(baseHeight * dpiScaleFactor);
        }

        /// <summary>
        /// 初始化AI网站面板
        /// </summary>
        private void InitializeAiWebsitePanel()
        {
            try
            {
                // 初始化网站列表
                InitializeWebsiteList();

                // 初始化搜索框
                InitializeWebsiteSearchBox();

                // 初始化新建按钮
                InitializeNewWebsiteButton();

                // 添加大小改变事件处理，与其他页面保持一致
                websiteListPanel.SizeChanged += WebsiteListPanel_SizeChanged;

                // 初始化内置浏览器
                InitializeWebsiteBrowser();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"初始化AI网站面板时出错: {ex.Message}");
                MessageBox.Show($"初始化AI网站面板时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 初始化网站列表
        /// </summary>
        private void InitializeWebsiteList()
        {
            try
            {
                // 获取所有网站
                var websites = _websiteManager.GetAllWebsites();

                // 清空现有的网站卡片
                websiteListPanel.Controls.Clear();

                // 重置选中状态
                _selectedWebsiteCard = null;

                // 添加各个网站的卡片
                foreach (var website in websites)
                {
                    var websiteCard = new WebsiteCardItem
                    {
                        Website = website,
                        Margin = new Padding(0, 1, 0, 1)
                    };

                    // 设置宽度和高度
                    websiteCard.Width = websiteListPanel.ClientSize.Width - websiteCard.Margin.Horizontal;
                    websiteCard.Height = GetScaledHeight(85, websiteListPanel);

                    // 添加点击事件
                    websiteCard.WebsiteClicked += (s, e) =>
                    {
                        // 取消之前选中卡片的高亮状态
                        if (_selectedWebsiteCard != null)
                        {
                            _selectedWebsiteCard.IsSelected = false;
                        }

                        // 设置当前卡片为选中状态
                        _selectedWebsiteCard = websiteCard;
                        websiteCard.IsSelected = true;

                        // 在浏览器中显示网站
                        DisplayWebsiteInBrowser(e.Website);
                    };

                    // 添加访问网站事件
                    websiteCard.VisitWebsiteClicked += (s, e) =>
                    {
                        VisitWebsite(e.Website);
                    };

                    // 添加编辑网站事件
                    websiteCard.EditWebsiteClicked += (s, e) =>
                    {
                        EditWebsite(e.Website);
                    };

                    // 添加删除网站事件
                    websiteCard.DeleteWebsiteClicked += (s, e) =>
                    {
                        DeleteWebsite(e.Website);
                    };

                    websiteListPanel.Controls.Add(websiteCard);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"初始化网站列表时出错: {ex.Message}");
                MessageBox.Show($"加载网站列表时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 初始化网站搜索框
        /// </summary>
        private void InitializeWebsiteSearchBox()
        {
            // 先解除旧的事件绑定，防止重复绑定
            websiteSearchBox.TextChanged -= WebsiteSearchBox_TextChanged;
            // 设置搜索框事件处理
            websiteSearchBox.TextChanged += WebsiteSearchBox_TextChanged;
        }

        /// <summary>
        /// 初始化新建网站按钮
        /// </summary>
        private void InitializeNewWebsiteButton()
        {
            // 先解除旧的事件绑定，防止重复绑定
            newWebsiteButton.Click -= NewWebsiteButton_Click;
            // 设置新建按钮事件处理
            newWebsiteButton.Click += NewWebsiteButton_Click;
        }

        /// <summary>
        /// 初始化内置浏览器
        /// </summary>
        private void InitializeWebsiteBrowser()
        {
            try
            {
                // 清空现有内容
                aiWebsiteContentPanel.Controls.Clear();

                // 创建内置浏览器控件
                _websiteBrowser = new WebsiteBrowser
                {
                    Dock = DockStyle.Fill
                };

                // 添加浏览器事件处理
                _websiteBrowser.NavigationCompleted += (s, e) =>
                {
                    // 导航完成后更新访问时间
                    if (_websiteBrowser.CurrentWebsite != null)
                    {
                        _websiteManager.UpdateWebsiteVisitTime(_websiteBrowser.CurrentWebsite.Id);
                    }
                };

                // 将浏览器添加到内容面板
                aiWebsiteContentPanel.Controls.Add(_websiteBrowser);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"初始化内置浏览器时出错: {ex.Message}");
                MessageBox.Show($"初始化内置浏览器时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 网站搜索框文本改变事件
        /// </summary>
        private void WebsiteSearchBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // 获取搜索框文本
                string searchText = websiteSearchBox.Text.Trim();

                List<AiWebsite> websites;

                // 根据搜索文本获取网站
                if (string.IsNullOrEmpty(searchText))
                {
                    websites = _websiteManager.GetAllWebsites();
                }
                else
                {
                    websites = _websiteManager.SearchWebsites(searchText);
                }

                // 清空现有的网站卡片
                websiteListPanel.Controls.Clear();

                // 重置选中状态
                _selectedWebsiteCard = null;

                // 添加匹配搜索文本的网站卡片
                foreach (var website in websites)
                {
                    var websiteCard = new WebsiteCardItem
                    {
                        Website = website,
                        Margin = new Padding(0, 1, 0, 1)
                    };

                    // 设置宽度和高度
                    websiteCard.Width = websiteListPanel.ClientSize.Width - websiteCard.Margin.Horizontal;
                    websiteCard.Height = GetScaledHeight(85, websiteListPanel);

                    // 添加点击事件
                    websiteCard.WebsiteClicked += (s, args) =>
                    {
                        // 取消之前选中卡片的高亮状态
                        if (_selectedWebsiteCard != null)
                        {
                            _selectedWebsiteCard.IsSelected = false;
                        }

                        // 设置当前卡片为选中状态
                        _selectedWebsiteCard = websiteCard;
                        websiteCard.IsSelected = true;

                        // 在浏览器中显示网站
                        DisplayWebsiteInBrowser(args.Website);
                    };

                    // 添加访问网站事件
                    websiteCard.VisitWebsiteClicked += (s, args) =>
                    {
                        VisitWebsite(args.Website);
                    };

                    // 添加编辑网站事件
                    websiteCard.EditWebsiteClicked += (s, args) =>
                    {
                        EditWebsite(args.Website);
                    };

                    // 添加删除网站事件
                    websiteCard.DeleteWebsiteClicked += (s, args) =>
                    {
                        DeleteWebsite(args.Website);
                    };

                    websiteListPanel.Controls.Add(websiteCard);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"搜索网站时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 网站列表面板大小改变事件
        /// </summary>
        private void WebsiteListPanel_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (Control control in websiteListPanel.Controls)
                {
                    if (control is WebsiteCardItem websiteCard)
                    {
                        websiteCard.Width = websiteListPanel.ClientSize.Width - websiteCard.Margin.Horizontal;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"调整网站卡片大小时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 新建网站按钮点击事件
        /// </summary>
        private void NewWebsiteButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 显示添加网站对话框
                using (var addWebsiteForm = new AddWebsiteForm())
                {
                    if (addWebsiteForm.ShowDialog() == DialogResult.OK)
                    {
                        // 创建新网站
                        var newWebsite = _websiteManager.CreateWebsite(
                            addWebsiteForm.WebsiteName,
                            addWebsiteForm.WebsiteUrl,
                            addWebsiteForm.WebsiteDescription,
                            addWebsiteForm.WebsiteCategory
                        );

                        // 刷新网站列表
                        InitializeWebsiteList();

                        MessageBox.Show("网站已添加成功！", "添加成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"添加新网站时出错: {ex.Message}");
                MessageBox.Show($"添加新网站时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 在浏览器中显示网站
        /// </summary>
        private void DisplayWebsiteInBrowser(AiWebsite website)
        {
            try
            {
                if (website == null || _websiteBrowser == null) return;

                // 导航到网站
                _websiteBrowser.NavigateToWebsite(website);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"在浏览器中显示网站时出错: {ex.Message}");
                MessageBox.Show($"打开网站时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 访问网站
        /// </summary>
        private void VisitWebsite(AiWebsite website)
        {
            try
            {
                if (website == null) return;

                // 更新访问时间
                _websiteManager.UpdateWebsiteVisitTime(website.Id);

                // 在内置浏览器中打开
                DisplayWebsiteInBrowser(website);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"访问网站时出错: {ex.Message}");
                MessageBox.Show($"访问网站时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 编辑网站
        /// </summary>
        private void EditWebsite(AiWebsite website)
        {
            try
            {
                if (website == null) return;

                // 显示编辑网站对话框
                using (var editWebsiteForm = new AddWebsiteForm(website))
                {
                    if (editWebsiteForm.ShowDialog() == DialogResult.OK)
                    {
                        // 更新网站信息
                        website.Name = editWebsiteForm.WebsiteName;
                        website.Url = editWebsiteForm.WebsiteUrl;
                        website.Description = editWebsiteForm.WebsiteDescription;
                        website.Category = editWebsiteForm.WebsiteCategory;

                        // 保存更改
                        _websiteManager.SaveWebsite(website);

                        // 刷新网站列表
                        InitializeWebsiteList();

                        MessageBox.Show("网站信息已更新！", "更新成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"编辑网站时出错: {ex.Message}");
                MessageBox.Show($"编辑网站时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 删除网站
        /// </summary>
        private void DeleteWebsite(AiWebsite website)
        {
            try
            {
                if (website == null) return;

                // 确认删除
                var result = MessageBox.Show(
                    $"确定要删除网站 \"{website.DisplayName}\" 吗？此操作不可恢复。",
                    "确认删除",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    // 删除网站
                    _websiteManager.DeleteWebsite(website.Id);

                    // 刷新网站列表
                    InitializeWebsiteList();

                    MessageBox.Show("网站已删除。", "删除成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"删除网站时出错: {ex.Message}");
                MessageBox.Show($"删除网站时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
