using MongoDB.Driver;
using MongoDB.Entities;
using System.Linq;
using WikiObjects.Data.Model;

namespace WikiObjects.Data.ModelInterface
{
    public class User : IACLMember, IApplyModel<UserModel, User>
    {
        public static User FromUserModel(UserModel um)
        {
            return new User() { Id = um.ID, Name = um.name, Email = um.email };
        }

        public void ApplyModel(UserModel um)
        {
            Id = um.ID;
            Name = um.name;
            Email = um.email;
        }

        public string Name { get; set; }
        public string Email { get; set; }

        public string Id { get; set; }
    }
    
    public class UserInterface : BaseInterface<UserModel, User>
    {
        public static User Create(string name, string email)
        {
            UserModel user = new UserModel() { name = name, email = email };
            user.Save();

            return User.FromUserModel(user);
        }

        public static User GetByEmail(string email)
        {
            var users = DB.Find<UserModel>()
                .Match(u => u.email.Equals(email))
                .Limit(1)
                .Execute();

            return users.Count > 0 ? User.FromUserModel(users.FirstOrDefault()) : null;
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
