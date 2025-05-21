using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using llm_agent.Model;

namespace llm_agent.UI.Controls.SimpleMessageDisplay
{
    /// <summary>
    /// 简化版消息显示控件，基于RichTextBox实现，用于测试表单中替代原有控件
    /// 支持ChatMessage格式化显示和流式响应更新
    /// </summary>
    public partial class SimpleMessageDisplay : UserControl
    {
        private RichTextBox txtOutput;
        private ContextMenuStrip contextMenu;
        
        /// <summary>
        /// 获取内部的RichTextBox控件
        /// </summary>
        public RichTextBox OutputTextBox => txtOutput;
        
        /// <summary>
        /// 获取或设置文本内容
        /// </summary>
        public string Text 
        {
            get { return txtOutput.Text; }
            set { txtOutput.Text = value; }
        }
        
        /// <summary>
        /// 获取文本长度
        /// </summary>
        public int TextLength => txtOutput.TextLength;
        
        /// <summary>
        /// 消息总字符数，用于性能测试
        /// </summary>
        public int TotalCharacters { get; private set; } = 0;

        public SimpleMessageDisplay()
        {
            InitializeComponent();
            InitializeContextMenu();
        }
        
        /// <summary>
        /// 初始化右键菜单
        /// </summary>
        private void InitializeContextMenu()
        {
            contextMenu = new ContextMenuStrip();
            
            // 添加复制菜单项
            ToolStripMenuItem copyMenuItem = new ToolStripMenuItem("复制");
            copyMenuItem.Click += (sender, e) => Copy();
            contextMenu.Items.Add(copyMenuItem);
            
            // 添加全选菜单项
            ToolStripMenuItem selectAllMenuItem = new ToolStripMenuItem("全选");
            selectAllMenuItem.Click += (sender, e) => SelectAll();
            contextMenu.Items.Add(selectAllMenuItem);
            
            // 添加清除菜单项
            ToolStripMenuItem clearMenuItem = new ToolStripMenuItem("清除");
            clearMenuItem.Click += (sender, e) => Clear();
            contextMenu.Items.Add(clearMenuItem);
            
            // 设置右键菜单
            txtOutput.ContextMenuStrip = contextMenu;
        }

        /// <summary>
        /// 添加格式化消息
        /// </summary>
        /// <param name="message">消息对象</param>
        /// <param name="includeRolePrefix">是否包含角色前缀</param>
        public void AppendFormattedMessage(ChatMessage message, bool includeRolePrefix = true)
        {
            if (message == null)
                return;

            Color textColor = GetRoleColor(message.Role);
            string rolePrefix = includeRolePrefix ? $"[{message.Role}]: " : string.Empty;
            
            // 添加时间戳和角色前缀
            AppendText($"{message.CreatedAt:yyyy-MM-dd HH:mm:ss} ", Color.Gray);
            AppendText(rolePrefix, textColor);
            
            // 添加消息内容
            AppendText(message.Content + Environment.NewLine, Color.Black);
            
            // 更新总字符数（用于性能测试）
            TotalCharacters += message.Content.Length;
            
            // 滚动到最后
            txtOutput.ScrollToCaret();
        }
        
        /// <summary>
        /// 追加文本（不标记为特定角色或时间）
        /// </summary>
        /// <param name="text">文本内容</param>
        public void AppendText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
                
            txtOutput.AppendText(text);
            TotalCharacters += text.Length;
            txtOutput.ScrollToCaret();
        }

