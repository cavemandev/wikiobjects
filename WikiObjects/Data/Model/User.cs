using MongoDB.Entities;
using MongoDB.Entities.Core;

namespace WikiObjects.Data.Model
{
    public class User : MongoModel, ACLMember
    {
        public string givenName { get; set; }
        public string lastName { get; set; }

        public string email { get; set; }

        public static void CreateIndices()
        {
            DB.Index<User>()
                .Key(x => x.email, KeyType.Descending)
                .Option(o => o.Unique = true)
                .Create();
        }
    }
}
