using MongoDB.Driver;
using MongoDB.Entities;
using System.Collections.Generic;
using WikiObjects.Data.Model;
using WikiObjects.Data.ModelInterface;

namespace WikiTests
{
    public class BaseTestContext
    {
        public List<User> adminUsers = new List<User>();
        public List<User> users { get; set; } = new List<User>();
        public Team adminTeam = null;
        public Team adminNestedTeam = null;

        public BaseTestContext()
        {
            new DB(MongoClientSettings.FromConnectionString(
                    "mongodb://root:asdfasdf@localhost:27025/"),
                    "test");

            ClearUsers();
            ClearTeams();
            ClearPages();
            ClearAttachments();

            UserModel.CreateIndices();
            TeamModel.CreateIndices();
            PageModel.CreateIndices();
        }

        public void CreateAdminTeam()
        {
            var userInterface = new UserInterface();
            var teamInterface = new TeamInterface();

            adminUsers = new List<User>();

            adminUsers.Add(userInterface.Create("Owner", "owner@wikiobjects.com"));
            adminUsers.Add(userInterface.Create("Admin1", "admin1@wikiobjects.com"));
            adminUsers.Add(userInterface.Create("Admin2", "admin2@wikiobjects.com"));
            adminUsers.Add(userInterface.Create("Admin3Nested", "admin3@wikiobjects.com"));
            adminUsers.Add(userInterface.Create("Admin4Nested", "admin4@wikiobjects.com"));

            adminTeam = teamInterface.Create(Team.AdminTeamName, adminUsers[0]);
            teamInterface.AddMember(adminTeam.Id, adminUsers[1], MemberList.admins);
            teamInterface.AddMember(adminTeam.Id, adminUsers[2], MemberList.readers);

            adminNestedTeam = teamInterface.Create("Nested Admin Team", adminUsers[0]);
            teamInterface.AddMember(adminNestedTeam.Id, adminUsers[3], MemberList.admins);
            teamInterface.AddMember(adminNestedTeam.Id, adminUsers[4], MemberList.readers);

            teamInterface.AddMember(adminTeam.Id, adminNestedTeam, MemberList.readers);
        }
        
        public void CreateUsers()
        {           
            UserInterface userInterface = new UserInterface();

            users = new List<User>();
            for (int i = 0; i < 30; i+=1)
            {
                users.Add(userInterface.Create(string.Format("user{0}", i), string.Format("user{0}@gmail.com", i)));
            }
        }

        public void ClearUsers()
        {
            DB.Delete<UserModel>(u => true);
        }

        public void ClearTeams()
        {
            DB.Delete<TeamModel>(u => true);
        }

        public void ClearPages()
        {
            DB.Delete<PageModel>(u => true);
        }

        public void ClearAttachments()
        {
            DB.Delete<AttachmentModel>(u => true);
        }
    }
}
