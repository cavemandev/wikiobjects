using System;
using System.Collections.Generic;
using System.Text;
using WikiObjects.Data.Model;
using WikiObjects.Data.ModelInterface;
using WikiObjects.Library;

namespace WikiObjects.Controllers
{
    class PageController
    {
        public Page Create(string name, User subject)
        {
            return PageInterface.Create(name, subject);
        }

        public Page Get(string pageId, User subject)
        {
            if (!PageInterface.IsReader(pageId, subject))
            {
                throw new NotAuthorized();
            }

            return PageInterface.GetById(pageId);
        }

        public Tuple<List<Page>, List<Attachment>> GetChildren(string pageId, User subject)
        {
            if (!PageInterface.IsReader(pageId, subject))
            {
                throw new NotAuthorized();
            }

            var pages = PageInterface.GetChildren(pageId);
            var attachments = AttachmentInterface.GetByParentId(pageId);

            return new Tuple<List<Page>, List<Attachment>>(pages, attachments);
        }

        public Page AddSubPage(string name, string pageId, User subject)
        {
            if (!PageInterface.IsAdmin(pageId, subject))
            {
                throw new NotAuthorized();
            }

            return PageInterface.Create(name, PageInterface.GetById(pageId), subject);
        }
       
        public Attachment AddAttachment(string name, string pageId, User subject)
        {
            if (!PageInterface.IsAdmin(pageId, subject))
            {
                throw new NotAuthorized();
            }

            return AttachmentInterface.Create(name, PageInterface.GetById(pageId), subject);
        }

        public long RemoveAttachment(string attachmentId, User subject)
        {
            if (!AttachmentInterface.IsAdmin(attachmentId, subject))
            {
                throw new NotAuthorized();
            }

            return AttachmentInterface.Delete(attachmentId);
        }
    }
}
