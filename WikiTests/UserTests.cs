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

        public UserTests(UserTestContext utc)
        {
            userTestContext = utc;
        }

        [Fact]
        public void CreateUserTest()
        {
            string email = "adam.swank@gmail.com";
            string givenName = "Adam";
            string lastName = "Swank";
            var user = UserInterface.Create(givenName, lastName, email);

            var fetchedUser = UserInterface.GetByEmail(email);

            Assert.NotNull(fetchedUser);

            Assert.Equal(user.ID, fetchedUser.ID);
            Assert.Equal(givenName, fetchedUser.givenName);
            Assert.Equal(lastName, fetchedUser.lastName);
            Assert.Equal(email, fetchedUser.email);
        }

        [Fact]
        public void GetNonExistentTest()
        {
            var fetchedUser = UserInterface.GetByEmail("shouldntbethere@somewhere.com");

            Assert.Null(fetchedUser);
        }
    }
}
