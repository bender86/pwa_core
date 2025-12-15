using dto.user.Models;
using dto.common.Models;
using dto.user.Enums;

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
        Task<ApiResponse<List<UserDetailResponse>>> GetAllUsers();

        /// <summary>
        /// Changer le mot de passe
        /// </summary>
        Task<ApiResponse<object>> ChangePassword(ChangePasswordRequest request);

        /// <summary>
        /// Obtenir l'utilisateur connect�
        /// </summary>
        Task<UserDetailResponse?> GetCurrentUser();
        Task<ApiResponse<UserDetailResponse>> UpdateUserStatus(int userId, UserStatus newStatus);
        
        /// <summary>
        /// Supprimer un utilisateur
        /// </summary>
        Task<ApiResponse<bool>> DeleteUser(int userId);
        Task<ApiResponse<UserInvitationResponse>> CreateUserInvitation(CreateUserInvitationRequest request);
        Task<ApiResponse<UserDetailResponse>> CompleteUserRegistration(CompleteUserRegistrationRequest request);

    }
}