using dto.user.Models;
using dto.common.Models;

namespace PWA.Auth.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Cr�er un nouvel utilisateur (Admin uniquement)
        /// </summary>
        Task<ApiResponse<UserResponse>> CreateUser(CreateUserRequest request);

        /// <summary>
        /// Mettre � jour un utilisateur
        /// </summary>
        Task<ApiResponse<UserResponse>> UpdateUser(UpdateUserRequest request);

        /// <summary>
        /// Obtenir un utilisateur par ID
        /// </summary>
        Task<ApiResponse<UserDetailResponse>> GetUserById(int id);

        /// <summary>
        /// Obtenir tous les utilisateurs (Admin uniquement)
        /// </summary>
        Task<ApiResponse<List<UserResponse>>> GetAllUsers();

        /// <summary>
        /// Changer le mot de passe
        /// </summary>
        Task<ApiResponse<object>> ChangePassword(ChangePasswordRequest request);

        /// <summary>
        /// Obtenir l'utilisateur connect�
        /// </summary>
        Task<UserDetailResponse?> GetCurrentUser();
    }
}