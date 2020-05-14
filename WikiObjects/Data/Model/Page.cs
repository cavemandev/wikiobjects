using MongoDB.Entities;

namespace WikiObjects.Data.Model
{
    [Name("Page")]
    public class PageModel : ACLObject
    {
        public PageModel(string ownerId) : base(ownerId) { }

        public string name { get; set; }

        public string parentId { get; set; }

        public static void CreateIndices()
        {

        }
    }
}
