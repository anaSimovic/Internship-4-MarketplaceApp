using Marketplace.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Data
{
    public class UserRepository
    {
        private readonly List<User> _users = new();

        public void AddUser(User user) => _users.Add(user);

        public User? GetUserByEmail(string email) =>
            _users.FirstOrDefault(u => u.Email == email);
    }
}
