using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using llm_agent.Model;

namespace llm_agent.API.Provider
{
    public class LLMProvider
    {
        private readonly HttpClient _httpClient;
        private BaseLLMProvider? _provider;

        public LLMProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void Initialize(string providerType, string apiKey, string apiHost = "")
        {
            _provider = ProviderFactory.CreateProvider(providerType, apiKey, apiHost);
        }

        public Task<string> ChatAsync(List<ChatMessage> messages, string modelId)
        {
            ValidateProvider();
            return _provider.ChatAsync(messages, modelId);
        }

        public IAsyncEnumerable<string> StreamChatAsync(List<ChatMessage> messages, string modelId, CancellationToken cancellationToken = default)
        {
            ValidateProvider();
            return _provider.StreamChatAsync(messages, modelId, cancellationToken);
        }

        public List<string> GetSupportedModels()
        {
            ValidateProvider();
            return _provider.GetSupportedModels();
        }

        public List<string> GetAvailableModels()
        {
            ValidateProvider();
            return _provider.GetAvailableModels();
        }

        public ProviderType GetProviderType()
        {
            ValidateProvider();
            return _provider.GetProviderType();
        }

        public void UpdateApiKey(string apiKey)
        {
            ValidateProvider();
            _provider.UpdateApiKey(apiKey);
        }

        public void UpdateApiHost(string apiHost)
        {
            ValidateProvider();
            _provider.UpdateApiHost(apiHost);
        }

        private void ValidateProvider()
        {
            if (_provider == null)
            {
                throw new InvalidOperationException("Provider not initialized. Call Initialize method first.");
            }
        }
    }
}
