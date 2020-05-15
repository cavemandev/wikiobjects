using MongoDB.Driver;
using MongoDB.Entities;
using System.Collections.Generic;
using WikiObjects.Data.Model;
using WikiObjects.Data.ModelInterface;
using Xunit;

namespace WikiTests
{
    public class AttachmentTestContext : BaseTestContext
    {
        public List<User> users { get; set; } = new List<User>();

        public List<Page> pages { get; set; } = new List<Page>();
        public AttachmentTestContext() : base()
        {
            UserInterface userInterface = new UserInterface();
            users.Add(userInterface.Create("Sarah1 Swank1", "sarah1.swank@gmail.com"));
            users.Add(userInterface.Create("Sarah2 Swank2", "sarah2.swank@gmail.com"));
            users.Add(userInterface.Create("Sarah3 Swank3", "sarah3.swank@gmail.com"));
            users.Add(userInterface.Create("Sarah4 Swank4", "sarah4.swank@gmail.com"));
            users.Add(userInterface.Create("Sarah5 Swank5", "sarah5.swank@gmail.com"));
            users.Add(userInterface.Create("Sarah6 Swank6", "sarah6.swank@gmail.com"));

            PageInterface pageInterface = new PageInterface();

            pages.Add(pageInterface.Create("New Attachment", users[0]));

        }
    }

    [Collection("Model")]
    public class AttachmentTests : IClassFixture<AttachmentTestContext>
    {
        private AttachmentTestContext attTestContext;
        private AttachmentInterface attachmentInterface = new AttachmentInterface();

        public AttachmentTests(AttachmentTestContext ptc)
        {
            attTestContext = ptc;
            DB.Delete<AttachmentModel>(t => true);
        }

        [Fact]
        public void CreateAttachmentTest()
        {
            string attName = "New Attachment";
            var att = attachmentInterface.Create(attName, attTestContext.pages[0], attTestContext.users[0]);

            var fetched = attachmentInterface.GetByName(attName);

            Assert.NotNull(fetched);

            Assert.Equal(att.Id, fetched.Id);

            Assert.Equal(attName, fetched.Name);
            Assert.Equal(fetched.Acl.ownerId, attTestContext.users[0].Id);
            Assert.Equal(fetched.ParentId, attTestContext.pages[0].Id);
        }

        [Fact]
        public void CreateAttachmentTest2()
        {
            string attName = "New Attachment";
            var att = attachmentInterface.Create(attName, attTestContext.pages[0], attTestContext.users[0]);

            var fetched = attachmentInterface.GetById(att.Id);

            Assert.NotNull(fetched);

            Assert.Equal(att.Id, fetched.Id);

            Assert.Equal(attName, fetched.Name);
            Assert.Equal(fetched.Acl.ownerId, attTestContext.users[0].Id);
            Assert.Equal(fetched.ParentId, attTestContext.pages[0].Id);
        }

        [Fact]
        public void DeleteAttachmentTest()
        {
            string attName = "New Attachment";
            var att = attachmentInterface.Create(attName, attTestContext.pages[0], attTestContext.users[0]);

            var fetched = attachmentInterface.GetByName(attName);

            Assert.NotNull(fetched);

            long count = attachmentInterface.Delete(att.Id);
            Assert.Equal(1, count);

            fetched = attachmentInterface.GetByName(attName);
            Assert.Null(fetched);
        }

        [Fact]
        public void AddTwoDeleteOneTest()
        {
            string attName = "New Attachment";
            string attName2 = "New Attachment 2";
            var att = attachmentInterface.Create(attName, attTestContext.pages[0], attTestContext.users[0]);
            var att2 = attachmentInterface.Create(attName2, attTestContext.pages[0], attTestContext.users[0]);

            var fetchedAttachment = attachmentInterface.GetByName(attName);
            var fetchedAttachment2 = attachmentInterface.GetByName(attName2);

            Assert.NotNull(fetchedAttachment);
            Assert.NotNull(fetchedAttachment2);

            long count = attachmentInterface.Delete(att.Id);
            Assert.Equal(1, count);

            fetchedAttachment = attachmentInterface.GetByName(attName);
            Assert.Null(fetchedAttachment);
            fetchedAttachment2 = attachmentInterface.GetByName(attName2);
            Assert.NotNull(fetchedAttachment2);
        }
    }
}
