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

            DB.Delete<UserModel>(u => true);
            DB.Delete<TeamModel>(u => true);
            DB.Delete<PageModel>(u => true);
            DB.Delete<AttachmentModel>(u => true);

            UserModel.CreateIndices();
            TeamModel.CreateIndices();
            PageModel.CreateIndices();
        }
    }
}
