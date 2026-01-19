// Models/UserFormModel.cs
using System.ComponentModel.DataAnnotations;

namespace PWA.Auth.Models
{
    public class UserFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères")]
        public string? Password { get; set; }

        public bool IsAdmin { get; set; } = false;
        public bool Active { get; set; } = true;
        public bool MfaEnabled { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}