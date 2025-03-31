using System;
using System.Windows.Forms;

namespace llm_agent.Common.Extensions
{
    public static class ControlExtensions
    {
        /// <summary>
        /// 线程安全地对控件进行操作
        /// </summary>
        public static void InvokeIfRequired(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// 安全地查找子控件
        /// </summary>
        public static T FindControl<T>(this Control parent, string name) where T : Control
        {
            if (parent == null || string.IsNullOrEmpty(name))
                return null;

            Control[] controls = parent.Controls.Find(name, true);
            if (controls.Length > 0 && controls[0] is T result)
            {
                return result;
            }

            return null;
        }
    }
}