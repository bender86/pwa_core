using dto.auth.Models;
using dto.common.Models;
using dto.user.Models;
using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace PWA.Auth.Services;
public interface IApplicationService
{
    Task<ApiResponse<List<ApplicationDto>>> GetAllApplications();
}