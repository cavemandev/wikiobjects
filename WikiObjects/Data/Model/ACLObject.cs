using MongoDB.Entities;
using MongoDB.Entities.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace WikiObjects.Data.Model
{
    public class AclObject : MongoModel
    {
        public Acl acl { get; set; }

        public AclObject(string ownerId)
        {
            acl = new Acl(ownerId);
        }

        public void ChangeOwner(UserModel newOwner)
        {
            acl.ownerId = newOwner.ID;
        }
    }
}
