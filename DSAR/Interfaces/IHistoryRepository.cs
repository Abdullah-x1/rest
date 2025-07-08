using System.Threading.Tasks;
using DSAR.Models;

namespace DSAR.Interfaces
{
    public interface IAppHistoryRepository
    {
        Task CreateHistoryAsync(User user, FormData request, int statusId, int levelId, string information);

        Task<List<History>> GetHistroyRequestsByUserId(string UserId);
        Task<List<History>> GetAllHistroyRequestsByUserId(string UserId);

        Task<List<History>> GetHistoriesByRequestIdAsync(int requestId);


    }
}
