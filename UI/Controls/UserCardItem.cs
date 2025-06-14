using llm_agent.Model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace llm_agent.UI.Controls
{
    public partial class UserCardItem : UserControl
    {
        private User _user;
        private bool _isCurrentUser;

        public event EventHandler<User> OnUserSelected;

        public UserCardItem()
        {
            InitializeComponent();
            this.Click += Card_Click;
            // 将事件处理程序附加到所有子控件
            foreach (Control control in this.Controls)
            {
                control.Click += Card_Click;
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

        public bool IsCurrentUser
        {
            get => _isCurrentUser;
            set
            {
                _isCurrentUser = value;
                this.BackColor = _isCurrentUser ? Color.LightBlue : Color.White;
                lblStatus.Text = _isCurrentUser ? "当前用户" : "";
            }
        }

        private void Card_Click(object sender, EventArgs e)
        {
            OnUserSelected?.Invoke(this, _user);
        }
    }
} 