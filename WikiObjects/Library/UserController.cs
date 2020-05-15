using System.Collections.Generic;
using WikiObjects.Data.ModelInterface;

namespace WikiObjects.Controllers
{
    public class UserController
    {
        private UserInterface userInterface = new UserInterface();

        
        public User Create(string name, string email)
        {
            return userInterface.Create(name, email);
        }

        
        public User Update(User subject, Dictionary<string, string> updates)
        {
            return userInterface.Update(subject, updates);   
        }

        public User Get(string userId)
        {
            return userInterface.GetById(userId);
        }
    }
}
