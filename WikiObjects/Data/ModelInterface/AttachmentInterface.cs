using MongoDB.Entities;
using System;
using System.Linq;
using WikiObjects.Data.Model;

namespace WikiObjects.Data.ModelInterface
{
    public class AttachmentInterface : ACLInterface<Attachment>
    {
        public static Attachment Create(string name, Page parent, User owner)
        {
            var att = new Attachment(owner) { name = name, parentId = parent.ID };
            att.Save();

            return att;
        }

        public static Attachment GetByName(string name)
        {
            var atts = DB.Find<Attachment>()
                .Match(t => t.name.Equals(name))
                .Limit(1)
                .Execute();

            return atts.Count > 0 ? atts.FirstOrDefault() : null;
        }

        public static long Delete(string attachmentId)
        {
            var result = DB.Delete<Attachment>(attachmentId);

            return result.DeletedCount;
        }
    }
}

