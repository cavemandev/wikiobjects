using MongoDB.Entities;
using MongoDB.Entities.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace WikiObjects.Data.Model
{
    public class ACLObject : MongoModel
    {
        public ACL acl { get; set; }

        public ACLObject(string ownerId)
        {
            acl = new ACL(ownerId);
        }

        public void ChangeOwner(UserModel newOwner)
        {
            acl.ownerId = newOwner.ID;
        }
    }
}
