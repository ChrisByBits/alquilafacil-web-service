using System.Text.Json.Serialization;
using AlquilaFacilPlatform.IAM.Domain.Model.ValueObjects;

namespace AlquilaFacilPlatform.IAM.Domain.Model.Aggregates
{
    public class User
    {
        public User(string username, string passwordHash, string email)
        {
            Username = username;
            PasswordHash = passwordHash;
            Email = email;
            RoleId = (int)EUserRoles.User;  // Valor predeterminado para un usuario normal.
        }

        public int Id { get; set; }
        public string Username { get; private set; }
        public string Email { get; set; }
        public int RoleId { get; set; }

        [JsonIgnore] 
        public string PasswordHash { get; private set; }

        public User UpdateUsername(string username)
        {
            Username = username;
            return this;
        }

        public User UpdatePasswordHash(string passwordHash)
        {
            PasswordHash = passwordHash;
            return this;
        }
        
        public void UpgradeToAdmin()
        {
            RoleId = (int)EUserRoles.Admin;
        }
    }
}