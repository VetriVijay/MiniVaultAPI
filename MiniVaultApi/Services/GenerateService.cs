using MiniVaultApi.Models;
using System.Text;

namespace MiniVaultApi.Services;

public class GenerateService : IGenerateService
{
    private readonly ILogger<GenerateService> _logger;
  private readonly HttpClient _httpClient;
    private readonly LlmSettings _llmSettings;

        public GenerateService(ILogger<GenerateService> logger, HttpClient httpClient, IOptions<LlmSettings> options)
        {
            _logger = logger;
             _httpClient = httpClient;
            _llmSettings = options.Value;
        }

        public async Task<GenerateResponse> GenerateAsync(GenerateRequest req)
        {
            try
            {
                var requestContent = new StringContent(
                    $"{{\"model\":\"llama2\",\"prompt\":\"{req.Prompt}\"}}",
                    Encoding.UTF8,
                    "application/json");

                var endpoint = $"{_llmSettings.BaseUrl}/generate";

                var response = await _httpClient.PostAsync(endpoint, requestContent);
                
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream);
                var stringBuilder = new StringBuilder();

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var json = System.Text.Json.JsonDocument.Parse(line);
                        if (json.RootElement.TryGetProperty("response", out var token))
                        {
                            stringBuilder.Append(token.GetString());
                        }
                    }
                }

                var finalResponse = stringBuilder.ToString();
                _logger.LogInformation("GenerateAsync Request: {Request} | Response: {Response}", req.Prompt, finalResponse);

                return new GenerateResponse { Response = finalResponse };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling local LLM");
                return new GenerateResponse { Response = $"Stub: {req.Prompt}" };
            }
        }

        

        public async IAsyncEnumerable<string> StreamGenerateAsync(GenerateRequest req)
        {
             _logger.LogInformation("StreamGenerateAsync called with prompt: {Prompt}", req.Prompt);

            HttpResponseMessage response = null;
            Exception caughtException = null;

            try
            {
                    var requestContent = new StringContent(
                    $"{{\"model\":\"llama2\",\"prompt\":\"{req.Prompt}\"}}",
                    Encoding.UTF8,
                    "application/json");

                     var endpoint = $"{_llmSettings.BaseUrl}/generate";

                    var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
                    {
                        Content = requestContent
                    };

                response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            if (caughtException != null)
            {
                _logger.LogError(caughtException, "Error starting stream from local LLM");
                yield break;
            }

            var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var json = System.Text.Json.JsonDocument.Parse(line);
                    if (json.RootElement.TryGetProperty("response", out var token))
                    {
                        var text = token.GetString();
                        if (!string.IsNullOrEmpty(text))
                        {
                            _logger.LogInformation("Streaming token: {Token}", text);
                            yield return text;
                        }
                    }
                }
            }

        _logger.LogInformation("StreamGenerateAsync completed for prompt: {Prompt}", req.Prompt);

    }
}
