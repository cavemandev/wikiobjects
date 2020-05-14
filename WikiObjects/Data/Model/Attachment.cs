using MongoDB.Entities;

namespace WikiObjects.Data.Model
{
    [Name("Attachment")]
    public class AttachmentModel : ACLObject
    {
        public AttachmentModel(string ownerId) : base (ownerId) { }

        public string name { get; set; }

        public string parentId { get; set; }
    }
}
