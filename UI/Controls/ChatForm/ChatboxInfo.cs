﻿namespace llm_agent.UI.Controls.ChatForm
{
    public class ChatboxInfo
    {
        public string User { get; set; }
        public string ChatPlaceholder = "Please enter a message...";
        public byte[] Attachment { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentType { get; set; }
    }
}
