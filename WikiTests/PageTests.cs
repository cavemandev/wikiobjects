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
            UserInterface userInterface = new UserInterface();
            users.Add(userInterface.Create("Sarah1 Swank1", "sarah1.swank@gmail.com"));
            users.Add(userInterface.Create("Sarah2 Swank2", "sarah2.swank@gmail.com"));
            users.Add(userInterface.Create("Sarah3 Swank3", "sarah3.swank@gmail.com"));
            users.Add(userInterface.Create("Sarah4 Swank4", "sarah4.swank@gmail.com"));
            users.Add(userInterface.Create("Sarah5 Swank5", "sarah5.swank@gmail.com"));
            users.Add(userInterface.Create("Sarah6 Swank6", "sarah6.swank@gmail.com"));
        }
    }

    [Collection("Model")]
    public class PageTests : IClassFixture<PageTestContext>
    {
        private PageTestContext pageTestContext;
        private PageInterface pageInterface = new PageInterface();

        public PageTests(PageTestContext ptc)
        {
            pageTestContext = ptc;
            DB.Delete<PageModel>(t => true);
        }

        [Fact]
        public void CreatePageTest()
        {
            string pageName = "New Page";
            var page = pageInterface.Create(pageName, pageTestContext.users[0]);

            var fetched = pageInterface.GetByName(pageName);

            Assert.NotNull(fetched);

            Assert.Equal(page.Id, fetched.Id);

            Assert.Equal(pageName, fetched.Name);
            Assert.Equal(fetched.Acl.ownerId, pageTestContext.users[0].Id);
        }

        [Fact]
        public void CreatePageTest2()
        {
            string pageName = "New Page";
            var page = pageInterface.Create(pageName, pageTestContext.users[0]);

            var fetched = pageInterface.GetById(page.Id);

            Assert.NotNull(fetched);

            Assert.Equal(page.Id, fetched.Id);

            Assert.Equal(pageName, fetched.Name);
            Assert.Equal(fetched.Acl.ownerId, pageTestContext.users[0].Id);
        }

        [Fact]
        public void DeletePageTest()
        {
            string pageName = "New Page";
            var page = pageInterface.Create(pageName, pageTestContext.users[0]);

            var fetched = pageInterface.GetByName(pageName);

            Assert.NotNull(fetched);

            long count = pageInterface.Delete(page.Id);
            Assert.Equal(1, count);

            fetched = pageInterface.GetByName(pageName);
            Assert.Null(fetched);
        }

        [Fact]
        public void AddTwoDeleteOneTest()
        {
            string pageName = "New Page";
            string pageName2 = "New Page 2";
            var page = pageInterface.Create(pageName, pageTestContext.users[0]);
            var page2 = pageInterface.Create(pageName2, pageTestContext.users[0]);

            var fetchedPage = pageInterface.GetByName(pageName);
            var fetchedPage2 = pageInterface.GetByName(pageName2);

            Assert.NotNull(fetchedPage);
            Assert.NotNull(fetchedPage2);

            long count = pageInterface.Delete(page.Id);
            Assert.Equal(1, count);

            fetchedPage = pageInterface.GetByName(pageName);
            Assert.Null(fetchedPage);
            fetchedPage2 = pageInterface.GetByName(pageName2);
            Assert.NotNull(fetchedPage2);
        }

        [Fact]
        public void AddPageTest()
        {
            string pageName = "New Page";
            var page = pageInterface.Create(pageName, pageTestContext.users[0]);

            var fetched = pageInterface.GetByName(pageName);

            Assert.NotNull(fetched);

            string secondPageName = "Second Page";

            pageInterface.Create(secondPageName, page, pageTestContext.users[1]);

            fetched = pageInterface.GetByName(secondPageName);

            Assert.NotNull(fetched);
            Assert.Equal(fetched.ParentId, page.Id);
        }
    }
}
