using System;
using System.Collections.Generic;
using System.Windows.Forms;
using llm_agent.API.Provider;
using llm_agent.Model;
using llm_agent.Models;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using llm_agent.DAL;

namespace llm_agent.UI.Forms
{
    public partial class ModelManagementForm : Form
    {
        private string _providerName;
        private ProviderType _providerType;
        private ProviderFactory _providerFactory;
        private List<ModelInfo> _allModels; // 所有模型列表
        private List<ModelInfo> _providerModels; // 当前提供商的模型
        private List<ModelInfo> _modifiedModels = new List<ModelInfo>(); // 已修改的模型
        private DatabaseManager _dbManager; // 数据库管理器

        public ModelManagementForm(string providerName, ProviderType providerType, ProviderFactory providerFactory)
        {
            InitializeComponent();
            _providerName = providerName;
            _providerType = providerType;
            _providerFactory = providerFactory;
            _dbManager = new DatabaseManager();
            LoadModelLists();
        }

        private void InitializeComponent()
        {
            lblProvider = new Label();
            txtProvider = new TextBox();
            modelListView = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            btnSave = new Button();
            btnCancel = new Button();
            btnFetchModels = new Button();
            btnClearAllModels = new Button();
            SuspendLayout();
            //
            // lblProvider
            //
            lblProvider.AutoSize = true;
            lblProvider.Location = new Point(21, 25);
            lblProvider.Margin = new Padding(5, 0, 5, 0);
            lblProvider.Name = "lblProvider";
            lblProvider.Size = new Size(140, 31);
            lblProvider.TabIndex = 0;
            lblProvider.Text = "服务提供商:";
            //
            // txtProvider
            //
            txtProvider.Location = new Point(184, 20);
            txtProvider.Margin = new Padding(5, 5, 5, 5);
            txtProvider.Name = "txtProvider";
            txtProvider.ReadOnly = true;
            txtProvider.Size = new Size(392, 38);
            txtProvider.TabIndex = 1;
            txtProvider.BackColor = System.Drawing.Color.WhiteSmoke;
            txtProvider.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            //
            // modelListView
            //
            modelListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            modelListView.CheckBoxes = true;
            modelListView.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4 });
            modelListView.FullRowSelect = true;
            modelListView.Location = new Point(21, 85);
            modelListView.Margin = new Padding(5, 5, 5, 5);
            modelListView.Name = "modelListView";
            modelListView.Size = new Size(1355, 385);
            modelListView.TabIndex = 2;
            modelListView.UseCompatibleStateImageBehavior = false;
            modelListView.View = View.Details;
            modelListView.BackColor = System.Drawing.Color.White;
            modelListView.ItemChecked += modelListView_ItemChecked;
            //
            // columnHeader1
            //
            columnHeader1.Text = "启用";
            //
            // columnHeader2
            //
            columnHeader2.Text = "模型ID";
            columnHeader2.Width = 500;
            //
            // columnHeader3
            //
            columnHeader3.Text = "模型名称";
            columnHeader3.Width = 500;
            //
            // columnHeader4
            //
            columnHeader4.Text = "提供商";
            columnHeader4.Width = 100;
            //
            // btnSave
            //
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.Location = new Point(1043, 490);
            btnSave.Margin = new Padding(5, 5, 5, 5);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(164, 45);
            btnSave.TabIndex = 3;
            btnSave.Text = "保存";
            btnSave.BackColor = System.Drawing.Color.FromArgb(100, 101, 165);
            btnSave.ForeColor = System.Drawing.Color.White;
            btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(76, 76, 128);
            btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(76, 76, 128);
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            //
            // btnCancel
            //
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(1214, 490);
            btnCancel.Margin = new Padding(5, 5, 5, 5);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(164, 45);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "取消";
            btnCancel.BackColor = System.Drawing.Color.Transparent;
            btnCancel.ForeColor = System.Drawing.Color.Gray;
            btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.UseVisualStyleBackColor = true;
            //
            // btnFetchModels
            //
            btnFetchModels.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFetchModels.Location = new Point(1111, 19);
            btnFetchModels.Margin = new Padding(5, 5, 5, 5);
            btnFetchModels.Name = "btnFetchModels";
            btnFetchModels.Size = new Size(268, 45);
            btnFetchModels.TabIndex = 5;
            btnFetchModels.Text = "从API获取模型列表";
            btnFetchModels.BackColor = System.Drawing.Color.FromArgb(100, 101, 165);
            btnFetchModels.ForeColor = System.Drawing.Color.White;
            btnFetchModels.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnFetchModels.FlatAppearance.BorderSize = 0;
            btnFetchModels.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(76, 76, 128);
            btnFetchModels.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(76, 76, 128);
            btnFetchModels.UseVisualStyleBackColor = false;
            btnFetchModels.Click += btnFetchModels_Click;
            //
            // btnClearAllModels
            //
            btnClearAllModels.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearAllModels.Location = new Point(833, 19);
            btnClearAllModels.Margin = new Padding(5, 5, 5, 5);
            btnClearAllModels.Name = "btnClearAllModels";
            btnClearAllModels.Size = new Size(268, 45);
            btnClearAllModels.TabIndex = 7;
            btnClearAllModels.Text = "清除所有";
            btnClearAllModels.BackColor = System.Drawing.Color.FromArgb(100, 101, 165);
            btnClearAllModels.ForeColor = System.Drawing.Color.White;
            btnClearAllModels.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnClearAllModels.FlatAppearance.BorderSize = 0;
            btnClearAllModels.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(76, 76, 128);
            btnClearAllModels.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(76, 76, 128);
            btnClearAllModels.UseVisualStyleBackColor = false;
            btnClearAllModels.Click += btnClearAllModels_Click;
            //
            // ModelManagementForm
            //
            AcceptButton = btnSave;
            AutoScaleDimensions = new SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(1400, 550);
            Controls.Add(btnFetchModels);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(modelListView);
            Controls.Add(txtProvider);
            Controls.Add(lblProvider);
            Controls.Add(btnClearAllModels);
            Margin = new Padding(5, 5, 5, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ModelManagementForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "模型管理";
            Load += ModelManagementForm_Load;
            BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            ResumeLayout(false);
            PerformLayout();
        }

        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblProvider;
        private System.Windows.Forms.TextBox txtProvider;
        private System.Windows.Forms.ListView modelListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnFetchModels;
        private System.Windows.Forms.Button btnClearAllModels;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void LoadModelLists()
        {
            try
            {
                // 获取所有模型列表
                _allModels = new List<ModelInfo>();
                _providerModels = new List<ModelInfo>();

                // 添加所有已知的模型
                string providerStr = _providerType.ToString().ToLower();
                if (providerStr == "azureopenai")
                    providerStr = "openai"; // 特殊处理Azure OpenAI

                // 先从数据库获取模型配置
                var dbModels = _dbManager.GetModels(providerStr);

                if (dbModels.Count > 0)
                {
                    // 如果数据库中有数据，使用数据库中的配置
                    _allModels = dbModels;
                    _providerModels = dbModels;
                }
                else
                {
                    // 否则使用默认配置
                    _providerModels = ModelsConfig.GetModelsByProvider(providerStr);

                    // 从所有模型中克隆一份，用于显示
                    foreach (var model in _providerModels)
                    {
                        _allModels.Add(new ModelInfo(
                            model.Id,
                            model.Name,
                            model.ProviderType,
                            model.ContextLength,
                            model.TokenPrice));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载模型列表时出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ModelManagementForm_Load(object sender, EventArgs e)
        {
            try
            {
                // 设置提供商名称
                txtProvider.Text = _providerName;

                // 显示模型列表
                RefreshModelListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载表单时出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshModelListView()
        {
            modelListView.Items.Clear();

            foreach (var model in _allModels)
            {
                ListViewItem item = new ListViewItem("");
                item.SubItems.Add(model.Id);
                item.SubItems.Add(model.Name);
                item.SubItems.Add(model.ProviderType); // 显示提供商类型而不是分类
                item.Checked = model.Enabled; // 使用模型的启用状态
                item.Tag = model;

                modelListView.Items.Add(item);
            }
        }

        private void modelListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            // 标记为已修改
            if (e.Item.Tag is ModelInfo model)
            {
                if (!_modifiedModels.Contains(model))
                {
                    _modifiedModels.Add(model);
                }
            }
        }

        private async void btnFetchModels_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                btnFetchModels.Enabled = false;
                btnFetchModels.Text = "获取中...";

                // 显示进度条
                ProgressBar progressBar = new ProgressBar();
                progressBar.Style = ProgressBarStyle.Marquee;
                progressBar.Location = new Point(btnFetchModels.Left, btnFetchModels.Bottom + 5);
                progressBar.Size = new Size(btnFetchModels.Width, 15);
                this.Controls.Add(progressBar);
                progressBar.BringToFront();
                progressBar.Visible = true;
                Application.DoEvents();

                // 获取提供商实例
                var provider = _providerFactory.GetProvider(_providerType);
                if (provider == null)
                {
                    MessageBox.Show($"无法获取提供商: {_providerType}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 更新API密钥和主机
                string apiKey = GetProviderApiKey(_providerType);
                string apiHost = GetProviderApiHost(_providerType);
                if (string.IsNullOrEmpty(apiKey))
                {
                    MessageBox.Show("API密钥未设置，无法从API获取模型列表", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                provider.UpdateApiKey(apiKey);
                if (!string.IsNullOrEmpty(apiHost))
                {
                    provider.UpdateApiHost(apiHost);
                }

                // 获取支持的模型列表 - 使用不同提供商特定的API方法
                List<string> supportedModels;

                if (provider is OpenAIProvider openAIProvider)
                {
                    // 使用OpenAI专用方法
                    supportedModels = await openAIProvider.GetModelsFromApiAsync();
                }
                else if (provider is AzureOpenAIProvider azureProvider)
                {
                    // Azure OpenAI的模型列表可能需要从部署列表获取
                    supportedModels = await azureProvider.GetModelsFromApiAsync();
                }
                else if (provider is AnthropicProvider anthropicProvider)
                {
                    // Anthropic专用方法 (如果实现了)
                    supportedModels = await GetAnthropicModelsAsync(anthropicProvider);
                }
                else if (provider is GeminiProvider geminiProvider)
                {
                    // Gemini专用方法 (如果实现了)
                    supportedModels = await GetGeminiModelsAsync(geminiProvider);
                }
                else if (provider is ZhipuProvider zhipuProvider)
                {
                    // 智谱专用方法 (如果实现了)
                    supportedModels = await GetZhipuModelsAsync(zhipuProvider);
                }
                else
                {
                    // 通用方法，仅使用硬编码的支持列表
                    supportedModels = provider.GetSupportedModels();
                }

                if (supportedModels == null || supportedModels.Count == 0)
                {
                    MessageBox.Show("从API获取模型列表失败。请检查网络连接、API密钥和主机设置是否正确。", "API调用失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 整合新模型到列表中
                string providerStr = _providerType.ToString().ToLower();
                if (providerStr == "azureopenai")
                    providerStr = "openai"; // 特殊处理Azure OpenAI

                int newModelsCount = 0;

                foreach (var modelId in supportedModels)
                {
                    // 检查是否已存在
                    bool exists = false;
                    foreach (var existingModel in _allModels)
                    {
                        if (existingModel.Id.Equals(modelId, StringComparison.OrdinalIgnoreCase))
                        {
                            exists = true;
                            break;
                        }
                    }

                    // 如果不存在，则添加
                    if (!exists)
                    {
                        var newModel = new ModelInfo(
                            modelId,
                            modelId, // 初始时名称与ID相同
                            providerStr
                        );

                        newModel.Enabled = true; // 确保新添加的模型默认启用
                        _allModels.Add(newModel);
                        newModelsCount++;
                    }
                }

                // 刷新列表
                RefreshModelListView();

                if (newModelsCount > 0)
                {
                    MessageBox.Show($"成功添加{newModelsCount}个新模型", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("未发现新模型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"从API获取模型时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 移除进度条
                foreach (Control c in this.Controls)
                {
                    if (c is ProgressBar pb && pb.Style == ProgressBarStyle.Marquee)
                    {
                        this.Controls.Remove(pb);
                        pb.Dispose();
                        break;
                    }
                }

                btnFetchModels.Text = "从API获取模型列表";
                btnFetchModels.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        // 为Anthropic获取模型列表的辅助方法
        private async Task<List<string>> GetAnthropicModelsAsync(AnthropicProvider provider)
        {
            try
            {
                // 调用Anthropic提供商的API获取模型列表
                return await provider.GetModelsFromApiAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取Anthropic模型列表失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // 不再返回默认模型列表
                return new List<string>(); // 返回空列表
            }
        }

        // 为Gemini获取模型列表的辅助方法
        private async Task<List<string>> GetGeminiModelsAsync(GeminiProvider provider)
        {
            try
            {
                // 调用Google Gemini提供商的API获取模型列表
                return await provider.GetModelsFromApiAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取Google Gemini模型列表失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // 不再返回默认模型列表
                return new List<string>(); // 返回空列表
            }
        }

        // 为智谱获取模型列表的辅助方法
        private async Task<List<string>> GetZhipuModelsAsync(ZhipuProvider provider)
        {
            try
            {
                // 调用智谱AI提供商的API获取模型列表
                return await provider.GetModelsFromApiAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取智谱AI模型列表失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // 不再返回默认模型列表
                return new List<string>(); // 返回空列表
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 准备要保存的模型列表
                List<ModelInfo> modelsToSave = new List<ModelInfo>();

                foreach (ListViewItem item in modelListView.Items)
                {
                    if (item.Tag is ModelInfo model)
                    {
                        // 设置模型启用状态
                        model.Enabled = item.Checked;
                        modelsToSave.Add(model);
                    }
                }

                // 获取提供商类型字符串
                string providerStr = _providerType.ToString().ToLower();
                if (providerStr == "azureopenai")
                    providerStr = "openai"; // 特殊处理Azure OpenAI

                // 保存到SQLite数据库
                _dbManager.SaveModels(modelsToSave, providerStr);

                int enabledCount = modelsToSave.Count(m => m.Enabled);
                int disabledCount = modelsToSave.Count - enabledCount;

                MessageBox.Show(
                    $"模型配置已更新并永久保存到数据库。共{enabledCount}个启用，{disabledCount}个禁用。",
                    "保存成功",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // 设置对话框结果为OK并关闭
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存模型信息时出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetProviderApiKey(ProviderType providerType)
        {
            switch (providerType)
            {
                case ProviderType.OpenAI:
                    return Properties.Settings.Default.OpenAIApiKey;
                case ProviderType.AzureOpenAI:
                    return Properties.Settings.Default.AzureApiKey;
                case ProviderType.Anthropic:
                    return Properties.Settings.Default.AnthropicApiKey;
                case ProviderType.Google:
                    return Properties.Settings.Default.GeminiApiKey;
                case ProviderType.ZhipuAI:
                    return Properties.Settings.Default.ZhipuApiKey;
                case ProviderType.Other:
                    return Properties.Settings.Default.OtherApiKey;
                default:
                    return string.Empty;
            }
        }

        private string GetProviderApiHost(ProviderType providerType)
        {
            switch (providerType)
            {
                case ProviderType.OpenAI:
                    return Properties.Settings.Default.OpenAIApiHost;
                case ProviderType.AzureOpenAI:
                    return Properties.Settings.Default.AzureApiHost;
                case ProviderType.Anthropic:
                    return Properties.Settings.Default.AnthropicApiHost;
                case ProviderType.Google:
                    return Properties.Settings.Default.GeminiApiHost;
                case ProviderType.ZhipuAI:
                    return Properties.Settings.Default.ZhipuApiHost;
                case ProviderType.Other:
                    return Properties.Settings.Default.OtherApiHost;
                default:
                    return string.Empty;
            }
        }

        private void btnClearAllModels_Click(object sender, EventArgs e)
        {
            try
            {
                // 确认对话框
                DialogResult result = MessageBox.Show(
                    $"确定要清除该渠道下的所有模型吗？此操作将删除数据库中保存的{_providerName}的所有模型，且无法撤销。",
                    "确认清除",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    // 获取提供商类型字符串
                    string providerStr = _providerType.ToString().ToLower();
                    if (providerStr == "azureopenai")
                        providerStr = "openai"; // 特殊处理Azure OpenAI

                    // 记录当前模型数量
                    int modelCount = _allModels.Count;

                    // 保存空的模型列表，相当于清除该渠道的所有模型
                    _dbManager.SaveModels(new List<ModelInfo>(), providerStr);

                    // 直接清空当前模型列表，而不是重新加载（避免加载默认模型）
                    _allModels.Clear();
                    _providerModels.Clear();

                    // 刷新列表视图
                    RefreshModelListView();

                    MessageBox.Show(
                        $"{_providerName}的所有模型已被清除，共删除{modelCount}个模型记录。",
                        "清除成功",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"清除模型时出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}