using MongoDB.Entities;

namespace WikiObjects.Data.Model
{
    [Name("Team")]
    public class TeamModel : AclObject
    {
        public TeamModel(string ownerId) : base(ownerId)
        {
        }

        public string name { get; set; }

        public static void CreateIndices()
        {
            DB.Index<TeamModel>()
                .Key(t => t.name, KeyType.Descending)
                .Option(t => t.Unique = true)
                .Create();
        }
    }
}
