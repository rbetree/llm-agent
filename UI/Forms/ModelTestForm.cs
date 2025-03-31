using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using llm_agent.API.Provider;
using llm_agent.Model;
using llm_agent.BLL;

namespace llm_agent.UI.Forms
{
    public partial class ModelTestForm : Form
    {
        private readonly Channel _channel;
        private readonly string _model;
        private readonly ChannelService _channelService;
        
        // 用于性能测试的变量
        private Stopwatch _stopwatch;
        private DateTime _startTime;
        private DateTime _firstTokenTime;
        private bool _receivedFirstToken = false;
        private int _totalTokens = 0;
        private int _totalCharacters = 0;
        
        public ModelTestForm(Channel channel, string model)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _channelService = new ChannelService(new System.Net.Http.HttpClient());
            _stopwatch = new Stopwatch();
            
            InitializeComponent();
            
            // 设置窗体标题
            this.Text = $"模型性能测试 - {model}";
            
            // 更新模型信息标签
            lblModelInfo.Text = $"渠道: {channel.Name} ({channel.ProviderType})  |  模型: {model}";
            
            // 添加默认测试提示词
            txtPrompt.Text = "你好，请简要介绍一下自己，并解释一下大语言模型的工作原理。然后用一个比喻来说明大语言模型的自注意力机制。最后，请给出一个能够看出你思维能力的数学问题的解答。";
        }

