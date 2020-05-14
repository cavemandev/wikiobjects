using MongoDB.Entities;
using System;
using System.Linq;
using WikiObjects.Data.Model;

// Assumption
// When we delete, we don't delete all the subpages too

namespace WikiObjects.Data.ModelInterface
{
    public class PageInterface : ACLInterface<Page>
    {
        public static Page Create(string name, User owner)
        {
            var page = new Page(owner) { name = name };
            page.Save();

            return page;
        }

        public static Page Create(string name, Page parent, User owner)
        {
            var page = new Page(owner) { name = name, parentId = parent.ID };
            page.Save();

            return page;
        }

        public static Page GetByName(string name)
        {
            var pages = DB.Find<Page>()
                .Match(t => t.name.Equals(name))
                .Limit(1)
                .Execute();

            return pages.Count > 0 ? pages.FirstOrDefault() : null;
        }

        public static long Delete(string pageId)
        {
            var result = DB.Delete<Page>(pageId);

            return result.DeletedCount;
        }
    }
}
