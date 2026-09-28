using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenAI.Application.Interfaces;


namespace GenAI.Infrastructure.Prompting
{
    public class PromptBuilder : IPromptBuilder
    {
        private readonly PromptOptions _promptOptions;

        public PromptBuilder(PromptOptions promptOptions)
        {
            _promptOptions = promptOptions;
        }
        public string BuildSystemPrompt()
        {
            return
           $"You are a {_promptOptions.Role}. " +
           $"Explain concepts at a {_promptOptions.Level} level. " +
           $"Use {_promptOptions.Language} examples when appropriate.";
        }
    }
}
