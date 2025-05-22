using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace llm_agent.UI.Controls.ChatForm
{
	public partial class ChatItem : UserControl
	{
        public IChatModel ChatModel { get; set; }

        /// <summary>
        /// 提供ChatModel的便捷访问属性
        /// </summary>
        public IChatModel Message { get => ChatModel; }

        public ChatItem()
        {
            InitializeComponent();
            bodyTextBox.Text = "No messages were found.";
            authorLabel.Text = "System " + DateTime.Now.ToShortTimeString();
        }

        public ChatItem(IChatModel chatModel)
        {
            InitializeComponent();

            if (chatModel == null)
            {
                chatModel = new TextChatModel()
                {
                    Author = "System",
                    Body = "No chat messages were found regarding this client.",
                    Inbound = true,
                    Time = DateTime.Now
                };
            }

            ChatModel = chatModel;

            if (chatModel.Inbound)
            {
                bodyPanel.Dock = DockStyle.Left;
                authorLabel.Dock = DockStyle.Left;
                bodyPanel.BackColor = Color.FromArgb(100, 101, 165);
                bodyTextBox.BackColor = Color.FromArgb(100, 101, 165);
            }
            else
            {
                bodyPanel.Dock = DockStyle.Right;
                authorLabel.Dock = DockStyle.Right;
                bodyTextBox.SelectionAlignment = HorizontalAlignment.Right;
            }

            //Fills in the label.
            if (chatModel.Time > DateTime.Today)
            {
                authorLabel.Text = $"{chatModel.Author ?? "System"}, {chatModel.Time.ToShortTimeString()}";
            }
            else
            {
                authorLabel.Text = $"{chatModel.Author ?? "System"}, {chatModel.Time.ToShortDateString()}";
            }

            switch (chatModel.Type)
            {
                case "text":
                    var textmodel = chatModel as TextChatModel;
                    // 保留原始文本格式，包括换行符
                    bodyTextBox.Text = textmodel.Body;
                    break;
                case "image":
                    var imagemodel = chatModel as ImageChatModel;
                    bodyTextBox.Visible = false;
                    bodyPanel.BackgroundImage = imagemodel.Image;
                    bodyPanel.BackColor = Color.GhostWhite;
                    bodyPanel.BackgroundImageLayout = ImageLayout.Stretch;
                    break;
                case "attachment":
                    var attachmentmodel = chatModel as AttachmentChatModel;
                    bodyPanel.BackColor = Color.OrangeRed;
                    bodyTextBox.BackColor = Color.OrangeRed;
                    bodyTextBox.Text = "Click to download: " + attachmentmodel.Filename;
                    bodyTextBox.Click += DownloadAttachment;
                    break;
                default:
                    break;
            }
        }

        void DownloadAttachment(object sender, EventArgs e)
        {
            var attachmentmodel = ChatModel as AttachmentChatModel;
            if (attachmentmodel.Attachment != null)
            {
                //Borrows the download logic of how browsers download label files. Note that if you are using Mac and Linux, first of all, how did you get this working?
                //But more importantly, fullpath will not lead into your downloads folder, mostly there is now Environment.SpecialFolder.Downloads.
                string fullpath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", attachmentmodel.Filename);
                int count = 1;
                while (System.IO.File.Exists(fullpath))
                {
                    string file = System.IO.Path.GetFileNameWithoutExtension(fullpath);
                    string ext = System.IO.Path.GetExtension(fullpath);
                    string dir = System.IO.Path.GetDirectoryName(fullpath);

                    fullpath = System.IO.Path.Combine(dir, $"{file}({count++}){ext}");
                }

                System.IO.File.WriteAllBytes(fullpath, attachmentmodel.Attachment);
                MessageBox.Show("Attachment " + attachmentmodel.Filename + " was downloaded to the path " + fullpath, "File Downloaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Attachment " + attachmentmodel.Filename + " could not be found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void ResizeBubbles(int maxwidth)
        {
            if (ChatModel == null)
            {
                return;
            }
            else
            {
                SuspendLayout();

                //The chat bubble is set to the Fill Dockstyle, which means that it recieves all increases in height and width. In order to change the height or width of the chat bubble then,
                //all we really need to do is change the height/width of the control, and add all the padding in between for calculations.
                switch (ChatModel.Type)
                {
                    case "image":
                        //The goal is to resize the image around the restrictions we are given. If the image's width is beyond MaxWidth, then resize it to be smaller.
                        var imagemodel = ChatModel as ImageChatModel;
                        //Have to consider the padding involved in both width and height.
                        if (imagemodel.Image.Width < maxwidth + (Width - bodyPanel.Width))
                        {
                            //Best case scenario: The image width is less than MaxWidth. Then we just need to resize the height so that it is a tight fit.
                            bodyPanel.Width = imagemodel.Image.Width;

                            //Can't set the height of the bodyPanel directly, but we know it scales linearly with Height.
                            Height = imagemodel.Image.Height + (Height - bodyPanel.Height);
                        }
                        else
                        {
                            //This is a slightly harder problem. If the image width is less than MaxWidth, bodyPanel will have to be resized until they match.
                            //This will resize the width, let's find how much the height changes.
                            double ratio = (double)maxwidth / (double)imagemodel.Image.Width;
                            int adjheight = (int)(imagemodel.Image.Height * ratio);

                            bodyPanel.Width = maxwidth;
                            Height = adjheight + (Height - bodyPanel.Height);
                        }
                        break;
                    case "text":
                        var textmodel = ChatModel as TextChatModel;
                        //Ah, this is hell. Alright, so the implementation for this is similar to the image one, except the width can't be automatically calculated. Instead, it
                        //has to be calculated through the size and length of the text. See TextChange().
                        string body = textmodel.Body;
                        TextChange(bodyTextBox.Text);
                        break;
                    case "attachment":
                        var attachmodel = ChatModel as AttachmentChatModel;
                        TextChange(bodyTextBox.Text);
                        break;
                    default:
                        break;
                }
            }

            ResumeLayout();

            void TextChange(string body)
            {
                // 使用RichTextBox的自动换行和测量功能
                int fontheight = bodyTextBox.Font.Height;

                // 确保文本已设置到RichTextBox，并保留换行符
                bodyTextBox.Text = body;

                // 计算文本宽度
                var gfx = this.CreateGraphics();
                double stringwidth = gfx.MeasureString(body, bodyTextBox.Font).Width;

                // 设置气泡宽度
                if (stringwidth < maxwidth + bodyPanel.Width - bodyTextBox.Width)
                {
                    // 设置宽度为文本宽度加上一些边距
                    bodyPanel.Width = (int)(stringwidth + bodyPanel.Width - bodyTextBox.Width + 15);
                }
                else
                {
                    // 设置为最大宽度
                    bodyPanel.Width = maxwidth + bodyPanel.Width - bodyTextBox.Width;
                }

                // 使用更准确的方法计算实际行数
                int actualLines = CalculateActualLines(body, bodyTextBox);

                // 调整控件高度，确保有足够空间显示所有行
                // 使用1.1的系数来增加一些额外空间
                int calculatedHeight = (int)(actualLines * fontheight * 1.2) + (Height - bodyTextBox.Height) + 12;
                
                // 设置最大高度限制，避免气泡过大
                // 如果内容太长，将显示滚动条
                int maxHeight = 500; // 最大高度限制
                Height = Math.Min(calculatedHeight, maxHeight);
                
                // 如果内容太长需要滚动条，则确保滚动条可见
                bodyTextBox.ScrollBars = (calculatedHeight > maxHeight) 
                    ? RichTextBoxScrollBars.Vertical 
                    : RichTextBoxScrollBars.None;
            }
        }

        /// <summary>
        /// 计算文本在RichTextBox中实际占用的行数
        /// </summary>
        /// <param name="text">要计算的文本</param>
        /// <param name="rtb">RichTextBox控件</param>
        /// <returns>实际行数</returns>
        private int CalculateActualLines(string text, RichTextBox rtb)
        {
            if (string.IsNullOrEmpty(text))
                return 1;

            // 计算自动换行导致的行数
            int textLength = rtb.TextLength;
            int lineCount = 1;  // 至少有一行

            if (textLength > 0)
            {
                // 使用RichTextBox的GetLineFromCharIndex方法获取行数
                // 这个方法会考虑自动换行和显式换行
                int lastLine = rtb.GetLineFromCharIndex(textLength - 1);
                lineCount = lastLine + 1;  // 行索引从0开始，所以加1得到行数
            }

            return lineCount;
        }
        private void ChatItem_Load(object sender, EventArgs e)
        {

        }
    }
}
