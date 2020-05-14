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

        public ACLObject(User owner)
        {
            acl = new ACL(owner.ID);
        }

        public void ChangeOwner(User newOwner)
        {
            acl.ownerId = newOwner.ID;
        }
    }
}
