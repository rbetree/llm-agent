using System;

namespace llm_agent.Common.Exceptions
{
    public class LLMAgentException : Exception
    {
        public LLMAgentException() : base() { }
        public LLMAgentException(string message) : base(message) { }
        public LLMAgentException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ApiException : LLMAgentException
    {
        public ApiException() : base() { }
        public ApiException(string message) : base(message) { }
        public ApiException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class DataAccessException : LLMAgentException
    {
        public DataAccessException() : base() { }
        public DataAccessException(string message) : base(message) { }
        public DataAccessException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ConfigurationException : LLMAgentException
    {
        public ConfigurationException() : base() { }
        public ConfigurationException(string message) : base(message) { }
        public ConfigurationException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class UserInputException : LLMAgentException
    {
        public UserInputException() : base() { }
        public UserInputException(string message) : base(message) { }
        public UserInputException(string message, Exception innerException) : base(message, innerException) { }
    }
}