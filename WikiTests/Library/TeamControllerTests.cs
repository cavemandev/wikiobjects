using System.Collections.Generic;
using WikiObjects.Controllers;
using WikiObjects.Data.ModelInterface;
using WikiObjects.Library.Exceptions;
using Xunit;

namespace WikiTests.Library
{
    public class TeamTestContext : BaseTestContext
    {
        public TeamTestContext() : base()
        {
        }
    }

    [Collection("Model")]
    public class TeamControllerTests : IClassFixture<TeamTestContext>
    {
        private TeamTestContext testContext;
        private TeamController controller = new TeamController();

        public TeamControllerTests(TeamTestContext utc)
        {
            testContext = utc;

            testContext.ClearTeams();
            testContext.ClearUsers();

            testContext.CreateUsers();
        }

        private Team CreateNestedTeam()
        {
            var team = controller.Create("team", testContext.users[0]);
            controller.AddAdmin(team.Id, testContext.users[1].Id, testContext.users[0]);
            controller.AddReader(team.Id, testContext.users[2].Id, testContext.users[0]);

            var tnw = controller.Create("nested writer", testContext.users[0]);
            controller.AddAdmin(tnw.Id, testContext.users[3].Id, testContext.users[0]);
            controller.AddReader(tnw.Id, testContext.users[4].Id, testContext.users[0]);
            controller.AddAdmin(team.Id, tnw.Id, testContext.users[0]);

            var tnr = controller.Create("nested reader", testContext.users[0]);
            controller.AddAdmin(tnr.Id, testContext.users[5].Id, testContext.users[0]);
            controller.AddReader(tnr.Id, testContext.users[6].Id, testContext.users[0]);
            controller.AddReader(team.Id, tnr.Id, testContext.users[0]);

            return team;
        }

        [Fact]
        public void CreateTeamTest()
        {
            string teamName = "New Team";
            var team = controller.Create(teamName, testContext.users[0]);

            var fetchedTeam = controller.Get(team.Id, testContext.users[0]);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.Id, fetchedTeam.Id);

            Assert.Equal(teamName, fetchedTeam.Name);
            Assert.Equal(fetchedTeam.Acl.ownerId, testContext.users[0].Id);
        }

        [Fact]
        public void DeleteTeamTest()
        {
            string teamName = "New Team";
            var team = controller.Create(teamName, testContext.users[0]);

            var fetchedTeam = controller.Get(team.Id, testContext.users[0]);

            Assert.NotNull(fetchedTeam);

            long count = controller.Delete(team.Id, testContext.users[0]);
            Assert.Equal(1, count);

            Assert.Throws<NotAuthorized>(() => fetchedTeam = controller.Get(team.Id, testContext.users[0]));
        }

        [Fact]
        public void AddTwoDeleteOneTest()
        {
            string teamName = "New Team";
            string teamName2 = "New Team 2";
            var team = controller.Create(teamName, testContext.users[0]);
            var team2 = controller.Create(teamName2, testContext.users[0]);

            var fetchedTeam = controller.Get(team.Id, testContext.users[0]);
            var fetchedTeam2 = controller.Get(team2.Id, testContext.users[0]);

            Assert.NotNull(fetchedTeam);
            Assert.NotNull(fetchedTeam2);

            long count = controller.Delete(team.Id, testContext.users[0]);
            Assert.Equal(1, count);

            Assert.Throws<NotAuthorized>(() => controller.Get(team.Id, testContext.users[0]));
            fetchedTeam2 = controller.Get(team2.Id, testContext.users[0]);
            Assert.NotNull(fetchedTeam2);
        }

        [Fact]
        public void CreateDuplicateTeamTest()
        {
            string teamName = "New Team";
            var team = controller.Create(teamName, testContext.users[0]);
            Assert.NotNull(team);

            Assert.Throws<InvalidArugment>(() => controller.Create(teamName, testContext.users[0]));
        }

