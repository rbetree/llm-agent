using System;
using System.Drawing;
using System.Windows.Forms;
using llm_agent.Model;

namespace llm_agent.UI.Forms
{
    /// <summary>
    /// 添加/编辑网站对话框
    /// </summary>
    public partial class AddWebsiteForm : Form
    {
        private AiWebsite? _website;
        private readonly bool _isEditMode;

        /// <summary>
        /// 网站名称
        /// </summary>
        public string WebsiteName => txtName.Text.Trim();

        /// <summary>
        /// 网站URL
        /// </summary>
        public string WebsiteUrl => txtUrl.Text.Trim();

        /// <summary>
        /// 网站描述
        /// </summary>
        public string WebsiteDescription => txtDescription.Text.Trim();

        /// <summary>
        /// 网站分类
        /// </summary>
        public string WebsiteCategory => cboCategory.Text.Trim();

        /// <summary>
        /// 构造函数 - 新建模式
        /// </summary>
        public AddWebsiteForm()
        {
            InitializeComponent();
            _isEditMode = false;
            this.Text = "添加网站";
            InitializeForm();
        }

        /// <summary>
        /// 构造函数 - 编辑模式
        /// </summary>
        /// <param name="website">要编辑的网站</param>
        public AddWebsiteForm(AiWebsite website)
        {
            InitializeComponent();
            _website = website;
            _isEditMode = true;
            this.Text = "编辑网站";
            InitializeForm();
            LoadWebsiteData();
        }

        /// <summary>
        /// 初始化表单
        /// </summary>
        private void InitializeForm()
        {
            // 设置表单属性
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(450, 350);

            // 初始化分类下拉框
            InitializeCategoryComboBox();

            // 设置事件处理
            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;
            txtUrl.Leave += TxtUrl_Leave;
        }

        /// <summary>
        /// 初始化分类下拉框
        /// </summary>
        private void InitializeCategoryComboBox()
        {
            cboCategory.Items.Clear();
            cboCategory.Items.AddRange(new string[]
            {
                "对话AI",
                "图像生成",
                "编程工具",
                "搜索工具",
                "AI平台",
                "学习工具",
                "办公工具",
                "其他"
            });
        }

        /// <summary>
        /// 加载网站数据（编辑模式）
        /// </summary>
        private void LoadWebsiteData()
        {
            if (_website == null) return;

            txtName.Text = _website.Name;
            txtUrl.Text = _website.Url;
            txtDescription.Text = _website.Description;
            cboCategory.Text = _website.Category;
        }

        /// <summary>
        /// URL文本框失去焦点事件 - 自动填充网站名称
        /// </summary>
        private void TxtUrl_Leave(object sender, EventArgs e)
        {
            try
            {
                // 如果名称为空且URL不为空，尝试从URL提取域名作为名称
                if (string.IsNullOrWhiteSpace(txtName.Text) && !string.IsNullOrWhiteSpace(txtUrl.Text))
                {
                    var url = txtUrl.Text.Trim();
                    if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                    {
                        url = "https://" + url;
                        txtUrl.Text = url;
                    }

                    var uri = new Uri(url);
                    var domain = uri.Host;

                    // 移除www前缀
                    if (domain.StartsWith("www."))
                    {
                        domain = domain.Substring(4);
                    }

                    // 首字母大写
                    if (!string.IsNullOrEmpty(domain))
                    {
                        txtName.Text = char.ToUpper(domain[0]) + domain.Substring(1);
                    }
                }
            }
            catch (Exception ex)
            {
                // 忽略URL解析错误
                Console.WriteLine($"URL解析错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 确定按钮点击事件
        /// </summary>
        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                // 验证输入
                if (!ValidateInput())
                {
                    return;
                }

                // 设置对话框结果
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 取消按钮点击事件
        /// </summary>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// 验证输入
        /// </summary>
        /// <returns>是否验证通过</returns>
        private bool ValidateInput()
        {
            // 验证网站名称
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("请输入网站名称。", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            // 验证网站URL
            if (string.IsNullOrWhiteSpace(txtUrl.Text))
            {
                MessageBox.Show("请输入网站URL。", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUrl.Focus();
                return false;
            }

            // 验证URL格式
            try
            {
                var url = txtUrl.Text.Trim();
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "https://" + url;
                    txtUrl.Text = url;
                }

                var uri = new Uri(url);
                if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                {
                    MessageBox.Show("请输入有效的HTTP或HTTPS网址。", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUrl.Focus();
                    return false;
                }
            }
            catch (UriFormatException)
            {
                MessageBox.Show("请输入有效的网址格式。", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUrl.Focus();
                return false;
            }

            return true;
        }
    }
}
