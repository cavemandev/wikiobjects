using MongoDB.Driver;
using MongoDB.Entities;
using WikiObjects.Data.Model;
using WikiObjects.Data.ModelInterface;

namespace WikiObjects
{
    class Program
    {
        static void Main(string[] args)
        {
            new DB(MongoClientSettings.FromConnectionString(
                "mongodb://root:asdfasdf@localhost:27025/"),
                "test");

            DB.Delete<User>(u => true);
            DB.Delete<Team>(t => true);

            var user = UserInterface.Create("Adam", "Swank", "adam.swank@gmail.com");
            var user2 = UserInterface.Create("Sarah", "Swank", "sarah1.swank@gmail.com");
            var user3 = UserInterface.Create("Sarah", "Swank", "sarah2.swank@gmail.com");
            var user4 = UserInterface.Create("Sarah", "Swank", "sarah3.swank@gmail.com");
            var user5 = UserInterface.Create("Sarah", "Swank", "sarah4.swank@gmail.com");
            var user6 = UserInterface.Create("Sarah", "Swank", "sarah5.swank@gmail.com");
            var user7 = UserInterface.Create("Sarah", "Swank", "sarah6.swank@gmail.com");

            var team = TeamInterface.Create("New Team", user);

            TeamInterface.AddMember(team, user2, MemberList.admins);
            TeamInterface.AddMember(team, user3, MemberList.admins);
            TeamInterface.AddMember(team, user4, MemberList.readers);
            TeamInterface.RemoveMember(team, user2);
            TeamInterface.AddMember(team, user5, MemberList.admins);
        }
    }
}
