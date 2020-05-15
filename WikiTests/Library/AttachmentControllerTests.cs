using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WikiObjects.Controllers;
using WikiObjects.Data.ModelInterface;
using WikiObjects.Library;
using WikiObjects.Library.Exceptions;
using Xunit;

namespace WikiTests.Library
{
    public class AttachmentTestContext : BaseTestContext
    {
        public AttachmentTestContext() : base()
        {
        }
    }

    [Collection("Model")]
    public class AttachmentControllerTests : IClassFixture<AttachmentTestContext>
    {
        private AttachmentTestContext testContext;
        private AttachmentController controller = new AttachmentController();

        public AttachmentControllerTests(AttachmentTestContext utc)
        {
            testContext = utc;

            testContext.ClearTeams();
            testContext.ClearUsers();
            testContext.ClearPages();
            testContext.ClearAttachments();

            testContext.CreateUsers();
        }

        private Page CreateNestedPage()
        {
            PageController pc = new PageController();
            var page = pc.Create("page", testContext.users[0]);
            var pageSub1 = pc.AddSubPage("sub page 1", page.Id, testContext.users[0]);
            var pageSub2 = pc.AddSubPage("sub page 2", page.Id, testContext.users[0]);

            var p = pc.AddSubPage("sub 1 sub page 1", pageSub1.Id, testContext.users[0]);
            pc.ChangeOwner(p.Id, testContext.users[17].Id, testContext.users[0]);

            pc.AddSubPage("sub 2 sub page 1", pageSub2.Id, testContext.users[0]);

            var t = pc.AddAttachment("sub 1 attachment 1", pageSub1.Id, testContext.users[0]);
            AttachmentController ac = new AttachmentController();
            ac.ChangeOwner(t.Id, testContext.users[25].Id, testContext.users[0]);

            pc.AddAttachment("page attachment", page.Id, testContext.users[0]);

            var teams = CreateTeams();

            pc.AddAdmin(page.Id, teams[0].Id, testContext.users[0]);
            pc.AddReader(page.Id, teams[1].Id, testContext.users[0]);

            pc.AddAdmin(page.Id, testContext.users[10].Id, testContext.users[0]);
            pc.AddReader(page.Id, testContext.users[11].Id, testContext.users[0]);

            return page;
        }

        private List<Team> CreateTeams()
        {
            TeamController tc = new TeamController();
            var tnw = tc.Create("writer team", testContext.users[0]);
            tc.AddAdmin(tnw.Id, testContext.users[3].Id, testContext.users[0]);
            tc.AddReader(tnw.Id, testContext.users[4].Id, testContext.users[0]);


            var tnr = tc.Create("reader team", testContext.users[0]);
            tc.AddAdmin(tnr.Id, testContext.users[5].Id, testContext.users[0]);
            tc.AddReader(tnr.Id, testContext.users[6].Id, testContext.users[0]);

            var teams = new List<Team>();
            teams.Add(tnw);
            teams.Add(tnr);

            return teams;
        }

