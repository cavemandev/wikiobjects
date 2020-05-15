using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using WikiObjects.Data.Model;

namespace WikiObjects.Data.ModelInterface
{
    public class Attachment : ACLContainer, IAclMember, IApplyModel<AttachmentModel, Attachment>
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

    public class AttachmentInterface : AclInterface<AttachmentModel, Attachment>, IAclMembershipInterface
    {
        private enum UpdateFields
        {
            name,
        }

        public Attachment Create(string name, Page parent, User owner)
        {
            var att = new AttachmentModel(owner.Id) { name = name, parentId = parent.Id };
            att.Save();

            return Attachment.FromModel(att);
        }

        public Attachment GetByName(string name)
        {
            var atts = DB.Find<AttachmentModel>()
                .Match(t => t.name.Equals(name))
                .Limit(1)
                .Execute();

            return atts.Count > 0 ? Attachment.FromModel(atts.FirstOrDefault()) : null;
        }

        public long Delete(string attachmentId)
        {
            var result = DB.Delete<AttachmentModel>(attachmentId);

            return result.DeletedCount;
        }

        public List<Attachment> GetByParentId(string parentId)
        {
            var pages = DB.Find<AttachmentModel>().Match(p => p.parentId == parentId).Execute();

            return pages.Select(pm => Attachment.FromModel(pm)).ToList();
        }

        override public bool IsAdmin(string attachmentId, User subject)
        {
            bool isAdmin = base.IsAdmin(attachmentId, subject);

            if (!isAdmin)
            {
                var att = DB.Find<AttachmentModel>()
                .One(attachmentId);

                if (att == null)
                {
                    return false;
                }

                if (att.parentId != null)
                {
                    PageInterface pageInterface = new PageInterface();
                    return pageInterface.IsAdmin(att.parentId, subject);
                }
            }

            return isAdmin;
        }

        override public bool IsReader(string attachmentId, User subject)
        {
            bool isReader = base.IsAdmin(attachmentId, subject);

            if (!isReader)
            {
                var att = DB.Find<AttachmentModel>()
                .One(attachmentId);

                if (att == null)
                {
                    return false;
                }

                if (att.parentId != null)
                {
                    PageInterface pageInterface = new PageInterface();
                    return pageInterface.IsReader(att.parentId, subject);
                }
            }

            return isReader;
        }

        public Attachment Update(string teamId, Dictionary<string, string> updates)
        {
            AttachmentModel attModel = DB.Find<AttachmentModel>().One(teamId);

            if (attModel == null)
            {
                return null;
            }

            if (updates.ContainsKey(UpdateFields.name.ToString()))
            {
                attModel.name = updates[UpdateFields.name.ToString()];
            }

            attModel.Save();

            return Attachment.FromModel(attModel);
        }
    }
}

