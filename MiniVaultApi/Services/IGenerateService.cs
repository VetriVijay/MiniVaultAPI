using MiniVaultApi.Models;

namespace MiniVaultApi.Services;

public interface IGenerateService
{
        Task<GenerateResponse> GenerateAsync(GenerateRequest req);
        IAsyncEnumerable<string> StreamGenerateAsync(GenerateRequest req);
}