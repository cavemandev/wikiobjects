using MongoDB.Driver;
using MongoDB.Entities;
using System.Collections.Generic;
using WikiObjects.Data.Model;
using WikiObjects.Data.ModelInterface;
using Xunit;

namespace WikiTests
{
    public class PageTestContext : BaseTestContext
    {
        public List<User> users { get; set; } = new List<User>();
        public PageTestContext() : base()
        {
            users.Add(UserInterface.Create("Sarah1 Swank1", "sarah1.swank@gmail.com"));
            users.Add(UserInterface.Create("Sarah2 Swank2", "sarah2.swank@gmail.com"));
            users.Add(UserInterface.Create("Sarah3 Swank3", "sarah3.swank@gmail.com"));
            users.Add(UserInterface.Create("Sarah4 Swank4", "sarah4.swank@gmail.com"));
            users.Add(UserInterface.Create("Sarah5 Swank5", "sarah5.swank@gmail.com"));
            users.Add(UserInterface.Create("Sarah6 Swank6", "sarah6.swank@gmail.com"));

            //    PageInterface.AddMember(page, user2, MemberLists.Admins);
            //    PageInterface.AddMember(page, user3, MemberLists.Admins);
            //    PageInterface.AddMember(page, user4, MemberLists.Readers);
            //    PageInterface.RemoveMember(page, user2, MemberLists.Admins);
            //    PageInterface.AddMember(page, user5, MemberLists.Admins);

            //    bool isAdmin = PageInterface.IsAdmin(page.ID, user2);
            //    isAdmin = PageInterface.IsAdmin(page.ID, user3);
            //    isAdmin = PageInterface.IsAdmin(page.ID, user);
            //    isAdmin = PageInterface.IsAdmin(page.ID, user4);
        }
    }

    [Collection("Model")]
    public class PageTests : IClassFixture<PageTestContext>
    {
        PageTestContext pageTestContext;

        public PageTests(PageTestContext ptc)
        {
            pageTestContext = ptc;
            DB.Delete<PageModel>(t => true);
        }

        [Fact]
        public void CreatePageTest()
        {
            string pageName = "New Page";
            var page = PageInterface.Create(pageName, pageTestContext.users[0]);

            var fetched = PageInterface.GetByName(pageName);

            Assert.NotNull(fetched);

            Assert.Equal(page.Id, fetched.Id);

            Assert.Equal(pageName, fetched.Name);
            Assert.Equal(fetched.Acl.ownerId, pageTestContext.users[0].Id);
        }

        [Fact]
        public void CreatePageTest2()
        {
            string pageName = "New Page";
            var page = PageInterface.Create(pageName, pageTestContext.users[0]);

            var fetched = PageInterface.GetById(page.Id);

            Assert.NotNull(fetched);

            Assert.Equal(page.Id, fetched.Id);

            Assert.Equal(pageName, fetched.Name);
            Assert.Equal(fetched.Acl.ownerId, pageTestContext.users[0].Id);
        }

        [Fact]
        public void DeletePageTest()
        {
            string pageName = "New Page";
            var page = PageInterface.Create(pageName, pageTestContext.users[0]);

            var fetched = PageInterface.GetByName(pageName);

            Assert.NotNull(fetched);

            long count = PageInterface.Delete(page.Id);
            Assert.Equal(1, count);

            fetched = PageInterface.GetByName(pageName);
            Assert.Null(fetched);
        }

        [Fact]
        public void AddTwoDeleteOneTest()
        {
            string pageName = "New Page";
            string pageName2 = "New Page 2";
            var page = PageInterface.Create(pageName, pageTestContext.users[0]);
            var page2 = PageInterface.Create(pageName2, pageTestContext.users[0]);

            var fetchedPage = PageInterface.GetByName(pageName);
            var fetchedPage2 = PageInterface.GetByName(pageName2);

            Assert.NotNull(fetchedPage);
            Assert.NotNull(fetchedPage2);

            long count = PageInterface.Delete(page.Id);
            Assert.Equal(1, count);

            fetchedPage = PageInterface.GetByName(pageName);
            Assert.Null(fetchedPage);
            fetchedPage2 = PageInterface.GetByName(pageName2);
            Assert.NotNull(fetchedPage2);
        }

        [Fact]
        public void AddPageTest()
        {
            string pageName = "New Page";
            var page = PageInterface.Create(pageName, pageTestContext.users[0]);

            var fetched = PageInterface.GetByName(pageName);

            Assert.NotNull(fetched);

            string secondPageName = "Second Page";

            PageInterface.Create(secondPageName, page, pageTestContext.users[1]);

            fetched = PageInterface.GetByName(secondPageName);

            Assert.NotNull(fetched);
            Assert.Equal(fetched.ParentId, page.Id);
        }
    }
}
