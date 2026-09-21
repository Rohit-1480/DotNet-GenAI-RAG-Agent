#pragma warning disable OPENAI001
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenAI.Application.Interfaces;
using OpenAI.Responses;
namespace GenAI.Infrastructure.AI
{
    public class OpenAIChatService : IChatService
    {
        private readonly ResponsesClient _client;
        private readonly string _systemPrompt;

        public OpenAIChatService(string apiKey, string systemPrompt)
        {
            _client = new ResponsesClient(apiKey);
            _systemPrompt = systemPrompt;
        }

        public async Task<string> GetResponseAsync(string message)
        {
            var response = await _client.CreateResponseAsync(
                "gpt-5.2",
                [
                    //ResponseItem.CreateUserMessageItem(message)
                    ResponseItem.CreateUserMessageItem(_systemPrompt),
                    
                   ResponseItem.CreateUserMessageItem(message)
                ]);

            return response.Value.GetOutputText();
        }
    }
}
