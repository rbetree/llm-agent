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
                lblChannelInfo.Text = $"发现 {_enabledChannels.Count} 个启用的渠道:";
                
                // 清空现有内容并添加标题
                txtChannelInfo.Clear();
                txtChannelInfo.AppendText($"共有 {_enabledChannels.Count} 个启用的渠道可供测试\r\n");
                txtChannelInfo.AppendText("每个渠道将使用其第一个可用模型进行联通测试\r\n\r\n");
                
                // 填充渠道列表
                channelListBox.Items.Clear();
                foreach (var channel in _enabledChannels)
                {
                    string modelInfo = channel.SupportedModels.Count > 0 ? 
                        $"将使用模型: {channel.SupportedModels[0]}" : "无可用模型";
                    
                    channelListBox.Items.Add($"{channel.Name} ({channel.ProviderType}) - {modelInfo}", true);
                    
                    // 在信息文本框中添加渠道信息
                    txtChannelInfo.AppendText($"- {channel.Name} ({channel.ProviderType})\r\n");
                    txtChannelInfo.AppendText($"  API主机: {channel.ApiHost}\r\n");
                    txtChannelInfo.AppendText($"  {modelInfo}\r\n\r\n");
                }
                
                // 更新状态
                lblStatus.Text = $"已加载 {_enabledChannels.Count} 个渠道，请选择要测试的渠道";
                btnTest.Text = "开始批量测试";
                btnTest.Enabled = true;
                
                // 隐藏单渠道测试的控件
                lblModel.Visible = false;
                cboModel.Visible = false;
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
                
                // 加载渠道信息
                txtChannelInfo.Text = $"渠道: {_channel.Name}\r\n";
                txtChannelInfo.Text += $"类型: {_channel.ProviderType}\r\n";
                txtChannelInfo.Text += $"API主机: {_channel.ApiHost}\r\n";
                txtChannelInfo.Text += $"使用流式响应: {(_channel.UseStreamResponse ? "是" : "否")}\r\n";
                
                // 隐藏渠道列表
                channelListBox.Visible = false;
                
                // 如果渠道没有模型，尝试获取
                if (_channel.SupportedModels == null || _channel.SupportedModels.Count == 0)
                {
                    txtChannelInfo.Text += "正在获取渠道模型...\r\n";
                    
                    try {
                        var models = _channelService.GetChannelModelsAsync(_channel);
                        
                        if (models != null && models.Count > 0)
                        {
                            _channel.SupportedModels = models;
                            txtChannelInfo.Text += $"可用模型数量: {models.Count}\r\n";
                        }
                        else
                        {
                            txtChannelInfo.Text += "未能获取到可用模型\r\n";
                        }
                    }
                    catch (Exception ex)
                    {
                        txtChannelInfo.Text += $"获取模型失败: {ex.Message}\r\n";
                    }
                }
                else
                {
                    txtChannelInfo.Text += $"可用模型数量: {_channel.SupportedModels.Count}\r\n";
                }
                
                // 更新状态
                lblStatus.Text = "渠道信息加载完成";
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
                
                // 执行测试
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
                
                // 添加测试信息
                AppendLog($"开始批量测试 {selectedIndexes.Count} 个渠道...");
                AppendLog($"时间: {DateTime.Now}");
                AppendLog("========================================");
                
                // 计数器
                int successCount = 0;
                int failCount = 0;
                
                // 依次测试每个选中的渠道
                for (int i = 0; i < selectedIndexes.Count; i++)
                {
                    int index = selectedIndexes[i];
                    var channel = _enabledChannels[index];
                    
                    // 选择要测试的模型 - 使用第一个可用模型
                    string modelToTest = channel.SupportedModels.Count > 0 ? 
                        channel.SupportedModels[0] : "default-model";
                    
                    // 更新状态栏
                    lblStatus.Text = $"正在测试 ({i+1}/{selectedIndexes.Count}): {channel.Name}";
                    
                    // 添加测试日志
                    AppendLog($"测试渠道 {i+1}/{selectedIndexes.Count}: {channel.Name}");
                    AppendLog($"模型: {modelToTest}");
                    AppendLog("----------------------------------------");
                    
                    // 执行测试
                    try
                    {
                        var result = await _channelService.TestChannelConnectionAsync(channel, modelToTest);
                        
                        if (result.success)
                        {
                            successCount++;
                            AppendLog("测试结果: 成功");
                            AppendLog($"响应: {result.response.Substring(0, Math.Min(100, result.response.Length))}...");
                        }
                        else
                        {
                            failCount++;
                            AppendLog("测试结果: 失败");
                            AppendLog($"错误信息: {result.response}");
                        }
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        AppendLog("测试结果: 异常");
                        AppendLog($"错误: {ex.Message}");
                    }
                    
                    AppendLog("----------------------------------------");
                }
                
                // 测试完成，显示汇总信息
                AppendLog("========================================");
                AppendLog($"测试完成! 成功: {successCount}, 失败: {failCount}");
                lblStatus.Text = $"批量测试完成 - 成功: {successCount}, 失败: {failCount}";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "测试过程中发生错误";
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
        
        // 添加日志的辅助方法
        private void AppendLog(string message)
        {
            txtLog.AppendText(message + Environment.NewLine);
        }
    }
} 