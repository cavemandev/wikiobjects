using Xunit;
using MongoDB.Entities;
using MongoDB.Driver;
using WikiObjects.Data.Model;
using WikiObjects.Data.ModelInterface;

namespace WikiTests
{
    public class UserTestContext : BaseTestContext
    {
        public UserTestContext() : base()
        {
        }
    }

    [Collection("Model")]
    public class UserTests: IClassFixture<UserTestContext>
    {
        private UserTestContext userTestContext;
        private UserInterface userInterface = new UserInterface();

        public UserTests(UserTestContext utc)
        {
            userTestContext = utc;
            DB.Delete<UserModel>(u => true);
        }

        [Fact]
        public void CreateUserTest()
        {
            string email = "adam.swank@gmail.com";
            string name= "Adam Swank";
            var user = userInterface.Create(name, email);

            var fetchedUser = userInterface.GetByEmail(email);

            Assert.NotNull(fetchedUser);

            Assert.Equal(user.Id, fetchedUser.Id);
            Assert.Equal(name, fetchedUser.Name);
            Assert.Equal(email, fetchedUser.Email);
        }

        [Fact]
        public void CreateUserTest2()
        {
            string email = "adam.swank@gmail.com";
            string name = "Adam Swank";
            var user = userInterface.Create(name, email);

            var fetchedUser = userInterface.GetById(user.Id);

            Assert.NotNull(fetchedUser);

            Assert.Equal(user.Id, fetchedUser.Id);
            Assert.Equal(name, fetchedUser.Name);
            Assert.Equal(email, fetchedUser.Email);
        }

        [Fact]
        public void GetNonExistentTest()
        {
            var fetchedUser = userInterface.GetByEmail("shouldntbethere@somewhere.com");

            Assert.Null(fetchedUser);
        }
    }
}
