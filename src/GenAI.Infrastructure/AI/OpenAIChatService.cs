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
        private readonly PromptOptions _promptOptions;

        public OpenAIChatService(string apiKey, PromptOptions promptOptions)
        {
            _client = new ResponsesClient(apiKey);
            _promptOptions = promptOptions;
        }

        public async Task<string> GetResponseAsync(string message)
        {
            var response = await _client.CreateResponseAsync(
                "gpt-5.2",
                [
                    //ResponseItem.CreateUserMessageItem(message)
                   // ResponseItem.CreateUserMessageItem(_promptOptions),
                    ResponseItem.CreateSystemMessageItem(
    $"You are a {_promptOptions.Role}. " +
    $"Explain concepts at a {_promptOptions.Level} level. " +
    $"Use {_promptOptions.Language} examples when appropriate."
),
                   ResponseItem.CreateUserMessageItem(message)
                ]);

            return response.Value.GetOutputText();
        }
    }
}
