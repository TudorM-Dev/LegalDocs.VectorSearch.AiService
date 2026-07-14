using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace LegalDocs.VectorSearch.Core
{
    public class LegalAiEngine
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatService;

        private ChatHistory _history;

        public LegalAiEngine()
        {
            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(
                modelId: "llama3",
                apiKey: "nu-avem-nevoie-de-cheie",
                endpoint: new Uri("http://localhost:11434/v1")
            );

            _kernel = builder.Build();
            _chatService = _kernel.GetRequiredService<IChatCompletionService>();

            _history = new ChatHistory();
        }

        public async Task<string> AnalyzeTextAsync(string userInput, string documentContext)
        {
            if (_history.Count == 0)
            {
                string systemInstructions = $@"You are an elite legal AI assistant. Use ONLY the provided LEGAL CONTEXT to analyze the case.
                                            NEVER invent laws. If the answer is not in the context, say 'I do not have sufficient legal information'.
                                            Always cite the specific Article (e.g., Art. 228).
                                            Reply in the same language as the user's input.

                                            LEGAL CONTEXT:
                                            {documentContext}";

                _history.AddSystemMessage(systemInstructions);
            }

            _history.AddUserMessage(userInput);

            var executionSettings = new OpenAIPromptExecutionSettings
            {
                Temperature = 0.0,
                TopP = 0.1
            };

            var result = await _chatService.GetChatMessageContentAsync(
                _history,
                executionSettings: executionSettings,
                kernel: _kernel
            );

            string finalResponse = result.Content ?? "Error: Unable to generate a response.";

            _history.AddAssistantMessage(finalResponse);

            return finalResponse;
        }
    }
}