using MongoDB.Driver;
using MongoDB.Entities;
using WikiObjects.Data.Model;

namespace WikiTests
{
    public class BaseTestContext
    {

        public BaseTestContext()
        {
            new DB(MongoClientSettings.FromConnectionString(
                    "mongodb://root:asdfasdf@localhost:27025/"),
                    "test");

            DB.Delete<User>(u => true);
            DB.Delete<Team>(u => true);
            DB.Delete<Page>(u => true);
            DB.Delete<Attachment>(u => true);

            User.CreateIndices();
            Team.CreateIndices();
            Page.CreateIndices();
        }
    }
}
