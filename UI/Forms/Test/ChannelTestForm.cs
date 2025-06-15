using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using llm_agent.Model;
using llm_agent.BLL;

namespace llm_agent.UI.Forms
{
    public partial class ChannelTestForm : Form
    {
        private readonly Channel _channel; // 单个渠道测试时使用
        private readonly ChannelService _channelService;
        private readonly ChannelManager _channelManager;
        private List<Channel> _enabledChannels = new List<Channel>(); // 存储所有启用的渠道
        private const string DEFAULT_TEST_MESSAGE = "你好，请简要介绍一下你自己"; // 默认测试消息，保留以供参考
        
        // 批量测试模式构造函数
        public ChannelTestForm()
        {
            _channelService = new ChannelService(new HttpClient());
            _channelManager = new ChannelManager();
            
            InitializeComponent();
            
            // 将窗体标题改为批量渠道测试
            this.Text = "渠道批量联通测试";
            
            // 在UI加载后加载所有启用的渠道列表
            this.Load += (s, e) => LoadEnabledChannels();
        }
        
        // 单个渠道测试模式构造函数（兼容原有代码）
        public ChannelTestForm(Channel channel)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _channelService = new ChannelService(new HttpClient());
            _channelManager = new ChannelManager();
            
            InitializeComponent();
            
            // 在UI加载后加载单个渠道信息
            this.Load += async (s, e) => await LoadSingleChannelInfo();
        }
        
        // 加载所有启用的渠道
        private void LoadEnabledChannels()
        {
            try
            {
                lblStatus.Text = "正在加载启用的渠道...";
                btnTest.Enabled = false;
                
                // 获取所有启用的渠道
                _enabledChannels = _channelManager.GetEnabledChannels();
                
                // 更新UI
                lblChannelInfo.Text = $"请选择要进行测试的渠道: (发现 {_enabledChannels.Count} 个启用的渠道)";
                
                // 填充渠道列表
                channelListBox.Items.Clear();
                foreach (var channel in _enabledChannels)
                {
                    string modelInfo = channel.SupportedModels.Count > 0 ? 
                        $"将使用模型: {channel.SupportedModels[0]}" : "无可用模型";
                    
                    channelListBox.Items.Add($"{channel.Name} ({channel.ProviderType}) - {modelInfo}", true);
                }
                
                // 更新状态
                lblStatus.Text = $"已加载 {_enabledChannels.Count} 个渠道，请选择要测试的渠道";
                btnTest.Text = "开始批量测试";
                btnTest.Enabled = true;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "加载渠道列表时出错";
                txtLog.Text = $"错误: {ex.Message}\r\n\r\n{ex.StackTrace}";
            }
        }
        
        // 加载单个渠道信息（兼容原有代码）
        private async Task LoadSingleChannelInfo()
        {
            try
            {
                lblStatus.Text = "正在加载渠道信息...";
                btnTest.Enabled = false;
                
                // 隐藏渠道列表
                channelListBox.Visible = false;
                
                // 更新标题
                lblChannelInfo.Text = $"渠道: {_channel.Name} ({_channel.ProviderType})";
                
                // 如果渠道没有模型，尝试获取
                if (_channel.SupportedModels == null || _channel.SupportedModels.Count == 0)
                {
                    try {
                        var models = _channelService.GetChannelModelsAsync(_channel);
                        
                        if (models != null && models.Count > 0)
                        {
                            _channel.SupportedModels = models;
                        }
                    }
                    catch (Exception ex)
                    {
                        txtLog.Text = $"获取模型失败: {ex.Message}";
                    }
                }
                
                // 更新状态
                lblStatus.Text = "渠道信息加载完成";
                btnTest.Text = "测试";
                btnTest.Enabled = true;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "加载渠道信息时出错";
                txtLog.Text = $"错误: {ex.Message}\r\n\r\n{ex.StackTrace}";
            }
        }
        
        private async void btnTest_Click(object sender, EventArgs e)
        {
            if (_channel != null)
            {
                // 单个渠道测试模式
                await TestSingleChannel();
            }
            else
            {
                // 批量测试模式
                await TestSelectedChannels();
            }
        }
        