        /// <summary>
        /// 更新最后一条助手消息的内容（用于流式响应）
        /// </summary>
        /// <param name="content">新的消息内容</param>
        /// <returns>更新的字符数（用于性能测试）</returns>
        public int UpdateLastAssistantMessageContent(string content)
        {
            // 查找最后一条助手消息的位置
            string text = txtOutput.Text;
            int lastAssistantIndex = text.LastIndexOf("[assistant]:", StringComparison.OrdinalIgnoreCase);
            
            if (lastAssistantIndex < 0)
                return 0;
            
            // 找到消息内容开始的位置
            int contentStartIndex = text.IndexOf(':', lastAssistantIndex) + 1;
            while (contentStartIndex < text.Length && char.IsWhiteSpace(text[contentStartIndex]))
                contentStartIndex++;
            
            // 找到该行结束的位置（考虑可能存在的多行内容）
            int lineEndIndex = text.IndexOf(Environment.NewLine, contentStartIndex);
            if (lineEndIndex < 0)
                lineEndIndex = text.Length;
            
            // 计算需要替换的长度
            int replaceLength = lineEndIndex - contentStartIndex;
            
            // 保存当前选择和光标位置
            int originalSelectionStart = txtOutput.SelectionStart;
            int originalSelectionLength = txtOutput.SelectionLength;
            
            // 替换内容
            txtOutput.Select(contentStartIndex, replaceLength);
            string oldContent = txtOutput.SelectedText;
            txtOutput.SelectedText = " " + content;
            
            // 计算新增的字符数
            int added = content.Length - oldContent.Length + 1; // +1 是因为添加了一个空格
            TotalCharacters += added;
            
            // 如果原来的选择在修改区域之后，调整选择位置
            if (originalSelectionStart > contentStartIndex)
            {
                int adjustment = content.Length + 1 - replaceLength;
                txtOutput.Select(originalSelectionStart + adjustment, originalSelectionLength);
            }
            else
            {
                // 否则恢复原来的选择
                txtOutput.Select(originalSelectionStart, originalSelectionLength);
            }
            
            // 滚动到最后
            txtOutput.ScrollToCaret();
            
            return added;
        }

        /// <summary>
        /// 清空显示内容
        /// </summary>
        public void Clear()
        {
            txtOutput.Clear();
            TotalCharacters = 0;
        }
        
        /// <summary>
        /// 复制选中内容到剪贴板
        /// </summary>
        public void Copy()
        {
            if (txtOutput.SelectionLength > 0)
            {
                txtOutput.Copy();
            }
            else if (txtOutput.TextLength > 0)
            {
                // 如果没有选中内容，则复制全部
                Clipboard.SetText(txtOutput.Text);
            }
        }
        
        /// <summary>
        /// 全选
        /// </summary>
        public void SelectAll()
        {
            txtOutput.SelectAll();
        }
        
        /// <summary>
        /// 添加带颜色的文本
        /// </summary>
        private void AppendText(string text, Color color)
        {
            txtOutput.SelectionStart = txtOutput.TextLength;
            txtOutput.SelectionLength = 0;
            txtOutput.SelectionColor = color;
            txtOutput.AppendText(text);
            txtOutput.SelectionColor = txtOutput.ForeColor;
        }

        /// <summary>
        /// 根据角色获取对应的颜色
        /// </summary>
        private Color GetRoleColor(ChatRole role)
        {
            return role switch
            {
                ChatRole.System => Color.Purple,
                ChatRole.Assistant => Color.Blue,
                ChatRole.User => Color.Green,
                _ => Color.Black
            };
        }
        
        /// <summary>
        /// 在文本结尾添加字符（用于流式测试）
        /// </summary>
        /// <param name="chunk">文本片段</param>
        public void AppendTextToEnd(string chunk)
        {
            if (string.IsNullOrEmpty(chunk))
                return;
                
            txtOutput.AppendText(chunk);
            txtOutput.SelectionStart = txtOutput.TextLength;
            txtOutput.ScrollToCaret();
            
            // 更新总字符数
            TotalCharacters += chunk.Length;
        }
        
        /// <summary>
        /// 重置字符计数
        /// </summary>
        public void ResetCharacterCount()
        {
            TotalCharacters = 0;
        }

        /// <summary>
        /// 设置消息总字符数（用于性能测试）
        /// </summary>
        /// <param name="totalChars">字符总数</param>
        public void SetTotalCharacters(int totalChars)
        {
            TotalCharacters = totalChars;
        }
    }
} 