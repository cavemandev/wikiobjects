using System.Collections.Generic;
using WikiObjects.Controllers;
using WikiObjects.Data.ModelInterface;
using WikiObjects.Library.Exceptions;
using Xunit;

namespace WikiTests.Library
{
    public class PageTestContext : BaseTestContext
    {
        public PageTestContext() : base()
        {
        }
    }

    [Collection("Model")]
    public class PageControllerTests : IClassFixture<PageTestContext>
    {
        private PageTestContext testContext;
        private PageController controller = new PageController();

        public PageControllerTests(PageTestContext utc)
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
            var page = controller.Create("page", testContext.users[0]);
            var pageSub1 = controller.AddSubPage("sub page 1", page.Id, testContext.users[0]);
            var pageSub2 = controller.AddSubPage("sub page 2", page.Id, testContext.users[0]);
            
            var p = controller.AddSubPage("sub 1 sub page 1", pageSub1.Id, testContext.users[0]);
            controller.ChangeOwner(p.Id, testContext.users[17].Id, testContext.users[0]);

            controller.AddSubPage("sub 2 sub page 1", pageSub2.Id, testContext.users[0]);

            controller.AddAttachment("sub 1 attachment 1", pageSub1.Id, testContext.users[0]);

            controller.AddAttachment("page attachment", page.Id, testContext.users[0]);

            var teams = CreateTeams();

            controller.AddAdmin(page.Id, teams[0].Id, testContext.users[0]);
            controller.AddReader(page.Id, teams[1].Id, testContext.users[0]);

            controller.AddAdmin(page.Id, testContext.users[10].Id, testContext.users[0]);
            controller.AddReader(page.Id, testContext.users[11].Id, testContext.users[0]);

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
        public void CreatePageTest()
        {
            string pageName = "New Page";
            var page = controller.Create(pageName, testContext.users[0]);

            var fetched = controller.Get(page.Id, testContext.users[0]);

            Assert.NotNull(fetched);

            Assert.Equal(page.Id, fetched.Id);

            Assert.Equal(pageName, fetched.Name);
            Assert.Equal(fetched.Acl.ownerId, testContext.users[0].Id);
        }

        [Fact]
        public void CreatePageTest2()
        {
            string pageName = "New Page";
            var page = controller.Create(pageName, testContext.users[0]);

            var fetched = controller.Get(page.Id, testContext.users[0]);

            Assert.NotNull(fetched);

            Assert.Equal(page.Id, fetched.Id);

            Assert.Equal(pageName, fetched.Name);
            Assert.Equal(fetched.Acl.ownerId, testContext.users[0].Id);
        }

        [Fact]
        public void DeletePageTest()
        {
            string pageName = "New Page";
            var page = controller.Create(pageName, testContext.users[0]);

            var fetched = controller.Get(page.Id, testContext.users[0]);

            Assert.NotNull(fetched);

            long count = controller.Delete(page.Id, testContext.users[0]);
            Assert.Equal(1, count);

            Assert.Throws<NotAuthorized>(() => controller.Get(page.Id, testContext.users[0]));
        }

        [Fact]
        public void AddTwoDeleteOneTest()
        {
            string pageName = "New Page";
            string pageName2 = "New Page 2";
            var page = controller.Create(pageName, testContext.users[0]);
            var page2 = controller.Create(pageName2, testContext.users[0]);

            var fetchedPage = controller.Get(page.Id, testContext.users[0]);
            var fetchedPage2 = controller.Get(page2.Id, testContext.users[0]);

            Assert.NotNull(fetchedPage);
            Assert.NotNull(fetchedPage2);

            long count = controller.Delete(page.Id, testContext.users[0]);
            Assert.Equal(1, count);

            Assert.Throws<NotAuthorized>(() => controller.Get(page.Id, testContext.users[0]));
            fetchedPage2 = controller.Get(page2.Id, testContext.users[0]);
            Assert.NotNull(fetchedPage2);
        }

        [Fact]
        public void AddPageTest()
        {
            string pageName = "New Page";
            var page = controller.Create(pageName, testContext.users[0]);

            var fetched = controller.Get(page.Id, testContext.users[0]);

            Assert.NotNull(fetched);

            string secondPageName = "Second Page";

            var subPage = controller.AddSubPage(secondPageName, page.Id, testContext.users[0]);

            fetched = controller.Get(subPage.Id, testContext.users[0]);

            Assert.NotNull(fetched);
            Assert.Equal(fetched.ParentId, page.Id);
        }

