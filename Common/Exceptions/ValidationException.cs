using System;

namespace llm_agent.Common.Exceptions
{
    /// <summary>
    /// 验证异常类
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ValidationException() : base()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常消息</param>
        public ValidationException(string message) : base(message)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="innerException">内部异常</param>
        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
