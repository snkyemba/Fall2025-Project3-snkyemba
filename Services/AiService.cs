using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace Fall2025_Project3_snkyemba.Services
{
    public class AiService
    {
        private readonly AzureOpenAIClient _client;
        private readonly string _deploymentName;

        // Constructor accepts IConfiguration
        public AiService(IConfiguration configuration)
        {
            var endpoint = new Uri(configuration["AzureAI:Endpoint"]);
            var key = new AzureKeyCredential(configuration["AzureAI:ApiKey"]);
            _client = new AzureOpenAIClient(endpoint, key);
            _deploymentName = configuration["AzureAI:DeploymentName"] ?? "gpt-4o-mini";
        }

        public async Task<List<string>> GenerateMovieReviewsAsync(string movieTitle)
        {
            var chatClient = _client.GetChatClient(_deploymentName);

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a movie critic who writes concise reviews."),
                new UserChatMessage($"Write 3 short, distinct movie reviews for '{movieTitle}'. Number each review (1-3). Each review should be 2-3 sentences.")
            };

            var response = await chatClient.CompleteChatAsync(messages);

            // Parse the response to extract individual reviews
            var reviews = response.Value.Content[0].Text
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim())
                .Where(line => line.Length > 10) // Filter out very short lines
                .Take(3)
                .ToList();

            return reviews;
        }

        public async Task<List<string>> GenerateActorTweetsAsync(string actorName)
        {
            var chatClient = _client.GetChatClient(_deploymentName);

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a social media user who tweets about actors and movies."),
                new UserChatMessage($"Write 5 short tweets about the actor '{actorName}'. Number each tweet (1-5). Each tweet should be realistic and varied in tone.")
            };

            var response = await chatClient.CompleteChatAsync(messages);

            // Parse the response to extract individual tweets
            var tweets = response.Value.Content[0].Text
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim())
                .Where(line => line.Length > 10) // Filter out very short lines
                .Take(5)
                .ToList();

            return tweets;
        }
    }
}