        private async void btnRunTest_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPrompt.Text))
            {
                MessageBox.Show("请输入提示词", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // 重置UI和性能测试数据
                ResetPerformanceData();
                
                // 开始计时
                _stopwatch.Restart();
                _startTime = DateTime.Now;
                
                // 准备测试环境
                btnRunTest.Enabled = false;
                txtOutput.Clear();
                lblResponseTime.Text = "响应中...";
                lblOutputSpeed.Text = "计算中...";
                progressBar.Style = ProgressBarStyle.Marquee;
                progressBar.Visible = true;
                
                // 如果使用流式响应
                if (chkStreamResponse.Checked)
                {
                    _channel.UseStreamResponse = true;
                    
                    string userInput = txtPrompt.Text.Trim();
                    var messages = new List<ChatMessage>
                    {
                        new ChatMessage { Role = ChatRole.User, Content = userInput }
                    };
                    
                    await _channelService.SendStreamMessageAsync(
                        _channel,
                        _model,
                        messages,
                        (chunk) => 
                        {
                            // 检查是否是第一个token
                            if (!_receivedFirstToken)
                            {
                                _receivedFirstToken = true;
                                _firstTokenTime = DateTime.Now;
                                
                                // 在UI线程上更新首次响应时间
                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new Action(() => {
                                        TimeSpan responseTime = _firstTokenTime - _startTime;
                                        lblResponseTime.Text = $"首次响应时间: {responseTime.TotalSeconds:F2} 秒";
                                    }));
                                }
                                else
                                {
                                    TimeSpan responseTime = _firstTokenTime - _startTime;
                                    lblResponseTime.Text = $"首次响应时间: {responseTime.TotalSeconds:F2} 秒";
                                }
                            }
                            
                            // 计算字符数
                            _totalCharacters += chunk.Length;
                            
                            // 估算token数（粗略估计，每个token约为4个字符）
                            _totalTokens = _totalCharacters / 4;
                            
                            // 更新UI
                            if (this.InvokeRequired)
                            {
                                this.Invoke(new Action(() => {
                                    txtOutput.AppendText(chunk);
                                    txtOutput.SelectionStart = txtOutput.TextLength;
                                    txtOutput.ScrollToCaret();
                                    
                                    // 更新输出速度
                                    double elapsedSeconds = _stopwatch.ElapsedMilliseconds / 1000.0;
                                    if (elapsedSeconds > 0)
                                    {
                                        double charsPerSecond = _totalCharacters / elapsedSeconds;
                                        double tokensPerSecond = _totalTokens / elapsedSeconds;
                                        lblOutputSpeed.Text = $"输出速度: {charsPerSecond:F1} 字符/秒 ({tokensPerSecond:F1} token/秒)";
                                    }
                                }));
                            }
                            else
                            {
                                txtOutput.AppendText(chunk);
                                txtOutput.SelectionStart = txtOutput.TextLength;
                                txtOutput.ScrollToCaret();
                                
                                // 更新输出速度
                                double elapsedSeconds = _stopwatch.ElapsedMilliseconds / 1000.0;
                                if (elapsedSeconds > 0)
                                {
                                    double charsPerSecond = _totalCharacters / elapsedSeconds;
                                    double tokensPerSecond = _totalTokens / elapsedSeconds;
                                    lblOutputSpeed.Text = $"输出速度: {charsPerSecond:F1} 字符/秒 ({tokensPerSecond:F1} token/秒)";
                                }
                            }
                        }
                    );
                }
                else
                {
                    _channel.UseStreamResponse = false;
                    
                    string userInput = txtPrompt.Text.Trim();
                    var messages = new List<ChatMessage>
                    {
                        new ChatMessage { Role = ChatRole.User, Content = userInput }
                    };
                    
                    var response = await _channelService.SendMessageAsync(
                        _channel,
                        _model,
                        messages
                    );
                    
                    // 记录结束时间
                    DateTime endTime = DateTime.Now;
                    _receivedFirstToken = true;
                    _firstTokenTime = endTime; // 非流式响应中，首次响应即为完整响应
                    
                    // 计算字符数
                    _totalCharacters = response.Length;
                    _totalTokens = _totalCharacters / 4; // 估算token数
                    
                    // 更新结果
                    txtOutput.Text = response;
                    
                    // 计算响应时间
                    TimeSpan responseTime = endTime - _startTime;
                    lblResponseTime.Text = $"响应时间: {responseTime.TotalSeconds:F2} 秒";
                    
                    // 计算输出速度
                    double elapsedSeconds = responseTime.TotalSeconds;
                    if (elapsedSeconds > 0)
                    {
                        double charsPerSecond = _totalCharacters / elapsedSeconds;
                        double tokensPerSecond = _totalTokens / elapsedSeconds;
                        lblOutputSpeed.Text = $"平均速度: {charsPerSecond:F1} 字符/秒 ({tokensPerSecond:F1} token/秒)";
                    }
                }
                
                // 测试完成后的UI更新
                _stopwatch.Stop();
                btnRunTest.Enabled = true;
                progressBar.Style = ProgressBarStyle.Blocks;
                progressBar.Visible = false;
                
                // 显示总结性能数据
                DisplayPerformanceSummary();
            }
            catch (Exception ex)
            {
                _stopwatch.Stop();
                btnRunTest.Enabled = true;
                progressBar.Style = ProgressBarStyle.Blocks;
                progressBar.Visible = false;
                
                txtOutput.Text = $"错误: {ex.Message}\r\n\r\n{ex.StackTrace}";
                lblResponseTime.Text = "测试失败";
                lblOutputSpeed.Text = "测试失败";
            }
        }
        
        private void ResetPerformanceData()
        {
            _stopwatch.Reset();
            _receivedFirstToken = false;
            _totalTokens = 0;
            _totalCharacters = 0;
        }
        
        private void DisplayPerformanceSummary()
        {
            TimeSpan totalTime = DateTime.Now - _startTime;
            TimeSpan ttft = _firstTokenTime - _startTime; // Time To First Token
            
            StringBuilder summary = new StringBuilder();
            summary.AppendLine("\r\n\r\n========= 性能测试汇总 =========");
            summary.AppendLine($"渠道: {_channel.Name} ({_channel.ProviderType})");
            summary.AppendLine($"模型: {_model}");
            summary.AppendLine($"测试时间: {_startTime:yyyy-MM-dd HH:mm:ss}");
            summary.AppendLine($"响应模式: {(chkStreamResponse.Checked ? "流式" : "非流式")}");
            summary.AppendLine("-----------------------------------");
            summary.AppendLine($"首次响应时间(TTFT): {ttft.TotalSeconds:F2} 秒");
            summary.AppendLine($"总响应时间: {totalTime.TotalSeconds:F2} 秒");
            summary.AppendLine($"输出总字符数: {_totalCharacters} 字符");
            summary.AppendLine($"估算Token数: ~{_totalTokens} tokens");
            
            double avgSpeed = _totalCharacters / totalTime.TotalSeconds;
            double avgTokenSpeed = _totalTokens / totalTime.TotalSeconds;
            
            summary.AppendLine($"平均输出速度: {avgSpeed:F1} 字符/秒 ({avgTokenSpeed:F1} token/秒)");
            summary.AppendLine("================================");
            
            txtOutput.AppendText(summary.ToString());
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
} 