        // 测试单个渠道
        private async Task TestSingleChannel()
        {
            try
            {
                // 禁用按钮
                btnTest.Enabled = false;
                lblStatus.Text = "正在测试...";
                txtLog.Clear();
                
                // 添加测试信息
                AppendLog("开始测试渠道...");
                AppendLog($"时间: {DateTime.Now}");
                AppendLog("----------------------------------------");
                
                // 选择第一个模型进行测试（如果有）
                string modelToTest = _channel.SupportedModels.Count > 0 ? 
                    _channel.SupportedModels[0] : "未知模型";
                
                AppendLog($"渠道: {_channel.Name}");
                AppendLog($"模型: {modelToTest}");
                AppendLog("使用系统默认测试消息");
                AppendLog("----------------------------------------");
                
                // 执行测试，移除第三个参数
                var result = await _channelService.TestChannelConnectionAsync(_channel, modelToTest);
                
                if (result.success)
                {
                    lblStatus.Text = "测试成功";
                    AppendLog("测试结果: 成功");
                    AppendLog("响应:");
                    AppendLog(result.response);
                }
                else
                {
                    lblStatus.Text = "测试失败";
                    AppendLog("测试结果: 失败");
                    AppendLog("错误信息:");
                    AppendLog(result.response);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "测试时发生错误";
                AppendLog($"错误: {ex.Message}");
                AppendLog(ex.StackTrace);
            }
            finally
            {
                // 恢复按钮状态
                btnTest.Enabled = true;
            }
        }
        
        // 批量测试选中的渠道
        private async Task TestSelectedChannels()
        {
            try
            {
                // 获取选中的渠道索引
                List<int> selectedIndexes = new List<int>();
                for (int i = 0; i < channelListBox.Items.Count; i++)
                {
                    if (channelListBox.GetItemChecked(i))
                    {
                        selectedIndexes.Add(i);
                    }
                }
                
                if (selectedIndexes.Count == 0)
                {
                    MessageBox.Show("请至少选择一个渠道进行测试", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                
                // 禁用按钮和列表
                btnTest.Enabled = false;
                channelListBox.Enabled = false;
                lblStatus.Text = "批量测试中...";
                txtLog.Clear();
                
                AppendLog($"开始批量测试 {selectedIndexes.Count} 个渠道");
                AppendLog($"时间: {DateTime.Now}");
                AppendLog("----------------------------------------");
                
                int successCount = 0;
                int failCount = 0;
                
                // 逐个测试选中的渠道
                for (int i = 0; i < selectedIndexes.Count; i++)
                {
                    int index = selectedIndexes[i];
                    Channel channel = _enabledChannels[index];
                    
                    // 更新状态
                    lblStatus.Text = $"正在测试 {i + 1}/{selectedIndexes.Count}: {channel.Name}";
                    
                    // 添加测试信息
                    AppendLog($"测试渠道 {i + 1}/{selectedIndexes.Count}: {channel.Name} ({channel.ProviderType})");
                    
                    // 选择第一个模型进行测试（如果有）
                    string modelToTest = channel.SupportedModels.Count > 0 ? 
                        channel.SupportedModels[0] : "未知模型";
                    
                    AppendLog($"模型: {modelToTest}");
                    
                    try
                    {
                        // 执行测试，移除第三个参数
                        var result = await _channelService.TestChannelConnectionAsync(channel, modelToTest);
                        
                        if (result.success)
                        {
                            successCount++;
                            AppendLog("测试结果: 成功");
                            AppendLog("响应摘要: " + (result.response.Length > 100 ? result.response.Substring(0, 100) + "..." : result.response));
                        }
                        else
                        {
                            failCount++;
                            AppendLog("测试结果: 失败");
                            AppendLog("错误信息: " + result.response);
                        }
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        AppendLog($"测试出错: {ex.Message}");
                    }
                    
                    AppendLog("----------------------------------------");
                }
                
                // 更新最终状态
                lblStatus.Text = $"测试完成: {successCount} 成功, {failCount} 失败";
                AppendLog($"批量测试完成: {successCount} 个渠道成功, {failCount} 个渠道失败");
                AppendLog($"完成时间: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                lblStatus.Text = "批量测试时发生错误";
                AppendLog($"错误: {ex.Message}");
                AppendLog(ex.StackTrace);
            }
            finally
            {
                // 恢复按钮和列表状态
                btnTest.Enabled = true;
                channelListBox.Enabled = true;
            }
        }
        
        private void AppendLog(string message)
        {
            txtLog.AppendText(message + Environment.NewLine);
        }
    }
} 