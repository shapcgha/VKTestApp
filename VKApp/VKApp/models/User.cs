using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace VKApp.models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [NotNull]
        public string Login { get; set; }
        [Required]
        [NotNull]
        public string Password { get; set; }

        public DateTime Created_date { get; set; }
        [Required]
        public int UserGroupId { get; set; }
        public UserGroup? UserGroup { get; set; }

        public int UserStateId { get; set; }
        public UserState? UserState { get; set; }
    }
}
