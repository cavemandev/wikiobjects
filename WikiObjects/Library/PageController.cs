using System;
using System.Collections.Generic;
using WikiObjects.Data.ModelInterface;
using WikiObjects.Library;
using WikiObjects.Library.Exceptions;

namespace WikiObjects.Controllers
{
    public class PageController: BaseMembership
    {
        private AttachmentInterface attachmentInterface = new AttachmentInterface();
        private PageInterface pageInterface;

        public PageController() : base(new PageInterface())
        {
            pageInterface = membershipInterface as PageInterface;
        }

        public Page Create(string name, User subject)
        {
            return pageInterface.Create(name, subject);
        }

        public Page Get(string pageId, User subject)
        {
            if (!membershipInterface.IsReader(pageId, subject))
            {
                throw new NotAuthorized();
            }

            return pageInterface.GetById(pageId);
        }

        public long Delete(string pageId, User subject)
        {
            if (!membershipInterface.IsAdmin(pageId, subject))
            {
                throw new NotAuthorized();
            }
            return pageInterface.Delete(pageId);
        }

        public Tuple<List<Page>, List<Attachment>> GetChildren(string pageId, User subject)
        {
            if (!membershipInterface.IsReader(pageId, subject))
            {
                throw new NotAuthorized();
            }

            var pages = pageInterface.GetChildren(pageId);
            var attachments = attachmentInterface.GetByParentId(pageId);

            return new Tuple<List<Page>, List<Attachment>>(pages, attachments);
        }

        public Page AddSubPage(string name, string pageId, User subject)
        {
            if (!membershipInterface.IsAdmin(pageId, subject))
            {
                throw new NotAuthorized();
            }

            return pageInterface.Create(name, pageInterface.GetById(pageId), subject);
        }
       
        public Attachment AddAttachment(string name, string pageId, User subject)
        {
            if (!membershipInterface.IsAdmin(pageId, subject))
            {
                throw new NotAuthorized();
            }

            return attachmentInterface.Create(name, pageInterface.GetById(pageId), subject);
        }

        public Page Update(string pageId, Dictionary<string, string> updates, User subject)
        {
            if (!pageInterface.IsAdmin(pageId, subject))
            {
                throw new NotAuthorized();
            }
            return pageInterface.Update(pageId, updates);
        }
    }
}
