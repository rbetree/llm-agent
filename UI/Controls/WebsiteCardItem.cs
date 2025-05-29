using System;
using System.Drawing;
using System.Windows.Forms;
using llm_agent.Model;

namespace llm_agent.UI.Controls
{
    /// <summary>
    /// 网站卡片用户控件
    /// </summary>
    public partial class WebsiteCardItem : UserControl
    {
        private AiWebsite _website;
        private Color _defaultBackColor;
        private Color _hoverBackColor = Color.FromArgb(240, 240, 240);
        private Color _selectedBackColor = Color.FromArgb(230, 240, 250);
        private bool _isSelected = false;

        /// <summary>
        /// 网站被点击时触发
        /// </summary>
        public event EventHandler<WebsiteCardClickEventArgs> WebsiteClicked;

        /// <summary>
        /// 网站被选择时触发
        /// </summary>
        public event EventHandler<WebsiteCardClickEventArgs> WebsiteSelected;

        /// <summary>
        /// 网站被双击时触发
        /// </summary>
        public event EventHandler<WebsiteCardClickEventArgs> WebsiteDoubleClicked;

        /// <summary>
        /// 编辑网站按钮被点击时触发
        /// </summary>
        public event EventHandler<WebsiteCardClickEventArgs> EditWebsiteClicked;

        /// <summary>
        /// 删除网站按钮被点击时触发
        /// </summary>
        public event EventHandler<WebsiteCardClickEventArgs> DeleteWebsiteClicked;

        /// <summary>
        /// 访问网站按钮被点击时触发
        /// </summary>
        public event EventHandler<WebsiteCardClickEventArgs> VisitWebsiteClicked;

        /// <summary>
        /// 网站信息
        /// </summary>
        public AiWebsite Website
        {
            get => _website;
            set
            {
                _website = value;
                UpdateDisplay();
            }
        }

        /// <summary>
        /// 是否选中状态
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                UpdateBackgroundColor();
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WebsiteCardItem()
        {
            InitializeComponent();
            _defaultBackColor = this.BackColor;
            SetupEvents();
        }

        /// <summary>
        /// 设置事件处理
        /// </summary>
        private void SetupEvents()
        {
            // 鼠标进入和离开事件
            this.MouseEnter += WebsiteCardItem_MouseEnter;
            this.MouseLeave += WebsiteCardItem_MouseLeave;
            this.Click += WebsiteCardItem_Click;
            this.DoubleClick += WebsiteCardItem_DoubleClick;

            // 为所有子控件添加相同的事件处理
            foreach (Control control in this.Controls)
            {
                control.MouseEnter += WebsiteCardItem_MouseEnter;
                control.MouseLeave += WebsiteCardItem_MouseLeave;
                control.Click += WebsiteCardItem_Click;
                control.DoubleClick += WebsiteCardItem_DoubleClick;
            }
        }

        /// <summary>
        /// 更新显示内容
        /// </summary>
        private void UpdateDisplay()
        {
            if (_website == null) return;

            // 更新网站名称
            if (lblWebsiteName != null)
            {
                lblWebsiteName.Text = _website.DisplayName;
            }



            // 更新网站URL
            if (lblWebsiteUrl != null)
            {
                lblWebsiteUrl.Text = _website.Url;
            }



            // 设置工具提示
            var tooltip = new ToolTip();
            tooltip.SetToolTip(this, $"{_website.DisplayName}\n{_website.Url}");
        }

        /// <summary>
        /// 更新背景颜色
        /// </summary>
        private void UpdateBackgroundColor()
        {
            if (_isSelected)
            {
                this.BackColor = _selectedBackColor;
            }
            else
            {
                this.BackColor = _defaultBackColor;
            }
        }

        /// <summary>
        /// 鼠标进入事件
        /// </summary>
        private void WebsiteCardItem_MouseEnter(object sender, EventArgs e)
        {
            if (!_isSelected)
            {
                this.BackColor = _hoverBackColor;
            }
        }

        /// <summary>
        /// 鼠标离开事件
        /// </summary>
        private void WebsiteCardItem_MouseLeave(object sender, EventArgs e)
        {
            UpdateBackgroundColor();
        }

        /// <summary>
        /// 点击事件
        /// </summary>
        private void WebsiteCardItem_Click(object sender, EventArgs e)
        {
            WebsiteClicked?.Invoke(this, new WebsiteCardClickEventArgs(_website));
            WebsiteSelected?.Invoke(this, new WebsiteCardClickEventArgs(_website));
        }

        /// <summary>
        /// 双击事件
        /// </summary>
        private void WebsiteCardItem_DoubleClick(object sender, EventArgs e)
        {
            WebsiteDoubleClicked?.Invoke(this, new WebsiteCardClickEventArgs(_website));
            VisitWebsiteClicked?.Invoke(this, new WebsiteCardClickEventArgs(_website));
        }

        /// <summary>
        /// 编辑按钮点击事件
        /// </summary>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditWebsiteClicked?.Invoke(this, new WebsiteCardClickEventArgs(_website));
        }

        /// <summary>
        /// 删除按钮点击事件
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteWebsiteClicked?.Invoke(this, new WebsiteCardClickEventArgs(_website));
        }

        /// <summary>
        /// 访问按钮点击事件
        /// </summary>
        private void btnVisit_Click(object sender, EventArgs e)
        {
            VisitWebsiteClicked?.Invoke(this, new WebsiteCardClickEventArgs(_website));
        }
    }

    /// <summary>
    /// 网站卡片点击事件参数
    /// </summary>
    public class WebsiteCardClickEventArgs : EventArgs
    {
        public AiWebsite Website { get; }

        public WebsiteCardClickEventArgs(AiWebsite website)
        {
            Website = website;
        }
    }
}
