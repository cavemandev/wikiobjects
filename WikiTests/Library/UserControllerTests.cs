using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using WikiObjects.Controllers;
using Xunit;

namespace WikiTests.Library
{
    public class UserTestContext : BaseTestContext
    {
        public UserTestContext() : base()
        {
        }
    }

    [Collection("Model")]
    public class UserControllerTests: IClassFixture<UserTestContext>
    {
        private UserTestContext userTestContext;
        private UserController userController = new UserController();

        public UserControllerTests(UserTestContext utc)
        {
            userTestContext = utc;
            userTestContext.ClearUsers();
        }

        [Fact]
        public void CreateUserTest()
        {
            string email = "adam.swank@gmail.com";
            string name = "Adam Swank";
            var user = userController.Create(name, email);

            var fetchedUser = userController.Get(user.Id);

            Assert.NotNull(fetchedUser);

            Assert.Equal(user.Id, fetchedUser.Id);
            Assert.Equal(name, fetchedUser.Name);
            Assert.Equal(email, fetchedUser.Email);
        }

        [Fact]
        public void Create2UsersTest()
        {
            string email = "adam.swank@gmail.com";
            string name = "Adam Swank";
            var user = userController.Create(name, email);
            var user2 = userController.Create("adam", "adam@adam.com");

            var fetchedUser = userController.Get(user2.Id);

            Assert.NotNull(fetchedUser);

            Assert.Equal(user2.Id, fetchedUser.Id);
            Assert.Equal("adam", fetchedUser.Name);
            Assert.Equal("adam@adam.com", fetchedUser.Email);
        }

        [Fact]
        public void ChangeEmailAndNameTest()
        {
            string email = "adam.swank@gmail.com";
            string name = "Adam Swank";
            var user = userController.Create(name, email);

            var fetchedUser = userController.Get(user.Id);

            Assert.NotNull(fetchedUser);

            var updates = new Dictionary<string, string>();
            updates.Add("email", "so@so.com");
            updates.Add("name", "me");

            userController.Update(user, updates);

            fetchedUser = userController.Get(user.Id);

            Assert.NotNull(fetchedUser);

            Assert.Equal(user.Id, fetchedUser.Id);
            Assert.Equal("so@so.com", fetchedUser.Email);
            Assert.Equal("me", fetchedUser.Name);
        }
    }
}
