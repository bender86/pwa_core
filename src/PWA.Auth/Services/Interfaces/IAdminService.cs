
using dto.worldbet.Requests;
namespace PWA.Auth.Services;

public interface IAdminService
{
    Task<bool> UpdateMatchScore(UpdateMatchScoreRequest request);
}