using llm_agent.Model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace llm_agent.UI.Controls
{
    public partial class UserCardItem : UserControl
    {
        private User _user;
        private bool _isSelected;
        private bool _isCurrentUser;
        private Color _defaultBackColor = Color.White;
        private Color _hoverBackColor = Color.FromArgb(240, 240, 240);
        private Color _selectedBackColor = Color.FromArgb(230, 230, 250);

        public event EventHandler<User> OnUserSelected;

        public UserCardItem()
        {
            InitializeComponent();
            this.Click += Card_Click;
            this.MouseEnter += UserCardItem_MouseEnter;
            this.MouseLeave += UserCardItem_MouseLeave;
            
            // 将事件处理程序附加到所有子控件
            foreach (Control control in this.Controls)
            {
                control.Click += Card_Click;
                control.MouseEnter += UserCardItem_MouseEnter;
                control.MouseLeave += UserCardItem_MouseLeave;
            }
        }

        public User User
        {
            get => _user;
            set
            {
                _user = value;
                if (_user != null)
                {
                    lblUsername.Text = _user.Username;
                    lblLastLogin.Text = _user.LastLoginAt.HasValue
                        ? $"上次登录: {_user.LastLoginAt.Value:yyyy-MM-dd HH:mm}"
                        : "上次登录: 从未";
                }
            }
        }

        // 修改IsCurrentUser属性，仅设置标签文本，不再影响背景色
        public bool IsCurrentUser
        {
            get => _isCurrentUser;
            set
            {
                _isCurrentUser = value;
                lblStatus.Text = _isCurrentUser ? "当前用户" : "";
            }
        }

        // IsSelected属性控制背景高亮
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                UpdateBackgroundColor();
            }
        }

        private void Card_Click(object sender, EventArgs e)
        {
            OnUserSelected?.Invoke(this, _user);
        }

        private void UserCardItem_MouseEnter(object sender, EventArgs e)
        {
            if (!_isSelected)
            {
                this.BackColor = _hoverBackColor;
            }
        }

        private void UserCardItem_MouseLeave(object sender, EventArgs e)
        {
            UpdateBackgroundColor();
        }

        private void UpdateBackgroundColor()
        {
            this.BackColor = _isSelected ? _selectedBackColor : _defaultBackColor;
        }
    }
} 