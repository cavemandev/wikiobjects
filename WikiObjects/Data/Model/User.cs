using MongoDB.Entities;

namespace WikiObjects.Data.Model
{
    [Name("User")]
    public class UserModel : MongoModel
    {
        public string name { get; set; }
        public string email { get; set; }

        public static void CreateIndices()
        {
            DB.Index<UserModel>()
                .Key(x => x.email, KeyType.Descending)
                .Option(o => o.Unique = true)
                .Create();
        }
    }
}
