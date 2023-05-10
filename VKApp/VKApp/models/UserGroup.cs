using System.Text.Json.Serialization;

namespace VKApp.models
{
    public class UserGroup
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        [JsonIgnore]
        public ICollection<User> Users { get; set; }
    }
}