        [Fact]
        public void Update()
        {
            testContext.CreateAdminTeam();
            var page = CreateNestedPage();
            PageController pc = new PageController();
            var children = pc.GetChildren(page.Id, testContext.adminUsers[0]);
            var subPage = children.Item1[0];
            var children2 = pc.GetChildren(subPage.Id, testContext.adminUsers[0]);
            var att = children2.Item2[0];

            // Admin Team Owner
            var updates = new Dictionary<string, string>();
            updates.Add("name", "better");
            controller.Update(att.Id, updates, testContext.adminUsers[0]);
            var fetchedPage = controller.Get(att.Id, testContext.adminUsers[0]);
            Assert.Equal("better", fetchedPage.Name);

            // Admin Team Writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even writer");
            controller.Update(att.Id, updates, testContext.adminUsers[1]);
            fetchedPage = controller.Get(att.Id, testContext.adminUsers[1]);
            Assert.Equal("even writer", fetchedPage.Name);

            // Admin Team Nested reader team writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even reader");
            controller.Update(att.Id, updates, testContext.adminUsers[2]);
            fetchedPage = controller.Get(att.Id, testContext.adminUsers[2]);
            Assert.Equal("even reader", fetchedPage.Name);

            // Admin Team Writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even writer");
            controller.Update(att.Id, updates, testContext.adminUsers[3]);
            fetchedPage = controller.Get(att.Id, testContext.adminUsers[3]);
            Assert.Equal("even writer", fetchedPage.Name);

            // Admin Team Nested reader team writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even reader");
            controller.Update(att.Id, updates, testContext.adminUsers[4]);
            fetchedPage = controller.Get(att.Id, testContext.adminUsers[4]);
            Assert.Equal("even reader", fetchedPage.Name);

            // Team Owner reader team writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even owner");
            controller.Update(att.Id, updates, testContext.users[0]);
            fetchedPage = controller.Get(att.Id, testContext.users[0]);
            Assert.Equal("even owner", fetchedPage.Name);

            // Team Writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even writer");
            controller.Update(att.Id, updates, testContext.users[10]);
            fetchedPage = controller.Get(att.Id, testContext.users[10]);
            Assert.Equal("even writer", fetchedPage.Name);

            // Team Reader
            updates = new Dictionary<string, string>();
            updates.Add("name", "even reader ");
            Assert.Throws<NotAuthorized>(() => controller.Update(att.Id, updates, testContext.users[11]));
            fetchedPage = controller.Get(att.Id, testContext.users[11]);
            Assert.Equal("even writer", fetchedPage.Name);

            // Nested Team Writer writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even nested writer");
            controller.Update(att.Id, updates, testContext.users[3]);
            fetchedPage = controller.Get(att.Id, testContext.users[3]);
            Assert.Equal("even nested writer", fetchedPage.Name);

            // Nested Team Writer reader
            updates = new Dictionary<string, string>();
            updates.Add("name", "even nested reader");
            controller.Update(att.Id, updates, testContext.users[4]);
            fetchedPage = controller.Get(att.Id, testContext.users[4]);
            Assert.Equal("even nested reader", fetchedPage.Name);

            // Nested Team Reader writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even nested reader writer");
            Assert.Throws<NotAuthorized>(() => controller.Update(att.Id, updates, testContext.users[5]));
            fetchedPage = controller.Get(att.Id, testContext.users[5]);
            Assert.Equal("even nested reader", fetchedPage.Name);

            // Nested Team reader reader
            updates = new Dictionary<string, string>();
            updates.Add("name", "even nested reader reader");
            Assert.Throws<NotAuthorized>(() => controller.Update(att.Id, updates, testContext.users[6]));
            fetchedPage = controller.Get(att.Id, testContext.users[6]);
            Assert.Equal("even nested reader", fetchedPage.Name);

            updates = new Dictionary<string, string>();
            updates.Add("name", "no permissions");
            Assert.Throws<NotAuthorized>(() => controller.Update(att.Id, updates, testContext.users[7]));
            Assert.Throws<NotAuthorized>(() => fetchedPage = controller.Get(att.Id, testContext.users[7]));
            Assert.Equal("even nested reader", fetchedPage.Name);

            // Check the owner
            updates = new Dictionary<string, string>();
            updates.Add("name", "owner for the win");
            controller.Update(att.Id, updates, testContext.users[25]);
            fetchedPage = controller.Get(att.Id, testContext.users[25]);
            Assert.Equal("owner for the win", fetchedPage.Name);
        }

