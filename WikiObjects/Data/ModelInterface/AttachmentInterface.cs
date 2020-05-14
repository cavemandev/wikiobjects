using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using WikiObjects.Data.Model;

namespace WikiObjects.Data.ModelInterface
{
    public class Attachment : ACLContainer, IACLMember, IApplyModel<AttachmentModel, Attachment>
    {
        public static Attachment FromModel(AttachmentModel um)
        {
            return new Attachment() { Id = um.ID, Name = um.name, Acl = um.acl, ParentId = um.parentId };
        }

        public void ApplyModel(AttachmentModel um)
        {
            Id = um.ID;
            Name = um.name;
            Acl = um.acl;
            ParentId = um.parentId;
        }

        public string Name { get; set; }
        public string Id { get; set; }

        public string ParentId { get; set; }
    }

    public class AttachmentInterface : ACLInterface<AttachmentModel, Attachment>
    {
        public static Attachment Create(string name, Page parent, User owner)
        {
            var att = new AttachmentModel(owner.Id) { name = name, parentId = parent.Id };
            att.Save();

            return Attachment.FromModel(att);
        }

        public static Attachment GetByName(string name)
        {
            var atts = DB.Find<AttachmentModel>()
                .Match(t => t.name.Equals(name))
                .Limit(1)
                .Execute();

            return atts.Count > 0 ? Attachment.FromModel(atts.FirstOrDefault()) : null;
        }

        public static long Delete(string attachmentId)
        {
            var result = DB.Delete<AttachmentModel>(attachmentId);

            return result.DeletedCount;
        }

        public static List<Attachment> GetByParentId(string parentId)
        {
            var pages = DB.Find<AttachmentModel>().Match(p => p.parentId == parentId).Execute();

            return pages.Select(pm => Attachment.FromModel(pm)).ToList();
        }
    }
}

