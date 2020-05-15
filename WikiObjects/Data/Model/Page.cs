using MongoDB.Entities;

namespace WikiObjects.Data.Model
{
    [Name("Page")]
    public class PageModel : AclObject
    {
        public PageModel(string ownerId) : base(ownerId) { }

        public string name { get; set; }

        public string parentId { get; set; }

        public static void CreateIndices()
        {

        }
    }
}