        [Fact]
        public void ChangeOwner()
        {
            testContext.CreateAdminTeam();
            var page = CreateNestedPage();
            PageController pc = new PageController();
            var children = pc.GetChildren(page.Id, testContext.adminUsers[0]);
            var subPage = children.Item1[0];
            var children2 = pc.GetChildren(subPage.Id, testContext.adminUsers[0]);
            var att = children2.Item2[0];

            controller.ChangeOwner(att.Id, testContext.users[14].Id, testContext.adminUsers[0]);
            var fetchedPage = controller.Get(att.Id, testContext.adminUsers[0]);
            Assert.Equal(testContext.users[14].Id, fetchedPage.Acl.ownerId);

            // Admin Team Writer
            controller.ChangeOwner(att.Id, testContext.users[15].Id, testContext.adminUsers[1]);
            fetchedPage = controller.Get(att.Id, testContext.adminUsers[1]);
            Assert.Equal(testContext.users[15].Id, fetchedPage.Acl.ownerId);

            // Admin Team Nested reader team writer
            controller.ChangeOwner(att.Id, testContext.users[14].Id, testContext.adminUsers[2]);
            fetchedPage = controller.Get(att.Id, testContext.adminUsers[2]);
            Assert.Equal(testContext.users[14].Id, fetchedPage.Acl.ownerId);

            // Admin Team Writer
            controller.ChangeOwner(att.Id, testContext.users[15].Id, testContext.adminUsers[3]);
            fetchedPage = controller.Get(att.Id, testContext.adminUsers[3]);
            Assert.Equal(testContext.users[15].Id, fetchedPage.Acl.ownerId);

            // Admin Team Nested reader team writer
            controller.ChangeOwner(att.Id, testContext.users[14].Id, testContext.adminUsers[4]);
            fetchedPage = controller.Get(att.Id, testContext.adminUsers[4]);
            Assert.Equal(testContext.users[14].Id, fetchedPage.Acl.ownerId);

            // Team Owner reader team writer
            controller.ChangeOwner(att.Id, testContext.users[15].Id, testContext.users[0]);
            fetchedPage = controller.Get(att.Id, testContext.users[0]);
            Assert.Equal(testContext.users[15].Id, fetchedPage.Acl.ownerId);

            // Team Writer
            controller.ChangeOwner(att.Id, testContext.users[14].Id, testContext.users[10]);
            fetchedPage = controller.Get(att.Id, testContext.users[10]);
            Assert.Equal(testContext.users[14].Id, fetchedPage.Acl.ownerId);

            // Team Reader
            Assert.Throws<NotAuthorized>(() => controller.ChangeOwner(att.Id, testContext.users[15].Id, testContext.users[11]));
            fetchedPage = controller.Get(att.Id, testContext.users[11]);
            Assert.Equal(testContext.users[14].Id, fetchedPage.Acl.ownerId);

            // Nested Team Writer writer
            controller.ChangeOwner(att.Id, testContext.users[15].Id, testContext.users[3]);
            fetchedPage = controller.Get(att.Id, testContext.users[3]);
            Assert.Equal(testContext.users[15].Id, fetchedPage.Acl.ownerId);

            // Nested Team Writer reader
            controller.ChangeOwner(att.Id, testContext.users[14].Id, testContext.users[4]);
            fetchedPage = controller.Get(att.Id, testContext.users[4]);
            Assert.Equal(testContext.users[14].Id, fetchedPage.Acl.ownerId);

            // Nested Team Reader writerter");
            Assert.Throws<NotAuthorized>(() => controller.ChangeOwner(att.Id, testContext.users[15].Id, testContext.users[5]));
            fetchedPage = controller.Get(att.Id, testContext.users[5]);
            Assert.Equal(testContext.users[14].Id, fetchedPage.Acl.ownerId);

            // Nested Team reader readerder");
            Assert.Throws<NotAuthorized>(() => controller.ChangeOwner(att.Id, testContext.users[16].Id, testContext.users[6]));
            fetchedPage = controller.Get(att.Id, testContext.users[6]);
            Assert.Equal(testContext.users[14].Id, fetchedPage.Acl.ownerId);

            Assert.Throws<NotAuthorized>(() => controller.ChangeOwner(att.Id, testContext.users[17].Id, testContext.users[7]));
            Assert.Throws<NotAuthorized>(() => fetchedPage = controller.Get(att.Id, testContext.users[7]));
            Assert.Equal(testContext.users[14].Id, fetchedPage.Acl.ownerId);
        }

