using MartinKulev.Dtos.AI;

namespace MartinKulev.Services.AI
{
    public interface IAIService
    {
        Task<string> GetAgentResponse(string prompt, string instructions);
    }
}
