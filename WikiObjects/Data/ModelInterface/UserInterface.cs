using MongoDB.Driver;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using WikiObjects.Data.Model;

namespace WikiObjects.Data.ModelInterface
{
    public class User : IAclMember, IApplyModel<UserModel, User>
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
        public enum UpdateFields
        {
            email,
            name
        }

        public User Create(string name, string email)
        {
            UserModel user = new UserModel() { name = name, email = email };
            user.Save();

            return User.FromUserModel(user);
        }

        public User GetByEmail(string email)
        {
            var users = DB.Find<UserModel>()
                .Match(u => u.email.Equals(email))
                .Limit(1)
                .Execute();

            return users.Count > 0 ? User.FromUserModel(users.FirstOrDefault()) : null;
        }

        public bool Permissions(string userId)
        {
            // TODO: Do search among teams,
            // Call team permissions until exhausted
            // Cache result
            return true;
        }

        public User Update(User user, Dictionary<string, string> content)
        {
            UserModel userModel = DB.Find<UserModel>().One(user.Id);

            if (userModel == null)
            {
                return null;
            }
            
            if (content.ContainsKey(UpdateFields.email.ToString()))
            {
                userModel.email = content[UpdateFields.email.ToString()];
            }

            if (content.ContainsKey(UpdateFields.name.ToString()))
            {
                userModel.name = content[UpdateFields.name.ToString()];
            }

            userModel.Save();
            
            return User.FromUserModel(userModel);
        }
    }
}
