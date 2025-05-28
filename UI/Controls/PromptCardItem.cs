using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using llm_agent.Model;

namespace llm_agent.UI.Controls
{
    /// <summary>
    /// 提示词卡片用户控件
    /// </summary>
    public partial class PromptCardItem : UserControl
    {
        private Prompt _prompt;
        private Color _defaultBackColor;
        private Color _hoverBackColor = Color.FromArgb(240, 240, 240);
        private Color _selectedBackColor = Color.FromArgb(230, 240, 250);
        private bool _isSelected = false;

        /// <summary>
        /// 提示词被点击时触发
        /// </summary>
        public event EventHandler<PromptCardClickEventArgs> PromptClicked;

        /// <summary>
        /// 提示词被选择时触发
        /// </summary>
        public event EventHandler<PromptCardClickEventArgs> PromptSelected;

        /// <summary>
        /// 提示词被双击时触发
        /// </summary>
        public event EventHandler<PromptCardClickEventArgs> PromptDoubleClicked;

        /// <summary>
        /// 提示词使用按钮被点击时触发
        /// </summary>
        public event EventHandler<PromptCardClickEventArgs> UsePromptClicked;

        /// <summary>
        /// 获取或设置当前提示词
        /// </summary>
        [Browsable(false)]
        public Prompt Prompt
        {
            get { return _prompt; }
            set
            {
                _prompt = value;
                UpdateDisplay();
            }
        }

        /// <summary>
        /// 获取或设置是否被选中
        /// </summary>
        [Browsable(false)]
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                BackColor = _isSelected ? _selectedBackColor : _defaultBackColor;
            }
        }

        /// <summary>
        /// 初始化提示词卡片控件
        /// </summary>
        public PromptCardItem()
        {
            InitializeComponent();
            _defaultBackColor = BackColor;
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            
            // 注册事件处理
            this.MouseEnter += PromptCardItem_MouseEnter;
            this.MouseLeave += PromptCardItem_MouseLeave;
            
            // 确保点击事件能够正确触发
            foreach (Control control in this.Controls)
            {
                // 为所有子控件添加点击事件处理
                control.Click += PromptCardItem_Click;
            }
            
            this.Click += PromptCardItem_Click;
            this.DoubleClick += PromptCardItem_DoubleClick;
            
            // 设置控件可接收焦点
            this.SetStyle(ControlStyles.Selectable, true);
            this.TabStop = true;
        }

        /// <summary>
        /// 使用提示词对象初始化卡片
        /// </summary>
        /// <param name="prompt">提示词对象</param>
        public PromptCardItem(Prompt prompt) : this()
        {
            _prompt = prompt;
            UpdateDisplay();
        }

        /// <summary>
        /// 更新卡片显示内容
        /// </summary>
        private void UpdateDisplay()
        {
            if (_prompt == null) return;

            lblTitle.Text = _prompt.Title;
            lblCategory.Text = _prompt.Category;
            lblUsageCount.Text = $"使用次数: {_prompt.UsageCount}";
            
            // 设置提示词内容的提示信息
            string contentPreview = _prompt.Content.Length > 100 
                ? _prompt.Content.Substring(0, 100) + "..." 
                : _prompt.Content;
            
            toolTip.SetToolTip(this, contentPreview);
        }

        /// <summary>
        /// 鼠标进入事件处理
        /// </summary>
        private void PromptCardItem_MouseEnter(object sender, EventArgs e)
        {
            if (!_isSelected)
            {
                BackColor = _hoverBackColor;
            }
        }

        /// <summary>
        /// 鼠标离开事件处理
        /// </summary>
        private void PromptCardItem_MouseLeave(object sender, EventArgs e)
        {
            if (!_isSelected)
            {
                BackColor = _defaultBackColor;
            }
        }

        /// <summary>
        /// 鼠标点击事件处理
        /// </summary>
        private void PromptCardItem_Click(object sender, EventArgs e)
        {
            // 添加调试输出
            Console.WriteLine($"PromptCardItem_Click: 提示词 {_prompt?.Title} 被点击");
            
            // 触发点击事件
            PromptClicked?.Invoke(this, new PromptCardClickEventArgs(_prompt));
            
            // 设置选中状态
            IsSelected = true;
            
            // 触发选择事件
            PromptSelected?.Invoke(this, new PromptCardClickEventArgs(_prompt));
        }

        /// <summary>
        /// 鼠标双击事件处理
        /// </summary>
        private void PromptCardItem_DoubleClick(object sender, EventArgs e)
        {
            PromptDoubleClicked?.Invoke(this, new PromptCardClickEventArgs(_prompt));
        }

        /// <summary>
        /// 使用按钮点击事件处理
        /// </summary>
        private void btnUse_Click(object sender, EventArgs e)
        {
            UsePromptClicked?.Invoke(this, new PromptCardClickEventArgs(_prompt));
        }
    }

    /// <summary>
    /// 提示词卡片点击事件参数
    /// </summary>
    public class PromptCardClickEventArgs : EventArgs
    {
        /// <summary>
        /// 关联的提示词
        /// </summary>
        public Prompt Prompt { get; private set; }

        /// <summary>
        /// 初始化事件参数
        /// </summary>
        /// <param name="prompt">提示词对象</param>
        public PromptCardClickEventArgs(Prompt prompt)
        {
            Prompt = prompt;
        }
    }
} 