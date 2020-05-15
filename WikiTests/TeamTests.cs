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
    }

    [Collection("Model")]
    public class TeamTests : IClassFixture<TeamTestContext>
    {
        private TeamTestContext teamTestContext;
        private TeamInterface teamInterface = new TeamInterface();

        public TeamTests(TeamTestContext ttc)
        {
            teamTestContext = ttc;
            teamTestContext.ClearTeams();
            teamTestContext.ClearUsers();

            ttc.CreateUsers();
        }
        
        [Fact]
        public void CreateTeamTest()
        {
            string teamName = "New Team";
            var team = teamInterface.Create(teamName, teamTestContext.users[0]);

            var fetchedTeam = teamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.Id, fetchedTeam.Id);

            Assert.Equal(teamName, fetchedTeam.Name);
            Assert.Equal(fetchedTeam.Acl.ownerId, teamTestContext.users[0].Id);
        }

        [Fact]
        public void CreateTeamTest2()
        {
            string teamName = "New Team";
            var team = teamInterface.Create(teamName, teamTestContext.users[0]);

            var fetchedTeam = teamInterface.GetById(team.Id);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.Id, fetchedTeam.Id);

            Assert.Equal(teamName, fetchedTeam.Name);
            Assert.Equal(fetchedTeam.Acl.ownerId, teamTestContext.users[0].Id);
        }

        [Fact]
        public void DeleteTeamTest()
        {
            string teamName = "New Team";
            var team = teamInterface.Create(teamName, teamTestContext.users[0]);

            var fetchedTeam = teamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            long count = teamInterface.Delete(team.Id);
            Assert.Equal(1, count);

            fetchedTeam = teamInterface.GetByName(teamName);
            Assert.Null(fetchedTeam);
        }

        [Fact]
        public void AddTwoDeleteOneTest()
        {
            string teamName = "New Team";
            string teamName2 = "New Team 2";
            var team = teamInterface.Create(teamName, teamTestContext.users[0]);
            var team2 = teamInterface.Create(teamName2, teamTestContext.users[0]);

            var fetchedTeam = teamInterface.GetByName(teamName);
            var fetchedTeam2 = teamInterface.GetByName(teamName2);

            Assert.NotNull(fetchedTeam);
            Assert.NotNull(fetchedTeam2);

            long count = teamInterface.Delete(team.Id);
            Assert.Equal(1, count);

            fetchedTeam = teamInterface.GetByName(teamName);
            Assert.Null(fetchedTeam);
            fetchedTeam2 = teamInterface.GetByName(teamName2);
            Assert.NotNull(fetchedTeam2);
        }

        [Fact]
        public void CreateDuplicateTeamTest()
        {
            string teamName = "New Team";
            var team = teamInterface.Create(teamName, teamTestContext.users[0]);
            Assert.NotNull(team);

            Assert.Throws<MongoWriteException>(() => teamInterface.Create(teamName, teamTestContext.users[0]));
        }

        [Fact]
        public void AddReader()
        {
            string teamName = "New Team";
            var team = teamInterface.Create(teamName, teamTestContext.users[0]);
            Assert.NotNull(team);

            teamInterface.AddMember(team.Id, teamTestContext.users[1], MemberList.readers);

            var fetchedTeam = teamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.Id, fetchedTeam.Id);

            Assert.Equal(teamName, fetchedTeam.Name);
            Assert.Equal(fetchedTeam.Acl.ownerId, teamTestContext.users[0].Id);
            Assert.True(fetchedTeam.Acl.readers.ContainsKey(teamTestContext.users[1].Id));
        }

        [Fact]
        public void AddAdmin()
        {
            string teamName = "New Team";
            var team = teamInterface.Create(teamName, teamTestContext.users[0]);
            Assert.NotNull(team);

            teamInterface.AddMember(team.Id, teamTestContext.users[1], MemberList.admins);

            var fetchedTeam = teamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.Id, fetchedTeam.Id);

            Assert.Equal(teamName, fetchedTeam.Name);
            Assert.Equal(fetchedTeam.Acl.ownerId, teamTestContext.users[0].Id);
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(teamTestContext.users[1].Id));
        }

        [Fact]
        public void AddOwnerAsACLMember()
        {
            string teamName = "New Team";
            var team = teamInterface.Create(teamName, teamTestContext.users[0]);
            Assert.NotNull(team);

            teamInterface.AddMember(team.Id, teamTestContext.users[0], MemberList.admins);

            var fetchedTeam = teamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.Id, fetchedTeam.Id);

            Assert.Equal(teamName, fetchedTeam.Name);
            Assert.Equal(fetchedTeam.Acl.ownerId, teamTestContext.users[0].Id);
            Assert.False(fetchedTeam.Acl.admins.ContainsKey(teamTestContext.users[0].Id));
        }

        [Fact]
        public void AddRemoveMultiple()
        {
            string teamName = "New Team";
            var team = teamInterface.Create(teamName, teamTestContext.users[0]);
            var team2 = teamInterface.Create("team 2", teamTestContext.users[0]);
            Assert.NotNull(team);

            teamInterface.AddMember(team.Id, teamTestContext.users[1], MemberList.admins);
            teamInterface.AddMember(team.Id, teamTestContext.users[2], MemberList.readers);
            teamInterface.AddMember(team.Id, teamTestContext.users[3], MemberList.admins);
            teamInterface.AddMember(team.Id, teamTestContext.users[4], MemberList.admins);
            teamInterface.AddMember(team.Id, teamTestContext.users[5], MemberList.readers);

            var fetchedTeam = teamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.Id, fetchedTeam.Id);

            Assert.Equal(teamName, fetchedTeam.Name);
            Assert.Equal(fetchedTeam.Acl.ownerId, teamTestContext.users[0].Id);
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(teamTestContext.users[1].Id));
            Assert.True(fetchedTeam.Acl.readers.ContainsKey(teamTestContext.users[2].Id));
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(teamTestContext.users[3].Id));
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(teamTestContext.users[4].Id));
            Assert.True(fetchedTeam.Acl.readers.ContainsKey(teamTestContext.users[5].Id));

            teamInterface.RemoveMember(team.Id, teamTestContext.users[1]);
            teamInterface.RemoveMember(team.Id, teamTestContext.users[5]);

            fetchedTeam = teamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.Id, fetchedTeam.Id);

            Assert.Equal(teamName, fetchedTeam.Name);
            Assert.Equal(fetchedTeam.Acl.ownerId, teamTestContext.users[0].Id);
            Assert.False(fetchedTeam.Acl.admins.ContainsKey(teamTestContext.users[1].Id));
            Assert.True(fetchedTeam.Acl.readers.ContainsKey(teamTestContext.users[2].Id));
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(teamTestContext.users[3].Id));
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(teamTestContext.users[4].Id));
            Assert.False(fetchedTeam.Acl.readers.ContainsKey(teamTestContext.users[5].Id));

            fetchedTeam = teamInterface.GetById(team2.Id);

            Assert.Empty(fetchedTeam.Acl.readers);
            Assert.Empty(fetchedTeam.Acl.admins);
        }
        
        [Fact]
        public void SwitchFromAdminToReader()
        {
            string teamName = "New Team";
            var team = teamInterface.Create(teamName, teamTestContext.users[0]);
            Assert.NotNull(team);

            teamInterface.AddMember(team.Id, teamTestContext.users[1], MemberList.admins);
            
            var fetchedTeam = teamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.Id, fetchedTeam.Id);

            Assert.Equal(teamName, fetchedTeam.Name);
            Assert.Equal(fetchedTeam.Acl.ownerId, teamTestContext.users[0].Id);
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(teamTestContext.users[1].Id));
            Assert.False(fetchedTeam.Acl.readers.ContainsKey(teamTestContext.users[1].Id));

            teamInterface.AddMember(team.Id, teamTestContext.users[1], MemberList.readers);
            
            fetchedTeam = teamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.Id, fetchedTeam.Id);

            Assert.Equal(teamName, fetchedTeam.Name);
            Assert.Equal(fetchedTeam.Acl.ownerId, teamTestContext.users[0].Id);
            Assert.False(fetchedTeam.Acl.admins.ContainsKey(teamTestContext.users[1].Id));
            Assert.True(fetchedTeam.Acl.readers.ContainsKey(teamTestContext.users[1].Id));
        }

        [Fact]
        public void ChangeOwner()
        {
            string teamName = "New Team";
            var team = teamInterface.Create(teamName, teamTestContext.users[0]);
            Assert.NotNull(team);

            
            var fetchedTeam = teamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.Id, fetchedTeam.Id);

            Assert.Equal(teamName, fetchedTeam.Name);
            Assert.Equal(fetchedTeam.Acl.ownerId, teamTestContext.users[0].Id);
            
            teamInterface.ChangeOwner(team.Id, teamTestContext.users[1]);

            fetchedTeam = teamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.Id, fetchedTeam.Id);

            Assert.Equal(teamName, fetchedTeam.Name);
            Assert.Equal(fetchedTeam.Acl.ownerId, teamTestContext.users[1].Id);
        }

        [Fact]
        public void AddTeamAndUser()
        {
            string teamName = "New Team";
            var team = teamInterface.Create(teamName, teamTestContext.users[0]);
            Assert.NotNull(team);

            var team2 = teamInterface.Create("Second Team", teamTestContext.users[0]);

            teamInterface.AddMember(team.Id, teamTestContext.users[1], MemberList.admins);
            teamInterface.AddMember(team.Id, team2, MemberList.admins);

            var fetchedTeam = teamInterface.GetByName(teamName);

            Assert.NotNull(fetchedTeam);

            Assert.Equal(team.Id, fetchedTeam.Id);

            Assert.Equal(teamName, fetchedTeam.Name);
            Assert.Equal(fetchedTeam.Acl.ownerId, teamTestContext.users[0].Id);
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(teamTestContext.users[1].Id));
            Assert.Equal(MemberType.user, fetchedTeam.Acl.admins[teamTestContext.users[1].Id]);
            Assert.True(fetchedTeam.Acl.admins.ContainsKey(team2.Id));
            Assert.Equal(MemberType.team, fetchedTeam.Acl.admins[team2.Id]);
        }
    }
}
