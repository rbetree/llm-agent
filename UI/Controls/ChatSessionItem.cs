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
        private Label _titleLabel;
        private Label _previewLabel;
        private Label _timeLabel;
        private ContextMenuStrip _contextMenu;
        private bool _isDragging = false;
        private Point _dragStartPoint;

        public event EventHandler<ChatSession> OnSessionSelected;
        public event EventHandler<ChatSession> OnSessionDeleted;

        // 默认构造函数
        public ChatSessionItem()
        {
            InitializeComponent();
            SetupEvents();
        }

        // 调整内部控件大小和位置
        private void AdjustControlSizes()
        {
            if (this.ClientSize.Width <= 0) return;

            // 创建标题标签
            _titleLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold),
                ForeColor = Color.Black,
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true,
                Enabled = true
            };

            // 创建预览标签
            _previewLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.None,
                Font = new Font("Microsoft YaHei UI", 9F),
                ForeColor = Color.Gray,
                Height = 30,
                AutoEllipsis = true,
                Enabled = false
            };

            // 创建时间标签
            _timeLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Bottom,
                Font = new Font("Microsoft YaHei UI", 8F),
                ForeColor = Color.DarkGray,
                Height = 20,
                TextAlign = ContentAlignment.MiddleRight,
                Enabled = false
            };

            // 添加控件到面板
            this.Controls.Add(_timeLabel);
            this.Controls.Add(_previewLabel);
            this.Controls.Add(_titleLabel);

            // 设置预览标签的位置
            _previewLabel.Location = new Point(10, _titleLabel.Bottom);
            _previewLabel.Width = this.ClientSize.Width - 20; // 考虑Padding

            // 创建右键菜单
            _contextMenu = new ContextMenuStrip();
            var deleteItem = new ToolStripMenuItem("删除会话");
            deleteItem.Click += DeleteSession;
            _contextMenu.Items.Add(deleteItem);
            this.ContextMenuStrip = _contextMenu;

            // 添加大小变化事件
            this.SizeChanged += (s, e) => AdjustControlSizes();
        }

        // 调整内部控件大小和位置
        private void AdjustControlSizes()
        {
            if (this.ClientSize.Width <= 0) return;

            // 计算DPI缩放因子
            float dpiScaleFactor = this.CreateGraphics().DpiX / 96f;
            
            // 调整标题标签高度
            _titleLabel.Height = (int)(30 * dpiScaleFactor);
            
            // 调整预览标签高度和位置
            _previewLabel.Height = (int)(30 * dpiScaleFactor);
            _previewLabel.Width = this.ClientSize.Width - 20; // 考虑Padding
            _previewLabel.Location = new Point(10, _titleLabel.Bottom);
            
            // 调整时间标签高度
            _timeLabel.Height = (int)(20 * dpiScaleFactor);
        }

        // 设置事件
        private void SetupEvents()
        {
            this.Click += (s, e) => OnSessionSelected?.Invoke(this, _session);
            this.MouseDown += ChatSessionItem_MouseDown;
            this.MouseMove += ChatSessionItem_MouseMove;
            this.MouseUp += ChatSessionItem_MouseUp;

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

        // 更新显示
        private void UpdateDisplay()
        {
            if (_session == null) return;

            lblTitle.Text = _session.Title ?? "新会话";

            if (_session.Messages.Count > 0)
            {
                var lastMessage = _session.Messages.Last();
                _previewLabel.Text = TruncateText(lastMessage.Content, 25);
            }
            else
            {
                _previewLabel.Text = "无消息";
            }
            
            _timeLabel.Text = _session.UpdatedAt.ToString("MM-dd HH:mm");
            
            // 调整控件大小
            AdjustControlSizes();
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