        [Fact]
        public void UpdateWithAdminUser()
        {
            testContext.CreateAdminTeam();

            var team = CreateNestedTeam();

            // Admin Team Owner
            var updates = new Dictionary<string, string>();
            updates.Add("name", "better");
            controller.Update(team.Id, updates, testContext.adminUsers[0]);
            var fetchedTeam = controller.Get(team.Id, testContext.adminUsers[0]);
            Assert.Equal("better", fetchedTeam.Name);

            // Admin Team Writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even writer");
            controller.Update(team.Id, updates, testContext.adminUsers[1]);
            fetchedTeam = controller.Get(team.Id, testContext.adminUsers[1]);
            Assert.Equal("even writer", fetchedTeam.Name);

            // Admin Team Nested reader team writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even reader");
            controller.Update(team.Id, updates, testContext.adminUsers[2]);
            fetchedTeam = controller.Get(team.Id, testContext.adminUsers[2]);
            Assert.Equal("even reader", fetchedTeam.Name);

            // Admin Team Writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even writer");
            controller.Update(team.Id, updates, testContext.adminUsers[3]);
            fetchedTeam = controller.Get(team.Id, testContext.adminUsers[3]);
            Assert.Equal("even writer", fetchedTeam.Name);

            // Admin Team Nested reader team writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even reader");
            controller.Update(team.Id, updates, testContext.adminUsers[4]);
            fetchedTeam = controller.Get(team.Id, testContext.adminUsers[4]);
            Assert.Equal("even reader", fetchedTeam.Name);

            // Team Owner reader team writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even owner");
            controller.Update(team.Id, updates, testContext.users[0]);
            fetchedTeam = controller.Get(team.Id, testContext.users[0]);
            Assert.Equal("even owner", fetchedTeam.Name);

            // Team Writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even writer");
            controller.Update(team.Id, updates, testContext.users[1]);
            fetchedTeam = controller.Get(team.Id, testContext.users[1]);
            Assert.Equal("even writer", fetchedTeam.Name);

            // Team Reader
            updates = new Dictionary<string, string>();
            updates.Add("name", "even reader ");
            Assert.Throws<NotAuthorized>(() => controller.Update(team.Id, updates, testContext.users[2]));
            fetchedTeam = controller.Get(team.Id, testContext.users[2]);
            Assert.Equal("even writer", fetchedTeam.Name);

            // Nested Team Writer writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even nested writer");
            controller.Update(team.Id, updates, testContext.users[3]);
            fetchedTeam = controller.Get(team.Id, testContext.users[3]);
            Assert.Equal("even nested writer", fetchedTeam.Name);

            // Nested Team Writer reader
            updates = new Dictionary<string, string>();
            updates.Add("name", "even nested reader");
            controller.Update(team.Id, updates, testContext.users[4]);
            fetchedTeam = controller.Get(team.Id, testContext.users[4]);
            Assert.Equal("even nested reader", fetchedTeam.Name);

            // Nested Team Reader writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even nested reader writer");
            Assert.Throws<NotAuthorized>(() => controller.Update(team.Id, updates, testContext.users[5]));
            fetchedTeam = controller.Get(team.Id, testContext.users[5]);
            Assert.Equal("even nested reader", fetchedTeam.Name);

            // Nested Team reader reader
            updates = new Dictionary<string, string>();
            updates.Add("name", "even nested reader reader");
            Assert.Throws<NotAuthorized>(() => controller.Update(team.Id, updates, testContext.users[6]));
            fetchedTeam = controller.Get(team.Id, testContext.users[6]);
            Assert.Equal("even nested reader", fetchedTeam.Name);

            updates = new Dictionary<string, string>();
            updates.Add("name", "no permissions");
            Assert.Throws<NotAuthorized>(() => controller.Update(team.Id, updates, testContext.users[7]));
            Assert.Throws<NotAuthorized>(() => fetchedTeam = controller.Get(team.Id, testContext.users[7]));
            Assert.Equal("even nested reader", fetchedTeam.Name);
        }

