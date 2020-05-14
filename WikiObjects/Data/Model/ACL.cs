using System.Collections.Generic;
using WikiObjects.Data.ModelInterface;

namespace WikiObjects.Data.Model
{
    public class ACL
    {
        public string ownerId { get; set; }
        public Dictionary<string, MemberType> readers { get; set; }
        public Dictionary<string, MemberType> admins { get; set; }

        public ACL(string ownerId)
        {
            this.ownerId = ownerId;
            readers = new Dictionary<string, MemberType>();
            admins = new Dictionary<string, MemberType>();
        }
    }
}
