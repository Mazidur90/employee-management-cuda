using System;
using System.Linq;
using SchoolMangementSystem.Data;
using SchoolMangementSystem.Models;

namespace SchoolMangementSystem.Services
{
    internal class AuthService
    {
        public AuthService()
        {
            EnsureSeedUser();
        }

        public string DefaultUsername => "admin";

        public string DefaultPasswordHint => "Admin@123";

        public bool TryLogin(string username, string password, out AppUser user)
        {
            var users = JsonFileStore.LoadList<AppUser>(AppPaths.UsersFile);
            var passwordHash = JsonFileStore.HashText(password);

            user = users.FirstOrDefault(candidate =>
                string.Equals(candidate.Username, username?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(candidate.PasswordHash, passwordHash, StringComparison.Ordinal));

            return user != null;
        }

        private void EnsureSeedUser()
        {
            var users = JsonFileStore.LoadList<AppUser>(AppPaths.UsersFile);

            if (users.Any())
            {
                return;
            }

            users.Add(new AppUser
            {
                Username = DefaultUsername,
                DisplayName = "Operations Admin",
                PasswordHash = JsonFileStore.HashText(DefaultPasswordHint)
            });

            JsonFileStore.SaveList(AppPaths.UsersFile, users);
        }
    }
}
