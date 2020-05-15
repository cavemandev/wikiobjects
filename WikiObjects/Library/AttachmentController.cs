using MongoDB.Entities;
using System.Collections.Generic;
using WikiObjects.Data.ModelInterface;
using WikiObjects.Library.Exceptions;

namespace WikiObjects.Library
{
    public class AttachmentController : BaseMembership
    {
        private AttachmentInterface attachmentInterface;

        public AttachmentController() : base(new AttachmentInterface())
        {
            attachmentInterface = membershipInterface as AttachmentInterface;
        }

        public Attachment Get(string attId, User subject)
        {
            if (!membershipInterface.IsReader(attId, subject))
            {
                throw new NotAuthorized();
            }

            return attachmentInterface.GetById(attId);
        }

        public long Delete(string attachmentId, User subject)
        {
            if (!attachmentInterface.IsAdmin(attachmentId, subject))
            {
                throw new NotAuthorized();
            }

            return attachmentInterface.Delete(attachmentId);
        }

        public Attachment Update(string attachmentId, Dictionary<string, string> updates, User subject)
        {
            if (!attachmentInterface.IsAdmin(attachmentId, subject))
            {
                throw new NotAuthorized();
            }

            return attachmentInterface.Update(attachmentId, updates);
        }
    }
}