        [Fact]
        public void ChangeOwner()
        {
            testContext.CreateAdminTeam();

            var team = CreateNestedTeam();

            controller.ChangeOwner(team.Id, testContext.users[10].Id, testContext.adminUsers[0]);
            var fetchedTeam = controller.Get(team.Id, testContext.adminUsers[0]);
            Assert.Equal(testContext.users[10].Id, fetchedTeam.Acl.ownerId);

            // Admin Team Writer
            controller.ChangeOwner(team.Id, testContext.users[11].Id, testContext.adminUsers[1]);
            fetchedTeam = controller.Get(team.Id, testContext.adminUsers[1]);
            Assert.Equal(testContext.users[11].Id, fetchedTeam.Acl.ownerId);

            // Admin Team Nested reader team writer
            controller.ChangeOwner(team.Id, testContext.users[10].Id, testContext.adminUsers[2]);
            fetchedTeam = controller.Get(team.Id, testContext.adminUsers[2]);
            Assert.Equal(testContext.users[10].Id, fetchedTeam.Acl.ownerId);

            // Admin Team Writer
            controller.ChangeOwner(team.Id, testContext.users[11].Id, testContext.adminUsers[3]);
            fetchedTeam = controller.Get(team.Id, testContext.adminUsers[3]);
            Assert.Equal(testContext.users[11].Id, fetchedTeam.Acl.ownerId);

            // Admin Team Nested reader team writer
            controller.ChangeOwner(team.Id, testContext.users[10].Id, testContext.adminUsers[4]);
            fetchedTeam = controller.Get(team.Id, testContext.adminUsers[4]);
            Assert.Equal(testContext.users[10].Id, fetchedTeam.Acl.ownerId);

            // Team Owner reader team writer
            controller.ChangeOwner(team.Id, testContext.users[11].Id, testContext.users[0]);
            fetchedTeam = controller.Get(team.Id, testContext.users[0]);
            Assert.Equal(testContext.users[11].Id, fetchedTeam.Acl.ownerId);

            // Team Writer
            controller.ChangeOwner(team.Id, testContext.users[10].Id, testContext.users[1]);
            fetchedTeam = controller.Get(team.Id, testContext.users[1]);
            Assert.Equal(testContext.users[10].Id, fetchedTeam.Acl.ownerId);

            // Team Reader
            Assert.Throws<NotAuthorized>(() => controller.ChangeOwner(team.Id, testContext.users[11].Id, testContext.users[2]));
            fetchedTeam = controller.Get(team.Id, testContext.users[2]);
            Assert.Equal(testContext.users[10].Id, fetchedTeam.Acl.ownerId);

            // Nested Team Writer writer
            controller.ChangeOwner(team.Id, testContext.users[11].Id, testContext.users[3]);
            fetchedTeam = controller.Get(team.Id, testContext.users[3]);
            Assert.Equal(testContext.users[11].Id, fetchedTeam.Acl.ownerId);

            // Nested Team Writer reader
            controller.ChangeOwner(team.Id, testContext.users[10].Id, testContext.users[4]);
            fetchedTeam = controller.Get(team.Id, testContext.users[4]);
            Assert.Equal(testContext.users[10].Id, fetchedTeam.Acl.ownerId);

            // Nested Team Reader writerter");
            Assert.Throws<NotAuthorized>(() => controller.ChangeOwner(team.Id, testContext.users[11].Id, testContext.users[5]));
            fetchedTeam = controller.Get(team.Id, testContext.users[5]);
            Assert.Equal(testContext.users[10].Id, fetchedTeam.Acl.ownerId);

            // Nested Team reader readerder");
            Assert.Throws<NotAuthorized>(() => controller.ChangeOwner(team.Id, testContext.users[12].Id, testContext.users[6]));
            fetchedTeam = controller.Get(team.Id, testContext.users[6]);
            Assert.Equal(testContext.users[10].Id, fetchedTeam.Acl.ownerId);

            Assert.Throws<NotAuthorized>(() => controller.ChangeOwner(team.Id, testContext.users[13].Id, testContext.users[7]));
            Assert.Throws<NotAuthorized>(() => fetchedTeam = controller.Get(team.Id, testContext.users[7]));
            Assert.Equal(testContext.users[10].Id, fetchedTeam.Acl.ownerId);
        }