        [Fact]
        public void Update()
        {
            testContext.CreateAdminTeam();
            var page = CreateNestedPage();
            var children = controller.GetChildren(page.Id, testContext.adminUsers[0]);
            var subPage = children.Item1[0];
            var children2 = controller.GetChildren(subPage.Id, testContext.adminUsers[0]);
            var subPage2 = children2.Item1[0];

            // Admin Team Owner
            var updates = new Dictionary<string, string>();
            updates.Add("name", "better");
            controller.Update(subPage2.Id, updates, testContext.adminUsers[0]);
            var fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[0]);
            Assert.Equal("better", fetchedPage.Name);

            // Admin Team Writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even writer");
            controller.Update(subPage2.Id, updates, testContext.adminUsers[1]);
            fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[1]);
            Assert.Equal("even writer", fetchedPage.Name);

            // Admin Team Nested reader team writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even reader");
            controller.Update(subPage2.Id, updates, testContext.adminUsers[2]);
            fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[2]);
            Assert.Equal("even reader", fetchedPage.Name);

            // Admin Team Writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even writer");
            controller.Update(subPage2.Id, updates, testContext.adminUsers[3]);
            fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[3]);
            Assert.Equal("even writer", fetchedPage.Name);

            // Admin Team Nested reader team writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even reader");
            controller.Update(subPage2.Id, updates, testContext.adminUsers[4]);
            fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[4]);
            Assert.Equal("even reader", fetchedPage.Name);

            // Team Owner reader team writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even owner");
            controller.Update(subPage2.Id, updates, testContext.users[0]);
            fetchedPage = controller.Get(subPage2.Id, testContext.users[0]);
            Assert.Equal("even owner", fetchedPage.Name);

            // Team Writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even writer");
            controller.Update(subPage2.Id, updates, testContext.users[10]);
            fetchedPage = controller.Get(subPage2.Id, testContext.users[10]);
            Assert.Equal("even writer", fetchedPage.Name);

            // Team Reader
            updates = new Dictionary<string, string>();
            updates.Add("name", "even reader ");
            Assert.Throws<NotAuthorized>(() => controller.Update(subPage2.Id, updates, testContext.users[11]));
            fetchedPage = controller.Get(subPage2.Id, testContext.users[11]);
            Assert.Equal("even writer", fetchedPage.Name);

            // Nested Team Writer writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even nested writer");
            controller.Update(subPage2.Id, updates, testContext.users[3]);
            fetchedPage = controller.Get(subPage2.Id, testContext.users[3]);
            Assert.Equal("even nested writer", fetchedPage.Name);

            // Nested Team Writer reader
            updates = new Dictionary<string, string>();
            updates.Add("name", "even nested reader");
            controller.Update(subPage2.Id, updates, testContext.users[4]);
            fetchedPage = controller.Get(subPage2.Id, testContext.users[4]);
            Assert.Equal("even nested reader", fetchedPage.Name);

            // Nested Team Reader writer
            updates = new Dictionary<string, string>();
            updates.Add("name", "even nested reader writer");
            Assert.Throws<NotAuthorized>(() => controller.Update(subPage2.Id, updates, testContext.users[5]));
            fetchedPage = controller.Get(subPage2.Id, testContext.users[5]);
            Assert.Equal("even nested reader", fetchedPage.Name);

            // Nested Team reader reader
            updates = new Dictionary<string, string>();
            updates.Add("name", "even nested reader reader");
            Assert.Throws<NotAuthorized>(() => controller.Update(subPage2.Id, updates, testContext.users[6]));
            fetchedPage = controller.Get(subPage2.Id, testContext.users[6]);
            Assert.Equal("even nested reader", fetchedPage.Name);

            updates = new Dictionary<string, string>();
            updates.Add("name", "no permissions");
            Assert.Throws<NotAuthorized>(() => controller.Update(subPage2.Id, updates, testContext.users[7]));
            Assert.Throws<NotAuthorized>(() => fetchedPage = controller.Get(subPage2.Id, testContext.users[7]));
            Assert.Equal("even nested reader", fetchedPage.Name);

            // Check the owner
            updates = new Dictionary<string, string>();
            updates.Add("name", "owner for the win");
            controller.Update(subPage2.Id, updates, testContext.users[17]);
            fetchedPage = controller.Get(subPage2.Id, testContext.users[17]);
            Assert.Equal("owner for the win", fetchedPage.Name);
        }

        [Fact]
        public void ChangeOwner()
        {
            testContext.CreateAdminTeam();
            var page = CreateNestedPage();
            var children = controller.GetChildren(page.Id, testContext.adminUsers[0]);
            var subPage = children.Item1[0];
            var children2 = controller.GetChildren(subPage.Id, testContext.adminUsers[0]);
            var subPage2 = children2.Item1[0];

            controller.ChangeOwner(subPage2.Id, testContext.users[14].Id, testContext.adminUsers[0]);
            var fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[0]);
            Assert.Equal(testContext.users[14].Id, fetchedPage.Acl.ownerId);

            // Admin Team Writer
            controller.ChangeOwner(subPage2.Id, testContext.users[15].Id, testContext.adminUsers[1]);
            fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[1]);
            Assert.Equal(testContext.users[15].Id, fetchedPage.Acl.ownerId);

            // Admin Team Nested reader team writer
            controller.ChangeOwner(subPage2.Id, testContext.users[14].Id, testContext.adminUsers[2]);
            fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[2]);
            Assert.Equal(testContext.users[14].Id, fetchedPage.Acl.ownerId);

            // Admin Team Writer
            controller.ChangeOwner(subPage2.Id, testContext.users[15].Id, testContext.adminUsers[3]);
            fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[3]);
            Assert.Equal(testContext.users[15].Id, fetchedPage.Acl.ownerId);

            // Admin Team Nested reader team writer
            controller.ChangeOwner(subPage2.Id, testContext.users[14].Id, testContext.adminUsers[4]);
            fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[4]);
            Assert.Equal(testContext.users[14].Id, fetchedPage.Acl.ownerId);

            // Team Owner reader team writer
            controller.ChangeOwner(subPage2.Id, testContext.users[15].Id, testContext.users[0]);
            fetchedPage = controller.Get(subPage2.Id, testContext.users[0]);
            Assert.Equal(testContext.users[15].Id, fetchedPage.Acl.ownerId);

            // Team Writer
            controller.ChangeOwner(subPage2.Id, testContext.users[14].Id, testContext.users[10]);
            fetchedPage = controller.Get(subPage2.Id, testContext.users[10]);
            Assert.Equal(testContext.users[14].Id, fetchedPage.Acl.ownerId);

            // Team Reader
            Assert.Throws<NotAuthorized>(() => controller.ChangeOwner(subPage2.Id, testContext.users[15].Id, testContext.users[11]));
            fetchedPage = controller.Get(subPage2.Id, testContext.users[11]);
            Assert.Equal(testContext.users[14].Id, fetchedPage.Acl.ownerId);

            // Nested Team Writer writer
            controller.ChangeOwner(subPage2.Id, testContext.users[15].Id, testContext.users[3]);
            fetchedPage = controller.Get(subPage2.Id, testContext.users[3]);
            Assert.Equal(testContext.users[15].Id, fetchedPage.Acl.ownerId);

            // Nested Team Writer reader
            controller.ChangeOwner(subPage2.Id, testContext.users[14].Id, testContext.users[4]);
            fetchedPage = controller.Get(subPage2.Id, testContext.users[4]);
            Assert.Equal(testContext.users[14].Id, fetchedPage.Acl.ownerId);

            // Nested Team Reader writerter");
            Assert.Throws<NotAuthorized>(() => controller.ChangeOwner(subPage2.Id, testContext.users[15].Id, testContext.users[5]));
            fetchedPage = controller.Get(subPage2.Id, testContext.users[5]);
            Assert.Equal(testContext.users[14].Id, fetchedPage.Acl.ownerId);

            // Nested Team reader readerder");
            Assert.Throws<NotAuthorized>(() => controller.ChangeOwner(subPage2.Id, testContext.users[16].Id, testContext.users[6]));
            fetchedPage = controller.Get(subPage2.Id, testContext.users[6]);
            Assert.Equal(testContext.users[14].Id, fetchedPage.Acl.ownerId);

            Assert.Throws<NotAuthorized>(() => controller.ChangeOwner(subPage2.Id, testContext.users[17].Id, testContext.users[7]));
            Assert.Throws<NotAuthorized>(() => fetchedPage = controller.Get(subPage2.Id, testContext.users[7]));
            Assert.Equal(testContext.users[14].Id, fetchedPage.Acl.ownerId);
        }

        [Fact]
        public void AddRemoveMember()
        {
            testContext.CreateAdminTeam();
            var page = CreateNestedPage();
            var children = controller.GetChildren(page.Id, testContext.adminUsers[0]);
            var subPage = children.Item1[0];
            var children2 = controller.GetChildren(subPage.Id, testContext.adminUsers[0]);
            var subPage2 = children2.Item1[0];

            controller.AddAdmin(subPage2.Id, testContext.users[20].Id, testContext.adminUsers[0]);
            var fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[0]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(subPage2.Id, testContext.users[20].Id, testContext.adminUsers[0]);
            fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[0]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));

            // Admin Team Writer
            controller.AddAdmin(subPage2.Id, testContext.users[20].Id, testContext.adminUsers[1]);
            fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[1]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(subPage2.Id, testContext.users[20].Id, testContext.adminUsers[1]);
            fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[1]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));

            // Admin Team Nested reader team writer
            controller.AddAdmin(subPage2.Id, testContext.users[20].Id, testContext.adminUsers[2]);
            fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[2]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(subPage2.Id, testContext.users[20].Id, testContext.adminUsers[2]);
            fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[2]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));

            // Admin Team Writer
            controller.AddAdmin(subPage2.Id, testContext.users[20].Id, testContext.adminUsers[3]);
            fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[3]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(subPage2.Id, testContext.users[20].Id, testContext.adminUsers[3]);
            fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[3]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));

            // Admin Team Nested reader team writer
            controller.AddAdmin(subPage2.Id, testContext.users[20].Id, testContext.adminUsers[4]);
            fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[4]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(subPage2.Id, testContext.users[20].Id, testContext.adminUsers[4]);
            fetchedPage = controller.Get(subPage2.Id, testContext.adminUsers[4]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));

            // Team Owner reader team writer
            controller.AddAdmin(subPage2.Id, testContext.users[20].Id, testContext.users[0]);
            fetchedPage = controller.Get(subPage2.Id, testContext.users[0]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(subPage2.Id, testContext.users[20].Id, testContext.users[0]);
            fetchedPage = controller.Get(subPage2.Id, testContext.users[0]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));

            // Team Writer
            controller.AddAdmin(subPage2.Id, testContext.users[20].Id, testContext.users[10]);
            fetchedPage = controller.Get(subPage2.Id, testContext.users[10]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(subPage2.Id, testContext.users[20].Id, testContext.users[10]);
            fetchedPage = controller.Get(subPage2.Id, testContext.users[10]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));

            // Team Reader
            Assert.Throws<NotAuthorized>(() => controller.AddAdmin(subPage2.Id, testContext.users[20].Id, testContext.users[11]));
            fetchedPage = controller.Get(subPage2.Id, testContext.users[11]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.AddAdmin(subPage2.Id, testContext.users[20].Id, testContext.users[0]);
            Assert.Throws<NotAuthorized>(() => controller.RemoveMember(subPage2.Id, testContext.users[20].Id, testContext.users[11]));
            fetchedPage = controller.Get(subPage2.Id, testContext.users[11]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(subPage2.Id, testContext.users[20].Id, testContext.users[0]);

            // Nested Team Writer writer
            controller.AddAdmin(subPage2.Id, testContext.users[20].Id, testContext.users[3]);
            fetchedPage = controller.Get(subPage2.Id, testContext.users[3]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(subPage2.Id, testContext.users[20].Id, testContext.users[3]);
            fetchedPage = controller.Get(subPage2.Id, testContext.users[3]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));

            // Nested Team Writer reader
            controller.AddAdmin(subPage2.Id, testContext.users[20].Id, testContext.users[4]);
            fetchedPage = controller.Get(subPage2.Id, testContext.users[4]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(subPage2.Id, testContext.users[20].Id, testContext.users[4]);
            fetchedPage = controller.Get(subPage2.Id, testContext.users[4]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));

            // Nested Team Reader writer
            Assert.Throws<NotAuthorized>(() => controller.AddAdmin(subPage2.Id, testContext.users[20].Id, testContext.users[5]));
            fetchedPage = controller.Get(subPage2.Id, testContext.users[5]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.AddAdmin(subPage2.Id, testContext.users[20].Id, testContext.users[0]);
            Assert.Throws<NotAuthorized>(() => controller.RemoveMember(subPage2.Id, testContext.users[20].Id, testContext.users[5]));
            fetchedPage = controller.Get(subPage2.Id, testContext.users[5]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(subPage2.Id, testContext.users[20].Id, testContext.users[0]);

            // Nested Team reader readerder");
            Assert.Throws<NotAuthorized>(() => controller.AddAdmin(subPage2.Id, testContext.users[20].Id, testContext.users[6]));
            fetchedPage = controller.Get(subPage2.Id, testContext.users[6]);
            Assert.False(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.AddAdmin(subPage2.Id, testContext.users[20].Id, testContext.users[0]);
            Assert.Throws<NotAuthorized>(() => controller.RemoveMember(subPage2.Id, testContext.users[20].Id, testContext.users[6]));
            fetchedPage = controller.Get(subPage2.Id, testContext.users[6]);
            Assert.True(fetchedPage.Acl.admins.ContainsKey(testContext.users[20].Id));
            controller.RemoveMember(subPage2.Id, testContext.users[20].Id, testContext.users[0]);

            Assert.Throws<NotAuthorized>(() => controller.AddAdmin(subPage2.Id, testContext.users[20].Id, testContext.users[7]));
            Assert.Throws<NotAuthorized>(() => fetchedPage = controller.Get(subPage2.Id, testContext.users[7]));
            Assert.Throws<NotAuthorized>(() => controller.RemoveMember(subPage2.Id, testContext.users[20].Id, testContext.users[7]));
        }
    }
}
