namespace WikiObjects.Data.Model
{
    public class Page : ACLObject
    {
        public Page(User owner) : base(owner) { }

        public string name { get; set; }

        public string parentId { get; set; }

        public static void CreateIndices()
        {

        }
    }
}