        [Fact]
        public void AddRemoveMember()
        {
            testContext.CreateAdminTeam();

            var team = CreateNestedTeam();

            controller.AddAdmin(team.Id, testContext.users[10].Id, testContext.adminUsers[0]);
            var fetchedTeam = controller.Get(team.Id, testContext.adminUsers[0]);
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));
            controller.RemoveMember(team.Id, testContext.users[10].Id, testContext.adminUsers[0]);
            fetchedTeam = controller.Get(team.Id, testContext.adminUsers[0]);
            Assert.False(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));

            // Admin Team Writer
            controller.AddAdmin(team.Id, testContext.users[10].Id, testContext.adminUsers[1]);
            fetchedTeam = controller.Get(team.Id, testContext.adminUsers[1]);
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));
            controller.RemoveMember(team.Id, testContext.users[10].Id, testContext.adminUsers[1]);
            fetchedTeam = controller.Get(team.Id, testContext.adminUsers[1]);
            Assert.False(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));

            // Admin Team Nested reader team writer
            controller.AddAdmin(team.Id, testContext.users[10].Id, testContext.adminUsers[2]);
            fetchedTeam = controller.Get(team.Id, testContext.adminUsers[2]);
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));
            controller.RemoveMember(team.Id, testContext.users[10].Id, testContext.adminUsers[2]);
            fetchedTeam = controller.Get(team.Id, testContext.adminUsers[2]);
            Assert.False(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));

            // Admin Team Writer
            controller.AddAdmin(team.Id, testContext.users[10].Id, testContext.adminUsers[3]);
            fetchedTeam = controller.Get(team.Id, testContext.adminUsers[3]);
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));
            controller.RemoveMember(team.Id, testContext.users[10].Id, testContext.adminUsers[3]);
            fetchedTeam = controller.Get(team.Id, testContext.adminUsers[3]);
            Assert.False(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));

            // Admin Team Nested reader team writer
            controller.AddAdmin(team.Id, testContext.users[10].Id, testContext.adminUsers[4]);
            fetchedTeam = controller.Get(team.Id, testContext.adminUsers[4]);
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));
            controller.RemoveMember(team.Id, testContext.users[10].Id, testContext.adminUsers[4]);
            fetchedTeam = controller.Get(team.Id, testContext.adminUsers[4]);
            Assert.False(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));

            // Team Owner reader team writer
            controller.AddAdmin(team.Id, testContext.users[10].Id, testContext.users[0]);
            fetchedTeam = controller.Get(team.Id, testContext.users[0]);
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));
            controller.RemoveMember(team.Id, testContext.users[10].Id, testContext.users[0]);
            fetchedTeam = controller.Get(team.Id, testContext.users[0]);
            Assert.False(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));

            // Team Writer
            controller.AddAdmin(team.Id, testContext.users[10].Id, testContext.users[1]);
            fetchedTeam = controller.Get(team.Id, testContext.users[1]);
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));
            controller.RemoveMember(team.Id, testContext.users[10].Id, testContext.users[1]);
            fetchedTeam = controller.Get(team.Id, testContext.users[1]);
            Assert.False(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));

            // Team Reader
            Assert.Throws<NotAuthorized>(() => controller.AddAdmin(team.Id, testContext.users[10].Id, testContext.users[2]));
            fetchedTeam = controller.Get(team.Id, testContext.users[2]);
            Assert.False(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));
            controller.AddAdmin(team.Id, testContext.users[10].Id, testContext.users[1]);
            Assert.Throws<NotAuthorized>(() => controller.RemoveMember(team.Id, testContext.users[10].Id, testContext.users[2]));
            fetchedTeam = controller.Get(team.Id, testContext.users[2]);
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));
            controller.RemoveMember(team.Id, testContext.users[10].Id, testContext.users[1]);

            // Nested Team Writer writer
            controller.AddAdmin(team.Id, testContext.users[10].Id, testContext.users[3]);
            fetchedTeam = controller.Get(team.Id, testContext.users[3]);
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));
            controller.RemoveMember(team.Id, testContext.users[10].Id, testContext.users[3]);
            fetchedTeam = controller.Get(team.Id, testContext.users[3]);
            Assert.False(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));

            // Nested Team Writer reader
            controller.AddAdmin(team.Id, testContext.users[10].Id, testContext.users[4]);
            fetchedTeam = controller.Get(team.Id, testContext.users[4]);
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));
            controller.RemoveMember(team.Id, testContext.users[10].Id, testContext.users[4]);
            fetchedTeam = controller.Get(team.Id, testContext.users[4]);
            Assert.False(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));

            // Nested Team Reader writer
            Assert.Throws<NotAuthorized>(() => controller.AddAdmin(team.Id, testContext.users[10].Id, testContext.users[5]));
            fetchedTeam = controller.Get(team.Id, testContext.users[5]);
            Assert.False(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));
            controller.AddAdmin(team.Id, testContext.users[10].Id, testContext.users[1]);
            Assert.Throws<NotAuthorized>(() => controller.RemoveMember(team.Id, testContext.users[10].Id, testContext.users[5]));
            fetchedTeam = controller.Get(team.Id, testContext.users[5]);
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));
            controller.RemoveMember(team.Id, testContext.users[10].Id, testContext.users[1]);

            // Nested Team reader readerder");
            Assert.Throws<NotAuthorized>(() => controller.AddAdmin(team.Id, testContext.users[10].Id, testContext.users[6]));
            fetchedTeam = controller.Get(team.Id, testContext.users[6]);
            Assert.False(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));
            controller.AddAdmin(team.Id, testContext.users[10].Id, testContext.users[1]);
            Assert.Throws<NotAuthorized>(() => controller.RemoveMember(team.Id, testContext.users[10].Id, testContext.users[6]));
            fetchedTeam = controller.Get(team.Id, testContext.users[6]);
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(testContext.users[10].Id));
            controller.RemoveMember(team.Id, testContext.users[10].Id, testContext.users[1]);

            Assert.Throws<NotAuthorized>(() => controller.AddAdmin(team.Id, testContext.users[10].Id, testContext.users[7]));
            Assert.Throws<NotAuthorized>(() => fetchedTeam = controller.Get(team.Id, testContext.users[7]));
            Assert.Throws<NotAuthorized>(() => controller.RemoveMember(team.Id, testContext.users[10].Id, testContext.users[7]));

        }
    }
}
