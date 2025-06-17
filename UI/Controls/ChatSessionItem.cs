using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using llm_agent.Model;

namespace llm_agent.UI.Controls
{
    public partial class ChatSessionItem : UserControl
    {
        private ChatSession _session;
        private bool _isDragging = false;
        private Point _dragStartPoint;
        private Color _defaultBackColor;
        private Color _hoverBackColor = Color.FromArgb(240, 240, 240);
        private Color _selectedBackColor = Color.FromArgb(230, 230, 250);
        private bool _isSelected = false;

        public event EventHandler<ChatSession> OnSessionSelected;
        public event EventHandler<ChatSession> OnSessionDeleted;

        // 默认构造函数
        public ChatSessionItem()
        {
            InitializeComponent();
            _defaultBackColor = BackColor;
            SetupEvents();
        }

        // 调整内部控件大小和位置
        private void AdjustControlSizes()
        {
            if (this.ClientSize.Width <= 0) return;

            // 计算DPI缩放因子
            float dpiScaleFactor = this.CreateGraphics().DpiX / 96f;

            // 调整标题标签高度
            lblTitle.Height = (int)(30 * dpiScaleFactor);

            // 调整预览标签高度和位置
            lblPreview.Height = (int)(18 * dpiScaleFactor);
            lblPreview.Width = this.ClientSize.Width - 10; // 考虑Padding
            lblPreview.Location = new Point(5, lblTitle.Bottom);

            // 调整时间标签高度
            lblTime.Height = (int)(20 * dpiScaleFactor);
        }

        // 设置事件
        private void SetupEvents()
        {
            this.Click += (s, e) => OnSessionSelected?.Invoke(this, _session);
            this.MouseDown += ChatSessionItem_MouseDown;
            this.MouseMove += ChatSessionItem_MouseMove;
            this.MouseUp += ChatSessionItem_MouseUp;
            this.MouseEnter += ChatSessionItem_MouseEnter;
            this.MouseLeave += ChatSessionItem_MouseLeave;

            // 添加大小变化事件
            this.SizeChanged += (s, e) => AdjustControlSizes();

            // 递归为所有子控件绑定点击事件
            BindClickEventsRecursively(this);
        }

        // 鼠标事件处理
        private void ChatSessionItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                _dragStartPoint = e.Location;
            }
        }

        private void ChatSessionItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                if (Math.Abs(e.X - _dragStartPoint.X) > 5 || Math.Abs(e.Y - _dragStartPoint.Y) > 5)
                {
                    this.DoDragDrop(this, DragDropEffects.Move);
                    _isDragging = false;
                }
            }
        }

        private void ChatSessionItem_MouseUp(object sender, MouseEventArgs e)
        {
            _isDragging = false;
        }

        // 鼠标进入事件处理
        private void ChatSessionItem_MouseEnter(object sender, EventArgs e)
        {
            if (!_isSelected)
            {
                BackColor = _hoverBackColor;
            }
        }

        // 鼠标离开事件处理
        private void ChatSessionItem_MouseLeave(object sender, EventArgs e)
        {
            UpdateBackgroundColor();
        }

        // 更新背景颜色
        private void UpdateBackgroundColor()
        {
            BackColor = _isSelected ? _selectedBackColor : _defaultBackColor;
        }

        // 属性
        [Browsable(false)]
        public ChatSession Session
        {
            get { return _session; }
            set
            {
                _session = value;
                UpdateDisplay();
            }
        }

        // 是否选中
        [Browsable(false)]
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                UpdateBackgroundColor();
            }
        }

        // 更新显示
        private void UpdateDisplay()
        {
            if (_session == null) return;

            lblTitle.Text = _session.Title ?? "新会话";

            if (_session.Messages.Count > 0)
            {
                var lastMessage = _session.Messages.Last();
                lblPreview.Text = TruncateText(lastMessage.Content, 20);
            }
            else
            {
                lblPreview.Text = "无消息";
            }

            lblTime.Text = _session.UpdatedAt.ToString("MM-dd HH:mm");

            // 调整控件大小
            AdjustControlSizes();

            // 确保事件绑定正确（防止动态更新时丢失事件绑定）
            BindClickEventsRecursively(this);
        }

        // 文本截断
        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
        }

        // 删除会话
        private void DeleteSession(object sender, EventArgs e)
        {
            OnSessionDeleted?.Invoke(this, _session);
        }

        /// <summary>
        /// 递归为所有子控件绑定点击事件
        /// </summary>
        /// <param name="parent">父控件</param>
        private void BindClickEventsRecursively(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                // 为子控件绑定点击事件
                control.Click -= (s, e) => OnSessionSelected?.Invoke(this, _session); // 先解除绑定，避免重复
                control.Click += (s, e) => OnSessionSelected?.Invoke(this, _session);

                // 递归处理子控件的子控件
                if (control.HasChildren)
                {
                    BindClickEventsRecursively(control);
                }
            }
        }
    }
}