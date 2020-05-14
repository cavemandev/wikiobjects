using MongoDB.Driver;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using WikiObjects.Data.Model;
using WikiObjects.Data.ModelInterface;
using Xunit;

namespace WikiTests
{
    public class TeamTestContext : BaseTestContext 
    {
        public List<User> users { get; set; } = new List<User>();
        public TeamTestContext() : base()
        {
            users.Add(UserInterface.Create("Sarah1", "Swank1", "sarah1.swank@gmail.com"));
            users.Add(UserInterface.Create("Sarah2", "Swank2", "sarah2.swank@gmail.com"));
            users.Add(UserInterface.Create("Sarah3", "Swank3", "sarah3.swank@gmail.com"));
            users.Add(UserInterface.Create("Sarah4", "Swank4", "sarah4.swank@gmail.com"));
            users.Add(UserInterface.Create("Sarah5", "Swank5", "sarah5.swank@gmail.com"));
            users.Add(UserInterface.Create("Sarah6", "Swank6", "sarah6.swank@gmail.com"));
            
            //    TeamInterface.AddMember(team, user2, MemberLists.Admins);
            //    TeamInterface.AddMember(team, user3, MemberLists.Admins);
            //    TeamInterface.AddMember(team, user4, MemberLists.Readers);
            //    TeamInterface.RemoveMember(team, user2, MemberLists.Admins);
            //    TeamInterface.AddMember(team, user5, MemberLists.Admins);

            //    bool isAdmin = TeamInterface.IsAdmin(team.ID, user2);
            //    isAdmin = TeamInterface.IsAdmin(team.ID, user3);
            //    isAdmin = TeamInterface.IsAdmin(team.ID, user);
            //    isAdmin = TeamInterface.IsAdmin(team.ID, user4);
        }
    }

    [Collection("Model")]
    public class TeamTests : IClassFixture<TeamTestContext>
    {
        TeamTestContext teamTestContext;

        public TeamTests(TeamTestContext ttc)
        {
            teamTestContext = ttc;
            DB.Delete<Team>(t => true);
        }
        
        [Fact]
        public void CreateTeamTest()
        {
            string teamName = "New Team";
            var team = TeamInterface.Create(teamName, teamTestContext.users[0]);

            var fetchedTeam = TeamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.ID, fetchedTeam.ID);

            Assert.Equal(teamName, fetchedTeam.name);
            Assert.Equal(fetchedTeam.acl.ownerId, teamTestContext.users[0].ID);
        }

        [Fact]
        public void DeleteTeamTest()
        {
            string teamName = "New Team";
            var team = TeamInterface.Create(teamName, teamTestContext.users[0]);

            var fetchedTeam = TeamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            long count = TeamInterface.Delete(team.ID);
            Assert.Equal(1, count);

            fetchedTeam = TeamInterface.GetByName(teamName);
            Assert.Null(fetchedTeam);
        }

        [Fact]
        public void AddTwoDeleteOneTest()
        {
            string teamName = "New Team";
            string teamName2 = "New Team 2";
            var team = TeamInterface.Create(teamName, teamTestContext.users[0]);
            var team2 = TeamInterface.Create(teamName2, teamTestContext.users[0]);

            var fetchedTeam = TeamInterface.GetByName(teamName);
            var fetchedTeam2 = TeamInterface.GetByName(teamName2);

            Assert.NotNull(fetchedTeam);
            Assert.NotNull(fetchedTeam2);

            long count = TeamInterface.Delete(team.ID);
            Assert.Equal(1, count);

            fetchedTeam = TeamInterface.GetByName(teamName);
            Assert.Null(fetchedTeam);
            fetchedTeam2 = TeamInterface.GetByName(teamName2);
            Assert.NotNull(fetchedTeam2);
        }

