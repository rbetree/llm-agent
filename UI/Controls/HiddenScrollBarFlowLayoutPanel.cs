using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace llm_agent.UI.Controls
{
    /// <summary>
    /// 隐藏滚动条的FlowLayoutPanel，保持滚动功能但不显示滚动条
    /// </summary>
    public class HiddenScrollBarFlowLayoutPanel : FlowLayoutPanel
    {
        // Win32 API常量
        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;
        private const int SB_HORZ = 0;
        private const int SB_VERT = 1;
        private const int SB_BOTH = 3;

        // Win32 API函数声明
        [DllImport("user32.dll")]
        private static extern int ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        public HiddenScrollBarFlowLayoutPanel()
        {
            // 启用自动滚动
            this.AutoScroll = true;
            
            // 设置样式以支持鼠标滚轮
            this.SetStyle(ControlStyles.UserPaint | 
                         ControlStyles.AllPaintingInWmPaint | 
                         ControlStyles.DoubleBuffer | 
                         ControlStyles.ResizeRedraw, true);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // 隐藏滚动条
            ShowScrollBar(this.Handle, SB_BOTH, false);
        }

        protected override void WndProc(ref Message m)
        {
            // 处理滚动消息但隐藏滚动条
            if (m.Msg == WM_HSCROLL || m.Msg == WM_VSCROLL)
            {
                base.WndProc(ref m);
                ShowScrollBar(this.Handle, SB_BOTH, false);
                return;
            }
            
            base.WndProc(ref m);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            // 确保鼠标滚轮事件正常工作
            base.OnMouseWheel(e);
            
            // 滚动后重新隐藏滚动条
            if (this.IsHandleCreated)
            {
                ShowScrollBar(this.Handle, SB_BOTH, false);
            }
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            
            // 大小改变后重新隐藏滚动条
            if (this.IsHandleCreated)
            {
                ShowScrollBar(this.Handle, SB_BOTH, false);
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            
            // 添加控件后重新隐藏滚动条
            if (this.IsHandleCreated)
            {
                ShowScrollBar(this.Handle, SB_BOTH, false);
            }
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            
            // 移除控件后重新隐藏滚动条
            if (this.IsHandleCreated)
            {
                ShowScrollBar(this.Handle, SB_BOTH, false);
            }
        }
    }
}
