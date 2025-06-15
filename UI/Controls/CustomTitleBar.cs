using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace llm_agent.UI.Controls
{
    public partial class CustomTitleBar : UserControl
    {
        private Form _parentForm;

        public CustomTitleBar()
        {
            InitializeComponent();
            this.Dock = DockStyle.Top;
            this.Height = 60;

            // Wire up events
            this.Load += CustomTitleBar_Load;
            this.lblTitle.MouseDown += TitleBar_MouseDown;
            this.MouseDown += TitleBar_MouseDown;
            this.picLogo.MouseDown += TitleBar_MouseDown;

            this.btnMinimize.Click += (s, e) =>
            {
                if (_parentForm != null) _parentForm.WindowState = FormWindowState.Minimized;
            };
            this.btnMaximize.Click += (s, e) =>
            {
                if (_parentForm != null)
                {
                    _parentForm.WindowState = _parentForm.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
                }
            };
            this.btnClose.Click += (s, e) =>
            {
                if (_parentForm != null) _parentForm.Close();
            };

            // Hover effects
            AddHoverEffect(btnMinimize, Color.FromArgb(90, 90, 140), this.BackColor);
            AddHoverEffect(btnMaximize, Color.FromArgb(90, 90, 140), this.BackColor);
            AddHoverEffect(btnClose, Color.Red, this.BackColor);
        }

        private void CustomTitleBar_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                _parentForm = this.FindForm();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public override string Text
        {
            get => lblTitle.Text;
            set => lblTitle.Text = value;
        }

        /// <summary>
        /// 设置标题栏的Logo图像
        /// </summary>
        /// <param name="image">要显示的图像</param>
        public void SetLogo(Image image)
        {
            if (image != null)
            {
                picLogo.Image = image;
            }
        }

        /// <summary>
        /// 获取或设置Logo图像
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        public Image Logo
        {
            get => picLogo.Image;
            set => picLogo.Image = value;
        }

        // Constants for moving the form
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(_parentForm.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void AddHoverEffect(Button button, Color hoverColor, Color leaveColor)
        {
            button.MouseEnter += (s, e) => button.BackColor = hoverColor;
            button.MouseLeave += (s, e) => button.BackColor = leaveColor;
        }
    }
} 