        [Fact]
        public void CreateDuplicateTeamTest()
        {
            string teamName = "New Team";
            var team = TeamInterface.Create(teamName, teamTestContext.users[0]);
            Assert.NotNull(team);

            Assert.Throws<MongoWriteException>(() => TeamInterface.Create(teamName, teamTestContext.users[0]));
        }

        [Fact]
        public void AddReader()
        {
            string teamName = "New Team";
            var team = TeamInterface.Create(teamName, teamTestContext.users[0]);
            Assert.NotNull(team);

            TeamInterface.AddMember(team.ID, teamTestContext.users[1], MemberList.readers);

            var fetchedTeam = TeamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.ID, fetchedTeam.ID);

            Assert.Equal(teamName, fetchedTeam.name);
            Assert.Equal(fetchedTeam.acl.ownerId, teamTestContext.users[0].ID);
            Assert.True(fetchedTeam.acl.readers.ContainsKey(teamTestContext.users[1].ID));
        }

        [Fact]
        public void AddAdmin()
        {
            string teamName = "New Team";
            var team = TeamInterface.Create(teamName, teamTestContext.users[0]);
            Assert.NotNull(team);

            TeamInterface.AddMember(team.ID, teamTestContext.users[1], MemberList.admins);

            var fetchedTeam = TeamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.ID, fetchedTeam.ID);

            Assert.Equal(teamName, fetchedTeam.name);
            Assert.Equal(fetchedTeam.acl.ownerId, teamTestContext.users[0].ID);
            Assert.True(fetchedTeam.acl.admins.ContainsKey(teamTestContext.users[1].ID));
        }

        [Fact]
        public void AddOwnerAsACLMember()
        {
            string teamName = "New Team";
            var team = TeamInterface.Create(teamName, teamTestContext.users[0]);
            Assert.NotNull(team);

            TeamInterface.AddMember(team.ID, teamTestContext.users[0], MemberList.admins);

            var fetchedTeam = TeamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.ID, fetchedTeam.ID);

            Assert.Equal(teamName, fetchedTeam.name);
            Assert.Equal(fetchedTeam.acl.ownerId, teamTestContext.users[0].ID);
            Assert.False(fetchedTeam.acl.admins.ContainsKey(teamTestContext.users[0].ID));
        }

        [Fact]
        public void AddRemoveMultiple()
        {
            string teamName = "New Team";
            var team = TeamInterface.Create(teamName, teamTestContext.users[0]);
            Assert.NotNull(team);

            TeamInterface.AddMember(team.ID, teamTestContext.users[1], MemberList.admins);
            TeamInterface.AddMember(team.ID, teamTestContext.users[2], MemberList.readers);
            TeamInterface.AddMember(team.ID, teamTestContext.users[3], MemberList.admins);
            TeamInterface.AddMember(team.ID, teamTestContext.users[4], MemberList.admins);
            TeamInterface.AddMember(team.ID, teamTestContext.users[5], MemberList.readers);

            var fetchedTeam = TeamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.ID, fetchedTeam.ID);

            Assert.Equal(teamName, fetchedTeam.name);
            Assert.Equal(fetchedTeam.acl.ownerId, teamTestContext.users[0].ID);
            Assert.True(fetchedTeam.acl.admins.ContainsKey(teamTestContext.users[1].ID));
            Assert.True(fetchedTeam.acl.readers.ContainsKey(teamTestContext.users[2].ID));
            Assert.True(fetchedTeam.acl.admins.ContainsKey(teamTestContext.users[3].ID));
            Assert.True(fetchedTeam.acl.admins.ContainsKey(teamTestContext.users[4].ID));
            Assert.True(fetchedTeam.acl.readers.ContainsKey(teamTestContext.users[5].ID));

            TeamInterface.RemoveMember(team.ID, teamTestContext.users[1]);
            TeamInterface.RemoveMember(team.ID, teamTestContext.users[5]);

            fetchedTeam = TeamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.ID, fetchedTeam.ID);

            Assert.Equal(teamName, fetchedTeam.name);
            Assert.Equal(fetchedTeam.acl.ownerId, teamTestContext.users[0].ID);
            Assert.False(fetchedTeam.acl.admins.ContainsKey(teamTestContext.users[1].ID));
            Assert.True(fetchedTeam.acl.readers.ContainsKey(teamTestContext.users[2].ID));
            Assert.True(fetchedTeam.acl.admins.ContainsKey(teamTestContext.users[3].ID));
            Assert.True(fetchedTeam.acl.admins.ContainsKey(teamTestContext.users[4].ID));
            Assert.False(fetchedTeam.acl.readers.ContainsKey(teamTestContext.users[5].ID));
        }
        
        [Fact]
        public void SwitchFromAdminToReader()
        {
            string teamName = "New Team";
            var team = TeamInterface.Create(teamName, teamTestContext.users[0]);
            Assert.NotNull(team);

            TeamInterface.AddMember(team.ID, teamTestContext.users[1], MemberList.admins);
            
            var fetchedTeam = TeamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.ID, fetchedTeam.ID);

            Assert.Equal(teamName, fetchedTeam.name);
            Assert.Equal(fetchedTeam.acl.ownerId, teamTestContext.users[0].ID);
            Assert.True(fetchedTeam.acl.admins.ContainsKey(teamTestContext.users[1].ID));
            Assert.False(fetchedTeam.acl.readers.ContainsKey(teamTestContext.users[1].ID));

            TeamInterface.AddMember(team.ID, teamTestContext.users[1], MemberList.readers);
            
            fetchedTeam = TeamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.ID, fetchedTeam.ID);

            Assert.Equal(teamName, fetchedTeam.name);
            Assert.Equal(fetchedTeam.acl.ownerId, teamTestContext.users[0].ID);
            Assert.False(fetchedTeam.acl.admins.ContainsKey(teamTestContext.users[1].ID));
            Assert.True(fetchedTeam.acl.readers.ContainsKey(teamTestContext.users[1].ID));
        }

        [Fact]
        public void ChangeOwner()
        {
            string teamName = "New Team";
            var team = TeamInterface.Create(teamName, teamTestContext.users[0]);
            Assert.NotNull(team);

            
            var fetchedTeam = TeamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.ID, fetchedTeam.ID);

            Assert.Equal(teamName, fetchedTeam.name);
            Assert.Equal(fetchedTeam.acl.ownerId, teamTestContext.users[0].ID);
            
            TeamInterface.ChangeOwner(team.ID, teamTestContext.users[1]);

            fetchedTeam = TeamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.ID, fetchedTeam.ID);

            Assert.Equal(teamName, fetchedTeam.name);
            Assert.Equal(fetchedTeam.acl.ownerId, teamTestContext.users[1].ID);
        }

        [Fact]
        public void AddTeamAndUser()
        {
            string teamName = "New Team";
            var team = TeamInterface.Create(teamName, teamTestContext.users[0]);
            Assert.NotNull(team);

            var team2 = TeamInterface.Create("Second Team", teamTestContext.users[0]);

            TeamInterface.AddMember(team.ID, teamTestContext.users[1], MemberList.admins);
            TeamInterface.AddMember(team.ID, team2, MemberList.admins);

            var fetchedTeam = TeamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.ID, fetchedTeam.ID);

            Assert.Equal(teamName, fetchedTeam.name);
            Assert.Equal(fetchedTeam.acl.ownerId, teamTestContext.users[0].ID);
            Assert.True(fetchedTeam.acl.admins.ContainsKey(teamTestContext.users[1].ID));
            Assert.Equal(MemberType.user, fetchedTeam.acl.admins[teamTestContext.users[1].ID]);
            Assert.True(fetchedTeam.acl.admins.ContainsKey(team2.ID));
            Assert.Equal(MemberType.team, fetchedTeam.acl.admins[team2.ID]);
        }
    }
}
