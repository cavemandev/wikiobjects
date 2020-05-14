using WikiObjects.Data.ModelInterface;

namespace WikiObjects.Controllers
{
    public class UserController
    {
        public User Create(string name, string email)
        {
            return UserInterface.Create(name, email);
        }

        //public User Update(string name)
        //{
        //    return UserInterface.Update(name);
        //}

        public User Get(string userId)
        {
            return UserInterface.GetById(userId);
        }
    }
}
