
namespace PWA.Auth.Services;
using dto.worldbet.Models;
using dto.worldbet.Requests;
public interface ISpecialBetService
{
    Task<SpecialBetStatusDto?> GetWinnerFinalStatusAsync();
    Task<bool> UpsertWinnerFinalAsync(string chosenValue);
}