using MongoDB.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WikiObjects.Data.Model;

// Assumption
// When we delete, we don't delete all the subpages too

namespace WikiObjects.Data.ModelInterface
{
    public class Page : ACLContainer, IApplyModel<PageModel, Page>
    {
        public static Page FromModel(PageModel um)
        {
            return new Page() { Id = um.ID, Name = um.name, Acl = um.acl, ParentId = um.parentId };
        }

        public void ApplyModel(PageModel um)
        {
            Id = um.ID;
            Name = um.name;
            Acl = um.acl;
            ParentId = um.parentId;
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public string ParentId { get; set; }
    }

    public class PageInterface : ACLInterface<PageModel, Page>
    {
        public static Page Create(string name, User owner)
        {
            var page = new PageModel(owner.Id) { name = name };
            page.Save();

            return Page.FromModel(page);
        }

        public static Page Create(string name, Page parent, User owner)
        {
            var page = new PageModel(owner.Id) { name = name, parentId = parent.Id };
            page.Save();

            return Page.FromModel(page);
        }

        public static Page GetByName(string name)
        {
            var pages = DB.Find<PageModel>()
                .Match(t => t.name.Equals(name))
                .Limit(1)
                .Execute();

            return pages.Count > 0 ? Page.FromModel(pages.FirstOrDefault()) : null;
        }

        public static long Delete(string pageId)
        {
            var result = DB.Delete<PageModel>(pageId);

            return result.DeletedCount;
        }

        public static List<Page> GetChildren(string pageId)
        {
            var pages = DB.Find<PageModel>().Match(p => p.parentId == pageId).Execute();

            return pages.Select(pm => Page.FromModel(pm)).ToList();
        }
    }
}