        [Fact]
        public void AddRemoveMember()
        {
            testContext.CreateAdminTeam();
            var page = CreateNestedPage();
            PageController pc = new PageController();
            var children = pc.GetChildren(page.Id, testContext.adminUsers[0]);
            var subPage = children.Item1[0];
            var children2 = pc.GetChildren(subPage.Id, testContext.adminUsers[0]);
            var att = children2.Item2[0];

            controller.AddAdmin(att.Id, testContext.users[20].Id, testContext.adminUsers[0]);
            var fetchedPage = controller.Get(att.Id, testContext.adminUsers[0]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(att.Id, testContext.users[20].Id, testContext.adminUsers[0]);
            fetchedPage = controller.Get(att.Id, testContext.adminUsers[0]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));

            // Admin Team Writer
            controller.AddAdmin(att.Id, testContext.users[20].Id, testContext.adminUsers[1]);
            fetchedPage = controller.Get(att.Id, testContext.adminUsers[1]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(att.Id, testContext.users[20].Id, testContext.adminUsers[1]);
            fetchedPage = controller.Get(att.Id, testContext.adminUsers[1]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));

            // Admin Team Nested reader team writer
            controller.AddAdmin(att.Id, testContext.users[20].Id, testContext.adminUsers[2]);
            fetchedPage = controller.Get(att.Id, testContext.adminUsers[2]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(att.Id, testContext.users[20].Id, testContext.adminUsers[2]);
            fetchedPage = controller.Get(att.Id, testContext.adminUsers[2]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));

            // Admin Team Writer
            controller.AddAdmin(att.Id, testContext.users[20].Id, testContext.adminUsers[3]);
            fetchedPage = controller.Get(att.Id, testContext.adminUsers[3]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(att.Id, testContext.users[20].Id, testContext.adminUsers[3]);
            fetchedPage = controller.Get(att.Id, testContext.adminUsers[3]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));

            // Admin Team Nested reader team writer
            controller.AddAdmin(att.Id, testContext.users[20].Id, testContext.adminUsers[4]);
            fetchedPage = controller.Get(att.Id, testContext.adminUsers[4]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(att.Id, testContext.users[20].Id, testContext.adminUsers[4]);
            fetchedPage = controller.Get(att.Id, testContext.adminUsers[4]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));

            // Team Owner reader team writer
            controller.AddAdmin(att.Id, testContext.users[20].Id, testContext.users[0]);
            fetchedPage = controller.Get(att.Id, testContext.users[0]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(att.Id, testContext.users[20].Id, testContext.users[0]);
            fetchedPage = controller.Get(att.Id, testContext.users[0]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));

            // Team Writer
            controller.AddAdmin(att.Id, testContext.users[20].Id, testContext.users[10]);
            fetchedPage = controller.Get(att.Id, testContext.users[10]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(att.Id, testContext.users[20].Id, testContext.users[10]);
            fetchedPage = controller.Get(att.Id, testContext.users[10]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));

            // Team Reader
            Assert.Throws<NotAuthorized>(() => controller.AddAdmin(att.Id, testContext.users[20].Id, testContext.users[11]));
            fetchedPage = controller.Get(att.Id, testContext.users[11]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.AddAdmin(att.Id, testContext.users[20].Id, testContext.users[0]);
            Assert.Throws<NotAuthorized>(() => controller.RemoveMember(att.Id, testContext.users[20].Id, testContext.users[11]));
            fetchedPage = controller.Get(att.Id, testContext.users[11]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(att.Id, testContext.users[20].Id, testContext.users[0]);

            // Nested Team Writer writer
            controller.AddAdmin(att.Id, testContext.users[20].Id, testContext.users[3]);
            fetchedPage = controller.Get(att.Id, testContext.users[3]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(att.Id, testContext.users[20].Id, testContext.users[3]);
            fetchedPage = controller.Get(att.Id, testContext.users[3]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));

            // Nested Team Writer reader
            controller.AddAdmin(att.Id, testContext.users[20].Id, testContext.users[4]);
            fetchedPage = controller.Get(att.Id, testContext.users[4]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(att.Id, testContext.users[20].Id, testContext.users[4]);
            fetchedPage = controller.Get(att.Id, testContext.users[4]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));

            // Nested Team Reader writer
            Assert.Throws<NotAuthorized>(() => controller.AddAdmin(att.Id, testContext.users[20].Id, testContext.users[5]));
            fetchedPage = controller.Get(att.Id, testContext.users[5]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.AddAdmin(att.Id, testContext.users[20].Id, testContext.users[0]);
            Assert.Throws<NotAuthorized>(() => controller.RemoveMember(att.Id, testContext.users[20].Id, testContext.users[5]));
            fetchedPage = controller.Get(att.Id, testContext.users[5]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(att.Id, testContext.users[20].Id, testContext.users[0]);

            // Nested Team reader readerder");
            Assert.Throws<NotAuthorized>(() => controller.AddAdmin(att.Id, testContext.users[20].Id, testContext.users[6]));
            fetchedPage = controller.Get(att.Id, testContext.users[6]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.AddAdmin(att.Id, testContext.users[20].Id, testContext.users[0]);
            Assert.Throws<NotAuthorized>(() => controller.RemoveMember(att.Id, testContext.users[20].Id, testContext.users[6]));
            fetchedPage = controller.Get(att.Id, testContext.users[6]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(att.Id, testContext.users[20].Id, testContext.users[0]);

            Assert.Throws<NotAuthorized>(() => controller.AddAdmin(att.Id, testContext.users[20].Id, testContext.users[7]));
            Assert.Throws<NotAuthorized>(() => fetchedPage = controller.Get(att.Id, testContext.users[7]));
            Assert.Throws<NotAuthorized>(() => controller.RemoveMember(att.Id, testContext.users[20].Id, testContext.users[7]));
        }
    }
}