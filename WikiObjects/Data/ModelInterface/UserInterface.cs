using MongoDB.Driver;
using MongoDB.Entities;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using WikiObjects.Data.Model;

namespace WikiObjects.Data.ModelInterface
{
    public class UserInterface
    {
        public static User Create(string givenName, string lastName, string email)
        {
            User user = new User() { givenName = givenName, lastName = lastName, email = email };
            user.Save();

            return user;
        }

        public static User GetByEmail(string email)
        {
            var users = DB.Find<User>()
                .Match(u => u.email.Equals(email))
                .Limit(1)
                .Execute();

            return users.Count > 0 ? users.FirstOrDefault() : null;
        }

        public static bool Permissions(string userId)
        {
            // TODO: Do search among teams,
            // Call team permissions until exhausted
            // Cache result
            return true;
        }
    }
}
