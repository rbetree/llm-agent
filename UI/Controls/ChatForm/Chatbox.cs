using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace llm_agent.UI.Controls.ChatForm
{
    public partial class Chatbox : UserControl
    {
        public ChatboxInfo chatbox_info;
        public OpenFileDialog fileDialog = new OpenFileDialog();
        public string initialdirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        // 流式响应控制属性
        public bool UseStreamResponse { get; set; } = true;

        // 模型选择相关事件
        public event EventHandler ModelSelectionChanged;
        public event EventHandler StreamResponseToggled;

        public Chatbox(ChatboxInfo _chatbox_info)
        {
            InitializeComponent();

            chatbox_info = _chatbox_info;

            clientnameLabel.Text = chatbox_info.NamePlaceholder;
            statusLabel.Text = chatbox_info.StatusPlaceholder;
            phoneLabel.Text = chatbox_info.PhonePlaceholder;
            chatTextbox.Text = chatbox_info.ChatPlaceholder;

            chatTextbox.Enter += ChatEnter;
            chatTextbox.Leave += ChatLeave;
            sendButton.Click += SendMessage;
            attachButton.Click += BuildAttachment;
            removeButton.Click += CancelAttachment;

            // 添加流式响应和模型选择事件处理
            streamCheckBox.CheckedChanged += StreamCheckBox_CheckedChanged;
            modelComboBox.SelectedIndexChanged += ModelComboBox_SelectedIndexChanged;

            chatTextbox.KeyDown += OnEnter;

            AddMessage(null);
        }

        // 流式响应复选框事件处理
        private void StreamCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UseStreamResponse = streamCheckBox.Checked;
            StreamResponseToggled?.Invoke(this, EventArgs.Empty);
        }

        // 模型选择下拉框事件处理
        private void ModelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (modelComboBox.SelectedItem != null)
            {
                ModelSelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 获取当前选中的模型
        /// </summary>
        public string GetSelectedModel()
        {
            return modelComboBox.SelectedItem?.ToString();
        }

        /// <summary>
        /// 设置模型列表
        /// </summary>
        /// <param name="models">模型列表</param>
        /// <param name="selectedModel">默认选中的模型</param>
        public void SetModelList(IEnumerable<string> models, string? selectedModel = null)
        {
            modelComboBox.Items.Clear();

            foreach (var model in models)
            {
                modelComboBox.Items.Add(model);
            }

            if (selectedModel != null && modelComboBox.Items.Contains(selectedModel))
            {
                modelComboBox.SelectedItem = selectedModel;
            }
            else if (modelComboBox.Items.Count > 0)
            {
                modelComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 设置流式响应状态
        /// </summary>
        /// <param name="useStream">是否使用流式响应</param>
        public void SetStreamResponse(bool useStream)
        {
            UseStreamResponse = useStream;
            streamCheckBox.Checked = useStream;
        }

        /// <summary>
        /// ChatItem objects are generated dynamically from IChatModel. By default, a ChatItem is allowed to be resized up to 60% of the entire screen.
        /// I've thought about it being editable from outside, but there's no real need to.
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(IChatModel message)
        {
            var chatItem = new ChatItem(message);
            chatItem.Name = "chatItem" + itemsPanel.Controls.Count;
            chatItem.Dock = DockStyle.Top;
            itemsPanel.Controls.Add(chatItem);
            chatItem.BringToFront();

            chatItem.ResizeBubbles((int)(itemsPanel.Width * 0.6));

            itemsPanel.ScrollControlIntoView(chatItem);
        }

        /// <summary>
        /// 更新最后一条指定作者的消息内容，支持流式响应更新
        /// </summary>
        /// <param name="author">消息作者</param>
        /// <param name="content">新的消息内容</param>
        /// <returns>是否成功更新</returns>
        public bool UpdateLastMessage(string author, string content)
        {
            // 寻找最后一条指定作者的消息
            foreach (Control control in itemsPanel.Controls)
            {
                if (control is ChatItem chatItem && chatItem.Message is TextChatModel textModel)
                {
                    // 检查是否是指定作者的消息
                    if (textModel.Author == author)
                    {
                        // 更新内容
                        textModel.Body = content;

                        // 查找bodyTextBox控件并更新
                        var bodyTextBox = chatItem.Controls.Find("bodyTextBox", true).FirstOrDefault() as RichTextBox;
                        if (bodyTextBox != null)
                        {
                            // 更新文本框内容，保留原始格式包括换行符
                            bodyTextBox.Text = content;

                            // 调整气泡大小以适应新内容
                            chatItem.ResizeBubbles((int)(itemsPanel.Width * 0.6));

                            // 确保视图滚动到该消息
                            itemsPanel.ScrollControlIntoView(chatItem);

                            return true;
                        }

                        // 找到了消息但无法更新文本框
                        return false;
                    }
                }
            }

            // 未找到指定作者的消息
            return false;
        }

        /// <summary>
        /// 清除所有消息
        /// </summary>
        public void ClearMessages()
        {
            itemsPanel.Controls.Clear();
        }

        /// <summary>
        /// 滚动到指定消息
        /// </summary>
        /// <param name="chatItem">要滚动到的消息项</param>
        public void ScrollToMessage(ChatItem chatItem)
        {
            if (chatItem != null)
            {
                itemsPanel.ScrollControlIntoView(chatItem);
            }
        }

        /// <summary>
        /// 获取消息项数量
        /// </summary>
        /// <returns>消息项数量</returns>
        public int GetMessageCount()
        {
            return itemsPanel.Controls.Count;
        }

        /// <summary>
        /// 获取指定索引的消息项
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>消息项，如果索引无效则返回null</returns>
        public ChatItem GetMessageAt(int index)
        {
            if (index >= 0 && index < itemsPanel.Controls.Count)
            {
                return itemsPanel.Controls[index] as ChatItem;
            }
            return null;
        }

        /// <summary>
        /// 提供公共方法，清除输入框内容
        /// 用于外部代码清除输入框，避免双重触发SendMessage事件
        /// </summary>
        public void ClearInputText()
        {
            if (chatTextbox != null)
            {
                chatTextbox.Text = string.Empty;
            }
        }

        //Improves the chat UI slightly by having a placeholder text. Note that this is implemented because Winforms doesn't have a native "placeholder" UI. Can be buggy.
        void ChatLeave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(chatTextbox.Text))
            {
                chatTextbox.Text = chatbox_info.ChatPlaceholder;
                chatTextbox.ForeColor = Color.Gray;
            }
        }

        //Improves the chat UI slightly by having a placeholder text. Note that this is implemented because Winforms doesn't have a native "placeholder" UI. Can be buggy.
        void ChatEnter(object sender, EventArgs e)
        {
            chatTextbox.ForeColor = Color.Black;
            if (chatTextbox.Text == chatbox_info.ChatPlaceholder)
            {
                chatTextbox.Text = "";
            }
        }

        //Cross-tested this with the Twilio API and the RingCentral API, and async messaging is the way to go.
        async void SendMessage(object sender, EventArgs e)
        {
            string tonumber = phoneLabel.Text;
            string chatmessage = chatTextbox.Text;

            IChatModel chatModel = null;
            TextChatModel textModel = null;

            //Each IChatModel is specifically built for a single purpose. For that reason, if you want to display a text item AND and image, you'd make two IChatModels for
            //their respective purposes. AttachmentChatModel and ImageChatModel, however, can really be used interchangeably.
            if (chatbox_info.Attachment != null && chatbox_info.AttachmentType.Contains("image"))
            {
                chatModel = new ImageChatModel()
                {
                    Author = chatbox_info.User,
                    Image = Image.FromStream(new MemoryStream(chatbox_info.Attachment)),
                    ImageName = chatbox_info.AttachmentName,
                    Inbound = false,
                    Read = true,
                    Time = DateTime.Now,
                };

            }
            else if (chatbox_info.Attachment != null)
            {
                chatModel = new AttachmentChatModel()
                {
                    Author = chatbox_info.User,
                    Attachment = chatbox_info.Attachment,
                    Filename = chatbox_info.AttachmentName,
                    Read = true,
                    Inbound = false,
                    Time = DateTime.Now
                };
            }

            if (!string.IsNullOrWhiteSpace(chatmessage) && chatmessage != chatbox_info.ChatPlaceholder)
            {
                textModel = new TextChatModel()
                {
                    Author = chatbox_info.User,
                    Body = chatmessage,
                    Inbound = false,
                    Read = true,
                    Time = DateTime.Now
                };
            }

            try
            {
                /*

                    INSERT SENDING LOGIC HERE. Again, this is just a UserControl, not a complete app. For the Ringcentral API, I was able to reduce this section
                    down to a single method.

                */

                if (chatModel != null)
                {
                    AddMessage(chatModel);
                    CancelAttachment(null, null);
                }
                if (textModel != null)
                {
                    AddMessage(textModel);
                    chatTextbox.Text = string.Empty;
                }
            }
            catch (Exception exc)
            {
                //If any exception is found, then it is printed on the screen. Feel free to change this method if you don't want people to see exceptions.
                textModel = new TextChatModel()
                {
                    Author = chatbox_info.User,
                    Body = "The message could not be processed. Please see the reason below.\r\n" + exc.Message,
                    Inbound = false,
                    Read = true,
                    Time = DateTime.Now
                };
                AddMessage(textModel);
                CancelAttachment(null, null);
            }
        }

        public void BuildAttachment(object sender, EventArgs e)
        {
            fileDialog.InitialDirectory = initialdirectory;
            fileDialog.FileName = "";
            fileDialog.Filter = "All files (*.*)|*.*";
            fileDialog.FilterIndex = 1;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var file = fileDialog.FileName;
                    var fi = new FileInfo(file);
                    initialdirectory = Path.GetDirectoryName(file);
                    if (fi.Length > 1450000)
                    {
                        MessageBox.Show("Attachments must be less than 1.45MB. Please choose a smaller file.", "File too large", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    chatbox_info.Attachment = File.ReadAllBytes(file);
                    chatbox_info.AttachmentName = Path.GetFileName(file);
                    chatbox_info.AttachmentType = Path.GetExtension(file).ToLower();

                    removeButton.Visible = true;
                    attachButton.Enabled = false;
                    attachButton.BackColor = Color.Gray;
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + exc.Message);
                }
            }
        }

        public void CancelAttachment(object sender, EventArgs e)
        {
            chatbox_info.Attachment = null;
            chatbox_info.AttachmentName = null;
            chatbox_info.AttachmentType = null;
            removeButton.Visible = false;
            attachButton.Enabled = true;
            attachButton.BackColor = Color.GhostWhite;
            attachButton.Text = "";
            attachButton.Width = 41;
        }

        //Inspired from Slack, you can also press Shift + Enter to enter text.
        async void OnEnter(object sender, KeyEventArgs e)
        {
            if (e.Shift && e.KeyValue == 13)
            {
                SendMessage(this, null);
            }
        }

        //When the Control resizes, it will trigger the resize event for all the ChatItem object inside as well, again with a default width of 60%.
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            foreach (var control in itemsPanel.Controls)
            {
                if (control is ChatItem)
                {
                    (control as ChatItem).ResizeBubbles((int)(itemsPanel.Width * 0.6));
                }
            }
        }

        private void topPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void statusLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
