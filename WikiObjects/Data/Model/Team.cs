using MongoDB.Entities;

namespace WikiObjects.Data.Model
{
    public class Team : ACLObject, ACLMember
    {
        public Team(User owner) : base(owner)
        {
        }

        public string name { get; set; }

        public static void CreateIndices()
        {
            DB.Index<Team>()
                .Key(t => t.name, KeyType.Descending)
                .Option(t => t.Unique = true)
                .Create();
        }
    }
}
